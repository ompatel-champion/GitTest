using System;
using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Models;
using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code.Login;

namespace Helpers
{
    public class Activities
    {

        public ActivityChartDataResponse GetActivityChartData(ActivityChartDataFilter filters)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);

            var response = new ActivityChartDataResponse
            {
                ActivityCountByTypes = new List<ActivityCountByType> { },
                ChartData = new List<ActivityChartData> { }
            };

            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            // new deals
            var deals = context.Deals.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId)
                                            && t.CreatedDate >= startDate && t.CreatedDate <= endDate).Select(t => t.CreatedDate).ToList();
            response.ActivityCountByTypes.Add(new ActivityCountByType { ActivityType = "New Deals", ActivityCount = deals.Count });
            dts.AddRange(deals);


            // won deals
            var wonDeals = context.Deals.Where(t => t.SubscriberId == filters.SubscriberId && locationCodes.Contains(t.LocationCode)
                                           && t.DateWon >= startDate && t.DateWon <= endDate && t.DateWon != null).Select(t => t.DateWon.Value).ToList();
            response.ActivityCountByTypes.Add(new ActivityCountByType { ActivityType = "Won Deals", ActivityCount = wonDeals.Count });
            dts.AddRange(wonDeals);

            // lost deals
            var lostDeals = context.Deals.Where(t => t.SubscriberId == filters.SubscriberId && locationCodes.Contains(t.LocationCode)
                                           && t.DateLost >= startDate && t.DateLost <= endDate && t.DateLost != null).Select(t => t.DateLost.Value).ToList();
            response.ActivityCountByTypes.Add(new ActivityCountByType { ActivityType = "Lost Deals", ActivityCount = lostDeals.Count });
            dts.AddRange(lostDeals);

            // tasks
            var tasks = sharedContext.Activities.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId) && (t.TaskId > 0 || t.ActivityType == "TASK")
                                            && t.CreatedDate >= startDate && t.CreatedDate <= endDate).Select(t => t.CreatedDate).ToList();
            response.ActivityCountByTypes.Add(new ActivityCountByType { ActivityType = "Tasks", ActivityCount = tasks.Count });
            dts.AddRange(tasks);

            // events
            var events = sharedContext.Activities.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId) && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                                         && t.CreatedDate >= startDate && t.CreatedDate <= endDate).Select(t => t.CreatedDate).ToList();
            response.ActivityCountByTypes.Add(new ActivityCountByType { ActivityType = "Events", ActivityCount = events.Count });
            dts.AddRange(events);

            // companies
            var companies = context.Companies.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId)
                                         && t.CreatedDate >= startDate && t.CreatedDate <= endDate).Select(t => t.CreatedDate).ToList();
            response.ActivityCountByTypes.Add(new ActivityCountByType { ActivityType = "New Companies", ActivityCount = companies.Count });
            dts.AddRange(companies);


            // logins
            var logins = context.UserActivities.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                        && t.UserActivityMessage == "Logged In"
                                        && t.UserActivityTimestamp >= startDate && t.UserActivityTimestamp <= endDate).Select(t => t.UserActivityTimestamp).ToList();
            response.ActivityCountByTypes.Add(new ActivityCountByType { ActivityType = "Logins", ActivityCount = logins.Count });
            dts.AddRange(logins);



            // notes
            var notes = context.Notes.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId)
                                         && t.CreatedDate >= startDate && t.CreatedDate <= endDate).Select(t => t.CreatedDate).ToList();
            response.ActivityCountByTypes.Add(new ActivityCountByType { ActivityType = "Notes", ActivityCount = notes.Count });
            dts.AddRange(notes);

            var q = (from i in dts
                     let dt = i
                     group i by new { y = dt.Year, m = dt.Month, d = dt.Day } into g
                     select new { ActivityDate = g.Key, ActivityCount = g.Count() }).ToList();

            foreach (var item in q)
            {
                response.ChartData.Add(new ActivityChartData
                {
                    ActivityDate = new DateTime(item.ActivityDate.y, item.ActivityDate.m, item.ActivityDate.d),
                    ActivityCount = item.ActivityCount
                });
            }

            return response;
        }

        public bool IsActivityRecurring(int activityId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);

            return sharedContext.Activities.Any(x => x.ActivityId == activityId && x.IsRecurring.HasValue && x.IsRecurring.Value);
        }

        public List<DealModel> GetActivityChartNewDeals(ActivityChartDataFilter filters)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            // new deals
            var deals = context.Deals.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId)
                                            && t.CreatedDate >= startDate && t.CreatedDate <= endDate).ToList();

            var finalList = new List<DealModel>();
            if (deals != null)
            {
                foreach (var deal in deals)
                {
                    var dealModel = new DealModel
                    {
                        SubscriberId = deal.SubscriberId,
                        DealId = deal.DealId,
                        DealName = deal.DealName,
                        CreatedDateStr = new Timezones().ConvertUtcToUserDateTime(deal.CreatedDate, filters.UserId).ToString("dd-MMM-yy @ HH:mm"),
                        CompanyName = deal.CompanyName,
                        SalesStageName = deal.SalesStageName,
                        LastActivityDateStr = deal.LastActivityDate.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.LastActivityDate.Value, filters.UserId).ToString("dd-MMM-yy") : "-",
                        NextActivityDateStr = deal.NextActionDate.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.NextActionDate.Value, filters.UserId).ToString("dd-MMM-yy") : "-",
                        WonLostReason = deal.ReasonWonLost ?? ""
                    };
                    finalList.Add(dealModel);
                }
            }
            return finalList;
        }



        public List<DealModel> GetActivityChartLostDeals(ActivityChartDataFilter filters)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            var lostDeals = context.Deals.Where(t => t.SubscriberId == filters.SubscriberId && locationCodes.Contains(t.LocationCode)
                                          && t.DateLost >= startDate && t.DateLost <= endDate && t.DateLost != null).ToList();

            // lost deals
            var finalList = new List<DealModel>();
            if (lostDeals != null)
            {
                foreach (var deal in lostDeals)
                {
                    var dealModel = new DealModel
                    {
                        SubscriberId = deal.SubscriberId,
                        DealId = deal.DealId,
                        DealName = deal.DealName,
                        CreatedDateStr = new Timezones().ConvertUtcToUserDateTime(deal.CreatedDate, filters.UserId).ToString("dd-MMM-yy @ HH:mm"),
                        CompanyName = deal.CompanyName,
                        SalesStageName = deal.SalesStageName,
                        LastActivityDateStr = deal.LastActivityDate.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.LastActivityDate.Value, filters.UserId).ToString("dd-MMM-yy") : "-",
                        NextActivityDateStr = deal.NextActionDate.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.NextActionDate.Value, filters.UserId).ToString("dd-MMM-yy") : "-",
                        WonLostReason = deal.ReasonWonLost ?? ""
                    };
                    dealModel.WonLostDateStr = "";
                    dealModel.WonLostDateStr = deal.DateLost.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.DateLost.Value, filters.UserId).ToString("dd-MMM-yy") : "-";
                    finalList.Add(dealModel);
                }
            }

            return finalList;
        }



        public List<DealModel> GetActivityChartWonDeals(ActivityChartDataFilter filters)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            // won deals
            var wonDeals = context.Deals.Where(t => t.SubscriberId == filters.SubscriberId && locationCodes.Contains(t.LocationCode)
                                          && t.DateWon >= startDate && t.DateWon <= endDate && t.DateWon != null).ToList();

            var finalList = new List<DealModel>();
            if (wonDeals != null)
            {
                foreach (var deal in wonDeals)
                {
                    var dealModel = new DealModel
                    {
                        SubscriberId = deal.SubscriberId,
                        DealId = deal.DealId,
                        DealName = deal.DealName,
                        CreatedDateStr = new Timezones().ConvertUtcToUserDateTime(deal.CreatedDate, filters.UserId).ToString("dd-MMM-yy @ HH:mm"),
                        CompanyName = deal.CompanyName,
                        SalesStageName = deal.SalesStageName,
                        LastActivityDateStr = deal.LastActivityDate.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.LastActivityDate.Value, filters.UserId).ToString("dd-MMM-yy") : "-",
                        NextActivityDateStr = deal.NextActionDate.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.NextActionDate.Value, filters.UserId).ToString("dd-MMM-yy") : "-",
                        WonLostReason = deal.ReasonWonLost ?? ""
                    };
                    dealModel.WonLostDateStr = "";
                    dealModel.WonLostDateStr = deal.DateWon.HasValue ? new Timezones().ConvertUtcToUserDateTime(deal.DateWon.Value, filters.UserId).ToString("dd-MMM-yy") : "-";
                    finalList.Add(dealModel);
                }
            }

            return finalList;
        }


        public List<UserActivity> GetActivityChartLogins(ActivityChartDataFilter filters)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            var logins = context.UserActivities.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                      && t.UserActivityMessage == "Logged In"
                                      && t.UserActivityTimestamp >= startDate && t.UserActivityTimestamp <= endDate).ToList();
            return logins;
        }


        public List<CalendarEventModel> GetActivityChartEvents(ActivityChartDataFilter filters)
        {
            var finalEventList = new List<CalendarEventModel>();
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);


            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            // events
            var events = sharedContext.Activities.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId) && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                                         && t.CreatedDate >= startDate && t.CreatedDate <= endDate).Select(t => new CalendarEventModel
                                         {
                                             DealName = t.DealNames,
                                             DealIds = t.DealIds,
                                             CalendarEvent = t
                                         });

            var userTimeZone = context.Users.Where(t => t.UserId == filters.UserId).Select(t => t.TimeZone).FirstOrDefault();
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
                    caEvent.ContactsStr = string.IsNullOrEmpty(caEvent.ContactsStr) ? "" : caEvent.ContactsStr;
                    if (cstZone != null)
                        caEvent.CalendarEvent.StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(caEvent.CalendarEvent.StartDateTime.Value, cstZone);
                    else
                        caEvent.CalendarEvent.StartDateTime = ConvertUtcToUserDateTime(caEvent.CalendarEvent.StartDateTime.Value, filters.UserId, userTimeZone, utcOffsetDefault);

                    caEvent.StartDateTimeStr = caEvent.CalendarEvent.StartDateTime.Value.ToString("dd-MMM-yy HH:mm");

                    // companies 
                    caEvent.CompaniesStr = string.IsNullOrEmpty(caEvent.CompaniesStr) ? "" : caEvent.CompaniesStr;

                    // deal
                    caEvent.DealName = string.IsNullOrEmpty(caEvent.DealName) ? "" : caEvent.DealName;

                    finalEventList.Add(caEvent);
                }
            }

            return finalEventList.OrderBy(t => t.CalendarEvent.StartDateTime).ToList();
        }


        public List<Activity> GetActivityChartTasks(ActivityChartDataFilter filters)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());


            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            // tasks
            var tasks = sharedContext.Activities.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId)
                                           && t.CreatedDate >= startDate && t.CreatedDate <= endDate).ToList();



            return tasks;
        }


        public List<NoteModel> GetActivityChartNotes(ActivityChartDataFilter filters)
        {
            var finalList = new List<NoteModel>();
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            // notes
            var notes = context.Notes.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId)
                                         && t.CreatedDate >= startDate && t.CreatedDate <= endDate).Select(t => new NoteModel
                                         {
                                             DealName = t.DealName,
                                             DealId = t.DealId,
                                             Note = t
                                         }).ToList();


            foreach (var note in notes)
            {
                note.CreatedDateStr = new Timezones().ConvertUtcToUserDateTime(note.Note.CreatedDate, filters.UserId).ToString("dd-MMM-yy");
                note.DealName = string.IsNullOrEmpty(note.DealName) ? "" : note.DealName;
                // contact name
                var contactName = new Contacts().GetContactNameFromId(note.Note.ContactId, note.Note.SubscriberId);
                note.ContactName = string.IsNullOrEmpty(contactName) ? "" : contactName;
                // company name
                var companyName = new Companies().GetCompanyNameFromId(note.Note.CompanyId, note.Note.SubscriberId);
                note.CompanyName = string.IsNullOrEmpty(companyName) ? "" : companyName;

                finalList.Add(note);
            }

            return finalList.OrderBy(t => t.Note.CreatedDate).ToList();



            return finalList;
        }


        public List<Company> GetActivityChartCompanies(ActivityChartDataFilter filters)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var startDate = Convert.ToDateTime(filters.StartDate);
            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 0, 0, 0);
            var endDate = Convert.ToDateTime(filters.EndDate);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var dts = new List<DateTime>();

            // get the user sales reps
            var userIds = GetUserSalesRepIds(filters.SubscriberId, filters.UserId);
            var locationCodes = context.Users.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.UserId)
                                                       && t.LocationCode != null && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();

            // companies
            var companies = context.Companies.Where(t => t.SubscriberId == filters.SubscriberId && userIds.Contains(t.CreatedUserId)
                                         && t.CreatedDate >= startDate && t.CreatedDate <= endDate).ToList();

            return companies;
        }




        /// <summary>
        /// This function returns all the accessible sales rep ids for a passed user id
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<int> GetUserSalesRepIds(int subscriberId, int userId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var finalUserList = new List<int>();

            // current user
            var user = context.Users.FirstOrDefault(t => t.UserId == userId);
            // users
            var users = context.Users.Where(t => t.SubscriberId == subscriberId && !t.Deleted);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.UserRoles))
                {
                    // get sales manager user's location codes
                    if (user.UserRoles.Contains("Sales Manager"))
                    {
                        var userIds = (from t in context.LinkUserToManagers
                                       where !t.Deleted && t.ManagerUserId == user.UserId
                                       select t.UserId).ToList();

                        // get users
                        finalUserList = users.Where(t => userIds.Contains(t.UserId)).Select(t => t.UserId).ToList();
                    }

                    if (user.UserRoles.Contains("CRM Admin") || user.UserRoles.Contains("Region Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.RegionName))
                        {
                            var locationCodes = context.Locations
                                                       .Where(t => t.RegionName == user.RegionName && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();
                            finalUserList.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                                                           t.RegionName != null && t.RegionName != "" &&
                                                           t.RegionName.Equals(user.RegionName)).Select(t => t.UserId).ToList());
                        }
                    }
                    else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                    {
                        if (!string.IsNullOrEmpty(user.CountryCode))
                        {
                            var locationCodes = context.Locations
                                                       .Where(t => t.CountryCode == user.CountryCode && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();
                            finalUserList.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                              t.CountryCode != null && t.CountryCode != "" && t.CountryCode.Equals(user.CountryCode)).Select(t => t.UserId).ToList());
                        }
                    }
                    else if (user.UserRoles.Contains("District Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.DistrictCode))
                        {
                            var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                            if (district != null)
                            {
                                var locationCodes = context.Locations
                                                           .Where(t => t.DistrictCode == district.DistrictCode && t.LocationCode != "")
                                                           .Select(t => t.LocationCode).ToList();
                                finalUserList.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                                                  t.DistrictCode != null && t.DistrictCode != ""
                                                  && t.DistrictCode.Equals(user.DistrictCode)).Select(t => t.UserId).ToList());
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
                                finalUserList.AddRange(users.Where(t => t.LocationCode == location.LocationCode
                                                        && t.CountryCode.Equals(user.CountryCode)).Select(t => t.UserId).ToList());
                            }
                        }
                    }
                    else
                    {
                        finalUserList.Add(user.UserId);
                    }
                }
            }

            // current user  
            finalUserList.Add(user.UserId);

            // return the distinct user id list
            return finalUserList.Distinct().ToList();
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


        public List<Activity> GetActivitiesForCalendar(ActivityFilter filters)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                            .GlobalSubscribers.Where(t => t.SubscriberId == filters.SubscriberId)
                                                            .Select(t => t.DataCenter).FirstOrDefault();

            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var currentUser = userContext.Users.Where(t => t.UserId == filters.UserId).FirstOrDefault();
            var userTimeZone = currentUser.TimeZone;

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


                var includeEvents = filters.ActivityTypes != null && filters.ActivityTypes.Contains("EVENT");
                var includeTasks = filters.ActivityTypes != null && filters.ActivityTypes.Contains("TASK");

                var activities = sharedContext.Activities.Where(t => !t.Deleted).Select(t => t);

                // filter events and tasks
                if (includeEvents && includeTasks)
                    activities = activities.Where(t => t.ActivityType == "EVENT" || t.ActivityType == "TASK");
                else if (includeEvents)
                    activities = activities.Where(t => t.ActivityType == "EVENT");
                else if (includeTasks)
                    activities = activities.Where(t => t.ActivityType == "TASK");

                var eventList = activities.ToList();


                // apply filters
                if (filters.SubscriberId > 0)
                {
                    // get linked subscriber notes as it always filters by the global company id afterwards
                    var linkedSubscribers = sharedContext.LinkGlobalSuscriberToSubscribers
                                                       .Where(s => s.GlobalSubscriberId == filters.SubscriberId && s.DataCenter != "")
                                                       .Select(s => s.LinkedSubscriberId)
                                                       .ToList();
                    eventList = eventList.Where(a => linkedSubscribers.Contains(a.SubscriberId)).ToList();
                }

                if (filters.OwnerUserIdGlobal > 0)
                {
                    var loginContext = new DbLoginDataContext(LoginUser.GetLoginConnection());
                    var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.GlobalUserId == filters.OwnerUserIdGlobal);
                    if (globalUser != null)
                    {
                        // get invited event for the user id
                        var invitedEvents = sharedContext.ActivititesMembers.Where(t => t.UserIdGlobal == filters.OwnerUserIdGlobal && !t.Deleted)
                                                         .Select(t => t.ActivitiesId).Distinct().ToList();
                        eventList = eventList.Where(t => t.OwnerUserIdGlobal == filters.OwnerUserIdGlobal || invitedEvents.Contains(t.ActivityId)).ToList();
                    }
                    else
                    {
                        return new List<Activity>();
                    }
                }

                if (includeEvents && includeTasks)
                {
                    if (filters.DateFrom != null)
                        eventList = eventList.Where(t => t.StartDateTime >= filters.DateFrom.Value || t.ActivityDate >= filters.DateFrom.Value).ToList();

                    if (filters.DateTo != null)
                        eventList = eventList.Where(t => t.EndDateTime == null || t.EndDateTime <= filters.DateTo.Value || t.ActivityDate <= filters.DateTo.Value).ToList();
                }
                else if (includeEvents)
                {
                    if (filters.DateFrom != null)
                        eventList = eventList.Where(t => t.StartDateTime >= filters.DateFrom.Value).ToList();

                    if (filters.DateTo != null)
                        eventList = eventList.Where(t => t.EndDateTime == null || t.EndDateTime <= filters.DateTo.Value).ToList();
                }
                else if (includeTasks)
                {
                    if (filters.DateFrom != null)
                        eventList = eventList.Where(t => t.ActivityDate >= filters.DateFrom.Value).ToList();
                    if (filters.DateTo != null)
                        eventList = eventList.Where(t => t.ActivityDate <= filters.DateTo.Value).ToList();
                }
                else
                {
                    return new List<Activity>();
                }

                // categories
                var categories = context.EventCategories.Where(t => t.SubscriberId == filters.SubscriberId).ToList();


                foreach (var eCalendarEvent in eventList)
                {
                    if (eCalendarEvent.StartDateTime.HasValue)
                    {
                        if (cstZone != null)
                            eCalendarEvent.StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(eCalendarEvent.StartDateTime.Value, cstZone);
                        else
                            eCalendarEvent.StartDateTime = ConvertUtcToUserDateTime(eCalendarEvent.StartDateTime.Value, filters.UserId, userTimeZone, utcOffsetDefault);
                    }

                    if (!eCalendarEvent.IsAllDay && eCalendarEvent.EndDateTime.HasValue)
                    {
                        if (cstZone != null)
                            eCalendarEvent.EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(eCalendarEvent.EndDateTime.Value, cstZone);
                        else
                            eCalendarEvent.EndDateTime = ConvertUtcToUserDateTime(eCalendarEvent.EndDateTime.Value, filters.UserId, userTimeZone, utcOffsetDefault);
                    }


                    if (cstZone != null)
                        eCalendarEvent.ActivityDate = TimeZoneInfo.ConvertTimeFromUtc(eCalendarEvent.ActivityDate, cstZone);
                    else
                        eCalendarEvent.ActivityDate = ConvertUtcToUserDateTime(eCalendarEvent.ActivityDate, filters.UserId, userTimeZone, utcOffsetDefault);



                    // set category color
                    if (categories != null)
                    {
                        var category = categories.FirstOrDefault(t => t.CategoryName == eCalendarEvent.CategoryName);
                        if (category != null)
                        {
                            eCalendarEvent.CategoryColor = category.CategoryColor;
                        }
                    }
                }
                return eventList;
            }

            return new List<Activity>();
        }


        public bool ChangeActivityTaskDate(Activity task)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == task.SubscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var fTask = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == task.ActivityId);
            if (fTask != null)
            {
                // Timezone conversion to UTC
                if (fTask.DueDate.HasValue)
                {
                    fTask.DueDate = task.DueDate;
                    fTask.ActivityDate = task.DueDate.Value;
                }

                fTask.LastUpdate = DateTime.UtcNow;
                fTask.UpdateUserId = task.UpdateUserId;
                var username = new Users().GetUserFullNameById(task.UpdateUserId, task.SubscriberId);
                fTask.UpdateUserName = username;
                sharedContext.SubmitChanges();

                return true;
            }
            return false;
        }


        public ActivitesResponse GetActivities(ActivitiesFilter filters)
        {
            var response = new ActivitesResponse
            {
                Activites = new List<Activity>()
            };

            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                .GlobalSubscribers.Where(t => t.SubscriberId == filters.SubscriberId)
                .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var currentUser = userContext.Users.Where(t => t.UserId == filters.UserId).FirstOrDefault();

            var userTimeZone = currentUser.TimeZone;
            var utcOffsetDefault = "";
            var timezone = sharedContext.TimeZones.FirstOrDefault(t => t.TimeZoneName == userTimeZone);
            if (timezone != null)
            {
                TimeZoneInfo cstZone = null;
                if (!string.IsNullOrWhiteSpace(timezone.EnumTimeZoneID)) cstZone = TimeZoneInfo.FindSystemTimeZoneById(timezone.EnumTimeZoneID.Trim());
                utcOffsetDefault = timezone.UtcOffset;
                var linkedSubscribers = sharedContext.LinkGlobalSuscriberToSubscribers
                    .Where(s => s.GlobalSubscriberId == filters.SubscriberId && s.DataCenter != "")
                    .Select(s => s.LinkedSubscriberId);
                var events = sharedContext.Activities.Where(t => !t.Deleted && linkedSubscribers.Contains(t.SubscriberId));

                if (filters.OwnerUserIdGlobal > 0)
                {
                    var loginContext = new DbLoginDataContext(LoginUser.GetLoginConnection());
                    var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.GlobalUserId == filters.OwnerUserIdGlobal);
                    if (globalUser != null)
                    {
                        var invitedEvents = sharedContext.ActivititesMembers.Where(t => t.UserIdGlobal == filters.OwnerUserIdGlobal && !t.Deleted)
                            .Select(t => t.ActivitiesId).Distinct().ToList();
                        events = events.Where(t => t.UserId == globalUser.UserId || t.OwnerUserId == filters.UserId || t.OwnerUserIdGlobal == filters.OwnerUserIdGlobal || invitedEvents.Contains(t.ActivityId));
                    }
                }
                if (filters.ContactId > 0)
                {
                    events = (from i in events
                              join j in sharedContext.ActivititesMembers on i.ActivityId equals j.ActivitiesId
                              where j.ContactId == filters.ContactId
                              select i);
                }

                if (filters.CompanyIdGlobal > 0) events = (from t in events where t.CompanyIdGlobal == filters.CompanyIdGlobal select t);
                if (filters.DealId > 0)
                {
                    var dealIdStr = filters.DealId.ToString();
                    events = events.Where(t => t.DealIds == dealIdStr ||
                        t.DealIds.Contains("," + dealIdStr + ",") ||
                        t.DealIds.StartsWith(dealIdStr + ",") ||
                        t.DealIds.EndsWith("," + dealIdStr));
                }

                if (filters.MatchActive) events = events.Where(i => i.Completed == false);
                if (filters.DateFrom != null) events = events.Where(t => t.StartDateTime >= filters.DateFrom.Value);
                if (filters.DateTo != null) events = events.Where(t => t.EndDateTime == null || t.EndDateTime <= filters.DateTo.Value);
                if (filters.getNextLast) {
                    events = events.Where(i => (i.ActivityType.ToLower().Contains("event") || i.ActivityType.ToLower().Contains("task") || i.ActivityType.ToLower().Contains("note")));
                    events = events.OrderByDescending(i => i.CreatedDate).Select(i => i);
                    response.LastActivity = events.FirstOrDefault();
                    response.NextActivity = events.Where(i => !i.ActivityType.ToLower().Contains("note") && i.ActivityDate >= DateTime.Now)
                        .OrderBy(t => t.ActivityDate)
                        .FirstOrDefault();
                }

                //sort activites
                if (!string.IsNullOrEmpty(filters.SortBy))
                {
                    switch (filters.SortBy)
                    {
                        case "createddate asc":
                            events = events.OrderBy(t => t.CreatedDate);
                            break;
                        case "createddate desc":
                            events = events.OrderByDescending(t => t.CreatedDate);
                            break;
                        case "eventdate asc":
                            events = events.OrderBy(t => t.StartDateTime);
                            break;
                        case "eventdate desc":
                            events = events.OrderByDescending(t => t.StartDateTime);
                            break;
                        default:
                            events = events.OrderBy(t => t.CreatedDate);
                            break;
                    }
                }

                // paging
                if (filters.RecordsPerPage > 0 && filters.CurrentPage > 0)
                    events = events.Skip(filters.RecordsPerPage * (filters.CurrentPage - 1)).Take(filters.RecordsPerPage);
                // categories
                var categories = context.EventCategories.Where(t => t.SubscriberId == filters.SubscriberId).ToList();

                // get activities list
                var eventList = events.ToList();
                foreach (var eCalendarEvent in eventList)
                {
                    if (eCalendarEvent.StartDateTime.HasValue)
                    {
                        if (cstZone != null)
                            eCalendarEvent.StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(eCalendarEvent.StartDateTime.Value, cstZone);
                        else
                            eCalendarEvent.StartDateTime = ConvertUtcToUserDateTime(eCalendarEvent.StartDateTime.Value, filters.UserId, userTimeZone, utcOffsetDefault);
                    }
                    if (!eCalendarEvent.IsAllDay && eCalendarEvent.EndDateTime.HasValue)
                    {
                        if (cstZone != null)
                            eCalendarEvent.EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(eCalendarEvent.EndDateTime.Value, cstZone);
                        else
                            eCalendarEvent.EndDateTime = ConvertUtcToUserDateTime(eCalendarEvent.EndDateTime.Value, filters.UserId, userTimeZone, utcOffsetDefault);
                    }
                    // set category color
                    if (categories != null)
                    {
                        var category = categories.FirstOrDefault(t => t.CategoryName == eCalendarEvent.CategoryName);
                        if (category != null)
                        {
                            eCalendarEvent.CategoryColor = category.CategoryColor;
                        }
                    }
                }
                response.Activites = eventList;
            }
            return response;
        }


        public List<ActivititesMember> AddEditActivityMembers(Activity activity, List<ActivititesMember> contacts)
        {
            try
            {
                var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                    .GlobalSubscribers.Where(t => t.SubscriberId == activity.SubscriberId)
                    .Select(t => t.DataCenter).FirstOrDefault();
                var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
                var sharedContext = new DbSharedDataContext(sharedConnection);

                // get current invites
                var invites = sharedContext.ActivititesMembers.Where(i => i.ActivitiesId == activity.ActivityId && !i.Deleted).ToList();
                // delete all the removed invites
                foreach (var invite in invites)
                {
                    ActivititesMember found = null;
                    if (invite.ContactId > 0)
                    {
                        found = contacts.FirstOrDefault(i => i.ContactId == invite.ContactId && i.ContactSubscriberId == invite.ContactSubscriberId && !i.Deleted);
                    }
                    // if not found - invite has been deleted
                    if (found == null)
                    {
                        invite.Deleted = true;
                        invite.DeletedDate = DateTime.Now;
                        invite.DeletedUserId = activity.UpdateUserId;
                        invite.DeletedUserName = activity.UpdateUserName;
                        sharedContext.SubmitChanges();
                    }
                }

                if (contacts != null)
                {
                    // add new invites
                    foreach (var contact in contacts)
                    {
                        ActivititesMember invite = null;
                        if (contact.ContactId > 0)
                        {
                            invite = sharedContext.ActivititesMembers.FirstOrDefault(t => t.ActivitiesId == activity.ActivityId && t.ContactId == contact.ContactId && t.ContactSubscriberId == contact.ContactSubscriberId && !t.Deleted);
                        }

                        if (invite == null)
                        {
                            invite = new ActivititesMember
                            {
                                ActivitiesId = activity.ActivityId,
                                InviteType = contact.InviteType,
                                ContactId = contact.ContactId,
                                ContactName = contact.ContactName,
                                ContactSubscriberId = contact.ContactId > 0 ? contact.SubscriberId : 0,
                                SubscriberId = activity.SubscriberId,
                                CreatedUserId = activity.UpdateUserId,
                                CreatedDate = DateTime.UtcNow,
                                CreatedUserName = activity.UpdateUserName,
                                LastUpdate = DateTime.UtcNow,
                                UpdateUserId = activity.UpdateUserId,
                                UpdateUserName = activity.UpdateUserName,
                                AttendeeType = contact.AttendeeType,
                                UserIdGlobal = activity.UserIdGlobal,
                                UserName = activity.UserName,
                                UserId = activity.UserId,
                                
                            };
                            sharedContext.ActivititesMembers.InsertOnSubmit(invite);
                        }
                        else
                        {
                            invite.LastUpdate = DateTime.UtcNow;
                            invite.UpdateUserId = activity.UpdateUserId;
                            invite.UpdateUserName = activity.UpdateUserName;
                            invite.AttendeeType = contact.AttendeeType;
                        }
                        sharedContext.SubmitChanges();
                    }
                }
            }
            catch (Exception)
            {
            }
            return contacts;
        }

    }
}