using Crm6.App_Code.Helpers;
using Crm6.App_Code.Login;
using Crm6.App_Code.Shared;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Helpers;
using Helpers.Sync;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Crm6.App_Code.Sync
{

    public class ReoccurrenceMetaData
    {
        public string Until { get; set; }
        public string ReoccurrenceType { get; set; }
    }

    public class GoogleSyncEngine
    {
        public int UserId { get; set; }
        public int SubscriberId { get; set; }
        public string FullName { get; set; }
        public User User { get; set; }
        public string Connection { get; set; }
        public string SyncToken { get; set; }

        public void InitGoogleSync(SyncUser syncUser)
        {
            Connection = syncUser.Connection;
            var context = new DbFirstFreightDataContext(Connection);
            UserId = syncUser.UserId;
            var user = context.Users.FirstOrDefault(t => t.UserId == syncUser.UserId);
            if (user != null)
            {
                var subscriber = context.Subscribers.FirstOrDefault(t => t.SubscriberId == user.SubscriberId);
                SubscriberId = user.SubscriberId;
                FullName = user.FullName;
                GoogleSyncAppSettings.GoogleApiRefreshToken = user.GoogleRefreshToken;
                GoogleSyncAppSettings.GoogleEmail = user.GoogleCalendarEmail;
                SyncToken = user.GoogleSyncToken;
                User = user;
            }
        }


        public void RunGoogleAppointmentsSync()
        {
            // ================================================================================================================
            // Get List of CRM Calendar Events and List of Google Appointments
            // ================================================================================================================
            List<Activity> crmCalendarEventsAll = null;
            List<Activity> crmCalendarEventsExcludingDeleted = null;
            List<Activity> crmCalendarEventsOnlyDeleted = null;
            List<Activity> googleEvents = null;
            var currentDate = DateTime.UtcNow;
            try
            {

                if (User.SyncAppointmentsLastDateTime == null)
                {
                    // Initial Appointment Sync 
                    // Get List of ALL Google Appointments Starting from 1 day ago
                    User.SyncAppointmentsLastDateTime = DateTime.UtcNow.AddDays(-1);
                    // Get List of ALL CRM Calendar Events
                    crmCalendarEventsAll = new CrmAppointments().GetCrmCalendarEventsForSyncUser(User.UserId, User.SubscriberId, Connection, null, DateTime.UtcNow);
                }
                else
                {
                    // Incremental Appointment Sync - Based On Last Modified Date or Created Date >= Last Appointment Sync Date
                    // Go 10 Seconds Back for Last Sync - for Whatever Variances...
                    var appointmentLastSyncTime = User.SyncAppointmentsLastDateTime.Value.AddSeconds(-10);
                    // Get List of Created/Modified Google Appointments
                    // Get List of Created/Modified CRM Appointments
                    crmCalendarEventsAll = new CrmAppointments().GetCrmCalendarEventsForSyncUser(User.UserId, User.SubscriberId, Connection, User.SyncAppointmentsLastDateTime);
                }
            }
            catch (Exception ex)
            {
                // Log Error
                // 100 Errors are for Appointments / Calendar
                new SyncInitializer().LogSyncError(User, 2, ex.ToString(), "Google: 101-1 - ManageAppointmentsSync - Error retrieving new/modified Appointments");
            }

            // Perform FUll 2-Way Appointment Sync


            googleEvents = GetGoogleEvents();

            var googleEventsDeleted = googleEvents.Where(x => x.Deleted)?.ToList();

            crmCalendarEventsExcludingDeleted = crmCalendarEventsAll.Where(x => !x.Deleted).ToList();
            crmCalendarEventsOnlyDeleted = crmCalendarEventsAll.Where(x => x.Deleted).ToList();

            // CRM Updates Google
            if (crmCalendarEventsExcludingDeleted != null && crmCalendarEventsExcludingDeleted.Count > 0)
            {
                CrmCalendarEventsUpdateGoogleAppointments(crmCalendarEventsExcludingDeleted, googleEvents);
            }

            // Google Updates CRM
            if (googleEvents != null && googleEvents.Count > 0)
            {
                GoogleAppointmentsUpdateCrmCalendarEvents(googleEvents);
            }

            if (crmCalendarEventsOnlyDeleted.Any())
                DeleteCrmAppointmentsInGoogle(crmCalendarEventsOnlyDeleted);

            if (googleEventsDeleted.Any())
                DeleteGoogleAppointmentsInCrm(googleEventsDeleted);

            // update last sync date
            new SyncInitializer().UpdateAppointmentsLastSyncdate(User.SubscriberId, User.UserId, currentDate.AddSeconds(-20));


            // update sync token
            GoogleSyncAppSettings.UpdateSyncToken(User.SubscriberId, User.UserId, SyncToken);
        }

        public static IAuthorizationCodeFlow GoogleAuthorizationCodeFlow(out string error)
        {
            IAuthorizationCodeFlow flow = null;
            error = string.Empty;

            try
            {
                flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = GoogleSyncAppSettings.GoogleClientSecrets,
                    Scopes = GoogleSyncAppSettings.Scopes
                });
            }
            catch (Exception ex)
            {
                flow = null;
                error = "Failed to AuthorizationCodeFlow Initialization: " + ex.ToString();
            }

            return flow;
        }


        public static UserCredential GetGoogleUserCredentialByRefreshToken(out string error)
        {
            TokenResponse respnseToken = null;
            UserCredential credential = null;
            string flowError;
            error = string.Empty;
            try
            {
                // Get a new IAuthorizationCodeFlow instance
                IAuthorizationCodeFlow flow = GoogleAuthorizationCodeFlow(out flowError);

                respnseToken = new TokenResponse() { RefreshToken = GoogleSyncAppSettings.GoogleApiRefreshToken };

                // Get a new Credential instance                
                if ((flow != null && string.IsNullOrWhiteSpace(flowError)) && respnseToken != null)
                {
                    credential = new UserCredential(flow, "user", respnseToken);
                }

                // Get a new Token instance
                if (credential != null)
                {
                    bool success = credential.RefreshTokenAsync(CancellationToken.None).Result;

                    // Set the new Token instance
                    if (credential.Token != null)
                    {
                        string newRefreshToken = credential.Token.RefreshToken;
                    }
                }
            }
            catch (Exception ex)
            {
                credential = null;
                error = "UserCredential failed: " + ex.ToString();
            }
            return credential;
        }


        public void CrmCalendarEventsUpdateGoogleAppointments(List<Activity> crmAppointments, List<Activity> googleAppointments)
        {

            // =========================================================================================================
            // Iterate Through CRM Appointments for User and Add/Update Google
            // =========================================================================================================
            try
            {
                foreach (var crmCalendarEvent in crmAppointments)
                {
                    Event googleEvent = null;

                    if (!string.IsNullOrEmpty(crmCalendarEvent.GoogleSyncId))
                    {
                        googleEvent = GetGoogleEvent(crmCalendarEvent.GoogleSyncId);
                    }

                    if (googleEvent != null)
                    {
                        // CRM Appointment is in Google
                        var crmLastModified = crmCalendarEvent.LastUpdate;
                        var googleLastModified = googleEvent.Updated;
                        var syncLastModified = crmCalendarEvent.GoogleSyncLastModified;
                        // Check if crmLastModified > googleLastModified
                        var syncWinner = CrmOrGoogleIsLastestVersion(crmLastModified, googleLastModified, syncLastModified);
                        if (syncWinner == "CRM")
                        {
                            // CRM Appointment is more recently modified - Update Google Appointment 
                            UpdateGoogleAppointmentUsingCrmCalendarEvent(crmCalendarEvent);
                        }
                    }
                    else
                    {

                        // Add event to google. If it is a recurring appointment, only add the parent (this is determined by checking
                        //that the activity id is the same as the parent activity id), not the children. The parent is then added as 
                        //recurring and the recurring type is specified, so Google then handles the rest from there.
                        if (crmCalendarEvent.IsRecurring.HasValue == false || crmCalendarEvent.IsRecurring == false ||
                            (crmCalendarEvent.IsRecurring.HasValue &&
                            crmCalendarEvent.IsRecurring.Value &&
                            crmCalendarEvent.ReoccurrenceParentActivityId.HasValue &&
                            crmCalendarEvent.ReoccurrenceParentActivityId.Value == crmCalendarEvent.ActivityId))
                        {
                            googleEvent = AddGoogleAppointmentUsingCrmCalendarEvent(crmCalendarEvent);

                            if (googleEvent != null)
                            {
                                // Use GoogleAppointmentId from the New Google Appointment to Update CRM Appointment
                                var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == User.SubscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
                                var sharedConnection = LoginUser.GetWritableSharedConnectionForDataCenter(subscriberDataCenter);
                                var sharedContext = new DbSharedDataContext(sharedConnection);

                                var calendarEvent = sharedContext.Activities.FirstOrDefault(c => c.ActivityId == crmCalendarEvent.ActivityId);
                                if (calendarEvent != null)
                                {
                                    calendarEvent.GoogleSyncId = googleEvent.Id;
                                    calendarEvent.GoogleSyncLastModified = googleEvent.Updated;
                                }
                                sharedContext.SubmitChanges();

                                //Updates GoogleSyncId of all children events of this parent recurring event (and leaves out the actual parent event).
                                var childrenEvents = sharedContext.Activities.Where(x => x.ReoccurrenceParentActivityId == calendarEvent.ActivityId && x.ActivityId != calendarEvent.ActivityId && x.GoogleSyncId == null);

                                var recurringEventsFromGoogle = GetRecurringEvents(calendarEvent.GoogleSyncId);

                                foreach (var childEvent in childrenEvents)
                                {
                                    //var utcStartTime = new Timezones().ConvertUserDateTimeToUtcCached(childEvent.StartDateTime.Value, User.UserId, SubscriberId);
                                    var utcStartTime = childEvent.StartDateTime.Value;
                                    var dateTimeRaw = $"{utcStartTime.ToString("yyyy-MM-dd")}T{utcStartTime.ToString("HH:mm:ss")}Z";

                                    //In some cases the Google Calendar API sends a null DateTime and a filled-in Date, and other times vice-versa.
                                    var matchingGoogleEvent = recurringEventsFromGoogle.FirstOrDefault(x => x.Start.DateTime.HasValue ? x.Start.DateTimeRaw.Equals(dateTimeRaw, StringComparison.InvariantCultureIgnoreCase) : x.Start.Date.Equals(childEvent.ActivityDate.Date.ToString("yyyy-MM-dd")));

                                    if (matchingGoogleEvent != null)
                                    {
                                        childEvent.GoogleSyncId = matchingGoogleEvent.Id;
                                        childEvent.GoogleSyncLastModified = matchingGoogleEvent.Updated;
                                    }
                                }

                                sharedContext.SubmitChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log Error
                // 100 Errors are for Appointments / Calendar
                new SyncInitializer().LogSyncError(User, 2, ex.ToString(), "Google 102 - CrmCalendarEventsUpdateGoogleAppointments - CRM => Google");
            }
        }


        public string CrmOrGoogleIsLastestVersion(DateTime? crmLastModified, DateTime? googleLastModified, DateTime? syncLastModified)
        {
            var updateType = "None";
            if (crmLastModified == null)
            {
                // =================================================================
                // CRM NO LastModified - Compare with googleLastModified
                // =================================================================
                if (googleLastModified != null)
                {
                    updateType = "Google";
                }
            }
            else
            {
                if (googleLastModified == null)
                {
                    // CRM HAS LastModified - Google NULL LastModified
                    updateType = "CRM";
                }
                else
                {
                    // =================================================================
                    // Compare Google with crmLastModified (CRM)
                    // =================================================================
                    if (crmLastModified > syncLastModified)
                    {
                        updateType = "CRM";
                    }
                    else if (crmLastModified == googleLastModified)
                    {
                        // -----------------------------------------------------------------
                        // CRM and Google Match
                        // -----------------------------------------------------------------
                        updateType = "None";
                    }
                    else if (crmLastModified < googleLastModified)
                    {
                        // -----------------------------------------------------------------
                        // Google LastModified > syncLastModified from CRM
                        // -----------------------------------------------------------------
                        updateType = "Google";
                    }
                    else if (crmLastModified > googleLastModified)
                    {
                        // -----------------------------------------------------------------
                        // crmLastModified > Google LastModified  
                        // -----------------------------------------------------------------
                        updateType = "CRM";
                    }
                }
            }
            return updateType;
        }


        private Event AddGoogleAppointmentUsingCrmCalendarEvent(Activity crmCalendarEvent)
        {
            string eventId = string.Empty;
            string serviceError;

            try
            {
                var calendarService = GetCalendarService(out serviceError);

                if (calendarService != null && string.IsNullOrWhiteSpace(serviceError))
                {
                    var list = calendarService.CalendarList.List().Execute();
                    var calendar = list.Items.SingleOrDefault(c => c.Primary.HasValue && c.Primary.Value);
                    //   var calendar = list.Items.SingleOrDefault(c => c.Summary == GoogleSyncAppSettings.GoogleEmail);
                    if (calendar != null)
                    {
                        var calendarEvent = new Event();

                        calendarEvent.Summary = crmCalendarEvent.Subject;
                        calendarEvent.Description = crmCalendarEvent.Description;
                        calendarEvent.Location = crmCalendarEvent.Location;

                        if (calendarEvent.End == null)
                        {
                            calendarEvent.End = calendarEvent.Start;
                        }

                        if (!crmCalendarEvent.IsAllDay)
                        {
                            //var utcStartTime = new Timezones().ConvertUserDateTimeToUtcCached(crmCalendarEvent.StartDateTime.Value, User.UserId, SubscriberId);
                            //var utcEndTime = new Timezones().ConvertUserDateTimeToUtcCached(crmCalendarEvent.EndDateTime.Value, User.UserId, SubscriberId);

                            var utcStartTime = crmCalendarEvent.StartDateTime.Value;
                            var utcEndTime = crmCalendarEvent.EndDateTime.Value;

                            calendarEvent.Start = new EventDateTime
                            {
                                DateTimeRaw = $"{utcStartTime.ToString("yyyy-MM-dd")}T{utcStartTime.ToString("HH:mm:ss")}Z",
                                TimeZone = "UTC"
                            };
                            calendarEvent.End = new EventDateTime
                            {
                                DateTimeRaw = $"{utcEndTime.ToString("yyyy-MM-dd")}T{utcEndTime.ToString("HH:mm:ss")}Z",
                                TimeZone = "UTC"
                            };
                        }
                        else
                        {
                            if (crmCalendarEvent.StartDateTime.HasValue)
                            {
                                var userDateTime = new Timezones().ConvertUtcToUserDateTime(crmCalendarEvent.StartDateTime.Value, User.UserId);

                                calendarEvent.Start = new EventDateTime
                                {
                                    Date = userDateTime.ToString("yyyy-MM-dd"),
                                    //TimeZone = "UTC"
                                };

                                calendarEvent.End = new EventDateTime
                                {
                                    Date = userDateTime.ToString("yyyy-MM-dd"),
                                    //TimeZone = "UTC"
                                };
                            }
                        }

                        #region Attendees
                        var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
                        var invites = new global::Helpers.CalendarEvents().GetCalendarEventInvites(crmCalendarEvent.CalendarEventId, crmCalendarEvent.SubscriberId);
                        if (invites != null && invites.Count > 0)
                        {
                            calendarEvent.Attendees = new List<EventAttendee>();
                            foreach (var invite in invites)
                            {
                                if (invite.UserId == User.UserId)
                                {
                                    continue;
                                }
                                var attendee = new EventAttendee() { Email = invite.Email };
                                if (invite.UserId > 0)
                                {
                                    var userEmialAddress = context.Users.Where(t => t.UserId == invite.UserId).Select(t => t.EmailAddress).FirstOrDefault();
                                    if (string.IsNullOrEmpty(userEmialAddress) || !global::Helpers.Utils.IsValidEmail(userEmialAddress))
                                    {
                                        continue;
                                    }
                                    attendee.Email = userEmialAddress;
                                    attendee.DisplayName = invite.UserName;
                                }
                                else if (invite.ContactId > 0)
                                {
                                    var contactEmialAddress = context.Contacts.Where(t => t.ContactId == invite.ContactId).Select(t => t.Email).FirstOrDefault();
                                    if (string.IsNullOrEmpty(contactEmialAddress) || !global::Helpers.Utils.IsValidEmail(contactEmialAddress))
                                    {
                                        continue;
                                    }
                                    attendee.Email = contactEmialAddress;
                                    attendee.DisplayName = invite.ContactName;
                                }
                                else if (!string.IsNullOrEmpty(invite.Email) && global::Helpers.Utils.IsValidEmail(invite.Email))
                                {
                                    attendee.Email = invite.Email;
                                }
                                calendarEvent.Attendees.Add(attendee);
                            }

                        }

                        #endregion

                        // reminder
                        if (crmCalendarEvent.ReminderMinutes > 0)
                        {
                            EventReminder rem = new EventReminder();
                            rem.Method = "popup";
                            rem.Minutes = crmCalendarEvent.ReminderMinutes;
                            Event.RemindersData rd = new Event.RemindersData();
                            rd.UseDefault = false;
                            var reminders = new List<EventReminder>();
                            reminders.Add(rem);
                            rd.Overrides = reminders;
                            calendarEvent.Reminders = rd;
                        }

                        if (crmCalendarEvent.IsRecurring.HasValue && string.IsNullOrWhiteSpace(crmCalendarEvent.ReoccurrenceIncrementType) == false && crmCalendarEvent.IsRecurring.Value)
                        {
                            var reoccurrenceMetaData = GenerateGoogleReoccurrenceMetaData(crmCalendarEvent.ReoccurrenceIncrementType, crmCalendarEvent.ActivityDate);

                            calendarEvent.Recurrence = new[] { $"RRULE:FREQ={reoccurrenceMetaData.ReoccurrenceType};UNTIL={reoccurrenceMetaData.Until}" };
                        }

                        var newEventRequest = calendarService.Events.Insert(calendarEvent, calendar.Id);
                        newEventRequest.SendNotifications = false;
                        var eventResult = newEventRequest.Execute();

                        return eventResult;
                    }
                }
            }
            catch (Exception ex)
            {
                var error = new WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    ErrorMessage = ex.ToString(),
                    PageCalledFrom = "Helper/DeleteCompany",
                    RoutineName = "SaveGlobalCompany",
                    SubscriberName = "",
                    //UserId = request.UserId,
                    //SubscriberId = request.CompanySubscriberId,
                };
                new Logging().LogWebAppError(error);
                eventId = string.Empty;
            }
            return null;
        }

        private ReoccurrenceMetaData GenerateGoogleReoccurrenceMetaData(string reoccurrenceIncrementType, DateTime activityDate)
        {
            DateTime finalDate;

            string recurringType;
            if (reoccurrenceIncrementType.Equals("Weekly", StringComparison.InvariantCultureIgnoreCase))
            {
                recurringType = "WEEKLY";
                finalDate = activityDate.AddDays((RecurringEventProperties.WeeklyNumberOfCalendarEvents - 1) * 7);
            }
            else if (reoccurrenceIncrementType.Equals("Monthly", StringComparison.InvariantCultureIgnoreCase))
            {
                recurringType = "MONTHLY";
                finalDate = activityDate.AddMonths(RecurringEventProperties.MonthlyNumberOfCalendarEvents - 1);
            }
            else
            {
                recurringType = "DAILY";
                finalDate = activityDate.AddDays(RecurringEventProperties.DailyNumberOfCalendarEvents - 1);
            }

            var until = $"{finalDate.ToString("yyyyMMdd")}T{ finalDate.ToString("HHmmss")}Z";

            return new ReoccurrenceMetaData
            {
                ReoccurrenceType = recurringType,
                Until = until
            };
        }


        private void UpdateGoogleAppointmentUsingCrmCalendarEvent(Activity crmCalendarEvent)
        {
            Event googlEvent = null;
            var error = string.Empty;
            string serviceError;
            try
            {
                var calendarService = GetCalendarService(out serviceError);
                if (calendarService != null)
                {
                    var list = calendarService.CalendarList.List().Execute();
                    var calendar = list.Items.SingleOrDefault(c => c.Primary.HasValue && c.Primary.Value);
                    // var calendar = list.Items.SingleOrDefault(c => c.Summary == GoogleSyncAppSettings.GoogleEmail);
                    if (calendar != null)
                    {
                        EventsResource.GetRequest request = calendarService.Events.Get("primary", crmCalendarEvent.GoogleSyncId);
                        request.TimeZone = "UTC";

                        // get requested event
                        googlEvent = request.Execute();

                        if (googlEvent != null)
                        {
                            googlEvent.Summary = crmCalendarEvent.Subject;
                            googlEvent.Description = crmCalendarEvent.Description;
                            googlEvent.Location = crmCalendarEvent.Location;

                            if (crmCalendarEvent.ReminderMinutes > 0)
                            {
                                EventReminder rem = new EventReminder();
                                rem.Method = "popup";
                                rem.Minutes = crmCalendarEvent.ReminderMinutes;
                                Event.RemindersData rd = new Event.RemindersData();
                                rd.UseDefault = false;
                                var reminders = new List<EventReminder>();
                                reminders.Add(rem);
                                rd.Overrides = reminders;
                                googlEvent.Reminders = rd;
                            }

                            // handle all day events
                            if (!crmCalendarEvent.IsAllDay)
                            {
                                googlEvent.Start = new EventDateTime
                                {
                                    DateTime = crmCalendarEvent.StartDateTime,
                                    TimeZone = "UTC"
                                };
                                //googlEvent.End = new EventDateTime
                                //{
                                //    DateTime = new DateTime(crmCalendarEvent.StartDateTime.Year, crmCalendarEvent.StartDateTime.Month,
                                //                           crmCalendarEvent.StartDateTime.Day, 23, 59, 9),
                                //    TimeZone = "UTC"
                                //};

                                googlEvent.End = new EventDateTime
                                {
                                    DateTime = crmCalendarEvent.EndDateTime.Value,
                                    TimeZone = "UTC"
                                };
                            }
                            else
                            {
                                if (crmCalendarEvent.StartDateTime.HasValue)
                                    googlEvent.Start = new EventDateTime
                                    {
                                        Date = crmCalendarEvent.StartDateTime.Value.ToString("yyyy-MM-dd"),
                                        TimeZone = "UTC"
                                    };
                            }

                            // add attendees 
                            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

                            var invites = new global::Helpers.CalendarEvents().GetCalendarEventInvites(crmCalendarEvent.CalendarEventId, crmCalendarEvent.SubscriberId);
                            if (invites != null && invites.Count > 0)
                            {
                                googlEvent.Attendees = new List<EventAttendee>();
                                foreach (var invite in invites)
                                {
                                    if (invite.UserId == User.UserId)
                                    {
                                        continue;
                                    }
                                    var attendee = new EventAttendee() { Email = invite.Email };
                                    if (invite.UserId > 0)
                                    {
                                        var userEmialAddress = context.Users.Where(t => t.UserId == invite.UserId).Select(t => t.EmailAddress).FirstOrDefault();
                                        if (string.IsNullOrEmpty(userEmialAddress) || !global::Helpers.Utils.IsValidEmail(userEmialAddress))
                                        {
                                            continue;
                                        }
                                        attendee.Email = userEmialAddress;
                                        attendee.DisplayName = invite.UserName;
                                    }
                                    else if (invite.ContactId > 0)
                                    {
                                        var contactEmialAddress = context.Contacts.Where(t => t.ContactId == invite.ContactId).Select(t => t.Email).FirstOrDefault();
                                        if (string.IsNullOrEmpty(contactEmialAddress) || !global::Helpers.Utils.IsValidEmail(contactEmialAddress))
                                        {
                                            continue;
                                        }
                                        attendee.Email = contactEmialAddress;
                                        attendee.DisplayName = invite.ContactName;
                                    }
                                    else if (!string.IsNullOrEmpty(invite.Email) && global::Helpers.Utils.IsValidEmail(invite.Email))
                                    {
                                        attendee.Email = invite.Email;
                                    }
                                    googlEvent.Attendees.Add(attendee);
                                }

                            }

                            var updateEventRequest = calendarService.Events.Update(googlEvent, calendar.Id, crmCalendarEvent.GoogleSyncId);
                            updateEventRequest.SendNotifications = false;
                            googlEvent = updateEventRequest.Execute();


                            // Update CRM SyncLastModified using Google LastModified
                            DateTime? googleLastModifiedTime = googlEvent.Updated;
                            new CrmAppointments().UpdateCrmCalendarEventSyncLastModified(Connection, crmCalendarEvent.CalendarEventId, googleLastModifiedTime, User.SyncType);

                            // =====================================================================================
                            var syncMessage = googlEvent.Summary + " | " + googlEvent.Start.DateTime.Value.ToShortDateString();
                            new SyncInitializer().LogSync(User, "UpdateGoogleAppointmentUsingCrmCalendarEvent", 0, crmCalendarEvent.CalendarEventId, syncMessage);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new SyncInitializer().LogSyncError(User, 2, ex.ToString(), "Google 107 - UpdateGoogleAppointmentUsingCrmCalendarEvent");
            }

        }


        public void GoogleAppointmentsUpdateCrmCalendarEvents(List<Activity> googleAppointments)
        {
            // =========================================================================================================
            // Iterate Through Google Appointments for User and Add/Update CRM
            // =========================================================================================================
            try
            {
                var listActivitiesToAdd = new List<Activity>();

                foreach (var googleAppointment in googleAppointments.Where(x => x.Deleted == false))
                {
                    // Check if Google  Appointment is in CRM 
                    // ---------------------------------------------------------------------------------------------------------
                    var googleLastModified = googleAppointment.LastUpdate;
                    var activityId = 0;
                    DateTime? crmLastModified = null;

                    googleAppointment.OriginSystem = "Google";

                    // Look for the Google appointment by id in CRM
                    activityId = SearchCrmForGoogleEvent(googleAppointment.GoogleSyncId);

                    // if not found, search by title, location, start and end dates 
                    if (activityId == 0)
                    {
                        activityId = SearchForGoogleEventInCrm(googleAppointment);
                    }

                    // ---------------------------------------------------------------------------------------------------------
                    if (activityId == 0)
                    {
                        // Google Appointment NOT in CRM - Add Appointment to CRM
                        googleAppointment.OriginSystem = "Google";

                        if (!listActivitiesToAdd.Any(x => x.Subject.Equals(googleAppointment.Subject) && x.StartDateTime == googleAppointment.StartDateTime && x.EndDateTime == googleAppointment.EndDateTime))
                            listActivitiesToAdd.Add(googleAppointment);
                    }
                    else
                    {
                        // Google Appointment IS in CRM
                        // Check if googleLastModified > crmLastModified
                        var syncWinner = CrmOrGoogleIsLastestVersion(crmLastModified, googleLastModified, googleAppointment.GoogleSyncLastModified);
                        if (syncWinner != "Google") continue;
                        // Google Appointment IS more recently modified - Update CRM Appointment
                        new CrmAppointments().UpdateCrmCalendarEvent(Connection, User, activityId, googleAppointment, googleAppointment.GoogleSyncLastModified);
                    }
                }

                new CrmAppointments().BulkAddCrmCalendarEventsFromGoogle(Connection, User, listActivitiesToAdd);
            }
            catch (Exception ex)
            {
                // Log Error
                new SyncInitializer().LogSyncError(User, 2, ex.ToString(), "Google 103 - CrmCalendarEventsUpdateExchangeAppointments - CRM => Exchange");
            }
        }




        public static CalendarService GetCalendarService(out string error)
        {
            CalendarService calendarService = null;
            string credentialError;
            error = string.Empty;
            try
            {
                var credential = GetGoogleUserCredentialByRefreshToken(out credentialError);
                if (credential != null && string.IsNullOrWhiteSpace(credentialError))
                {
                    calendarService = new CalendarService(new BaseClientService.Initializer()
                    {
                        HttpClientInitializer = credential,
                        ApplicationName = GoogleSyncAppSettings.ApplicationName
                    });
                }
                else
                {
                    error = credentialError;
                }
            }
            catch (Exception ex)
            {
                calendarService = null;
                error = "Calendar service failed: " + ex.ToString();
            }
            return calendarService;
        }




        /// <summary>
        /// get Google event by id
        /// </summary>
        /// <param name="googleEventId"></param>
        /// <returns></returns>
        public Event GetGoogleEvent(string googleEventId)
        {
            Event eventResult = null;
            var error = string.Empty;
            string serviceError;
            try
            {
                var calendarService = GetCalendarService(out serviceError);
                if (calendarService != null)
                {
                    var list = calendarService.CalendarList.List().Execute();
                    var calendar = list.Items.SingleOrDefault(c => c.Primary.HasValue && c.Primary.Value);
                    if (calendar != null)
                    {
                        // Define parameters of request
                        EventsResource.GetRequest request = calendarService.Events.Get("primary", googleEventId);
                        request.TimeZone = "UTC";
                        // Get selected event
                        eventResult = request.Execute();
                    }
                }
            }
            catch (Exception) { }
            return eventResult;
        }


        public void DeleteCrmAppointmentsInGoogle(List<Activity> listOfDeletedEvents)
        {
            var result = string.Empty;
            string serviceError;
            try
            {
                var calendarService = GetCalendarService(out serviceError);
                if (calendarService != null)
                {
                    foreach (Activity eventToDelete in listOfDeletedEvents)
                    {
                        var list = calendarService.CalendarList.List().Execute();
                        var calendar = list.Items.SingleOrDefault(c => c.Primary.HasValue && c.Primary.Value);
                        // var calendar = list.Items.FirstOrDefault(c => c.Summary == GoogleSyncAppSettings.GoogleEmail);
                        if (calendar != null)
                        {
                            // Define parameters of request
                            EventsResource.ListRequest request = calendarService.Events.List("primary");
                            request.TimeMin = DateTime.Now;
                            request.ShowDeleted = false;
                            request.SingleEvents = true;
                            request.TimeZone = "UTC";
                            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                            // Get selected event
                            Events events = request.Execute();
                            var selectedEvent = events.Items.FirstOrDefault(c => c.Id == eventToDelete.GoogleSyncId);

                            //Google has issues with the GoogleSyncId of the parent event not matching what was originally generated.
                            //The following fixes that (it checks to make sure that this event is a parent recurring event).
                            if (selectedEvent == null && eventToDelete.StartDate.HasValue && eventToDelete.ActivityId == eventToDelete.ReoccurrenceParentActivityId && eventToDelete.IsRecurring.HasValue && eventToDelete.IsRecurring.Value)
                            {
                                var eventsMatchingStartOfGoogleSyncId = events.Items.Where(c => c.Id.StartsWith(eventToDelete.GoogleSyncId));

                                foreach (var currentEvent in eventsMatchingStartOfGoogleSyncId)
                                {
                                    DateTime comparisonDate;

                                    if (currentEvent.Start.DateTime.HasValue)
                                    {
                                        DateTime.TryParseExact(currentEvent.Start.DateTimeRaw.Substring(0, 19).Replace("T", " "), "yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out comparisonDate);
                                    }
                                    else
                                    {
                                        DateTime.TryParseExact(currentEvent.Start.Date, "yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out comparisonDate);
                                    }

                                    if (comparisonDate.Year == eventToDelete.StartDate.Value.Year &&
                                        comparisonDate.Month == eventToDelete.StartDate.Value.Month &&
                                        comparisonDate.Day == eventToDelete.StartDate.Value.Day)
                                    {
                                        //Accounts for differences in how Google works with recurring and all-day events.
                                        if (currentEvent.Start.DateTime.HasValue)
                                        {
                                            if (comparisonDate.Hour == eventToDelete.StartDateTime.Value.Hour &&
                                                comparisonDate.Minute == eventToDelete.StartDateTime.Value.Minute)
                                            {
                                                selectedEvent = currentEvent;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            selectedEvent = currentEvent;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (selectedEvent != null)
                            {
                                var deleteEventRequest = calendarService.Events.Delete(calendar.Id, selectedEvent.Id);
                                deleteEventRequest.SendNotifications = true;
                                deleteEventRequest.Execute();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new SyncInitializer().LogSyncError(User, 2, ex.ToString(), "Google 108 - ManageDeletedCrmAppointments");
                result = string.Empty;
            }
        }

        public void DeleteGoogleAppointmentsInCrm(List<Activity> listOfDeletedEvents)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                             .GlobalSubscribers.Where(t => t.SubscriberId == User.SubscriberId)
                                                             .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetWritableSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var listGoogleSyncIds = listOfDeletedEvents.Select(x => x.GoogleSyncId);

            var listDeletedEventsInDb = (from t in sharedContext.Activities
                                         where listGoogleSyncIds.Contains(t.GoogleSyncId)
                                         select t)?.ToList();

            var listEventsNotFound = (from t in listOfDeletedEvents
                                      where !listDeletedEventsInDb.Contains(t)
                                      select t)?.ToList();

            //Since Google API seems to change the GoogleSyncId of the parent event in the case of recurring events
            //this is done to check whether any of the unmatching events actually match with the parent.
            foreach (var eventNotFound in listEventsNotFound)
            {
                if (eventNotFound.GoogleSyncId.Contains("_"))
                {
                    var matchedEvent = GetParentActivityOnNonMatchingGoogleSyncId(eventNotFound.GoogleSyncId);

                    if (matchedEvent != null)
                    {
                        listDeletedEventsInDb.Add(matchedEvent);
                    }
                }
            }

            foreach (Activity eventToDelete in listDeletedEventsInDb)
            {
                var fEvent = sharedContext.Activities.FirstOrDefault(t => t.GoogleSyncId == eventToDelete.GoogleSyncId);

                if (fEvent != null)
                {
                    fEvent.Deleted = true;
                    fEvent.DeletedUserId = User.UserId;
                    fEvent.DeletedDate = DateTime.UtcNow;
                    fEvent.LastUpdate = DateTime.UtcNow;
                    fEvent.DeletedUserName = User.FullName;
                }

                if (eventToDelete.GoogleSyncId.Contains("_"))
                {
                    var splitSyncId = eventToDelete.GoogleSyncId.Split('_');

                    //Google does not return the parent event when deleting all recurring events, so the following checks if there
                    //is more than one event with the same parent GoogleSyncId. If so, delete the parent too.
                    if (listDeletedEventsInDb.Count(x => x.GoogleSyncId.StartsWith(splitSyncId[0])) > 1)
                    {
                        var parentEvent = sharedContext.Activities.FirstOrDefault(t => t.ActivityId == eventToDelete.ReoccurrenceParentActivityId);

                        if (parentEvent != null)
                        {
                            parentEvent.Deleted = true;
                            parentEvent.DeletedUserId = User.UserId;
                            parentEvent.DeletedDate = DateTime.UtcNow;
                            parentEvent.LastUpdate = DateTime.UtcNow;
                            parentEvent.DeletedUserName = User.FullName;
                        }
                    }
                }
            }

            sharedContext.SubmitChanges();
        }


        public List<Activity> GetGoogleEvents()
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                            .GlobalSubscribers.Where(t => t.SubscriberId == User.SubscriberId)
                                                            .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var listTimezonesCached = sharedContext.TimeZones.ToList();
            var listTimezonesDstCached = sharedContext.TimeZonesDaylightSavings.ToList();

            var listActivities = sharedContext.Activities.Where(x => x.GoogleSyncId != null && x.Deleted == false).ToList();

            var googleEvents = new List<Activity>();
            var error = "";
            var service = GetCalendarService(out error);

            if (!string.IsNullOrEmpty(error) || service == null)
            {
                new SyncInitializer().LogSync(User, "GetGoogleEvents: ", 0, 0, "Error initiating the calendar service. " + error);

                return null;
            }

            // set up the request
            EventsResource.ListRequest request = service.Events.List("primary");

            // check if the sync token is empty
            if (SyncToken == null)
            {
                request.TimeMin = DateTime.UtcNow.AddDays(-1);
            }
            else
            {
                request.SyncToken = SyncToken;
            }

            request.ShowDeleted = true;
            request.SingleEvents = true;
            request.TimeZone = "UTC";
            // retrieve the events, one page at a time.
            string pageToken = null;
            Events events = null;

            List<Event> listRecurringEventsAlreadyAdded = new List<Event>();

            do
            {
                try
                {
                    request.PageToken = pageToken;
                    request.SyncToken = SyncToken;
                    events = request.Execute();

                    if (events.Items != null && events.Items.Count > 0)
                    {
                        var items = events.Items.OrderBy(x => x.Start.DateTime);

                        foreach (var eventItem in items)
                        {
                            // ignore private appointments
                            if (eventItem.Visibility == "private")
                            {
                                continue;
                            }

                            var gEvent = new Activity
                            {
                                CreatedDate = eventItem.Created.Value,
                                DeletedDate = null,
                                Deleted = eventItem.Status == "cancelled",
                                Subject = eventItem.Summary,
                                Location = eventItem.Location,
                                LastUpdate = eventItem.Updated.Value,
                                OriginSystem = "Google",
                                GoogleSyncId = eventItem.Id,
                                GoogleSyncLastModified = eventItem.Updated.Value,
                                IsRecurring = !string.IsNullOrWhiteSpace(eventItem.RecurringEventId)
                            };

                            // in Google all day events Start.DateTime is null 
                            if (string.IsNullOrEmpty(eventItem.Start.Date))
                            {
                                var utcStartTime = new Timezones().ConvertUserDateTimeToUtcCached(eventItem.Start.DateTime.Value, User, listTimezonesDstCached, listTimezonesCached);
                                var utcEndTime = new Timezones().ConvertUserDateTimeToUtcCached(eventItem.End.DateTime.Value, User, listTimezonesDstCached, listTimezonesCached);

                                gEvent.StartDateTime = utcStartTime;
                                gEvent.EndDateTime = utcEndTime;
                            }
                            else
                            {
                                if (!string.IsNullOrWhiteSpace(eventItem.Start.Date))
                                {
                                    gEvent.StartDate = DateTime.ParseExact(eventItem.Start.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                    gEvent.StartDateTime = DateTime.ParseExact(eventItem.Start.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                                    gEvent.EndDateTime = DateTime.ParseExact(eventItem.Start.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                                    var utcStartDate = new Timezones().ConvertUserDateTimeToUtcCached(gEvent.StartDate.Value, User, listTimezonesDstCached, listTimezonesCached);
                                    gEvent.StartDateTime = utcStartDate;
                                    gEvent.EndDateTime = utcStartDate;
                                }

                                gEvent.IsAllDay = true;
                            }

                            DateTime startDateFromGoogleInUtc;
                            DateTime endDateFromGoogleInUtc;

                            if (gEvent.IsAllDay == false)
                            {
                                if (DateTime.TryParseExact(eventItem.Start.DateTimeRaw.Replace("T", " ").Replace("Z", ""), "yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out startDateFromGoogleInUtc))
                                    //gEvent.StartDateTime = new Timezones().ConvertUtcToUserDateTime(startDateFromGoogleInUtc, User.UserId);
                                    gEvent.StartDateTime = startDateFromGoogleInUtc;

                                if (DateTime.TryParseExact(eventItem.End.DateTimeRaw.Replace("T", " ").Replace("Z", ""), "yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo, DateTimeStyles.None, out endDateFromGoogleInUtc))
                                    //gEvent.EndDateTime = new Timezones().ConvertUtcToUserDateTime(endDateFromGoogleInUtc, User.UserId);
                                    gEvent.EndDateTime = endDateFromGoogleInUtc;
                            }

                            //If the event is marked for deletion, add it as a new activity anyway since deletion is
                            //then carried out from this final list, where .IsDeleted = true.
                            if (gEvent.IsRecurring.HasValue && gEvent.IsRecurring.Value && gEvent.Deleted == false)
                            {
                                var countOfRecurrenceId = listRecurringEventsAlreadyAdded.Count(x => x.RecurringEventId.Equals(eventItem.RecurringEventId));

                                //Only add first x days of a recurring event.
                                if (countOfRecurrenceId < RecurringEventProperties.DailyNumberOfCalendarEvents)
                                {
                                    listRecurringEventsAlreadyAdded.Add(eventItem);
                                    googleEvents.Add(gEvent);
                                }
                            }
                            else
                            {
                                googleEvents.Add(gEvent);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No events found.");
                    }
                    SyncToken = events.NextSyncToken;
                    pageToken = events.NextPageToken;
                }
                catch (Google.GoogleApiException ex)
                {
                    if (ex.Error.Code == 410)
                    {
                        SyncToken = "";
                        GetGoogleEvents();
                    }
                    pageToken = null;
                }
            } while (pageToken != null);

            return googleEvents;
        }

        private List<Event> GetRecurringEvents(string parentGoogleSyncId)
        {
            string error = "";
            var service = GetCalendarService(out error);

            var request = service.Events.Instances("primary", parentGoogleSyncId);

            request.TimeZone = "UTC";

            // retrieve the events, one page at a time.
            string pageToken = null;
            Events events = null;

            List<Event> listRecurringEvents = new List<Event>();

            do
            {
                try
                {
                    request.PageToken = pageToken;
                    events = request.Execute();

                    if (events.Items != null && events.Items.Count > 0)
                    {
                        var items = events.Items.OrderBy(x => x.Start.DateTime);

                        foreach (var eventItem in items)
                        {
                            // ignore private appointments
                            if (eventItem.Visibility == "private")
                            {
                                continue;
                            }

                            listRecurringEvents.Add(eventItem);
                        }
                    }

                    pageToken = events.NextPageToken;
                }
                catch (Google.GoogleApiException ex)
                {
                    pageToken = null;
                }
            } while (pageToken != null);

            return listRecurringEvents;
        }

        /// <summary>
        /// search google event in CRM by Id
        /// </summary>
        /// <param name="googleSyncId"></param>
        /// <returns></returns>
        private int SearchCrmForGoogleEvent(string googleSyncId)
        {
            var activityId = 0;

            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                            .GlobalSubscribers.Where(t => t.SubscriberId == User.SubscriberId)
                                                            .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var crmEvent = sharedContext.Activities.FirstOrDefault(t => t.GoogleSyncId == googleSyncId && t.SubscriberId == User.SubscriberId);
            if (crmEvent != null)
            {
                activityId = crmEvent.ActivityId;
            }
            else
            {
                //Recurring events in Google have a different GoogleSyncId than when first added. If no Google Sync Id matches in the database
                //then get the date and time and prefix of the GoogleSyncId, and match based on those.
                activityId = GetParentActivityOnNonMatchingGoogleSyncId(googleSyncId)?.ActivityId ?? 0;

            }
            return activityId;
        }

        private Activity GetParentActivityOnNonMatchingGoogleSyncId(string googleSyncId)
        {
            if (googleSyncId.Contains("_"))
            {
                var deconstructedGoogleSyncId = googleSyncId.Split('_');

                var parentId = deconstructedGoogleSyncId[0];
                var timestamp = deconstructedGoogleSyncId[1];

                var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();
                var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
                var sharedContext = new DbSharedDataContext(sharedConnection);

                var parentEvent = sharedContext.Activities.FirstOrDefault(x => x.GoogleSyncId == parentId);

                if (parentEvent != null)
                {
                    //We have a match - this GoogleSyncId is referring to the parent ID.
                    if (timestamp.StartsWith(parentEvent.ActivityDate.ToString("yyyyMMdd")))
                    {
                        return parentEvent;
                    }
                }
            }

            return null;
        }


        /// <summary>
        /// search event by subject, location and start/end dates
        /// </summary>
        /// <param name="googleEvent"></param>
        /// <returns></returns>
        private int SearchForGoogleEventInCrm(Activity googleEvent)
        {
            var activityId = 0;

            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                             .GlobalSubscribers.Where(t => t.SubscriberId == User.SubscriberId)
                                                             .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetWritableSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // search the event by subject, location and start date 
            var crmActivity = sharedContext.Activities.FirstOrDefault(t => !t.Deleted &&
                                                    t.Subject == googleEvent.Subject &&
                                                    t.StartDateTime == googleEvent.StartDateTime &&
                                                    t.SubscriberId == User.SubscriberId);
            if (crmActivity != null)
            {
                crmActivity.GoogleSyncId = googleEvent.GoogleSyncId;
                sharedContext.SubmitChanges();
                activityId = crmActivity.ActivityId;
            }
            else
            {

            }
            return activityId;
        }
    }
}