﻿using System;
using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Models;
using System.Data;
using ClosedXML.Excel;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Crm6.App_Code.Shared;
using System.Data.SqlClient;

namespace Helpers
{
    public class WeeklyActivityReport
    {

        /// <summary>
        /// get report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public WeeklyActivityReportResponse GetReport(WeeklyActivityFilters filters)
        { 
            var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());
            var subscriberIds = new List<int> { filters.SubscriberId }; 
            if (filters.Campaigns != null && filters.Campaigns.Count > 0)
            {
                var campaign = sharedContext.Campaigns.FirstOrDefault(t => filters.Campaigns.Contains(t.CampaignName) && t.SubscriberId == filters.SubscriberId);
                if (campaign != null && campaign.CampaignType == "Global")
                {
                    var sids = sharedContext.LinkGlobalSuscriberToSubscribers
                                                    .Where(s => s.GlobalSubscriberId == filters.SubscriberId && s.DataCenter != "")
                                                    .Select(s => s.LinkedSubscriberId)
                                                    .ToList().Distinct();
                    subscriberIds.AddRange(sids);
                }
            } 
            return GetReport(filters, subscriberIds);
        }

 

        private WeeklyActivityReportResponse GetReport(WeeklyActivityFilters filters, List<int> subscriberIds)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
             
            var response = new WeeklyActivityReportResponse
            {
                ActivitiesByDay = new List<ActivitiesByDay>()
            };

