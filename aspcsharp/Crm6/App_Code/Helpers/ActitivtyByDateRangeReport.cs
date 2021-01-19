using System;
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
    public class ActitivtyByDateRangeReport
    {

        public ActivityByDateRangeReportResponse GetReport(ActivityByDateRangReportFilters filters)
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


        public ActivityByDateRangeReportResponse GetReport(ActivityByDateRangReportFilters filters, List<int> subscriberIds)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var response = new ActivityByDateRangeReportResponse
            {
                Activities = new List<ActivityReportItem>()
            };

            // user time zones
            var userTimeZone = context.Users.Where(t => t.UserId == filters.LoggedInUserId).Select(t => t.TimeZone).FirstOrDefault();
            var currentUser = context.Users.Where(t => t.UserId == filters.LoggedInUserId).FirstOrDefault();
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
                    if (filters.NoUsersSelectedInForGlobalCampaigns && subscriberIds.Count > 1)
                    {
                        // global campaigns selected, no users selected as filters
                    }
                    else
                    {
                        sqlStr += " AND ( Activities.OwnerUserIdGlobal IN (" + string.Join(",", filters.GlobalUserIds) + ") OR Activities.UserIdGlobal IN " +
                      "(" + string.Join(",", filters.GlobalUserIds) + ") OR ActivititesMembers.UserIdGlobal IN (" + string.Join(",", filters.GlobalUserIds) + ")) ";
                    }
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


                // deal type
                if (!string.IsNullOrEmpty(filters.DealType))
                {
                    sqlStr += " AND Activities.DealTypes LIKE '%" + filters.DealType + "%' ";
                }

                // global company ids
                if (filters.GlobalComapnyIds != null && filters.GlobalComapnyIds.Count > 0)
                {
                    sqlStr += " AND Activities.CompanyIdGlobal IN ('" + string.Join("','", filters.GlobalComapnyIds) + "')";
                }


                // competitors
                if (filters.Competitors != null && filters.Competitors.Count > 0)
                {
                    var count = 1;
                    sqlStr += " AND (";
                    foreach (var competitor in filters.Competitors)
                    {
                        sqlStr += " Activities.Competitors LIKE '%" + competitor + "%'";
                        if (count < filters.Competitors.Count)
                        {
                            sqlStr += " OR ";
                        }
                        count += 1;
                    }
                    sqlStr += " ) ";

                }

                if (filters.DateFrom != null)
                {
                    var dateFrom = Convert.ToDateTime(filters.DateFrom);
                    dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 0, 0, 0);
                    // convert to UTC 
                    var dateFromUTC = ConvertUtcToUserDateTime(dateFrom, filters.LoggedInUserId, userTimeZone, utcOffsetDefault);
                    sqlStr += " AND Activities.ActivityDate >= '" + dateFromUTC.ToString("s") + "'";
                }

                if (filters.DateTo != null)
                {
                    var dateTo = Convert.ToDateTime(filters.DateTo);
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
                        DataTable dt = new DataTable();
                        dt.Load(dataReader);

                        // iterate though salesStage totals records
                        foreach (DataRow dr in dt.Rows)
                        {
                            var reportItem = new ActivityReportItem();
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
                            reportItem.ContactsStr = dr["ContactNames"] is DBNull ? "" : dr["ContactNames"].ToString();
                            reportItem.Subject = dr["Subject"] is DBNull ? "" : dr["Subject"].ToString();
                            reportItem.TaskName = dr["TaskName"] is DBNull ? "" : dr["TaskName"].ToString();
                            reportItem.CreatedDate = Convert.ToDateTime(dr["CreatedDate"].ToString());
                            reportItem.LastUpdatedDate = Convert.ToDateTime(dr["LastUpdate"].ToString());
                            reportItem.ActivityDate = Convert.ToDateTime(dr["ActivityDate"].ToString());
                            reportItem.Location = dr["UserLocation"] is DBNull ? "" : dr["UserLocation"].ToString();
                            reportItem.User = dr["OwnerUserName"] is DBNull ? "" : dr["OwnerUserName"].ToString(); 
                            reportItem.Campaigns = dr["Campaigns"] is DBNull ? "" : dr["Campaigns"].ToString();
                            reportItem.DealType = dr["DealTypes"] is DBNull ? "" : dr["DealTypes"].ToString();
                            reportItem.Competitors = dr["Competitors"] is DBNull ? "" : dr["Competitors"].ToString(); 
                            // description
                            reportItem.Description = dr["Description"] is DBNull ? "" : dr["Description"].ToString();
                            reportItem.Description = reportItem.ActivityType == "NOTE" ? dr["NoteContent"].ToString() : Utils.StripHtml(reportItem.Description + "");

                            // activity date
                            if (cstZone != null)
                                reportItem.ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(reportItem.ActivityDate, cstZone);
                            else
                                reportItem.ActivityDate = ConvertUtcToUserDateTime(reportItem.ActivityDate, filters.LoggedInUserId, userTimeZone, utcOffsetDefault);


                            if (filters.DateTo != null)
                            {
                                var dateTo = Convert.ToDateTime(filters.DateTo);
                                dateTo = new DateTime(dateTo.Year, dateTo.Month, dateTo.Day, 23, 59, 0);
                                if (reportItem.ActivityDate > dateTo)
                                {
                                    continue;
                                }
                            }
                            if (reportItem.ActivityType == "TASK")
                            {
                                reportItem.ActivityDateStr = reportItem.ActivityDate.ToString("dd-MMM-yy");
                            }else
                                reportItem.ActivityDateStr = reportItem.ActivityDate.ToString("dd-MMM-yy HH:mm");

                            response.Activities.Add(reportItem);
                            conn.Close();
                        }



                    }
                    conn.Close();
                }
            }

            response.Activities = response.Activities.OrderBy(t => t.ActivityDate).ToList();
            // create excel
            response.ExcelUri = CreateExcel(response.Activities, filters);
            return response;
        }



        private string CreateExcel(List<ActivityReportItem> reportData, ActivityByDateRangReportFilters filters)
        {
            try
            {
                var dt = new DataTable("Activity By Date Range");
                dt.Clear();
                dt.Columns.Add("Activity Type/Category");
                dt.Columns.Add("Subject");
                dt.Columns.Add("User");
                dt.Columns.Add("Activity Date");
                dt.Columns.Add("Description");
                dt.Columns.Add("Location");
                dt.Columns.Add("Completed");
                dt.Columns.Add("Deal");
                dt.Columns.Add("Companies");
                dt.Columns.Add("Contacts");
                dt.Columns.Add("Competitors");
                dt.Columns.Add("Campaigns");
                dt.Columns.Add("Deal Type");
                dt.Columns.Add("Created Date");
                dt.Columns.Add("Last Update");

                foreach (var reportItem in reportData)
                {
                    DataRow dr = dt.NewRow();
                    dr["Activity Type/Category"] = reportItem.ActivityType + (string.IsNullOrEmpty(reportItem.Category) ? "" : " / " + reportItem.Category);
                    dr["Subject"] = reportItem.ActivityType == "EVENT" ? reportItem.Subject : reportItem.TaskName;
                    dr["User"] = reportItem.User;
                    dr["Activity Date"] = reportItem.ActivityDateStr;
                    if (reportItem.Description.Length > 32002)
                    {
                        dr["Description"] = (reportItem.Description + "").Substring(0, 32000);
                    }
                    else
                        dr["Description"] = reportItem.Description;

                    dr["Location"] = reportItem.Location;
                    dr["Completed"] = reportItem.ActivityType == "Task" ? (reportItem.Completed ? "YES" : "NO") : "";
                    dr["Deal"] = reportItem.Deals;
                    dr["Companies"] = reportItem.CompanyName;
                    dr["Contacts"] = reportItem.ContactsStr;
                    dr["Competitors"] = reportItem.Competitors;
                    dr["Campaigns"] = reportItem.Campaigns;
                    dr["Deal Type"] = reportItem.DealType;
                    dr["Created Date"] = reportItem.CreatedDate.ToString("dd MMMM, yyyy @ HH:mm");
                    dr["Last Update"] = reportItem.LastUpdatedDate.ToString("dd MMMM, yyyy @ HH:mm");

                    dt.Rows.Add(dr);
                }

                var wb = new XLWorkbook();
                var dataTable = dt;

                // Add a DataTable as a worksheet
                wb.Worksheets.Add(dataTable);
                var st = new MemoryStream();
                wb.SaveAs(st);

                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

                // Create the blob client
                var blobClient = storageAccount.CreateCloudBlobClient();
                // Retrieve a reference to a container
                var containerReference = "temp";
                var container = blobClient.GetContainerReference(containerReference);

                var fileName = "activityBydateRange.xlsx";
                var blobReference = "activityBydateRange_" + DateTime.Now.ToString("yyyymmddhhmm") + Path.GetExtension(fileName);

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
                    PageCalledFrom = "Reports/ActivityByDateRangeReport",
                    SubscriberId = filters.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = filters.LoggedInUserId
                };
                new Logging().LogWebAppError(error);
                return "";
            }
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


        public List<User> GetAccessibleGlobalUserIdsForUser(int globalUserId, int subscriberId)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var finalUserList = new List<User>();

            // get current user Id
            var user = context.Users.FirstOrDefault(t => t.UserIdGlobal == globalUserId);

            // users
            var users = context.Users.Where(t => t.SubscriberId == subscriberId && !t.Deleted).ToList();

            if (user != null)
            {
                // logged in user
                var userIds = new List<int> { user.UserIdGlobal };

                if (!string.IsNullOrEmpty(user.UserRoles))
                {
                    if (user.UserRoles.Contains("Sales Manager"))
                    {
                        userIds = (from t in context.LinkUserToManagers
                                   join j in context.Users on t.UserId equals j.UserId
                                   where t.ManagerUserId == user.UserId && !t.Deleted
                                   select j.UserIdGlobal).Distinct().ToList();
                    }

                    if (user.UserRoles.Contains("CRM Admin"))
                    {
                        userIds.AddRange(users.Select(t => t.UserIdGlobal).ToList());
                    }
                    else if (user.UserRoles.Contains("Region Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.RegionName))
                        {
                            // this user is a region manager, get all the companies this user and his district users linked to 
                            userIds.AddRange(context.Users.Where(u => u.RegionName.Equals(user.RegionName))
                                .Select(u => u.UserIdGlobal).ToList());
                        }
                    }
                    else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                    {
                        if (!string.IsNullOrEmpty(user.CountryCode))
                        {
                            // this user is a country manager, get all the companies this user and his district users linked to 
                            userIds.AddRange(context.Users.Where(c => c.CountryCode.Equals(user.CountryCode))
                                .Select(c => c.UserIdGlobal).ToList());
                        }
                    }
                    else if (user.UserRoles.Contains("District Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.DistrictCode))
                        {
                            var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                            if (district != null)
                            {
                                // this user is a district manager, get all the companies this user and his district users linked to 
                                userIds.AddRange(context.Users.Where(u => u.DistrictCode.Equals(district.DistrictCode))
                                        .Select(u => u.UserIdGlobal).ToList());
                            }
                        }
                    }
                    else if (user.UserRoles.Contains("Location Manager"))
                    {
                        if (user.LocationId > 0)
                        {
                            var location = new Locations().GetLocation(user.LocationId, subscriberId);
                            if (location != null)
                            {
                                // this user is a location manager, get all the companies this user and his location users linked to 
                                userIds.AddRange(context.Users.Where(u => u.LocationCode.Equals(location.LocationCode)).Select(u => u.UserIdGlobal)
                                    .ToList());
                            }
                        }
                    }
                    else
                    {
                        userIds.Add(user.UserIdGlobal);
                    }
                }


                // add users to the final list
                userIds = userIds.Distinct().ToList();
                // get final users
                finalUserList = users.Where(t => userIds.Contains(t.UserIdGlobal) && t.UserIdGlobal > 0).OrderBy(t => t.FullName).ToList();

            }

            return finalUserList;
        }


        public string FormatDate(DateTime? dateIn, string format = "ydd-mmm-yy")
        {
            string returnValue;
            if (dateIn == null) return "";
            try
            {
                var dtm = Convert.ToDateTime(dateIn);
                returnValue = dtm.ToString(format);
            }
            catch (Exception)
            {
                returnValue = "";
            }
            return returnValue;
        }

    }
}