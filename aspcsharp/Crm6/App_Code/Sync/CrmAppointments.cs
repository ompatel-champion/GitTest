using System;
using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code.Login;
using Helpers.Sync;
using Crm6.App_Code.Shared;

namespace Crm6.App_Code.Sync
{
    public class CrmAppointments
    {
        /// <summary>
        /// get CRM calendar events for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="subscriberId"></param>
        /// <param name="connection"></param>
        /// <param name="lastmodifiedtime"></param>
        /// <param name="syncStartDateTime"></param>
        /// <returns></returns>
        public List<Activity> GetCrmCalendarEventsForSyncUser(int userId, int subscriberId, string connection, DateTime? lastmodifiedtime = null, DateTime? syncStartDateTime = null, bool onlyDeleted = false)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                             .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                             .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var sharedContext = new DbSharedDataContext(sharedConnection);



            var context = new DbFirstFreightDataContext(connection);
            var calendarEvents = (from calendarEvent in sharedContext.Activities
                                  where /*!calendarEvent.Deleted &&*/ (calendarEvent.CalendarEventId > 0 || calendarEvent.ActivityType == "EVENT")
                                  select calendarEvent);

            // invited events
            var invitedEvents = sharedContext.ActivititesMembers.Where(t => t.UserId == userId && !t.Deleted)
                                             .Select(t => t.ActivitiesId).Distinct().ToList();

            // filter for userId + subscriberId
            calendarEvents = calendarEvents.Where(c => c.SubscriberId == subscriberId &&
                                (c.OwnerUserId == userId || invitedEvents.Contains(c.ActivityId)));

            if (lastmodifiedtime != null)
            {
                // Not Initial Sync
                calendarEvents = calendarEvents.Where(c => c.LastUpdate > lastmodifiedtime);
            }
            if (syncStartDateTime != null)
            {
                // Initial Sync
                calendarEvents = calendarEvents.Where(c => c.StartDateTime > syncStartDateTime);
            }
            var calendarEventsListCrm = calendarEvents.OrderBy(c => c.StartDateTime).ToList();
            return calendarEventsListCrm;
        }

        /// <summary>
        /// update CRM calendar event sync last modified
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="calendarEventId"></param>
        /// <param name="syncLastModified"></param>
        /// <param name="syncType"></param>
        /// <returns></returns>
        public string UpdateCrmCalendarEventSyncLastModified(string connection, int calendarEventId, DateTime? syncLastModified, string syncType)
        {
            string response = "";
            if (syncLastModified == null)
            {
                syncLastModified = DateTime.UtcNow;
            }

            var context = new DbFirstFreightDataContext(connection);
            var calendarEvent = context.CalendarEvents.FirstOrDefault(c => c.CalendarEventId == calendarEventId);
            if (calendarEvent != null)
            {
                calendarEvent.LastUpdate = (DateTime)syncLastModified;
                if (syncType == "Google")
                    calendarEvent.GoogleSyncLastModified = syncLastModified;
                else if (syncType == "Exchange")
                    calendarEvent.ExchangeSyncLastModified = syncLastModified;
                else if (syncType == "Office365")
                    calendarEvent.ExchangeSyncLastModified = syncLastModified;

                calendarEvent.UpdateUserId = 999;
                calendarEvent.UpdateUserName = syncType;
                context.SubmitChanges();
                response = "Updated CRM Calendar Event SyncLastModified";
            }

            return response;
        }