            // iterate through days
            var weekDate = Convert.ToDateTime(filters.DateMonday);
            for (int i = 0; i < 7; i++)
            {
                var activitiesByDay = new ActivitiesByDay
                {
                    DayOfWeek = weekDate,
                    DayOfWeekStr = weekDate.ToString("dd-MMM-yy"),
                    Activities = new List<WeeklyActivityReportItem>()
                };
                var dateFrom = new DateTime(weekDate.Year, weekDate.Month, weekDate.Day, 0, 0, 0);
                var dateTo = new DateTime(weekDate.Year, weekDate.Month, weekDate.Day, 23, 59, 0);


                var userTimeZone = context.Users.Where(t => t.UserId == filters.LoggedInUserId).Select(t => t.TimeZone).FirstOrDefault();
                var utcOffsetDefault = "";
                var timezone = sharedContext.TimeZones.FirstOrDefault(t => t.TimeZoneName == userTimeZone);
                if (timezone != null)
                {
                    TimeZoneInfo cstZone = null;
                    if (!string.IsNullOrWhiteSpace(timezone.EnumTimeZoneID))
                    {
                        cstZone = TimeZoneInfo.FindSystemTimeZoneById(timezone.EnumTimeZoneID.Trim());
                    }
                    utcOffsetDefault = timezone.UtcOffset;


                    // construct the SQL statement
                    var sqlStr = "SELECT DISTINCT Activities.* From Activities LEFT JOIN ActivititesMembers ON Activities.ActivityId = ActivititesMembers.ActivitiesId ";
                    sqlStr += "WHERE Activities.Deleted = 0 AND Activities.SubscriberId IN (" + string.Join(",", subscriberIds) + ") ";

                    // report should always pass at least one global user id
                    if (filters.GlobalUserIds != null && filters.GlobalUserIds.Count > 0)
                    {
                        sqlStr += " AND ( Activities.OwnerUserIdGlobal IN (" + string.Join(",", filters.GlobalUserIds) + ") OR Activities.UserIdGlobal IN " +
                                  "(" + string.Join(",", filters.GlobalUserIds) + ") OR ActivititesMembers.UserIdGlobal IN (" + string.Join(",", filters.GlobalUserIds) + ")) ";
                    }

                    // campaigns
                    if (filters.Campaigns != null && filters.Campaigns.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var campaign in filters.Campaigns)
                        {
                            sqlStr += " Activities.Campaigns LIKE '%" + campaign + "%'";
                            if (count < filters.Campaigns.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }

                    // categories
                    if (filters.Categories != null && filters.Categories.Count > 0)
                    {
                        var count = 1;
                        sqlStr += " AND (";
                        foreach (var category in filters.Categories)
                        {
                            sqlStr += " Activities.CategoryName LIKE '%" + category + "%'";
                            if (count < filters.Categories.Count)
                            {
                                sqlStr += " OR ";
                            }
                            count += 1;
                        }
                        sqlStr += " ) ";
                    }



                    if (dateFrom != null)
                    { 
                        dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                        // convert to UTC 
                        var dateFromUTC = ConvertUtcToUserDateTime(dateFrom, filters.LoggedInUserId, userTimeZone, utcOffsetDefault);
                        sqlStr += " AND Activities.ActivityDate >= '" + dateFromUTC.ToString("s") + "'";
                    }

                    if (dateTo != null)
                    { 
                        dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                        // convert to UTC 
                        var dateToUTC = ConvertUtcToUserDateTime(dateTo, filters.LoggedInUserId, userTimeZone, utcOffsetDefault);
                        sqlStr += " AND Activities.ActivityDate <= '" + dateToUTC.ToString("s") + "'";
                    }

                    var activityTypes = new List<string>();
                    if (filters.ActivityTypes.Contains("events"))
                    {
                        activityTypes.Add(" Activities.ActivityType = 'EVENT' ");
                    }
                    if (filters.ActivityTypes.Contains("tasks"))
                    {
                        activityTypes.Add(" Activities.ActivityType = 'TASK' ");
                    }
                    if (filters.ActivityTypes.Contains("notes"))
                    {
                        activityTypes.Add(" Activities.ActivityType = 'NOTE' ");
                    }
                    sqlStr += " AND ( " + string.Join(" OR ", activityTypes) + " ) ";

                     


                    using (var conn = new SqlConnection(sharedConnection))
                    {
                        conn.Open();
                        // populate dataReader
                        var cmd = new SqlCommand(sqlStr, conn);
                        SqlDataReader dataReader = cmd.ExecuteReader();

                        // check if no records
                        if (dataReader.HasRows)
                        {
                            var dt = new DataTable();
                            dt.Load(dataReader);

                            // iterate though salesStage totals records
                            foreach (DataRow dr in dt.Rows)
                            {
                                var reportItem = new WeeklyActivityReportItem();
                                reportItem.ActivtyId = int.Parse(dr["ActivityId"].ToString());
                                reportItem.SubscriberId = int.Parse(dr["SubscriberId"].ToString());
                                reportItem.ActivityType = dr["ActivityType"] is DBNull ? "" : dr["ActivityType"].ToString();
                                reportItem.DealIds = dr["DealIds"] is DBNull ? "" : dr["DealIds"].ToString();
                                reportItem.Deals = dr["DealNames"] is DBNull ? "" : dr["DealNames"].ToString();
                                reportItem.Category = dr["CategoryName"] is DBNull ? "" : dr["CategoryName"].ToString();
                                reportItem.CompanyId = dr["CompanyId"] is DBNull ? 0 : int.Parse(dr["CompanyId"].ToString());
                                reportItem.CompanyIdGlobal = dr["CompanyIdGlobal"] is DBNull ? 0 : int.Parse(dr["CompanyIdGlobal"].ToString());
                                reportItem.CompanySubscriberId = dr["CompanySubscriberId"] is DBNull ? 0 : int.Parse(dr["CompanySubscriberId"].ToString());
                                reportItem.CompanyName = dr["CompanyName"] is DBNull ? "" : dr["CompanyName"].ToString();
                                reportItem.ContactStr = dr["ContactNames"] is DBNull ? "" : dr["ContactNames"].ToString();
                                reportItem.Subject = dr["Subject"] is DBNull ? "" : dr["Subject"].ToString();
                                reportItem.TaskName = dr["TaskName"] is DBNull ? "" : dr["TaskName"].ToString();
                                reportItem.CreatedDate = Convert.ToDateTime(dr["CreatedDate"].ToString());
                                reportItem.LastUpdatedDate = Convert.ToDateTime(dr["LastUpdate"].ToString());
                                reportItem.ActivityDate = Convert.ToDateTime(dr["ActivityDate"].ToString());
                                reportItem.Location = dr["UserLocation"] is DBNull ? "" : dr["UserLocation"].ToString();
                                reportItem.User = dr["OwnerUserName"] is DBNull ? "" : dr["OwnerUserName"].ToString();

                                // description
                                reportItem.Description = dr["Description"] is DBNull ? "" : dr["Description"].ToString();
                                reportItem.Description = reportItem.ActivityType == "NOTE" ? dr["NoteContent"].ToString() : Utils.StripHtml(reportItem.Description + "");

                                // activity date
                                if (cstZone != null)
                                    reportItem.ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(reportItem.ActivityDate, cstZone);
                                else
                                    reportItem.ActivityDate = ConvertUtcToUserDateTime(reportItem.ActivityDate, filters.LoggedInUserId, userTimeZone, utcOffsetDefault);
                                 
                                if (dateTo != null)
                                { 
                                    dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                                    if (reportItem.ActivityDate > dateTo)
                                    {
                                        continue;
                                    }
                                }

                                reportItem.ActivityDateStr = reportItem.ActivityDate.ToString("dd-MMM-yy");
                                
                                activitiesByDay.Activities.Add(reportItem);
                                conn.Close();
                            } 
                        }
                        conn.Close();
                    } 
                }

                response.ActivitiesByDay.Add(activitiesByDay);

                // go to next date
                weekDate = weekDate.AddDays(1);
            }

            // create excel
            response.ExcelUri = CreateExcel(response.ActivitiesByDay, filters);
            return response;
        }


