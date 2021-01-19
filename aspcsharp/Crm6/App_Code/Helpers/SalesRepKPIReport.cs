using ClosedXML.Excel;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using Models;

namespace Helpers
{
    public class SalesRepKPIReport
    {

        /// <summary>
        /// Generate Sales Rep KPI report
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public SalesRepKPIReportResponse GetReport(SalesRepKPIReportFilters filters)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(LoginUser.GetSharedConnection());

            var response = new SalesRepKPIReportResponse
            {
                SalesReps = new List<SalesRepKPIReportItem>()
            };

            var users = new List<User>();
            if (filters.SalesRepId > 0)
            {
                var salesRep = context.Users.FirstOrDefault(t => t.UserId == filters.SalesRepId);
                if (salesRep != null)
                    users.Add(salesRep);
            }
            else
            {
                var user = context.Users.FirstOrDefault(t => t.UserId == filters.UserId);
                if (user != null)
                {
                    var userRole = user.UserRoles;

                    // get sales manager linked users
                    if (user.UserRoles.Contains("Sales Manager"))
                    {
                        users.AddRange((from t in context.LinkUserToManagers
                                        join j in context.Users on t.UserId equals j.UserId
                                        where t.ManagerUserId == filters.UserId && !t.Deleted && t.SubscriberId == filters.SubscriberId
                                        select j).Distinct().ToList());
                    }

                    if (!string.IsNullOrEmpty(userRole))
                    {
                        if (user.UserRoles.Contains("CRM Admin"))
                        {
                            users.AddRange(context.Users.Where(t => !t.Deleted && t.SubscriberId == filters.SubscriberId).Select(t => t).ToList());
                        }
                        else if (user.UserRoles.Contains("Region Manager"))
                        {
                            if (!string.IsNullOrEmpty(user.RegionName))
                            {
                                // this user is a region manager, get all the companies this user and his district users linked to
                                users.AddRange(context.Users.Where(t => t.RegionName.Equals(user.RegionName) && !t.Deleted && t.SubscriberId == filters.SubscriberId).Select(t => t).ToList());
                            }
                        }
                        else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                        {
                            if (!string.IsNullOrEmpty(user.CountryCode))
                            {
                                // this user is a country manager, get all the companies this user and his district users linked to 
                                users.AddRange(context.Users.Where(t => t.CountryCode.Equals(user.CountryCode) && !t.Deleted && t.SubscriberId == filters.SubscriberId).Select(t => t)
                                                        .ToList());
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
                                    users.AddRange(context.Users.Where(t => t.DistrictCode.Equals(district.DistrictCode) && !t.Deleted && t.SubscriberId == filters.SubscriberId)
                                                    .Select(t => t).ToList());
                                }
                            }
                        }
                        else if (user.UserRoles.Contains("Location Manager"))
                        {
                            if (user.LocationId > 0)
                            {
                                var location = new Locations().GetLocation(user.LocationId, user.SubscriberId);
                                if (location != null)
                                {
                                    // this user is a location manager, get all the companies this user and his location users linked to 
                                    users.AddRange(context.Users.Where(t => t.LocationCode.Equals(location.LocationCode) && !t.Deleted && t.SubscriberId == filters.SubscriberId)
                                        .Select(t => t).ToList());
                                }
                            }
                        }
                        else
                        {
                            users.Add(user);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(filters.CountryName))
                    users = users.Where(t => t.CountryName == filters.CountryName).ToList();
                if (!string.IsNullOrEmpty(filters.LocationCode))
                {
                    users = users.Where(t => t.LocationCode == filters.LocationCode).ToList();
                }

                // only LoginEnabled users
                users = users.Where(t => !t.AdminUser && t.LoginEnabled).ToList();
            }

            users = users.Distinct().OrderBy(t => t.FullName).ToList();

            foreach (var cUser in users)
            {
                var salesRep = new SalesRepKPIReportItem();
                salesRep.SalesRepId = cUser.UserId;
                salesRep.SalesRepName = cUser.FullName;
                salesRep.Country = cUser.CountryName;
                salesRep.Location = cUser.LocationName;

                // logins
                var logins = context.UserActivities.Where(t => t.UserActivityMessage.Equals("Logged In") && t.UserId == cUser.UserId);
                if (filters.DateFrom != null)
                {
                    var dateFrom = Convert.ToDateTime(filters.DateFrom);
                    logins = logins.Where(t => t.UserActivityTimestamp >= dateFrom);
                }
                if (filters.DateTo != null)
                {
                    var dt = Convert.ToDateTime(filters.DateTo);
                    var dateTo = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);
                    logins = logins.Where(t => t.UserActivityTimestamp <= dateTo);
                }
                salesRep.Logins = logins.Count();

                // meetings
                var events = sharedContext.Activities.Where(t => !t.Deleted && cUser.UserId == t.OwnerUserId && (t.CalendarEventId > 0 || t.ActivityType == "EVENT"));
                if (filters.DateFrom != null)
                {
                    var dateFrom = Convert.ToDateTime(filters.DateFrom);
                    events = events.Where(t => t.StartDateTime >= dateFrom);
                }
                if (filters.DateTo != null)
                {
                    var dt = Convert.ToDateTime(filters.DateTo);
                    var dateTo = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);
                    events = events.Where(t => t.EndDateTime <= dateTo);
                }