        /// <summary>
        /// add crm calendar event
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="user"></param>
        /// <param name="syncAppointment"></param>
        /// <param name="syncLastModified"></param>
        /// <returns></returns>
        public void BulkAddCrmCalendarEventsFromGoogle(string connection, User user, List<Activity> listAppointments)
        {
            var invitesToAdd = new List<Activity>();

            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                               .GlobalSubscribers.Where(t => t.SubscriberId == user.SubscriberId)
                                                               .Select(t => t.DataCenter).FirstOrDefault();
            var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
            var context = new DbSharedDataContext(sharedConnection);

            foreach (Activity syncAppointment in listAppointments)
            {
                var syncLastModified = syncAppointment.GoogleSyncLastModified;

                if (syncLastModified == null) { syncLastModified = DateTime.UtcNow; }

                // only add if has start datetime
                if (syncAppointment.StartDateTime != null)
                {
                    var eventDescription = syncAppointment.Description;
                    if (!string.IsNullOrEmpty(syncAppointment.Description) && syncAppointment.Description.Length > 3999)
                    {
                        syncAppointment.Description.Substring(1, 3999);
                    }

                    var calendarEvent = new Activity();
                    calendarEvent.CreatedDate = DateTime.UtcNow;
                    calendarEvent.CreatedUserId = 999;
                    calendarEvent.CreatedUserName = user.SyncType;
                    if (user.SyncType == "Google")
                    {
                        calendarEvent.GoogleSyncLastModified = syncAppointment.GoogleSyncLastModified;
                        calendarEvent.GoogleSyncId = syncAppointment.GoogleSyncId;
                    }
                    else
                    {
                        calendarEvent.ExchangeAppointmentItemId = syncAppointment.ExchangeAppointmentItemId;
                        calendarEvent.ExchangeAppointmentId = syncAppointment.ExchangeAppointmentId;
                        calendarEvent.ExchangeSyncLastModified = syncLastModified;
                    }
                    // TODO: Description length seems to be the problem - also if error - continue syncing - don't just bomb because of ONE error on record add
                    // calendarEvent.Description = eventDescription;

                    calendarEvent.ActivityType = "EVENT";

                    calendarEvent.ActivityDate = (DateTime)syncAppointment.StartDateTime;
                    calendarEvent.StartDateTime = (DateTime)syncAppointment.StartDateTime;
                    calendarEvent.EndDateTime = syncAppointment.EndDateTime;
                    calendarEvent.IsAllDay = syncAppointment.IsAllDay;
                    calendarEvent.IsRecurring = syncAppointment.IsRecurring;
                    calendarEvent.ReoccurrenceIncrementType = syncAppointment.ReoccurrenceIncrementType;
                    calendarEvent.ReoccurrenceParentActivityId = syncAppointment.ReoccurrenceParentActivityId;

                    calendarEvent.LastUpdate = DateTime.UtcNow;

                    calendarEvent.Location = syncAppointment.Location;
                    calendarEvent.OriginSystem = syncAppointment.OriginSystem;
                    calendarEvent.OwnerUserId = user.UserId;
                    calendarEvent.ReoccurNumberOfTimes = 0;
                    calendarEvent.Subject = syncAppointment.Subject;
                    calendarEvent.SubscriberId = user.SubscriberId;
                    calendarEvent.UpdateUserId = 999;
                    calendarEvent.UpdateUserName = user.SyncType;
                    calendarEvent.UserTimeZone = new global::Helpers.Users().GetUserTimeZone(user.UserId, user.SubscriberId);
                    calendarEvent.UtcOffset = "";
                    calendarEvent.UserIdGlobal = user.UserIdGlobal;
                    calendarEvent.OwnerUserIdGlobal = user.UserIdGlobal;

                    // add calendar event to CRM
                    try
                    {
                        context.Activities.InsertOnSubmit(calendarEvent);
                    }
                    catch (Exception ex)
                    {
                        // Log Error
                        new SyncInitializer().LogSyncError(user, 2, ex.Message, "AddCrmCalendarEvent");
                    }

                    // add the invite
                    if (calendarEvent.CalendarEventId > 0)
                        invitesToAdd.Add(calendarEvent);

                    // sync log - AddCalenderEvent
                    var syncMessage = calendarEvent.Subject + " | " + calendarEvent.Description + " | Start Time: " + calendarEvent.StartDateTime + " | " + calendarEvent.CompanyName;
                    new SyncInitializer().LogSync(user, "AddCalendarEvent", 0, calendarEvent.CalendarEventId, syncMessage);
                }
            }

            context.SubmitChanges();

            AddCalendarEventInvites(user, connection, invitesToAdd);
        }



