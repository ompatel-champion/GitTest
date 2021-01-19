using Crm6.App_Code;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code.Helpers;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using Crm6.App_Code.Shared;
using Crm6.App_Code.Login;
using Crm6.App_Code.Sync;

namespace Helpers
{
    public class CalendarEvents : IActivity
    {

        public List<Activity> GetCalendarEvents(CalendarEventFilter filters, bool onlyOne = false)
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

                var events = sharedContext.Activities.Where(t => !t.Deleted && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")).Select(t => t);

                // apply filters
                if (filters.SubscriberId > 0)
                {
                    // get linked subscriber notes as it always filters by the global company id afterwards
                    var linkedSubscribers = sharedContext.LinkGlobalSuscriberToSubscribers
                                                       .Where(s => s.GlobalSubscriberId == filters.SubscriberId && s.DataCenter != "")
                                                       .Select(s => s.LinkedSubscriberId)
                                                       .ToList();
                    events = events.Where(a => linkedSubscribers.Contains(a.SubscriberId));
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
                        events = events.Where(t => t.UserId == globalUser.UserId || t.OwnerUserId == filters.UserId || t.OwnerUserIdGlobal == filters.OwnerUserIdGlobal || invitedEvents.Contains(t.ActivityId));
                    }
                    else
                    {
                        return new List<Activity>();
                    }
                }

                if (filters.DateFrom != null)
                    events = events.Where(t => t.StartDateTime >= filters.DateFrom.Value);

                if (filters.DateTo != null)
                    events = events.Where(t => t.EndDateTime == null || t.EndDateTime <= filters.DateTo.Value);

                if (filters.DealId > 0)
                    events = events.Where(t => t.DealIds == filters.DealId.ToString() || t.DealIds.Contains(filters.DealId.ToString() + ",") || t.DealIds.Contains("," + filters.DealId.ToString()));

                if (filters.ContactId > 0)
                {
                    events = (from t in events
                              join j in sharedContext.ActivititesMembers on t.ActivityId equals j.ActivitiesId
                              where j.ContactId == filters.ContactId && (t.ActivityType == "EVENT" || t.CalendarEventId > 0)
                              select t);
                }

                if (filters.CompanyIdGlobal > 0)
                {
                    events = (from t in events where t.CompanyIdGlobal == filters.CompanyIdGlobal select t);
                }

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
                return eventList;
            }