                if (filters.Categories != null && filters.Categories.Count > 0)
                    events = events.Where(t => filters.Categories.Contains(t.CategoryName));

                salesRep.Events = events.Count();

                // tasks 
                var tasks = sharedContext.Activities.Where(t => !t.Deleted && t.UserId == cUser.UserId && t.SubscriberId == filters.SubscriberId && (t.TaskId > 0 || t.ActivityType == "TASK"));
                if (filters.DateFrom != null)
                {
                    var dateFrom = Convert.ToDateTime(filters.DateFrom);
                    tasks = tasks.Where(t => t.CreatedDate >= dateFrom);
                }
                if (filters.DateTo != null)
                {
                    var dt = Convert.ToDateTime(filters.DateTo);
                    var dateTo = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);
                    tasks = tasks.Where(t => t.CreatedDate <= dateTo);
                }
                salesRep.Tasks = tasks.Count();

                // notes 
                var notes = sharedContext.Activities.Where(a => !a.Deleted && (a.NoteId > 0 || a.ActivityType == "NOTE") && a.CreatedUserId == cUser.UserId && a.SubscriberId == filters.SubscriberId);
                if (filters.DateFrom != null)
                {
                    var dateFrom = Convert.ToDateTime(filters.DateFrom);
                    notes = notes.Where(t => t.CreatedDate >= dateFrom);
                }
                if (filters.DateTo != null)
                {
                    var dt = Convert.ToDateTime(filters.DateTo);
                    var dateTo = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);
                    notes = notes.Where(t => t.CreatedDate <= dateTo);
                }
                salesRep.Notes = notes.Count();

                // new deals
                var deals = (from deal in context.Deals
                             join j in context.LinkUserToDeals on deal.DealId equals j.DealId
                             where j.UserId == cUser.UserId && !deal.Deleted && deal.SubscriberId == cUser.SubscriberId
                             select deal).ToList();

                var newDeals = deals;
                var wonDeals = deals;
                var lostDeals = deals;

                if (filters.DateFrom != null && filters.DateTo != null)
                {
                    var dateFrom = Convert.ToDateTime(filters.DateFrom);
                    var dt = Convert.ToDateTime(filters.DateTo);
                    var dateTo = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);
                    newDeals = newDeals.Where(t => t.CreatedDate >= dateFrom && t.CreatedDate <= dateTo).ToList();
                    wonDeals = wonDeals.Where(t => t.DateWon >= dateFrom && t.DateWon <= dateTo).ToList();
                    lostDeals = lostDeals.Where(t => t.DateLost >= dateFrom && t.DateLost <= dateTo).ToList();
                }
                else if (filters.DateFrom != null)
                {
                    var dateFrom = Convert.ToDateTime(filters.DateFrom);
                    newDeals = newDeals.Where(t => t.CreatedDate >= dateFrom).ToList();
                    wonDeals = wonDeals.Where(t => t.DateWon >= dateFrom).ToList();
                    lostDeals = lostDeals.Where(t => t.DateLost >= dateFrom).ToList();
                }
                else if (filters.DateTo != null)
                {
                    var dt = Convert.ToDateTime(filters.DateTo);
                    var dateTo = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 0);
                    newDeals = newDeals.Where(t => t.CreatedDate <= dateTo).ToList();
                    wonDeals = wonDeals.Where(t => t.DateWon <= dateTo).ToList();
                    lostDeals = lostDeals.Where(t => t.DateLost <= dateTo).ToList();
                }

                salesRep.NewDeals = newDeals.Count();
                salesRep.WonDeals = wonDeals.Count(t => t.SalesStageName.Equals("Won"));
                salesRep.LostDeals = lostDeals.Count(t => t.SalesStageName.Equals("Lost"));

                response.SalesReps.Add(salesRep);

            }

            // create excel
            response.ExcelUri = CreateExcel(response.SalesReps, filters);
            return response;

        }


        private string CreateExcel(List<SalesRepKPIReportItem> reportData, SalesRepKPIReportFilters filters)
        {
            try
            {
                var dt = new DataTable("KPI Report");
                dt.Clear();
                dt.Columns.Add("User");
                dt.Columns.Add("Country");
                dt.Columns.Add("Location");
                dt.Columns.Add("Logins");
                dt.Columns.Add("Meetings");
                dt.Columns.Add("Tasks");
                dt.Columns.Add("Notes");
                dt.Columns.Add("New Deals");
                dt.Columns.Add("Won Deals");
                dt.Columns.Add("Lost Deals");

                foreach (var reportItem in reportData)
                {
                    DataRow dr = dt.NewRow();
                    dr["User"] = reportItem.SalesRepName;
                    dr["Country"] = reportItem.Country;
                    dr["Location"] = reportItem.Location;
                    dr["Logins"] = reportItem.Logins;
                    dr["Meetings"] = reportItem.Events;
                    dr["Tasks"] = reportItem.Tasks;
                    dr["Notes"] = reportItem.Notes;
                    dr["New Deals"] = reportItem.NewDeals;
                    dr["Won Deals"] = reportItem.WonDeals;
                    dr["Lost Deals"] = reportItem.LostDeals;
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

                var fileName = "kpi_report.xlsx";
                var blobReference = "kpi_report_" + DateTime.Now.ToString("yyyymmddhhmm") + Path.GetExtension(fileName);
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
            catch (Exception)
            {
            }
            return "";
        }


        public List<UserActivity> GetLogins(int subscriberId, int userId, string datefromStr, string dateToStr)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(LoginUser.GetSharedConnection());

            var userTimeZone = context.Users.Where(t => t.UserId == userId).Select(t => t.TimeZone).FirstOrDefault();
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

                var userActivities = context.UserActivities.Where(t => t.UserActivityMessage.Equals("Logged In") && t.UserId == userId);

                if (!string.IsNullOrEmpty(datefromStr))
                {
                    var dateFrom = Convert.ToDateTime(datefromStr);
                    userActivities = userActivities.Where(t => t.UserActivityTimestamp >= dateFrom);
                }
                if (!string.IsNullOrEmpty(dateToStr))
                {
                    var dateTo = Convert.ToDateTime(dateToStr);
                    userActivities = userActivities.Where(t => t.UserActivityTimestamp <= dateTo);
                }

                // convert to users timezone

                foreach (var userActivity in userActivities)
                {
                    if (cstZone != null)
                        userActivity.UserActivityTimestamp = TimeZoneInfo.ConvertTimeFromUtc(userActivity.UserActivityTimestamp, cstZone);
                    else
                        userActivity.UserActivityTimestamp = ConvertUtcToUserDateTime(userActivity.UserActivityTimestamp, userId, userTimeZone, utcOffsetDefault);

                }
                return userActivities.OrderByDescending(t => t.UserActivityTimestamp).ToList();
            }

            return new List<UserActivity>();


        }


        public List<Crm6.App_Code.Shared.Activity> GetEvents(int subscriberId, int userId, int loggedInUserId, string datefromStr, string dateToStr)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(LoginUser.GetSharedConnection());
            var events = sharedContext.Activities.Where(t => !t.Deleted && t.SubscriberId == subscriberId
                                        && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")).Select(t => t);

            // apply filters
            if (userId > 0)
                events = events.Where(t => t.OwnerUserId == userId);
            if (!string.IsNullOrEmpty(datefromStr))
            {
                var dateFrom = Convert.ToDateTime(datefromStr);
                events = events.Where(t => t.StartDateTime >= dateFrom);
            }
            if (!string.IsNullOrEmpty(dateToStr))
            {
                var dateTo = Convert.ToDateTime(dateToStr);
                events = events.Where(t => t.EndDateTime <= dateTo);
            }

            // events = events.Where(t => t.CalendarEvent.CategoryName == "Meeting External");

            // user time zones
            var userTimeZone = context.Users.Where(t => t.UserId == loggedInUserId).Select(t => t.TimeZone).FirstOrDefault();
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

                foreach (var caEvent in events)
                {
                    // contacts 

                    if (cstZone != null)
                        caEvent.StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(caEvent.StartDateTime.Value, cstZone);
                    else
                        caEvent.StartDateTime = ConvertUtcToUserDateTime(caEvent.StartDateTime.Value, loggedInUserId, userTimeZone, utcOffsetDefault);

                    // caEvent.StartDateTimeStr = caEvent.CalendarEvent.StartDateTime.Value.ToString("dd-MMM-yy HH:mm");

                }
            }

            return events.OrderBy(t => t.StartDateTime).ToList();
        }


        public List<Crm6.App_Code.Shared.Activity> GetTasks(int subscriberId, int userId, int loggedInUserId, string datefromStr, string dateToStr)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);

            var tasks = sharedContext.Activities.Where(t => !t.Deleted && t.UserId == userId && (t.TaskId > 0 || t.ActivityType == "TASK")
                                && t.SubscriberId == subscriberId);

            if (!string.IsNullOrEmpty(datefromStr))
            {
                var dateFrom = Convert.ToDateTime(datefromStr);
                tasks = tasks.Where(t => t.CreatedDate >= dateFrom);
            }
            if (!string.IsNullOrEmpty(dateToStr))
            {
                var dateTo = Convert.ToDateTime(dateToStr);
                tasks = tasks.Where(t => t.CreatedDate <= dateTo);
            }

            return tasks.OrderBy(t => t.DueDate).ToList();
        }


        public List<DealModel> GetDeals(int subscriberId, int userId, int loggedInUserId, string status, string datefromStr, string dateToStr)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var deals = (from deal in context.Deals
                         join j in context.LinkUserToDeals on deal.DealId equals j.DealId
                         where j.UserId == userId && !deal.Deleted && deal.SubscriberId == subscriberId
                         select deal);

            var newDeals = deals;
            var wonDeals = deals;
            var lostDeals = deals;

            if (!string.IsNullOrEmpty(datefromStr) && !string.IsNullOrEmpty(dateToStr))
            {
                var dateFrom = Convert.ToDateTime(datefromStr);
                var dateTo = Convert.ToDateTime(dateToStr);
                newDeals = newDeals.Where(t => t.CreatedDate >= dateFrom && t.CreatedDate <= dateTo);
                wonDeals = wonDeals.Where(t => t.DateWon >= dateFrom && t.CreatedDate <= dateTo);
                lostDeals = lostDeals.Where(t => t.DateLost >= dateFrom && t.CreatedDate <= dateTo);
            }
            else if (!string.IsNullOrEmpty(datefromStr))
            {
                var dateFrom = Convert.ToDateTime(datefromStr);
                newDeals = newDeals.Where(t => t.CreatedDate >= dateFrom);
                wonDeals = wonDeals.Where(t => t.DateWon >= dateFrom);
                lostDeals = lostDeals.Where(t => t.DateLost >= dateFrom);
            }
            else if (!string.IsNullOrEmpty(dateToStr))
            {
                var dateTo = Convert.ToDateTime(dateToStr);
                newDeals = newDeals.Where(t => t.CreatedDate <= dateTo);
                wonDeals = wonDeals.Where(t => t.DateWon <= dateTo);
                lostDeals = lostDeals.Where(t => t.DateLost <= dateTo);
            }

            IQueryable<Deal> filteredDealList = null;

            if (!string.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "New":
                        filteredDealList = newDeals;
                        break;
                    case "Lost":
                        filteredDealList = lostDeals.Where(t => t.SalesStageName.Equals("Lost"));
                        break;
                    case "Won":
                        filteredDealList = wonDeals.Where(t => t.SalesStageName.Equals("Won"));
                        break;
                    default:
                        break;
                }
            }

            // set the final list
            var finalModelList = new List<DealModel>();
            if (filteredDealList != null)
            {
                foreach (var deal in filteredDealList)
                {
                    var dealModel = new DealModel
                    {
                        SubscriberId = deal.SubscriberId,
                        DealId = deal.DealId,
                        DealName = deal.DealName,
                        CreatedDateStr = new Timezones().ConvertUtcToUserDateTime(deal.CreatedDate, loggedInUserId).ToString("dd-MMM-yy @ HH:mm"),
                        CompanyName = deal.CompanyName,
                        SalesStageName = deal.SalesStageName,
                        LastActivityDateStr = deal.LastActivityDate.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.LastActivityDate.Value, loggedInUserId).ToString("dd-MMM-yy") : "-",
                        NextActivityDateStr = deal.NextActionDate.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.NextActionDate.Value, loggedInUserId).ToString("dd-MMM-yy") : "-",
                        WonLostReason = deal.ReasonWonLost ?? ""
                    };

                    // won/lost dates
                    dealModel.WonLostDateStr = "";
                    if (!string.IsNullOrEmpty(status) && status.Equals("Lost"))
                        dealModel.WonLostDateStr = deal.DateLost.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.DateLost.Value, loggedInUserId).ToString("dd-MMM-yy") : "-";
                    if (!string.IsNullOrEmpty(status) && status.Equals("Won"))
                        dealModel.WonLostDateStr = deal.DateWon.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.DateWon.Value, loggedInUserId).ToString("dd-MMM-yy") : "-";

                    finalModelList.Add(dealModel);
                }
            }

            return finalModelList;

        }


        public List<Crm6.App_Code.Shared.Activity> GetNotes(int subscriberId, int userId, int loggedInUserId, string datefromStr, string dateToStr)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);
            var notes = sharedContext.Activities.Where(t => !t.Deleted && t.CreatedUserId == userId && (t.NoteId > 0 || t.ActivityType == "NOTE")
                               && t.SubscriberId == subscriberId)
                                .Select(t => t);

            if (!string.IsNullOrEmpty(datefromStr))
            {
                var dateFrom = Convert.ToDateTime(datefromStr);
                notes = notes.Where(t => t.CreatedDate >= dateFrom);
            }
            if (!string.IsNullOrEmpty(dateToStr))
            {
                var dateTo = Convert.ToDateTime(dateToStr);
                notes = notes.Where(t => t.CreatedDate <= dateTo);
            }

            var finalList = new List<NoteModel>();
            foreach (var note in notes)
            {
                note.ActivityDate = new Timezones().ConvertUtcToUserDateTime(note.ActivityDate, loggedInUserId); ;
            }
            return notes.OrderBy(t => t.ActivityDate).ToList();
        }


        public DateTime ConvertUtcToUserDateTime(DateTime utcDateTime, int userId, string userTimeZone, string utcOffsetDefault)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);
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


    }
}