        /// <summary>
        /// update CRM calendar event
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="user"></param>
        /// <param name="calendarEventId"></param>
        /// <param name="syncAppointment"></param>
        /// <param name="syncLastModified"></param>
        /// <returns></returns>
        public int UpdateCrmCalendarEvent(string connection, User user, int calendarEventId, Activity syncAppointment, DateTime? syncLastModified)
        {
            var calendareEventId = 0;
            if (syncLastModified == null)
            {
                syncLastModified = DateTime.UtcNow;
            }

            // only update if has start date time
            if (syncAppointment.StartDateTime != null)
            {
                var context = new DbFirstFreightDataContext(connection);
                var calendarEvent = context.CalendarEvents.FirstOrDefault(c => c.CalendarEventId == calendarEventId);
                if (calendarEvent != null)
                {
                    if (user.SyncType == "Google")
                    {
                        calendarEvent.GoogleSyncLastModified = syncAppointment.GoogleSyncLastModified;
                        calendarEvent.GoogleSyncId = syncAppointment.GoogleSyncId;
                    }
                    else
                    {
                        calendarEvent.ExchangeAppointmentItemId = syncAppointment.ExchangeAppointmentItemId;
                        calendarEvent.ExchangeAppointmentId = syncAppointment.ExchangeAppointmentId;
                        calendarEvent.ExchangeSyncLastModified = syncLastModified;
                    }


                    //  calendarEvent.Description = syncAppointment.Description;
                    calendarEvent.EndDateTime = syncAppointment.EndDateTime;
                    calendarEvent.IsAllDay = syncAppointment.IsAllDay;
                    calendarEvent.IsRecurring = syncAppointment.IsRecurring;
                    calendarEvent.LastUpdate = syncLastModified.GetValueOrDefault();
                    calendarEvent.Location = syncAppointment.Location;
                    calendarEvent.StartDateTime = (DateTime)syncAppointment.StartDateTime;
                    calendarEvent.Subject = syncAppointment.Subject;
                    calendarEvent.UpdateUserId = 999;
                    calendarEvent.UpdateUserName = user.SyncType;

                    //calendarEvent.CategoryColor = syncAppointment.CategoryColor??
                    //calendarEvent.CategoryName = syncAppointment.CategoryName??

                    // organizer
                    // required attendees
                    // optional attendees

                    try
                    {
                        context.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        // Log Error
                        new SyncInitializer().LogSyncError(user, 2, ex.Message, "UpdateCrmCalendarEvent");
                    }

                    calendareEventId = calendarEvent.CalendarEventId;

                    // log sync calendar update
                    var syncMessage = calendarEvent.Subject + " | " + calendarEvent.Description + " | Start Time: " + calendarEvent.StartDateTime + " | " + calendarEvent.CompanyName;
                    new SyncInitializer().LogSync(user, "UpdateCalendarEvent", 0, calendarEvent.CalendarEventId, syncMessage);
                }
            }

            return calendareEventId;
        }


        /// <summary>
        /// add calendar event invite 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="connection"></param>
        /// <param name="calendarEventItem"></param>
        /// <returns></returns>
        private void AddCalendarEventInvites(User user, string connection, List<Activity> activities)
        {
            var context = new DbFirstFreightDataContext(connection);

            try
            {
                foreach (Activity calendarEventItem in activities)
                {
                    var invite = context.CalendarInvites.FirstOrDefault(t => t.CalendarEventId == calendarEventItem.CalendarEventId && t.UserId == user.UserId && !t.Deleted);

                    if (invite == null)
                    {
                        invite = new CalendarInvite
                        {
                            CalendarEventId = calendarEventItem.CalendarEventId,
                            InviteType = "organizer",
                            UserId = user.UserId,
                            UserName = user.FullName ?? "",
                            SubscriberId = calendarEventItem.SubscriberId,
                            CreatedUserId = calendarEventItem.UpdateUserId,
                            CreatedDate = DateTime.UtcNow,
                            CreatedUserName = calendarEventItem.UpdateUserName,
                            LastUpdate = DateTime.UtcNow,
                            UpdateUserId = calendarEventItem.UpdateUserId,
                            UpdateUserName = calendarEventItem.UpdateUserName,
                            AttendeeType = "Required"
                        };
                        context.CalendarInvites.InsertOnSubmit(invite);
                     
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                context.SubmitChanges();
            }
        }



        public bool AddCalendarEventInvite(string connection, User user, Activity calendarEventItem, string emailAddress)
        {
            try
            {
                var context = new DbFirstFreightDataContext(connection);

                // check in current event invites list
                var invite = context.CalendarInvites.FirstOrDefault(t => t.CalendarEventId == calendarEventItem.CalendarEventId && t.UserId == user.UserId && !t.Deleted);

                if (invite == null)
                {
                    invite = new CalendarInvite
                    {
                        CalendarEventId = calendarEventItem.CalendarEventId,
                        InviteType = "organizer",
                        UserId = user.UserId,
                        UserName = user.FullName ?? "",
                        SubscriberId = calendarEventItem.SubscriberId,
                        CreatedUserId = calendarEventItem.UpdateUserId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = calendarEventItem.UpdateUserName,
                        LastUpdate = DateTime.UtcNow,
                        UpdateUserId = calendarEventItem.UpdateUserId,
                        UpdateUserName = calendarEventItem.UpdateUserName,
                        AttendeeType = "Required"
                    };
                    context.CalendarInvites.InsertOnSubmit(invite);
                    context.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }



    }
}