            return new List<Activity>();
        }


        public List<ActivityExtended> GetCompanyCalendarEvents(CalendarEventFilter filters)
        {
            var response = new List<ActivityExtended>();
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            // get global company
            var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == filters.CompanyIdGlobal);
            if (globalCompany != null)
            {
                // get global company linked subscribers
                var linkedSubscriberIds = new Subscribers().GetLinkedSubscriberIds(globalCompany.SubscriberId);
                var listExtendedEvents = new List<ActivityExtended>();

                var events = (from t in sharedContext.Activities
                              where t.CompanyIdGlobal == globalCompany.GlobalCompanyId
                                    && linkedSubscriberIds.Contains(t.SubscriberId)
                                    && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                                    && !t.Deleted
                              select t).ToList();

                if (events.Count > 0)
                {
                    foreach (var currentEvent in events)
                    {
                        var extendedEvent = new ActivityExtended
                        {
                            ActivityId = currentEvent.ActivityId,
                            BusyFree = currentEvent.BusyFree,
                            CalendarEventId = currentEvent.CalendarEventId,
                            CategoryId = currentEvent.CategoryId,
                            CategoryName = currentEvent.CategoryName,
                            CompanySubscriberId = currentEvent.CompanySubscriberId,
                            CompanyId = currentEvent.CompanyId,
                            CompanyIdGlobal = currentEvent.CompanyIdGlobal,
                            CompanyName = currentEvent.CompanyName,
                            CreatedDate = currentEvent.CreatedDate,
                            CreatedUserId = currentEvent.CreatedUserId,
                            CreatedUserName = currentEvent.CreatedUserName,
                            DealIds = currentEvent.DealIds,
                            DealNames = currentEvent.DealNames,
                            Description = currentEvent.Description,
                            Duration = currentEvent.Duration,
                            EndDateTime = currentEvent.EndDateTime,
                            EventTimeZone = currentEvent.EventTimeZone,
                            EventType = currentEvent.EventType,
                            IsAllDay = currentEvent.IsAllDay,
                            IsRecurring = currentEvent.IsRecurring,
                            LastUpdate = currentEvent.LastUpdate,
                            Location = currentEvent.Location,
                            OriginSystem = currentEvent.OriginSystem,
                            OwnerUserId = currentEvent.OwnerUserId,
                            Priority = currentEvent.Priority,
                            PublicPrivate = currentEvent.PublicPrivate,
                            SalesPurpose = currentEvent.SalesPurpose,
                            SalesStage = currentEvent.SalesStage,
                            SourceDataCenter = currentEvent.SourceDataCenter,
                            SourceDataCenterCalendarEventId = currentEvent.SourceDataCenterCalendarEventId,
                            SourceSubscriberId = currentEvent.SourceSubscriberId,
                            Subject = currentEvent.Subject,
                            SubscriberId = currentEvent.SubscriberId,
                            StartDateTime = currentEvent.StartDateTime,
                            Tags = currentEvent.Tags,
                            UpdateUserId = currentEvent.UpdateUserId,
                            UpdateUserName = currentEvent.UpdateUserName,
                            UserTimeZone = currentEvent.UserTimeZone,
                            UtcOffset = currentEvent.UtcOffset
                        };

                        if (extendedEvent.StartDateTime.HasValue)
                            extendedEvent.StartDateTime = new Timezones().ConvertUtcToUserDateTime(extendedEvent.StartDateTime.Value, filters.UserId);

                        var invites = new Helpers.CalendarEvents().GetCalendarEventInvites(extendedEvent.ActivityId, filters.SubscriberId);

                        if (invites.Any())
                        {
                            var listInviteNames = invites.Where(t => t.UserName != null && t.UserName != "").Select(person => person.UserName).ToList();
                            listInviteNames.AddRange(invites.Where(t => t.ContactName != null && t.ContactName != "").Select(person => person.ContactName).ToList());
                            listInviteNames.AddRange(invites.Where(t => t.Email != null && t.Email != "" && t.InviteType == "external").Select(person => person.Email).ToList());
                            extendedEvent.Invites = string.Join("<br>", listInviteNames);
                        }

                        if (currentEvent.EndDateTime.HasValue)
                            extendedEvent.EventStartEndTime = currentEvent.StartDateTime.Value.ToString("HH:mm") + " - " +
                                                              currentEvent.EndDateTime.Value.ToString("HH:mm") + " " +
                                                              currentEvent.EventTimeZone;
                        else
                            extendedEvent.EventStartEndTime = currentEvent.StartDateTime.Value.ToString("HH:mm") +
                                                              currentEvent.EventTimeZone;

                        listExtendedEvents.Add(extendedEvent);
                    }
                }

                return listExtendedEvents.OrderByDescending(t => t.StartDateTime).ToList();

            }
            return new List<ActivityExtended>();
        }


        public List<ActivityExtended> GetContactCalendarEvents(CalendarEventFilter filters)
        {
            var response = new List<Activity>();
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var events = (from t in sharedContext.Activities
                          join j in sharedContext.ActivititesMembers on t.ActivityId equals j.ActivitiesId
                          where j.ContactId == filters.ContactId
                              && t.SubscriberId == filters.SubscriberId
                              && !t.Deleted && !j.Deleted
                              && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                          select t).ToList();

            var listExtendedEvents = new List<ActivityExtended>();
            if (events.Count > 0)
            {
                foreach (var currentEvent in events)
                {
                    var extendedEvent = new ActivityExtended
                    {
                        ActivityId = currentEvent.ActivityId,
                        BusyFree = currentEvent.BusyFree,
                        CalendarEventId = currentEvent.CalendarEventId,
                        CategoryId = currentEvent.CategoryId,
                        CategoryName = currentEvent.CategoryName,
                        CompanySubscriberId = currentEvent.CompanySubscriberId,
                        CompanyId = currentEvent.CompanyId,
                        CompanyIdGlobal = currentEvent.CompanyIdGlobal,
                        CompanyName = currentEvent.CompanyName,
                        CreatedDate = currentEvent.CreatedDate,
                        CreatedUserId = currentEvent.CreatedUserId,
                        CreatedUserName = currentEvent.CreatedUserName,
                        DealIds = currentEvent.DealIds,
                        DealNames = currentEvent.DealNames,
                        Description = currentEvent.Description,
                        Duration = currentEvent.Duration,
                        EndDateTime = currentEvent.EndDateTime,
                        EventTimeZone = currentEvent.EventTimeZone,
                        EventType = currentEvent.EventType,
                        IsAllDay = currentEvent.IsAllDay,
                        IsRecurring = currentEvent.IsRecurring,
                        LastUpdate = currentEvent.LastUpdate,
                        Location = currentEvent.Location,
                        OriginSystem = currentEvent.OriginSystem,
                        OwnerUserId = currentEvent.OwnerUserId,
                        Priority = currentEvent.Priority,
                        PublicPrivate = currentEvent.PublicPrivate,
                        SalesPurpose = currentEvent.SalesPurpose,
                        SalesStage = currentEvent.SalesStage,
                        SourceDataCenter = currentEvent.SourceDataCenter,
                        SourceDataCenterCalendarEventId = currentEvent.SourceDataCenterCalendarEventId,
                        SourceSubscriberId = currentEvent.SourceSubscriberId,
                        Subject = currentEvent.Subject,
                        SubscriberId = currentEvent.SubscriberId,
                        StartDateTime = currentEvent.StartDateTime,
                        Tags = currentEvent.Tags,
                        UpdateUserId = currentEvent.UpdateUserId,
                        UpdateUserName = currentEvent.UpdateUserName,
                        UserTimeZone = currentEvent.UserTimeZone,
                        UtcOffset = currentEvent.UtcOffset
                    };

                    if (extendedEvent.StartDateTime.HasValue)
                        extendedEvent.StartDateTime = new Timezones().ConvertUtcToUserDateTime(extendedEvent.StartDateTime.Value, filters.UserId);

                    var invites = new Helpers.CalendarEvents().GetCalendarEventInvites(extendedEvent.ActivityId, filters.SubscriberId);

                    if (invites.Any())
                    {
                        var listInviteNames = invites.Where(t => t.UserName != null && t.UserName != "").Select(person => person.UserName).ToList();
                        listInviteNames.AddRange(invites.Where(t => t.ContactName != null && t.ContactName != "").Select(person => person.ContactName).ToList());
                        listInviteNames.AddRange(invites.Where(t => t.Email != null && t.Email != "" && t.InviteType == "external").Select(person => person.Email).ToList());
                        extendedEvent.Invites = string.Join("<br>", listInviteNames);
                    }

                    if (currentEvent.EndDateTime.HasValue)
                        extendedEvent.EventStartEndTime = currentEvent.StartDateTime.Value.ToString("HH:mm") + " - " +
                                                          currentEvent.EndDateTime.Value.ToString("HH:mm") + " " +
                                                          currentEvent.EventTimeZone;
                    else
                        extendedEvent.EventStartEndTime = currentEvent.StartDateTime.Value.ToString("HH:mm") +
                                                          currentEvent.EventTimeZone;

                    listExtendedEvents.Add(extendedEvent);
                }
            }
            return listExtendedEvents.OrderByDescending(t => t.StartDateTime).ToList();
        }


        public List<ActivityExtended> GetDealEvents(int dealId, int userId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                             .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                             .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var events = (from t in sharedContext.Activities
                          where (t.DealIds == dealId.ToString() || t.DealIds.Contains(dealId.ToString() + ",") || t.DealIds.Contains("," + dealId.ToString()))
                            && t.SubscriberId == subscriberId
                            && !t.Deleted
                            && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                          select t).ToList();

            var listExtendedEvents = new List<ActivityExtended>();

            if (events.Count > 0)
            {
                foreach (var currentEvent in events)
                {
                    var extendedEvent = new ActivityExtended
                    {
                        ActivityId = currentEvent.ActivityId,
                        BusyFree = currentEvent.BusyFree,
                        CalendarEventId = currentEvent.CalendarEventId,
                        CategoryId = currentEvent.CategoryId,
                        CategoryName = currentEvent.CategoryName,
                        CompanySubscriberId = currentEvent.CompanySubscriberId,
                        CompanyId = currentEvent.CompanyId,
                        CompanyIdGlobal = currentEvent.CompanyIdGlobal,
                        CompanyName = currentEvent.CompanyName,
                        CreatedDate = currentEvent.CreatedDate,
                        CreatedUserId = currentEvent.CreatedUserId,
                        CreatedUserName = currentEvent.CreatedUserName,
                        DealIds = currentEvent.DealIds,
                        DealNames = currentEvent.DealNames,
                        Description = currentEvent.Description,
                        Duration = currentEvent.Duration,
                        EndDateTime = currentEvent.EndDateTime,
                        EventTimeZone = currentEvent.EventTimeZone,
                        EventType = currentEvent.EventType,
                        IsAllDay = currentEvent.IsAllDay,
                        IsRecurring = currentEvent.IsRecurring,
                        LastUpdate = currentEvent.LastUpdate,
                        Location = currentEvent.Location,
                        OriginSystem = currentEvent.OriginSystem,
                        OwnerUserId = currentEvent.OwnerUserId,
                        Priority = currentEvent.Priority,
                        PublicPrivate = currentEvent.PublicPrivate,
                        SalesPurpose = currentEvent.SalesPurpose,
                        SalesStage = currentEvent.SalesStage,
                        SourceDataCenter = currentEvent.SourceDataCenter,
                        SourceDataCenterCalendarEventId = currentEvent.SourceDataCenterCalendarEventId,
                        SourceSubscriberId = currentEvent.SourceSubscriberId,
                        Subject = currentEvent.Subject,
                        SubscriberId = currentEvent.SubscriberId,
                        StartDateTime = currentEvent.StartDateTime,
                        Tags = currentEvent.Tags,
                        UpdateUserId = currentEvent.UpdateUserId,
                        UpdateUserName = currentEvent.UpdateUserName,
                        UserTimeZone = currentEvent.UserTimeZone,
                        UtcOffset = currentEvent.UtcOffset
                    };

                    if (extendedEvent.StartDateTime.HasValue)
                        extendedEvent.StartDateTime = new Timezones().ConvertUtcToUserDateTime(extendedEvent.StartDateTime.Value, userId);

                    var invites = new Helpers.CalendarEvents().GetCalendarEventInvites(extendedEvent.ActivityId, subscriberId);

                    if (invites.Any())
                    {
                        var listInviteNames = invites.Where(t => t.UserName != null && t.UserName != "").Select(person => person.UserName).ToList();
                        listInviteNames.AddRange(invites.Where(t => t.ContactName != null && t.ContactName != "").Select(person => person.ContactName).ToList());
                        listInviteNames.AddRange(invites.Where(t => t.Email != null && t.Email != "" && t.InviteType == "external").Select(person => person.Email).ToList());
                        extendedEvent.Invites = string.Join("<br>", listInviteNames);
                    }

                    if (currentEvent.EndDateTime.HasValue)
                        extendedEvent.EventStartEndTime = currentEvent.StartDateTime.Value.ToString("HH:mm") + " - " +
                                                          currentEvent.EndDateTime.Value.ToString("HH:mm") + " " +
                                                          currentEvent.EventTimeZone;
                    else if (currentEvent.StartDateTime.HasValue)
                        extendedEvent.EventStartEndTime = currentEvent.StartDateTime.Value.ToString("HH:mm") +
                                                          currentEvent.EventTimeZone;

                    listExtendedEvents.Add(extendedEvent);
                }
            }
            return listExtendedEvents.OrderByDescending(t => t.StartDateTime).ToList(); ;
        }


        public List<ActivityExtended> GetUserEventsForActivities(int userId, int subscriberId)
        {
            // return 10 events ordered by date
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                             .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                             .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new DbLoginDataContext(loginConnection);

            var listExtendedEvents = new List<ActivityExtended>();
            var filterDate = DateTime.UtcNow.AddDays(-1);
            var currentUserDateTime = new Timezones().ConvertUtcToUserDateTime(DateTime.Now, userId);

            var currentUser = context.Users.Where(t => t.UserId == userId).FirstOrDefault();
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

                var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == userId && u.SubscriberId == subscriberId);
                if (globalUser != null)
                {
                    // get invited event for the user id
                    var invitedEvents = sharedContext.ActivititesMembers.Where(t => t.UserIdGlobal == globalUser.GlobalUserId && !t.Deleted).Select(t => t.ActivitiesId).Distinct().ToList();

                    // TODO: filter for tasks and/or events
                    var events = (from t in sharedContext.Activities
                                  where t.SubscriberId == subscriberId && !t.Deleted
                                    && (t.OwnerUserId == userId || invitedEvents.Contains(t.ActivityId))
                                    && t.StartDateTime >= filterDate
                                    && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                                  select t).OrderBy(t => t.StartDateTime).Take(15).ToList();

                    if (events.Count > 0)
                    {
                        foreach (var currentEvent in events)
                        {
                            var extendedEvent = new ActivityExtended
                            {

                                ActivityId = currentEvent.ActivityId,
                                BusyFree = currentEvent.BusyFree,
                                CalendarEventId = currentEvent.CalendarEventId,
                                CategoryId = currentEvent.CategoryId,
                                CategoryName = currentEvent.CategoryName,
                                CompanySubscriberId = currentEvent.CompanySubscriberId,
                                CompanyId = currentEvent.CompanyId,
                                CompanyIdGlobal = currentEvent.CompanyIdGlobal,
                                CompanyName = currentEvent.CompanyName,
                                CreatedDate = currentEvent.CreatedDate,
                                CreatedUserId = currentEvent.CreatedUserId,
                                CreatedUserName = currentEvent.CreatedUserName,
                                DealIds = currentEvent.DealIds,
                                DealNames = currentEvent.DealNames,
                                Description = currentEvent.Description,
                                Duration = currentEvent.Duration,
                                EndDateTime = currentEvent.EndDateTime,
                                EventTimeZone = currentEvent.EventTimeZone,
                                EventType = currentEvent.EventType,
                                IsAllDay = currentEvent.IsAllDay,
                                IsRecurring = currentEvent.IsRecurring,
                                LastUpdate = currentEvent.LastUpdate,
                                Location = currentEvent.Location,
                                OriginSystem = currentEvent.OriginSystem,
                                OwnerUserId = currentEvent.OwnerUserId,
                                Priority = currentEvent.Priority,
                                PublicPrivate = currentEvent.PublicPrivate,
                                SalesPurpose = currentEvent.SalesPurpose,
                                SalesStage = currentEvent.SalesStage,
                                SourceDataCenter = currentEvent.SourceDataCenter,
                                SourceDataCenterCalendarEventId = currentEvent.SourceDataCenterCalendarEventId,
                                SourceSubscriberId = currentEvent.SourceSubscriberId,
                                Subject = currentEvent.Subject,
                                SubscriberId = currentEvent.SubscriberId,
                                StartDateTime = currentEvent.StartDateTime,
                                Tags = currentEvent.Tags,
                                UpdateUserId = currentEvent.UpdateUserId,
                                UpdateUserName = currentEvent.UpdateUserName,
                                UserTimeZone = currentEvent.UserTimeZone,
                                UtcOffset = currentEvent.UtcOffset
                            };

                            if (extendedEvent.StartDateTime.HasValue)
                            {
                                if (cstZone != null)
                                    extendedEvent.StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(extendedEvent.StartDateTime.Value, cstZone);
                                else
                                    extendedEvent.StartDateTime = ConvertUtcToUserDateTime(extendedEvent.StartDateTime.Value, userId, userTimeZone, utcOffsetDefault);
                            }

                            if (extendedEvent.StartDateTime < currentUserDateTime)
                            {
                                continue;
                            }

                            if (!extendedEvent.IsAllDay && extendedEvent.EndDateTime.HasValue)
                            {
                                if (cstZone != null)
                                    extendedEvent.EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(extendedEvent.EndDateTime.Value, cstZone);
                                else
                                    extendedEvent.EndDateTime = ConvertUtcToUserDateTime(extendedEvent.EndDateTime.Value, userId, userTimeZone, utcOffsetDefault);
                            }

                            if (currentEvent.EndDateTime.HasValue)
                                extendedEvent.EventStartEndTime = currentEvent.StartDateTime.Value.ToString("HH:mm") + " - " +
                                                                  currentEvent.EndDateTime.Value.ToString("HH:mm") + " " +
                                                                  currentEvent.EventTimeZone;
                            else
                                extendedEvent.EventStartEndTime = currentEvent.StartDateTime.Value.ToString("HH:mm") +
                                                                  currentEvent.EventTimeZone;

                            var invites = new CalendarEvents().GetCalendarEventInvites(extendedEvent.ActivityId, subscriberId);
                            if (invites.Any())
                            {
                                var listInviteNames = invites.Where(t => t.UserName != null && t.UserName != "").Select(person => person.UserName).ToList();
                                listInviteNames.AddRange(invites.Where(t => t.ContactName != null && t.ContactName != "").Select(person => person.ContactName).ToList());
                                listInviteNames.AddRange(invites.Where(t => t.Email != null && t.Email != "" && t.InviteType == "external").Select(person => person.Email).ToList());
                                extendedEvent.Invites = string.Join("<br>", listInviteNames);
                            }

                            listExtendedEvents.Add(extendedEvent);
                            if (listExtendedEvents.Count == 10)
                            {
                                return listExtendedEvents;
                            }
                        }
                    }
                }
            }
            return listExtendedEvents;
        }


        public ActivityModel GetCalendarEvent(int activityId, int userId, int subscriberId = 0)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                            .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                            .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var calendarEventItem = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == activityId);
            if (calendarEventItem != null)
            {
                var eventModel = new ActivityModel
                {
                    CalendarEvent = calendarEventItem,
                    Deals = new List<Deal>(),
                    Documents = new Documents().GetDocumentsByDocType(8, activityId, subscriberId),
                    Invites = GetCalendarEventInvites(activityId, subscriberId)
                };

                var dealIds = (calendarEventItem.DealIds + "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var dealId in dealIds)
                {
                    var deal = new Deals().GetDeal(int.Parse(dealId), calendarEventItem.CompanySubscriberId);
                    if (deal != null)
                    {
                        eventModel.Deals.Add(deal);
                    }
                }

                if (eventModel.CalendarEvent.StartDateTime.HasValue)
                    eventModel.CalendarEvent.StartDateTime = new Timezones().ConvertUtcToUserDateTime(eventModel.CalendarEvent.StartDateTime.Value, userId);

                if (!eventModel.CalendarEvent.IsAllDay && eventModel.CalendarEvent.EndDateTime.HasValue)
                {
                    eventModel.CalendarEvent.EndDateTime = new Timezones().ConvertUtcToUserDateTime(eventModel.CalendarEvent.EndDateTime.Value, userId);
                }
                return eventModel;
            }
            return null;
        }


        public int SaveCalendarEvent(ActivityModel calendarEventItem)
        {
            var sharedContext = new DbSharedDataContext(LoginUser.GetWritableSharedConnectionForSubscriberId(calendarEventItem.CalendarEvent.SubscriberId));
            var loginContext = new DbLoginDataContext(LoginUser.GetLoginConnection());
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var eventCampaigns = new List<string>();

            var fEvent = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == calendarEventItem.CalendarEvent.ActivityId) ?? new Activity();

            fEvent.Description = calendarEventItem.CalendarEvent.Description;
            fEvent.IsAllDay = calendarEventItem.CalendarEvent.IsAllDay;
            fEvent.LastUpdate = DateTime.UtcNow;
            fEvent.Location = calendarEventItem.CalendarEvent.Location;
            fEvent.Subject = calendarEventItem.CalendarEvent.Subject;
            fEvent.UpdateUserId = calendarEventItem.CalendarEvent.UpdateUserId;
            fEvent.UpdateUserIdGlobal = calendarEventItem.CalendarEvent.UpdateUserIdGlobal;
            fEvent.IsRecurring = calendarEventItem.CalendarEvent.IsRecurring;
            fEvent.ReoccurrenceIncrementType = calendarEventItem.CalendarEvent.ReoccurrenceIncrementType;

            var amountOfEvents = 0;
            var recurringType = RecurringType.NONE;
            bool isRecurring = calendarEventItem.CalendarEvent.IsRecurring ?? false;

            if (string.IsNullOrWhiteSpace(fEvent.ReoccurrenceIncrementType) == false && isRecurring)
            {
                if (fEvent.ReoccurrenceIncrementType.Equals("Weekly", StringComparison.InvariantCultureIgnoreCase))
                {
                    recurringType = RecurringType.WEEKLY;
                    amountOfEvents = RecurringEventProperties.WeeklyNumberOfCalendarEvents;
                }
                else if (fEvent.ReoccurrenceIncrementType.Equals("Monthly", StringComparison.InvariantCultureIgnoreCase))
                {
                    recurringType = RecurringType.MONTHLY;
                    amountOfEvents = RecurringEventProperties.MonthlyNumberOfCalendarEvents;
                }
                else
                {
                    recurringType = RecurringType.DAILY;
                    amountOfEvents = RecurringEventProperties.DailyNumberOfCalendarEvents;
                }
            }

            var username = context.Users.Where(u => u.UserId == calendarEventItem.CalendarEvent.UpdateUserId).Select(u => u.FullName).FirstOrDefault() ?? "";
            fEvent.UpdateUserName = username;

            // owner user
            fEvent.OwnerUserId = calendarEventItem.CalendarEvent.OwnerUserId;
            if (calendarEventItem.CalendarEvent.OwnerUserIdGlobal > 0)
            {
                var user = loginContext.GlobalUsers.FirstOrDefault(u => u.GlobalUserId == fEvent.OwnerUserIdGlobal);
                if (user != null)
                {
                    fEvent.OwnerUserName = user.FullName;
                    fEvent.UserLocation = user.LocationName;
                }
            }

            var companySubscriberId = 0;
            // company details
            if (calendarEventItem.CalendarEvent.CompanyIdGlobal > 0)
            {
                fEvent.CompanyIdGlobal = calendarEventItem.CalendarEvent.CompanyIdGlobal;
                // get global company
                var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == fEvent.CompanyIdGlobal);

                if (globalCompany != null)
                {
                    companySubscriberId = globalCompany.SubscriberId;
                    fEvent.CompanyId = globalCompany.CompanyId;
                    fEvent.CompanyName = globalCompany.CompanyName;
                    fEvent.CompanySubscriberId = companySubscriberId;
                    var company = context.Companies.FirstOrDefault(t => t.CompanyIdGlobal == fEvent.CompanyIdGlobal);
                    if (company != null && !string.IsNullOrWhiteSpace(company.CampaignName))
                    {
                        // company campaigns
                        var campaigns = company.CampaignName.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (campaigns.Count > 0)
                        {
                            eventCampaigns.AddRange(campaigns);
                        }
                    }
                }
            }
            else
            {
                fEvent.CompanyIdGlobal = 0;
                fEvent.CompanyId = 0;
                fEvent.CompanyName = "";
            }

            // Timezone conversion to UTC
            if (calendarEventItem.CalendarEvent.StartDateTime.HasValue)
            {
                fEvent.StartDateTime = new Timezones().ConvertUserDateTimeToUtc(calendarEventItem.CalendarEvent.StartDateTime.Value, calendarEventItem.CalendarEvent.UpdateUserId, calendarEventItem.CalendarEvent.SubscriberId);
               // fEvent.StartDateTime = calendarEventItem.CalendarEvent.StartDateTime.Value;
                fEvent.ActivityDate = fEvent.StartDateTime.Value;

            }
            if (!fEvent.IsAllDay && calendarEventItem.CalendarEvent.EndDateTime.HasValue)
            {
                //fEvent.EndDateTime = calendarEventItem.CalendarEvent.EndDateTime.Value;
                fEvent.EndDateTime = new Timezones().ConvertUserDateTimeToUtc(calendarEventItem.CalendarEvent.EndDateTime.Value, calendarEventItem.CalendarEvent.UpdateUserId, calendarEventItem.CalendarEvent.SubscriberId);
            }
            else
            {
                fEvent.Duration = 0;
                fEvent.EndDateTime = null;
            }

            fEvent.BusyFree = calendarEventItem.CalendarEvent.BusyFree;
            fEvent.Campaigns = string.Join(",", eventCampaigns.Distinct().ToList());
            fEvent.CategoryId = calendarEventItem.CalendarEvent.CategoryId;
            fEvent.CategoryName = calendarEventItem.CalendarEvent.CategoryName;
            fEvent.EventType = calendarEventItem.CalendarEvent.EventType ?? "Meeting";
            fEvent.PublicPrivate = calendarEventItem.CalendarEvent.PublicPrivate;
            fEvent.ReminderMinutes = calendarEventItem.CalendarEvent.ReminderMinutes;

            var listActivities = new List<Activity>();

            // insert new calendar event
            if (fEvent.ActivityId < 1)
            {
                fEvent.SubscriberId = calendarEventItem.CalendarEvent.SubscriberId;
                fEvent.CreatedDate = fEvent.LastUpdate;
                fEvent.CreatedUserId = calendarEventItem.CalendarEvent.UpdateUserId;
                fEvent.CreatedUserName = username;
                fEvent.UserTimeZone = new Users().GetUserTimeZone(calendarEventItem.CalendarEvent.UpdateUserId, fEvent.SubscriberId);
                fEvent.EventTimeZone = fEvent.UserTimeZone;
                fEvent.UtcOffset = "";
                fEvent.ActivityType = "EVENT";
                fEvent.OwnerUserId = calendarEventItem.CalendarEvent.OwnerUserId;
                fEvent.OwnerUserIdGlobal = calendarEventItem.CalendarEvent.OwnerUserIdGlobal;
                fEvent.OwnerUserName = username;
                fEvent.SavedAsActivity = true;
                fEvent.TaskId = 0;
                fEvent.Completed = false;
                fEvent.CompletionPercent = 0;
                fEvent.DueDate = fEvent.EndDateTime;
                fEvent.StartDate = fEvent.StartDateTime;
                fEvent.TaskName = "";
                fEvent.NoteId = 0;
                fEvent.NoteContent = "";
                sharedContext.Activities.InsertOnSubmit(fEvent);
                sharedContext.SubmitChanges();

                fEvent.ReoccurrenceParentActivityId = fEvent.ActivityId;

                listActivities.Add(fEvent);

                if (isRecurring)
                {
                    //Starts from 1 since the first event was already added.
                    for (int i = 1; i < amountOfEvents; i++)
                    {
                        var newEvent = new Activity()
                        {
                            ActivityId = fEvent.ActivityId,
                            SubscriberId = fEvent.SubscriberId,
                            ActivityDate = fEvent.ActivityDate,
                            AdminActiveOveride = fEvent.AdminActiveOveride,
                            AdminActiveOverideDate = fEvent.AdminActiveOverideDate,
                            ActivityType = fEvent.ActivityType,
                            BusyFree = fEvent.BusyFree,
                            CalendarEventId = fEvent.CalendarEventId,
                            Campaigns = fEvent.Campaigns,
                            CategoryId = fEvent.CategoryId,
                            CategoryColor = fEvent.CategoryColor,
                            CategoryName = fEvent.CategoryName,
                            CompanyId = fEvent.CompanyId,
                            CompanyIdGlobal = fEvent.CompanyIdGlobal,
                            CompanyName = fEvent.CompanyName,
                            CompanySubscriberId = fEvent.CompanySubscriberId,
                            Competitors = fEvent.Competitors,
                            Completed = fEvent.Completed,
                            CompletionPercent = fEvent.CompletionPercent,
                            ContactIds = fEvent.ContactIds,
                            ContactNames = fEvent.ContactNames,
                            ConversionActivityId = fEvent.ConversionActivityId,
                            ConversionCreatedDateTime = fEvent.ConversionCreatedDateTime,
                            ConversionUpdatedDateTime = fEvent.ConversionUpdatedDateTime,
                            CreatedDate = fEvent.CreatedDate,
                            CreatedUserId = fEvent.CreatedUserId,
                            CreatedUserIdGlobal = fEvent.CreatedUserIdGlobal,
                            CreatedUserName = fEvent.CreatedUserName,
                            CreatedUserLocation = fEvent.CreatedUserLocation,
                            DealIds = fEvent.DealIds,
                            DealNames = fEvent.DealNames,
                            DealTypes = fEvent.DealTypes,
                            Deleted = fEvent.Deleted,
                            DeletedDate = fEvent.DeletedDate,
                            DeletedUserId = fEvent.DeletedUserId,
                            DeletedUserIdGlobal = fEvent.DeletedUserIdGlobal,
                            DeletedUserName = fEvent.DeletedUserName,
                            Description = fEvent.Description,
                            DueDate = fEvent.DueDate,
                            Duration = fEvent.Duration,
                            EndDateTime = fEvent.EndDateTime,
                            EventSubject = fEvent.EventSubject,
                            EventTimeZone = fEvent.EventTimeZone,
                            EventType = fEvent.EventType,
                            ExchangeAppointmentId = fEvent.ExchangeAppointmentId,
                            ExchangeAppointmentItemId = fEvent.ExchangeAppointmentItemId,
                            ExchangeSyncId = fEvent.ExchangeSyncId,
                            ExchangeSyncLastModified = fEvent.ExchangeSyncLastModified,
                            GoogleSyncId = fEvent.GoogleSyncId,
                            GoogleSyncLastModified = fEvent.GoogleSyncLastModified,
                            IsAllDay = fEvent.IsAllDay,
                            IsRecurring = fEvent.IsRecurring,
                            LastUpdate = fEvent.LastUpdate,
                            Location = fEvent.Location,
                            NoteContent = fEvent.NoteContent,
                            NoteId = fEvent.NoteId,
                            OriginSystem = fEvent.OriginSystem,
                            OwnerUserId = fEvent.OwnerUserId,
                            OwnerUserIdGlobal = fEvent.OwnerUserIdGlobal,
                            OwnerUserName = fEvent.OwnerUserName,
                            PublicPrivate = fEvent.PublicPrivate,
                            Priority = fEvent.Priority,
                            Reminder = fEvent.Reminder,
                            ReminderDate = fEvent.ReminderDate,
                            ReminderIncrement = fEvent.ReminderIncrement,
                            ReminderIncrementType = fEvent.ReminderIncrementType,
                            ReminderMinutes = fEvent.ReminderMinutes,
                            ReminderType = fEvent.ReminderType,
                            ReoccurNumberOfTimes = fEvent.ReoccurNumberOfTimes,
                            ReoccurrenceIncrement = fEvent.ReoccurrenceIncrement,
                            ReoccurrenceIncrementType = fEvent.ReoccurrenceIncrementType,
                            ReoccurrenceParentActivityId = fEvent.ReoccurrenceParentActivityId,
                            SalesPurpose = fEvent.SalesPurpose,
                            SalesStage = fEvent.SalesStage,
                            SavedAsActivity = fEvent.SavedAsActivity,
                            SourceDataCenter = fEvent.SourceDataCenter,
                            SourceDataCenterCalendarEventId = fEvent.SourceDataCenterCalendarEventId,
                            SourceDataCenterNoteId = fEvent.SourceDataCenterNoteId,
                            SourceDataCenterTaskId = fEvent.SourceDataCenterTaskId,
                            SourceSubscriberId = fEvent.SourceSubscriberId,
                            StartDate = fEvent.StartDate,
                            StartDateTime = fEvent.StartDateTime,
                            Subject = fEvent.Subject,
                            SyncType = fEvent.SyncType,
                            Tags = fEvent.Tags,
                            TaskId = fEvent.TaskId,
                            TaskName = fEvent.TaskName,
                            UpdateUserId = fEvent.UpdateUserId,
                            UpdateUserIdGlobal = fEvent.UpdateUserIdGlobal,
                            UpdateUserName = fEvent.UpdateUserName,
                            UserId = fEvent.UserId,
                            UserIdGlobal = fEvent.UserIdGlobal,
                            UserLocation = fEvent.UserLocation,
                            UserName = fEvent.UserName,
                            UserTimeZone = fEvent.UserTimeZone,
                            UtcOffset = fEvent.UtcOffset
                        };

                        if (recurringType == RecurringType.DAILY)
                        {
                            newEvent.ActivityDate = newEvent.ActivityDate.AddDays(i);

                            if (newEvent.StartDateTime.HasValue)
                                newEvent.StartDateTime = newEvent.StartDateTime.Value.AddDays(i);

                            if (newEvent.EndDateTime.HasValue)
                                newEvent.EndDateTime = newEvent.EndDateTime.Value.AddDays(i);

                            if (newEvent.DueDate.HasValue)
                                newEvent.DueDate = newEvent.DueDate.Value.AddDays(i);

                            if (newEvent.StartDate.HasValue)
                                newEvent.StartDate = newEvent.StartDate.Value.AddDays(i);
                        }
                        else if (recurringType == RecurringType.MONTHLY)
                        {
                            newEvent.ActivityDate = newEvent.ActivityDate.AddMonths(i);

                            if (newEvent.StartDateTime.HasValue)
                                newEvent.StartDateTime = newEvent.StartDateTime.Value.AddMonths(i);

                            if (newEvent.EndDateTime.HasValue)
                                newEvent.EndDateTime = newEvent.EndDateTime.Value.AddMonths(i);

                            if (newEvent.DueDate.HasValue)
                                newEvent.DueDate = newEvent.DueDate.Value.AddMonths(i);

                            if (newEvent.StartDate.HasValue)
                                newEvent.StartDate = newEvent.StartDate.Value.AddMonths(i);
                        }
                        else if (recurringType == RecurringType.WEEKLY)
                        {
                            newEvent.ActivityDate = newEvent.ActivityDate.AddDays(i * 7);

                            if (newEvent.StartDateTime.HasValue)
                                newEvent.StartDateTime = newEvent.StartDateTime.Value.AddDays(i * 7);

                            if (newEvent.EndDateTime.HasValue)
                                newEvent.EndDateTime = newEvent.EndDateTime.Value.AddDays(i * 7);

                            if (newEvent.DueDate.HasValue)
                                newEvent.DueDate = newEvent.DueDate.Value.AddDays(i * 7);

                            if (newEvent.StartDate.HasValue)
                                newEvent.StartDate = newEvent.StartDate.Value.AddDays(i * 7);
                        }

                        listActivities.Add(newEvent);
                        sharedContext.Activities.InsertOnSubmit(newEvent);
                    }
                }

            }
            sharedContext.SubmitChanges();

            foreach (Activity activity in listActivities)
            {
                var currentEvent = sharedContext.Activities.FirstOrDefault(x => x.ActivityId == activity.ActivityId);

                // add calendar event invited users and external emails 
                var invites = AddCalendarEventInvites(currentEvent, calendarEventItem.Invites, calendarEventItem);
                //set events contact ids and contact names
                if (invites != null)
                {
                    currentEvent.ContactIds = string.Join(",", invites.Where(t => t.ContactId > 0).Select(t => t.ContactId.ToString()));
                    currentEvent.ContactNames = string.Join(",", invites.Where(t => t.ContactId > 0).Select(t => t.ContactName.ToString()));
                }

                sharedContext.SubmitChanges();

                foreach (var invite in invites) new Contacts().UpdateContactLastActivityDate(invite.ContactId ?? default(int));

                // add calendr documents
                if (calendarEventItem.Documents != null)
                {
                    foreach (var doc in calendarEventItem.Documents)
                    {
                        doc.DocumentTypeId = Convert.ToInt32(DocumentTypeEnum.CalendarEvents);
                        doc.CalendarEventId = currentEvent.ActivityId;
                        new Documents().SaveDocument(doc);
                    }
                }

                // deal details
                currentEvent.DealIds = calendarEventItem.CalendarEvent.DealIds;

                // delete current linked deals
                var activityDeals = sharedContext.LinkActivityToDeals.Where(t => t.ActivityId == currentEvent.ActivityId).ToList();
                if (activityDeals.Count > 0)
                {
                    sharedContext.LinkActivityToDeals.DeleteAllOnSubmit(activityDeals);
                    sharedContext.SubmitChanges();
                }

                if (!string.IsNullOrEmpty(currentEvent.DealIds))
                {
                    var dealIds = calendarEventItem.CalendarEvent.DealIds.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    var dealNames = new List<string>();
                    var dealCompetitors = new List<string>();
                    var dealTypes = new List<string>();
                    foreach (var dealId in dealIds)
                    {
                        var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                .GlobalSubscribers.Where(t => t.SubscriberId == companySubscriberId)
                                                                .Select(t => t.DataCenter).FirstOrDefault();
                        var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
                        var dealContext = new DbFirstFreightDataContext(connection);

                        var deal = dealContext.Deals.FirstOrDefault(u => u.DealId == int.Parse(dealId) && u.SubscriberId == companySubscriberId);
                        if (deal != null)
                        {
                            dealNames.Add(deal.DealName);
                            dealCompetitors.Add(deal.Competitors);
                            dealTypes.Add(deal.DealType);

                            if (!string.IsNullOrWhiteSpace(deal.Campaign))
                            {
                                // deal campaigns
                                var campaigns = deal.Campaign.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                if (campaigns.Count > 0)
                                    eventCampaigns.AddRange(campaigns);
                            }

                            if (currentEvent.Subject.Contains("Proposal due for") || currentEvent.Subject.Contains("Decision due for") || currentEvent.Subject.Contains("First Shipment due for") || currentEvent.Subject.Contains("Contract ending for"))
                            {
                                switch (currentEvent.Subject)
                                {
                                    case "Proposal due for": deal.DateProposalDue = currentEvent.StartDateTime; break;
                                    case "Decision due for": deal.DecisionDate = currentEvent.StartDateTime; break;
                                    case "First Shipment due for": deal.EstimatedStartDate = currentEvent.StartDateTime; break;
                                    case "Contract ending for": deal.EstimatedStartDate = currentEvent.StartDateTime; break;
                                    default: break;
                                }
                                deal.LastUpdate = DateTime.UtcNow;
                                deal.UpdateUserId = deal.UpdateUserId;
                                deal.UpdateUserName = currentEvent.UpdateUserName;
                                dealContext.SubmitChanges();
                            }

                            // add activity deals
                            sharedContext.LinkActivityToDeals.InsertOnSubmit(new LinkActivityToDeal
                            {
                                ActivityId = currentEvent.ActivityId,
                                CreatedDate = DateTime.Now,
                                CreatedUserId = calendarEventItem.CalendarEvent.UpdateUserId,
                                CreatedUserName = username,
                                DealId = int.Parse(dealId),
                                DealName = deal.DealName,
                                DealSubscriberId = companySubscriberId,
                                LastUpdate = DateTime.UtcNow,
                                SubscriberId = companySubscriberId,
                                UpdateUserId = calendarEventItem.CalendarEvent.UpdateUserId,
                                UpdateUserName = username
                            });
                            sharedContext.SubmitChanges();
                        }
                    }

                    currentEvent.Campaigns = string.Join(",", eventCampaigns.Distinct().ToList());
                    currentEvent.Competitors = string.Join(",", dealCompetitors);
                    currentEvent.DealNames = string.Join(",", dealNames);
                    currentEvent.DealTypes = string.Join(",", dealTypes);
                }
                sharedContext.SubmitChanges();

                // calendar sync
                // new Sync.SyncInitializer().SyncExchangeForUser(fEvent.UpdateUserId, fEvent.SubscriberId);

                var contactId = 0;
                int.TryParse(currentEvent.ContactIds, out contactId);

                var currentDealId = 0;
                int.TryParse(currentEvent.DealIds, out currentDealId);

                new Logging().LogUserAction(new UserActivity
                {
                    UserId = currentEvent.CreatedUserId,
                    CalendarEventId = currentEvent.CalendarEventId,
                    CompanyId = currentEvent.CompanyId,
                    ContactId = contactId,
                    CalendarEventSubject = currentEvent.Subject,
                    DealId = currentDealId,
                    DealName = currentEvent.DealNames,
                    UserActivityMessage = "Saved Calendar Event | " + currentEvent.Subject
                });

                // intercom Journey Step event
                var eventName = "Created calendar event";
                var intercomeHelper = new IntercomHelper();
                intercomeHelper.IntercomTrackEvent(currentEvent.CreatedUserId, currentEvent.SubscriberId, eventName);

                // update company last activity/ last update date
                if (currentEvent.CompanyId > 0)
                {
                    new Companies().UpdateCompanyLastActivityDate(currentEvent.CompanyId, currentEvent.CompanySubscriberId);
                    new Companies().UpdateCompanyLastUpdateDate(currentEvent.CompanyId, currentEvent.UpdateUserId, currentEvent.CompanySubscriberId);
                }
            }

            return fEvent.ActivityId;
        }


        public List<ActivititesMember> AddCalendarEventInvites(Activity calendarEventItem, List<ActivititesMember> invites, ActivityModel inviteModel)
        {
            try
            {
                var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == calendarEventItem.SubscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
                var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
                var sharedContext = new DbSharedDataContext(sharedConnection);

                // get current event invites
                var currentInvites = sharedContext.ActivititesMembers.Where(t => t.ActivitiesId == calendarEventItem.ActivityId && !t.Deleted).ToList();

                // delete all the removed invites
                foreach (var caInvite in currentInvites)
                {
                    ActivititesMember found = null;
                    if (caInvite.UserIdGlobal > 0)
                    {
                        found = invites.FirstOrDefault(t => t.UserIdGlobal == caInvite.UserIdGlobal && !t.Deleted);
                    }
                    else if (caInvite.ContactId > 0)
                    {
                        found = invites.FirstOrDefault(t => t.ContactId == caInvite.ContactId && t.ContactSubscriberId == caInvite.ContactSubscriberId && !t.Deleted);
                    }
                    else if (!string.IsNullOrEmpty(caInvite.Email))
                    {
                        found = invites.FirstOrDefault(t => t.Email == caInvite.Email && !t.Deleted);
                    }

                    // if not found - invite has been deleted
                    if (found == null)
                    {
                        caInvite.Deleted = true;
                        caInvite.DeletedDate = DateTime.Now;
                        caInvite.DeletedUserId = calendarEventItem.UpdateUserId;
                        caInvite.DeletedUserName = calendarEventItem.UpdateUserName;
                        sharedContext.SubmitChanges();
                        // Notify the attendee saying the attendee removed
                    }
                }

                if (invites != null)
                {
                    // add new invites
                    foreach (var caInvite in invites)
                    {
                        ActivititesMember invite = null;
                        if (caInvite.UserIdGlobal > 0)
                        {
                            invite = sharedContext.ActivititesMembers.FirstOrDefault(t => t.ActivitiesId == calendarEventItem.ActivityId && t.UserIdGlobal == caInvite.UserIdGlobal && !t.Deleted);
                        }
                        else if (caInvite.ContactId > 0)
                        {
                            invite = sharedContext.ActivititesMembers.FirstOrDefault(t => t.ActivitiesId == calendarEventItem.ActivityId && t.ContactId == caInvite.ContactId && t.ContactSubscriberId == caInvite.ContactSubscriberId && !t.Deleted);
                        }
                        else if (!string.IsNullOrEmpty(caInvite.Email))
                        {
                            invite = sharedContext.ActivititesMembers.FirstOrDefault(t => t.ActivitiesId == calendarEventItem.ActivityId && t.Email.ToLower() == caInvite.Email.ToLower() && !t.Deleted);
                        }
                        if (invite == null)
                        {
                            invite = new ActivititesMember
                            {
                                ActivitiesId = calendarEventItem.ActivityId,
                                AttendeeType = caInvite.AttendeeType,
                                ContactId = caInvite.ContactId,
                                ContactName = caInvite.ContactName,
                                ContactSubscriberId = caInvite.ContactId > 0 ? caInvite.SubscriberId : 0,
                                CreatedUserId = calendarEventItem.UpdateUserId,
                                CreatedDate = DateTime.UtcNow,
                                CreatedUserName = calendarEventItem.UpdateUserName,
                                Email = caInvite.Email,
                                InviteType = caInvite.InviteType,
                                LastUpdate = DateTime.UtcNow,
                                SubscriberId = calendarEventItem.SubscriberId,
                                UserId = caInvite.UserId,
                                UserIdGlobal = caInvite.UserIdGlobal,
                                UpdateUserId = calendarEventItem.UpdateUserId,
                                UpdateUserName = calendarEventItem.UpdateUserName,
                                UserName = caInvite.UserName
                            };
                            sharedContext.ActivititesMembers.InsertOnSubmit(invite);
                            sharedContext.SubmitChanges();

                            // update contact and user names
                            caInvite.ContactName = invite.ContactName;
                            caInvite.UserName = invite.UserName;
                        }
                        else
                        {
                            invite.LastUpdate = DateTime.UtcNow;
                            invite.UpdateUserId = calendarEventItem.UpdateUserId;
                            invite.UpdateUserName = calendarEventItem.UpdateUserName;
                            invite.AttendeeType = caInvite.AttendeeType;
                            sharedContext.SubmitChanges();
                        }

                        if (calendarEventItem.OwnerUserIdGlobal != invite.UserIdGlobal && invite.AttendeeType != "Organizer")
                        {
                            // send calendar invites notification - DO NOT SEND NOTIFICATIONS TO OWNER/ORGANIZER
                            if (invite.UserIdGlobal > 0 && inviteModel.NotifyInternalAttendees)
                            {
                                SendEventNotification(calendarEventItem, invite);
                            }
                            else if (inviteModel.NotifyExternalAttendees && (invite.ContactId > 0 || !string.IsNullOrEmpty(invite.Email)))
                            {
                                SendEventNotification(calendarEventItem, invite);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return invites;
        }


        public List<ActivititesMember> GetCalendarEventInvites(int activityId, int subscriberId = 0)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return sharedContext.ActivititesMembers.Where(t => t.ActivitiesId == activityId && !t.Deleted).ToList();
        }


        public bool DeleteCalendarEvent(int activityId, bool deleteRecurring, int userId, int subscriberId = 0)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetWritableSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var fEvent = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == activityId);

            if (fEvent == null)
                return false;

            var listEventsToDelete = new List<Activity> { fEvent };

            if (deleteRecurring)
            {
                listEventsToDelete = sharedContext.Activities.Where(t => t.ReoccurrenceParentActivityId == fEvent.ReoccurrenceParentActivityId)?.ToList();
            }

            foreach (Activity activity in listEventsToDelete)
            {
                if (activity != null)
                {
                    activity.Deleted = true;
                    activity.DeletedUserId = userId;
                    activity.DeletedDate = DateTime.UtcNow;
                    activity.LastUpdate = DateTime.UtcNow;
                    activity.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);

                    var contactId = 0;
                    int.TryParse(activity.ContactIds, out contactId);

                    var dealIds = 0;
                    int.TryParse(activity.DealIds, out dealIds);

                    new Logging().LogUserAction(new UserActivity
                    {
                        UserId = activity.CreatedUserId,
                        CalendarEventId = activity.ActivityId,
                        CompanyId = activity.CompanyId,
                        ContactId = contactId,
                        DealId = dealIds,
                        CalendarEventSubject = activity.Subject,
                        DealName = activity.DealNames,
                        UserActivityMessage = "Deleted Calendar Event | " + activity.Subject
                    });
                }
            }

            sharedContext.SubmitChanges();

            return true;
        }


        public bool UpdateCalendarEventDates(Activity calendarEvent)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == calendarEvent.SubscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var fEvent = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == calendarEvent.ActivityId);
            if (fEvent != null)
            {
                // Timezone conversion to UTC
                if (calendarEvent.StartDateTime.HasValue)
                    fEvent.StartDateTime = new Timezones().ConvertUserDateTimeToUtc(calendarEvent.StartDateTime.Value, calendarEvent.UpdateUserId, calendarEvent.SubscriberId);
                if (!fEvent.IsAllDay)
                {
                    fEvent.Duration = fEvent.Duration;
                    // Timezone conversion to UTC
                    fEvent.EndDateTime = new Timezones().ConvertUserDateTimeToUtc(fEvent.EndDateTime.Value, calendarEvent.UpdateUserId, calendarEvent.SubscriberId);
                }

                fEvent.LastUpdate = DateTime.UtcNow;
                fEvent.UpdateUserId = calendarEvent.UpdateUserId;
                var username = new Users().GetUserFullNameById(calendarEvent.UpdateUserId, calendarEvent.SubscriberId);
                fEvent.UpdateUserName = username;
                sharedContext.SubmitChanges();

                return true;
            }
            return false;
        }


        private void SendEventNotification(Activity ca, ActivititesMember invite)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var emailAddress = "";
            var fullname = "";
            var startDateStr = "";
            if (ca.StartDateTime.HasValue)
                startDateStr = ca.StartDateTime.Value.ToString("dd MMMM, yyyy @ HH:mm");
            var timezone = "UTC";
            var firstName = "";

            User createdUser = null;
            if (ca.CreatedUserId > 0)
            {
                createdUser = context.Users.FirstOrDefault(t => t.UserId == ca.CreatedUserId);
            }

            if (invite.UserId > 0)
            {
                var user = context.Users.FirstOrDefault(t => t.UserId == invite.UserId);
                if (user != null)
                {
                    emailAddress = user.EmailAddress;
                    fullname = user.FullName;
                    if (ca.StartDateTime.HasValue)
                        startDateStr = new Timezones().ConvertUtcToUserDateTime(ca.StartDateTime.Value, user.UserId).ToString("dd MMMM, yyyy @ HH:mm");
                    timezone = user.TimeZone;
                    // first name from CRM User table (invited attendee)
                    firstName = user.FirstName;
                }
                else
                {
                    return;
                }
            }
            else if (invite.ContactId > 0)
            {
                var contact = context.Contacts.FirstOrDefault(t => t.ContactId == invite.ContactId);
                if (contact != null)
                {
                    emailAddress = contact.Email;
                    fullname = contact.FirstName + " " + contact.LastName;
                    firstName = contact.FirstName;
                }
            }
            else if (!string.IsNullOrEmpty(invite.Email))
            {
                emailAddress = invite.Email;
            }

            var emailTemplate = "";
            using (WebClient client = new WebClient())
            {
                string path = HttpContext.Current.Server.MapPath("~/_email_templates/event-invite.html");
                emailTemplate = client.DownloadString(path);
            }

            // email recipient first name
            if (string.IsNullOrEmpty(firstName))
            {
                emailTemplate = emailTemplate.Replace("Dear #inviteduser#,", "");
            }
            else
            {
                emailTemplate = emailTemplate.Replace("#inviteduser#", firstName);
            }

            // set the body
            emailTemplate = emailTemplate.Replace("#subscriberlogo#", new Subscribers().GetLogo(ca.SubscriberId));
            var invitedUser = new Users().GetUserFullNameById(ca.UpdateUserId, ca.SubscriberId);
            emailTemplate = emailTemplate.Replace("#invitedbyuser#", invitedUser);

            // set the event content
            var eventContentStr = "";
            if (!string.IsNullOrEmpty(ca.Subject))
                eventContentStr += "<p style=\"margin: 0; font-size: 14px;\">Subject: <strong>" + ca.Subject + "</strong></p>";
            if (!string.IsNullOrEmpty(ca.Location))
                eventContentStr += "<p style=\"margin: 0; font-size: 14px;\">Where: <strong>" + ca.Location + "</strong></p>";

            eventContentStr += "<p style=\"margin: 0; font-size: 14px;\">When: <strong>" + startDateStr + "(" + timezone + ")</strong></p>";

            if (!string.IsNullOrEmpty(ca.DealNames))
                eventContentStr += "<p style=\"margin: 0; font-size: 14px;\">Deal: <strong>" + ca.DealNames + "</strong></p>";

            if (!string.IsNullOrEmpty(ca.CompanyName))
                eventContentStr += "<p style=\"margin: 0; font-size: 14px;\">Company: <strong>" + ca.CompanyName + "</strong></p>";

            emailTemplate = emailTemplate.Replace("#eventcontent#", eventContentStr);

            var body = emailTemplate;
            var CRM_AdminEmailSender =
                            new Recipient
                            {
                                EmailAddress = "admin@firstfreight.com",
                                Name = "First Freight CRM"
                            };

            var request = new SendEmailRequest
            {
                Sender = CRM_AdminEmailSender,
                Subject = "Event Invite",
                HtmlBody = body,
                OtherRecipients = new List<Recipient> {
                        new Recipient{
                            EmailAddress = emailAddress,
                            Name = fullname,
                            UserId = invite.UserId ?? 0
                        },
                        // send copy of email to archive + dev
                        new Recipient{EmailAddress = "sendgrid@firstfreight.com" },
                        new Recipient{EmailAddress = "charles@firstfreight.com" }
                    },
                Attachments = new List<System.Net.Mail.Attachment>()
            };

            // create iCal appointment
            StringBuilder sb = new StringBuilder();
            string DateFormat = "yyyyMMddTHHmmssZ";
            string now = DateTime.Now.ToUniversalTime().ToString(DateFormat);
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("PRODID:-//Compnay Inc//Product Application//EN");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("METHOD:PUBLISH");

            sb.AppendLine("BEGIN:VEVENT");
            if (ca.StartDateTime.HasValue)
                sb.AppendLine("DTSTART:" + ca.StartDateTime.Value.ToString(DateFormat));
            if (ca.EndDateTime.HasValue)
            {
                sb.AppendLine("DTEND:" + ca.EndDateTime.Value.ToString(DateFormat));
            }

            sb.AppendLine("DTSTAMP:" + now);
            sb.AppendLine("UID:" + Guid.NewGuid());
            sb.AppendLine("CREATED:" + now);
            sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + ca.Description);
            if (createdUser != null)
            {
                sb.AppendLine("ORGANIZER:" + "mailto:" + createdUser.EmailAddress);
                request.ReplyToEmail = createdUser.EmailAddress;
            }

            // Don't sync Description between CRM and O365 - could accidentally expose meeting notes to other attendees
            //sb.AppendLine("DESCRIPTION:" + res.Details);

            sb.AppendLine("LAST-MODIFIED:" + now);
            sb.AppendLine("LOCATION:" + ca.Location);
            sb.AppendLine("SEQUENCE:0");
            sb.AppendLine("STATUS:CONFIRMED");
            sb.AppendLine("SUMMARY:" + ca.Subject);
            sb.AppendLine("TRANSP:OPAQUE");
            sb.AppendLine("END:VEVENT");

            sb.AppendLine("END:VCALENDAR");
            var calendarBytes = Encoding.UTF8.GetBytes(sb.ToString());
            var ms = new MemoryStream(calendarBytes);
            var attachment = new System.Net.Mail.Attachment(ms, "event.ics", "text/calendar");

            request.Attachments.Add(attachment);

            // send email + iCal attachment
            new SendGridHelper().SendEmail(request);
        }


        public List<EventCategory> GetEventCategories(int subscriberId)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            return context.EventCategories.Where(t => !t.Deleted && t.SubscriberId == subscriberId).ToList();

        }


        public bool DeleteEventCategory(int id, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var category = context.EventCategories.FirstOrDefault(t => t.EventCategoryId == id && t.SubscriberId == subscriberId);
            if (category != null)
            {
                category.PrepareForSoftDelete(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public int SaveEventCategory(EventCategory categoryDetails)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            // get the category by id or create new commodity object
            var category = context.EventCategories.FirstOrDefault(t => t.SubscriberId == categoryDetails.SubscriberId &&
                                                    t.EventCategoryId == categoryDetails.EventCategoryId && t.Deleted == false) ?? new EventCategory();

            category.CategoryName = categoryDetails.CategoryName;
            category.CategoryColor = categoryDetails.CategoryColor;
            category.AddUpdateStamp(categoryDetails.CreatedUserId, categoryDetails.SubscriberId);

            if (category.EventCategoryId < 1)
            {
                category.CategoryName = categoryDetails.CategoryName;
                category.CategoryColor = categoryDetails.CategoryColor;
                category.PrepareForInsert(categoryDetails.CreatedUserId, categoryDetails.SubscriberId);
                context.EventCategories.InsertOnSubmit(category);
            }

            context.SubmitChanges();
            return category.EventCategoryId;
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

    }
}