        public DateTime ConvertUtcToUserDateTime(DateTime utcDateTime, int userId, string userTimeZone, string utcOffsetDefault)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            // Get Offset Hours from User TimeZone to UTC
            var utcOffset = sharedContext.TimeZonesDaylightSavings.Where(t => t.TimeZoneName.Trim() == userTimeZone.Trim()
                                                     && t.DstStartDate <= utcDateTime
                                                     && t.DstEndDate >= utcDateTime
                                                   ).Select(t => t.UtcOffset).FirstOrDefault();

            if (utcOffset == null)
            {
                utcOffset = utcOffsetDefault;
            }
            var offSetWithoutPlus = utcOffset;
            if (utcOffset.Contains("+"))
            {
                offSetWithoutPlus = offSetWithoutPlus.Replace("+", "");
            }

            if (string.IsNullOrWhiteSpace(offSetWithoutPlus) == false)
            {
                TimeSpan offSet = TimeSpan.Parse(offSetWithoutPlus);
                DateTime userDateTime = utcDateTime + offSet;
                return userDateTime;
            }
            return utcDateTime;
        }


        private string CreateExcel(List<ActivitiesByDay> activitiesByDay, WeeklyActivityFilters filters)
        {
            try
            {
                var dt = new DataTable("Activity By Week  " + activitiesByDay[0].DayOfWeekStr.Replace("-", "_"));
                dt.Clear();
                dt.Columns.Add("Activity Type");
                dt.Columns.Add("Subject");
                dt.Columns.Add("User");
                dt.Columns.Add("Activity Date");
                dt.Columns.Add("Description");
                dt.Columns.Add("Deals");
                dt.Columns.Add("Company");
                dt.Columns.Add("Contacts");
                dt.Columns.Add("Created Date");
                dt.Columns.Add("Last Update");

                foreach (var activityCollection in activitiesByDay)
                {
                    DataRow dr = dt.NewRow();
                    // set this as the date
                    dr["Activity Type"] = activityCollection.DayOfWeek.ToString("ddd") + " - " + activityCollection.DayOfWeekStr;
                    dt.Rows.Add(dr);

                    if (activityCollection.Activities.Count > 0)
                    {
                        // bind activities
                        foreach (var activity in activityCollection.Activities)
                        {
                            dr = dt.NewRow();
                            dr["Activity Type"] = activity.ActivityType;
                            dr["Subject"] = activity.Subject;
                            dr["User"] = activity.User;
                            dr["Activity Date"] = activity.ActivityDateStr;
                            if (activity.Description.Length > 32002)
                            {
                                dr["Description"] = (activity.Description + "").Substring(0, 32000);
                            }
                            else
                                dr["Description"] = activity.Description;

                            dr["Deals"] = activity.Deals; 
                            dr["Company"] = activity.CompanyName;
                            var contactStr = string.Join(",", activity.Contacts.Select(t => t.name).ToList());
                            dr["Contacts"] = contactStr;
                            dr["Created Date"] = activity.CreatedDate.ToString("dd-MMM-yy @ HH:mm");
                            dr["Last Update"] = activity.LastUpdatedDate.ToString("dd-MMM-yy @ HH:mm");
                            dt.Rows.Add(dr);
                        }
                    }
                    else
                    {
                        dr = dt.NewRow();
                        // set this as the date
                        dr["Activity Type"] = "No activities found";
                        dt.Rows.Add(dr);
                    }

                    // empty row
                    dr = dt.NewRow();
                    dt.Rows.Add(dr);
                    dr = dt.NewRow();
                    dt.Rows.Add(dr);
                }

                var wb = new XLWorkbook();
                var dataTable = dt;

                // Add a DataTable as a worksheet
                wb.Worksheets.Add(dataTable);
                var st = new MemoryStream();
                wb.SaveAs(st);

                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

                // Create the blob client.
                var blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container.
                var containerReference = "temp";
                var container = blobClient.GetContainerReference(containerReference);

                var fileName = "activityByweek" + activitiesByDay[0].DayOfWeekStr.Replace("/", "_") + ".xlsx";

                var blobReference = "activityByweek_" + DateTime.Now.ToString("yyyymmddhhmm") + Path.GetExtension(fileName);

                var blockBlob = container.GetBlockBlobReference(blobReference);
                blockBlob.Properties.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                using (st)
                {
                    long streamlen = st.Length;
                    st.Position = 0;
                    blockBlob.UploadFromStream(st);
                }

                return new BlobStorageHelper().GetBlob(containerReference, blobReference);

            }
            catch (Exception ex)
            {
                var error = new WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "CreateExcel",
                    PageCalledFrom = "Reports/WeeklyActivityReport",
                    SubscriberId = filters.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = filters.LoggedInUserId
                };
                new Logging().LogWebAppError(error);
                return "";
            }
        }
    }
}
