using System;
using System.Collections.Generic;
using Microsoft.Exchange.WebServices.Data;
using Crm6.App_Code;
using System.Linq;
using System.Text;
using Crm6.App_Code.Shared;

namespace Helpers.Sync
{

    public class ExchangeAppointmentSync
    {
        public ExchangeSyncEngine SyncEngine { get; set; }
        // ExchangeAppointmentId Extended Property - for matching Exchange and CRM Appointments
        private static readonly PropertyDefinitionBase ExchangeAppointmentIdPropertyDefinition = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, "ffcrmSyncAppointmentId", MapiPropertyType.String);
        public static readonly PropertySet PropertySet = new PropertySet(BasePropertySet.FirstClassProperties, ExchangeAppointmentIdPropertyDefinition);

        // CrmCalendarEventId Extended Property - for matching Exchange and CRM Appointments
        private static readonly PropertyDefinitionBase CrmCalendarEventIdPropertyDefinition = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, "ffcrmActivityId", MapiPropertyType.String);
        public static PropertySet PropertySetCrmCalendarEventId = new PropertySet(BasePropertySet.FirstClassProperties, CrmCalendarEventIdPropertyDefinition);


        // constructor with sync engine object
        // constructor with sync engine object
        public ExchangeAppointmentSync(ExchangeSyncEngine _syncEngine)
        {
            SyncEngine = _syncEngine;
        }


        public void RunAppointmentsSync()
        {
            // log sync
            SyncEngine.LogExchangeSync("Exchange Sync", 0, 0, "Sync Started.");

            // Add / Update Appointments in Exchange and Calendar Events in CRM
            // Based on LastModifiedDate or CreatedDate + SyncAppointmentsLastDateTime

            // ================================================================================================================
            // Get List of CRM Calendar Events and List of Exchange Appointments
            // ================================================================================================================
            List<Activity> crmCalendarEvents = null;
            List<Activity> exchangeAppointments = null;

            try
            {

                if (SyncEngine.User.SyncAppointmentsLastDateTime == null)
                {
                    // ========================================================================================================
                    // Initial Appointment Sync
                    // ========================================================================================================

                    // Get List of ALL Exchange Appointments Starting from 1 day ago
                    SyncEngine.User.SyncAppointmentsLastDateTime = DateTime.UtcNow.AddDays(-1);
                    exchangeAppointments = GetExchangeAppointmentsForSyncUser(SyncEngine.User.SyncAppointmentsLastDateTime);

                    // Get List of ALL CRM Calendar Events
                    crmCalendarEvents = GetCrmCalendarEventsForSyncUser(null, DateTime.UtcNow);
                }
                else
                {
                    // ========================================================================================================
                    // Incremental Appointment Sync - Based On Last Modified Date or Created Date >= Last Appointment Sync Date
                    // ========================================================================================================

                    // Go 10 Seconds Back for Last Sync - for Whatever Variances...
                    var appointmentLastSyncTime = SyncEngine.User.SyncAppointmentsLastDateTime.Value.AddSeconds(-10);

                    // Get List of Created/Modified Exchange Appointments
                    exchangeAppointments = GetExchangeAppointmentsForSyncUser(null, appointmentLastSyncTime);

                    // Get List of Created/Modified CRM Appointments
                    crmCalendarEvents = GetCrmCalendarEventsForSyncUser(SyncEngine.User.SyncAppointmentsLastDateTime);
                }
            }
            catch (Exception ex)
            {
                // 100 Errors are for Appointments / Calendar
                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "101-1 - ManageAppointmentsSync - Error retrieving new/modified Appointments");
            }

            // CRM Updates Exchange
            if (crmCalendarEvents != null && crmCalendarEvents.Count > 0)
            {
                CrmCalendarEventsUpdateExchangeAppointments(crmCalendarEvents, exchangeAppointments);
            }

            // Exchange Updates CRM
            if (exchangeAppointments != null && exchangeAppointments.Count > 0)
            {
                ExchangeAppointmentsUpdateCrmCalendarEvents(crmCalendarEvents, exchangeAppointments);
            }

            // Manage Deleted Exchange Appointments
            ManageDeletedExchangeAppointments();

            // Manage Deleted crm Appointments
            ManageDeletedCrmAppointments();

            // update last sync date
            SyncEngine.UpdateAppointmentsLastSyncdate(DateTime.UtcNow.AddSeconds(10));

            // update sync state
            SyncEngine.UpdateSyncState();

            // log sync
            SyncEngine.LogExchangeSync("Exchange Sync", 0, 0, "Sync Finished.");

        }


        private string AddExchangeAppointmentUsingCrmCalendarEvent(Activity crmCalendarEvent, ref DateTime? exchangeLastModifiedTime)
        {
            string exchangeAppointmentId = "";

            try
            {
                var exchangeAppointment = new Appointment(SyncEngine.ExService);

                // Get Exchange Server TimeZone from tblSubscribers
                var exchangeServerTimeZone = SyncEngine.GetExchangeServerTimeZone();
                if (string.IsNullOrEmpty(exchangeServerTimeZone)) exchangeServerTimeZone = "UTC";
                var tz = TimeZoneInfo.FindSystemTimeZoneById(exchangeServerTimeZone);

                // Start DateTime

                if (crmCalendarEvent.StartDateTime.HasValue)
                    exchangeAppointment.Start = TimeZoneInfo.ConvertTime(crmCalendarEvent.StartDateTime.Value, tz);

                exchangeAppointment.StartTimeZone = tz;
                if (SyncEngine.ExchangeServerVersion != ServerVersion.Ex2007Sp1)
                {
                    // 2010 SP1 and later
                    exchangeAppointment.EndTimeZone = tz;
                    // End DateTime
                    if (crmCalendarEvent.EndDateTime != null)
                    {
                        exchangeAppointment.End = TimeZoneInfo.ConvertTime(crmCalendarEvent.EndDateTime.GetValueOrDefault(), tz);
                    }
                }
                else
                {
                    // 2007 SP1
                    // End DateTime
                    if (crmCalendarEvent.EndDateTime != null)
                    {
                        //exchangeAppointment.End = crmCalendarEvent.UtcActivityEndDate.Value;
                        exchangeAppointment.End = TimeZoneInfo.ConvertTime(crmCalendarEvent.EndDateTime.Value, tz);
                    }
                }
                //  exchangeAppointment.Body = crmCalendarEvent.Description;
                exchangeAppointment.Subject = crmCalendarEvent.Subject;
                exchangeAppointment.Location = crmCalendarEvent.Location;
                exchangeAppointment.IsAllDayEvent = crmCalendarEvent.IsAllDay;
                if (exchangeAppointment.IsAllDayEvent)
                {
                    if (crmCalendarEvent.StartDateTime.HasValue)
                        exchangeAppointment.End = TimeZoneInfo.ConvertTime(crmCalendarEvent.StartDateTime.Value, tz);
                }
                //exchangeAppointment.IsRecurring = Util.CheckBoolean(crmCalendarEvent.IsRecurring);

                // ========================================================================

                // TODO: set category color for event
                var categoryColor = crmCalendarEvent.CategoryColor;
                exchangeAppointment.Categories.Add(categoryColor);
                exchangeAppointment.Categories.Add("CRM");

                if (crmCalendarEvent.ReminderMinutes.HasValue && crmCalendarEvent.ReminderMinutes.Value > 0)
                    exchangeAppointment.ReminderMinutesBeforeStart = crmCalendarEvent.ReminderMinutes.Value;

                // TODO: don't sync private calendar events
                // Appointment.Load(PropertySet.FirstClassProperties)
                // Item.Sensitivity

                //exchangeAppointment.AppointmentReplyTime
                //exchangeAppointment.AppointmentState - appointment, a meeting, a response to a meeting, or a cancelled meeting
                //exchangeAppointment.AppointmentType - used for recurring appointments
                //exchangeAppointment.ConferenceType -  type of conferencing that will be used during the meeting
                //exchangeAppointment.EnhancedLocation
                //exchangeAppointment.IsCancelled
                //exchangeAppointment.IsMeeting
                //exchangeAppointment.IsOnlineMeeting
                //exchangeAppointment.IsResponseRequested
                //exchangeAppointment.JoinOnlineMeetingUrl
                //exchangeAppointment.Location
                //exchangeAppointment.MeetingRequestWasSent
                //exchangeAppointment.OnlineMeetingSettings
                //exchangeAppointment.OptionalAttendees
                //exchangeAppointment.Organizer
                //exchangeAppointment.RequiredAttendees
                //exchangeAppointment.MeetingWorkspaceUrl
                //exchangeAppointment.NetShowUrl
                //exchangeAppointment.Resources
                //exchangeAppointment.When

                // ========================================================================
                // Add Exchange Appointment and Get ID
                exchangeAppointment.Save(SendInvitationsMode.SendToNone);

                // Set ExchangeAppointmentId in Exchange
                SetGuidForAppointment(exchangeAppointment);
                exchangeAppointmentId = GetGuidForAppointement(exchangeAppointment);

                // Re-Bind to Exchange Appointment to get ACTUAL LastModifiedTime - Stupid Microsoft Fucks!
                var exchangeEntryId = GetExchangeEntryIdFromExchangeAppointmentId(exchangeAppointmentId);
                var exchangeItemId = SyncEngine.GetExchangeIdFromEntryId(exchangeEntryId);
                var exchangeUpdatedAppointment = Appointment.Bind(SyncEngine.ExService, new ItemId(exchangeItemId));

                // Update CRM SyncLastModified using Exchange LastModified
                exchangeLastModifiedTime = exchangeUpdatedAppointment.LastModifiedTime;
                UpdateCrmCalendarEventSyncLastModified(crmCalendarEvent.CalendarEventId, exchangeLastModifiedTime);
                // ========================================================================
                var syncMessage = crmCalendarEvent.Subject + " | " + crmCalendarEvent.StartDateTime /*+ " | " + crmCalendarEvent.Description*/;
                SyncEngine.LogExchangeSync("AddExchangeAppointment", crmCalendarEvent.CalendarEventId, 0, syncMessage);
            }
            catch (Exception ex)
            {

                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "106 - AddExchangeAppointmentUsingCrmCalendarEvent");
                exchangeAppointmentId = "Error";
            }
            return exchangeAppointmentId;
        }


        public void CrmCalendarEventsUpdateExchangeAppointments(List<Activity> crmAppointments, List<Activity> exchangeAppointments)
        {

            // =========================================================================================================
            // Iterate Through CRM Appointments for User and Add/Update Exchange
            // =========================================================================================================
            try
            {
                foreach (var crmCalendarEvent in crmAppointments)
                {
                    // Check if CRM Calendar Event is in Exchange
                    DateTime? syncLastModifiedTime = null;
                    var exchangeAppointmentId = "";
                    var crmCalendarEventId = 0;
                    SearchForExchangeAppointmentByExchangeAppointmentId(crmCalendarEvent,
                                                                        ref syncLastModifiedTime,
                                                                        ref exchangeAppointmentId,
                                                                        ref crmCalendarEventId);
                    if (syncLastModifiedTime == null)
                    {

                        // CRM Appointment NOT in Exchange - Add Appointment to Exchange
                        exchangeAppointmentId = AddExchangeAppointmentUsingCrmCalendarEvent(crmCalendarEvent, ref syncLastModifiedTime);

                        // Use exchangeAppointmentId from the New Exchange Appointment to Update CRM Appointment

                        var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == crmCalendarEvent.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

                        var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));
                        var calendarEvent = context.Activities.FirstOrDefault(c => c.CalendarEventId == crmCalendarEvent.CalendarEventId);
                        if (calendarEvent != null)
                        {
                            calendarEvent.ExchangeAppointmentId = exchangeAppointmentId;
                            calendarEvent.ExchangeSyncLastModified = syncLastModifiedTime;
                        }
                        context.SubmitChanges();
                    }
                    else
                    {
                        // CRM Appointment IS in Exchange
                        var crmLastModified = crmCalendarEvent.LastUpdate;
                        var exchangeLastModified = syncLastModifiedTime;
                        var syncLastModified = exchangeLastModified;
                        // Check if crmLastModified > exchangeLastModified
                        var syncWinner = SyncEngine.CrmOrExchangeIsLastestVersion(crmLastModified, exchangeLastModified, syncLastModified);
                        if (syncWinner == "CRM")
                        {
                            // CRM Appointment IS more recently modified - Update Exchange Appointment
                            UpdateExchangeAppointmentUsingCrmCalendarEvent(crmCalendarEvent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // 100 Errors are for Appointments / Calendar
                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "102 - CrmCalendarEventsUpdateExchangeAppointments - CRM => Exchange");
            }
        }


        /// <summary>
        /// Get the ExchangeAppointmentId Property for the Appointment
        /// </summary>
        /// <param name="appointment"></param>
        public string GetGuidForAppointement(Appointment appointment)
        {
            var result = "";
            try
            {
                appointment.Load(PropertySet);
                foreach (var extendedProperty in appointment.ExtendedProperties)
                {
                    if (extendedProperty.PropertyDefinition.Name == "ffcrmSyncAppointmentId")
                    {
                        result = extendedProperty.Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "111 - GetGuidForAppointement");
            }
            return result;
        }


        /// <summary>
        /// Text Body - Description for exchangeId
        /// </summary>
        /// <param name="exchangeId"></param>
        /// <param name="ews"></param>
        /// <returns>String - TextBody</returns>
        private string GetExchangeBodyFromExchangeId(string exchangeId)
        {
            var exchangeAppointment = Appointment.Bind(SyncEngine.ExService, new ItemId(exchangeId), new PropertySet(BasePropertySet.FirstClassProperties) { RequestedBodyType = BodyType.Text });
            var body = exchangeAppointment.Body;
            return body;
        }


        public string GetExchangeEntryIdFromExchangeAppointmentId(string exchangeAppointmentId)
        {
            var exchangeEntryId = "";
            var view = new ItemView(1)
            {
                PropertySet = new PropertySet(BasePropertySet.IdOnly)
            };
            // Set Search Filter for Extended Property - exchangeAppointmentId
            var searchFilter = new SearchFilter.IsEqualTo(ExchangeAppointmentIdPropertyDefinition, exchangeAppointmentId);
            // Do the Find Query
            var resultsFound = SyncEngine.ExService.FindItems(new FolderId(WellKnownFolderName.Calendar), searchFilter, view);
            foreach (var item in resultsFound)
            {
                exchangeEntryId = SyncEngine.GetEntryIdFromExchangeId(item.Id.ToString());
            }
            return exchangeEntryId;
        }


        public void ExchangeAppointmentsUpdateCrmCalendarEvents(List<Activity> crmCalendarEvents, List<Activity> exchangeAppointments)
        {
            // =========================================================================================================
            // Iterate Through Exchange Appointments for User and Add/Update CRM
            // =========================================================================================================
            try
            {
                foreach (var exchangeAppointment in exchangeAppointments)
                {
                    // Check if CRM Appointment is in Exchange - Using ExchangeAppointmentId
                    // ---------------------------------------------------------------------------------------------------------
                    var exchangeAppointmentId = exchangeAppointment.ExchangeAppointmentId;
                    var exchangeLastModified = exchangeAppointment.LastUpdate;
                    var crmCalendarEventId = 0;
                    DateTime? crmLastModified = null;

                    exchangeAppointment.OriginSystem = "Exchange";
                    var entryId = GetExchangeEntryIdFromExchangeAppointmentId(exchangeAppointment.ExchangeAppointmentId);
                    var exchangeItemId = SyncEngine.GetExchangeIdFromEntryId(entryId);
                    exchangeAppointment.ExchangeAppointmentItemId = exchangeItemId;

                    // Initialize CRM Appointment Object
                    bool found = SearchForCrmCalendarEventByExchangeAppointmentId(exchangeAppointmentId, exchangeItemId, ref crmLastModified, ref crmCalendarEventId);


                    // CRM SyncLastModified
                    var syncLastModified = exchangeAppointment.ExchangeSyncLastModified;
                    // exchangeAppointment.Description = GetExchangeBodyFromExchangeId(exchangeItemId);
                    // ---------------------------------------------------------------------------------------------------------
                    if (found == false)
                    {
                        // Exchange Appointment NOT in CRM - Add Appointment to CRM
                        exchangeAppointment.OriginSystem = "Exchange";
                        crmCalendarEventId = AddCrmCalendarEvent(exchangeAppointment, syncLastModified);
                        //if (crmCalendarEventId <= 0) continue;
                        // Add crmAppointmentId into Exchange Extended Property
                        SetCrmCalendarEventIdUsingExchangeAppointmentId(exchangeAppointmentId, crmCalendarEventId);
                    }
                    else
                    {
                        // Exchange Appointment IS in CRM
                        // Check if exchangeLastModified > crmLastModified
                        var syncWinner = SyncEngine.CrmOrExchangeIsLastestVersion(crmLastModified, exchangeLastModified, syncLastModified);
                        if (syncWinner != "Exchange") continue;
                        // Exchange Appointment IS more recently modified - Update CRM Appointment
                        UpdateCrmCalendarEvent(crmCalendarEventId, exchangeAppointment, syncLastModified);
                    }
                }
            }
            catch (Exception ex)
            {

                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "103 - CrmCalendarEventsUpdateExchangeAppointments - CRM => Exchange");
            }
        }


        private void SearchForExchangeAppointmentByExchangeAppointmentId(Activity crmAppointment,
                                                    ref DateTime? syncLastModifiedTime,
                                                    ref string exchangeAppointmentId,
                                                    ref int crmCalendarEventId)
        {
            try
            {
                // Check if Appointment ExchangeAppointmentId from CRM Appointment matches Exchange Appointment
                if (!string.IsNullOrEmpty(crmAppointment.ExchangeAppointmentId))
                {
                    exchangeAppointmentId = crmAppointment.ExchangeAppointmentId.Trim();
                    crmCalendarEventId = crmAppointment.CalendarEventId;
                    var view = new ItemView(1)
                    {
                        PropertySet = new PropertySet(
                            BasePropertySet.IdOnly,
                            ItemSchema.LastModifiedTime
                            )
                    };
                    // Set Search Filter for Extended Property - exchangeAppointmentId
                    var searchFilter = new SearchFilter.IsEqualTo(ExchangeAppointmentIdPropertyDefinition, exchangeAppointmentId);
                    // Do the Find Query
                    var resultsFound = SyncEngine.ExService.FindItems(new FolderId(WellKnownFolderName.Calendar), searchFilter, view);
                    foreach (var item in resultsFound)
                    {
                        syncLastModifiedTime = item.LastModifiedTime;
                    }
                }
            }
            catch (Exception ex)
            {
                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "104 - SearchForSyncAppointment");
            }
        }


        public void SetCrmCalendarEventIdForExchangeAppointment(Appointment appointment, int crmActivityId)
        {
            try
            {
                appointment.SetExtendedProperty((ExtendedPropertyDefinition)CrmCalendarEventIdPropertyDefinition, crmActivityId);
                appointment.Update(ConflictResolutionMode.AlwaysOverwrite, SendInvitationsOrCancellationsMode.SendToNone);
            }
            catch (Exception ex)
            {
                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "110.121 - SetCrmCalendarEventIdForExchangeAppointment");
            }
        }


        public int SetCrmCalendarEventIdUsingExchangeAppointmentId(string exchangeAppointmentId, int crmActivityId)
        {
            var records = 0;
            var searchFilter = new SearchFilter.IsEqualTo(ExchangeAppointmentIdPropertyDefinition, exchangeAppointmentId);
            var resultsFound = SyncEngine.ExService.FindItems(new FolderId(WellKnownFolderName.Calendar), searchFilter, new ItemView(10));
            foreach (var item in resultsFound)
            {
                var exchangeAppointment = (Appointment)item;
                try
                {
                    exchangeAppointment.SetExtendedProperty((ExtendedPropertyDefinition)CrmCalendarEventIdPropertyDefinition, crmActivityId);
                    exchangeAppointment.Update(ConflictResolutionMode.AlwaysOverwrite, SendInvitationsOrCancellationsMode.SendToNone);
                }
                catch (Exception ex)
                {
                    SyncEngine.LogExchangeSyncError(2, ex.ToString(), "118 - SetCrmCalendarEventIdUsingExchangeAppointmentId");
                }
                records += 1;
            }
            return records;
        }


        /// <summary>
        /// Set the ExchangeAppointmentId Property for the Appointment
        /// </summary>
        /// <param name="appointment"></param>
        public void SetGuidForAppointment(Appointment appointment)
        {
            try
            {
                appointment.SetExtendedProperty((ExtendedPropertyDefinition)ExchangeAppointmentIdPropertyDefinition, Guid.NewGuid().ToString());
                appointment.Update(ConflictResolutionMode.AlwaysOverwrite, SendInvitationsOrCancellationsMode.SendToNone);
            }
            catch (Exception ex)
            {

                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "110 - SetGuidForAppointment");
            }
        }


        private void UpdateExchangeAppointmentUsingCrmCalendarEvent(Activity crmCalendarEvent)
        {
            try
            {
                // Get Exchange Appointment entrId from CRM Appointment
                var exchangeAppointmentId = crmCalendarEvent.ExchangeAppointmentId;
                var exchangeEntryId = GetExchangeEntryIdFromExchangeAppointmentId(exchangeAppointmentId);
                var exchangeItemId = SyncEngine.GetExchangeIdFromEntryId(exchangeEntryId);

                var exchangeAppointment = Appointment.Bind(SyncEngine.ExService, new ItemId(exchangeItemId));

                // Set TimeZone to Server's Local TimeZone (UTC)
                exchangeAppointment.StartTimeZone = TimeZoneInfo.Local;
                exchangeAppointment.EndTimeZone = TimeZoneInfo.Local;
                exchangeAppointment.Start = crmCalendarEvent.StartDateTime.Value;

                if (crmCalendarEvent.EndDateTime != null)
                {
                    exchangeAppointment.End = crmCalendarEvent.EndDateTime.Value;
                }

                // exchangeAppointment.Body = crmCalendarEvent.Description;
                exchangeAppointment.Subject = crmCalendarEvent.Subject;
                exchangeAppointment.Location = crmCalendarEvent.Location;
                exchangeAppointment.IsAllDayEvent = crmCalendarEvent.IsAllDay;

                if (crmCalendarEvent.ReminderMinutes.HasValue && crmCalendarEvent.ReminderMinutes.Value > 0)
                    exchangeAppointment.ReminderMinutesBeforeStart = crmCalendarEvent.ReminderMinutes.Value;

                // TODO - Recurring Appointments
                //exchangeAppointment.IsRecurring = Util.CheckBoolean(crmCalendarEvent.IsRecurring;

                // Add 'CRM' as a Category if Not Already Set
                var itemCategories = exchangeAppointment.Categories;
                if (!itemCategories.Contains("CRM"))
                {
                    exchangeAppointment.Categories.Add("CRM");
                }

                // Update Exchange Appointment
                exchangeAppointment.Update(ConflictResolutionMode.AlwaysOverwrite, SendInvitationsOrCancellationsMode.SendToNone);

                // Re-Bind to Exchange Appointment to get ACTUAL LastModifiedTime - Stupid Microsoft Fucks!
                var exchangeUpdatedAppointment = Appointment.Bind(SyncEngine.ExService, new ItemId(exchangeItemId));

                // Update CRM SyncLastModified using Exchange LastModified
                DateTime? exchangeLastModifiedTime = exchangeUpdatedAppointment.LastModifiedTime;
                UpdateCrmCalendarEventSyncLastModified(crmCalendarEvent.CalendarEventId, exchangeLastModifiedTime.Value);

                // =====================================================================================
                var syncMessage = exchangeAppointment.Subject + " | " + exchangeAppointment.When;
                SyncEngine.LogExchangeSync("UpdateExchangeAppointment", 0, crmCalendarEvent.CalendarEventId, syncMessage);
            }
            catch (Exception ex)
            {
                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "107 - UpdateExchangeAppointmentUsingCrmCalendarEvent");
            }
        }


        #region * Appointment Lists *

        // Converts List of Exchange Appointments into CrmCalendarEventss List Type
        // Makes it Easier to Compare and Manage Sync
        public List<Activity> ConvertFilteredExchangeAppointmentsIntoCrmCalendarEvents(FindItemsResults<Item> exchangeFilterResultFound)
        {
            var crmAppointmentItems = new List<Activity>();
            try
            {
                if (exchangeFilterResultFound.TotalCount != 0)
                {
                    foreach (var item in exchangeFilterResultFound)
                    {
                        var exchangeAppointment = (Appointment)item;
                        try
                        {
                            // ignore private appointments
                            if (exchangeAppointment.Sensitivity == Sensitivity.Private)
                            {
                                continue;
                            }

                            //  exchangeAppointment. = AppointmentType.
                            // Get ExchangeAppointmentId Extended Property
                            var exchangeAppointmentId = GetGuidForAppointement(exchangeAppointment);
                            if (exchangeAppointmentId == "")
                            {
                                // Create ExchangeAppointmentId if It Does Not Exist
                                SetGuidForAppointment(exchangeAppointment);
                                exchangeAppointmentId = GetGuidForAppointement(exchangeAppointment);
                            }

                            var calendarEvent = new Activity
                            {
                                // Exchange stores dates as UTC - Convert to User TimeZone
                                CreatedDate = exchangeAppointment.DateTimeCreated,
                                DeletedDate = null,
                                Deleted = false,
                                Subject = exchangeAppointment.Subject,
                                Description = exchangeAppointment.Body,
                                Location = exchangeAppointment.Location,
                                IsAllDay = exchangeAppointment.IsAllDayEvent,
                                IsRecurring = exchangeAppointment.IsRecurring,
                                LastUpdate = exchangeAppointment.LastModifiedTime,
                                OriginSystem = "Exchange",
                                ExchangeAppointmentId = exchangeAppointmentId,
                                ExchangeSyncLastModified = exchangeAppointment.LastModifiedTime,
                                // Exchange Stores Dates in UTC
                                StartDateTime = exchangeAppointment.Start,
                                EndDateTime = exchangeAppointment.End
                            };

                            crmAppointmentItems.Add(calendarEvent);
                        }
                        catch (Exception ex)
                        {
                            SyncEngine.LogExchangeSyncError(2, ex.ToString(), "107 - ConvertFilteredExchangeAppointmentsIntoCrmCalendarEvents");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "110 - ConvertFilteredExchangeAppointmentsIntoCrmCalendarEvents");
            }
            return crmAppointmentItems;
        }


        public List<Activity> GetExchangeAppointmentsForSyncUser(DateTime? appointmentStartDate = null, DateTime? lastAppointmentSyncDate = null)
        {
            try
            {
                var view = new ItemView(1000)
                // Set Exchange Appointment Properties
                {
                    PropertySet = new PropertySet(
                        BasePropertySet.IdOnly,
                        ItemSchema.Subject,
                        AppointmentSchema.Start,
                        AppointmentSchema.End,
                        ItemSchema.DateTimeCreated,
                        AppointmentSchema.IsAllDayEvent,
                        AppointmentSchema.IsRecurring,
                        ItemSchema.LastModifiedTime,
                        AppointmentSchema.Location
                        )
                };
                // Set Default Filter for 'CRM' Category
                SearchFilter.SearchFilterCollection fCollection;
                // SearchFilter categoryFilter = new SearchFilter.IsEqualTo(ItemSchema.Categories, "CRM");

                //If there is no last sync date, get appointments from a year ago.
                if (lastAppointmentSyncDate == null)
                {
                    lastAppointmentSyncDate = DateTime.UtcNow.AddYears(-1);
                }

                SearchFilter lastSyncFilter = new SearchFilter.IsGreaterThan(ItemSchema.LastModifiedTime, lastAppointmentSyncDate);
                fCollection = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new[]
                                                                        {
                                                                                lastSyncFilter
                                                                        });

                var exchangeAppointmentsForUser = SyncEngine.ExService.FindItems(new FolderId(WellKnownFolderName.Calendar), fCollection, view);
                SyncEngine.ExService.LoadPropertiesForItems(exchangeAppointmentsForUser, PropertySet.FirstClassProperties);
                // Return List of Exchange Appointments
                return ConvertFilteredExchangeAppointmentsIntoCrmCalendarEvents(exchangeAppointmentsForUser);
            }
            catch (Exception ex)
            {
                //100 Errors are for Appointments / Calendar 
                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "108 - GetExchangeAppointmentsForSyncUser");
            }
            return null;
        }



        private void ManageDeletedExchangeAppointments()
        {
            try
            {
                // Initialize the flag that will indicate when there are no more changes.
                bool isEndOfChanges = false;

                // Call SyncFolderItems repeatedly until no more changes are available.
                // sSyncState represents the sync state value that was returned in the prior synchronization response.
                do
                {
                    // Get a list of changes (up to a maximum of 5) that have occurred on normal items in the Inbox folder since the prior synchronization.
                    ChangeCollection<ItemChange> icc = SyncEngine.ExService.SyncFolderItems(new FolderId(WellKnownFolderName.Calendar),
                        PropertySet.FirstClassProperties, null, 5, SyncFolderItemsScope.NormalItems, SyncEngine.SyncState);

                    if (icc.Count == 0)
                    {
                        Console.WriteLine("There are no item changes to synchronize.");
                    }
                    else
                    {
                        foreach (ItemChange ic in icc)
                        {
                            if (ic.ChangeType == ChangeType.Create)
                            {
                                //TODO: Create item on the client.
                            }
                            else if (ic.ChangeType == ChangeType.Update)
                            {
                                //TODO: Update item on the client.
                            }
                            else if (ic.ChangeType == ChangeType.Delete)
                            {
                                //TODO: Delete item on the client.

                                var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == SyncEngine.User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

                                var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));


                                var found = context.Activities.FirstOrDefault(c => c.ExchangeAppointmentItemId == ic.ItemId.UniqueId
                                && c.OwnerUserId == SyncEngine.UserId && !c.Deleted);
                                if (found != null)
                                {
                                    found.Deleted = true;
                                    found.DeletedDate = DateTime.Now;
                                    found.DeletedUserId = -50; // 
                                    found.DeletedUserName = "Exchange Sync";
                                    context.SubmitChanges();
                                    continue;
                                }
                                else
                                {
                                    // not found
                                    // if owner user id is different, check if the sync user is invites list
                                    found = (from t in context.Activities
                                             join j in context.ActivititesMembers on t.ActivityId equals j.ActivitiesId
                                             where t.ExchangeAppointmentItemId == ic.ItemId.UniqueId && !t.Deleted && j.UserId == SyncEngine.UserId && !j.Deleted
                                             select t).FirstOrDefault();
                                    if (found != null)
                                    {
                                        found.Deleted = true;
                                        found.DeletedDate = DateTime.Now;
                                        found.DeletedUserId = -50; // 
                                        found.DeletedUserName = "Exchange Sync";
                                        context.SubmitChanges();
                                        continue;
                                    }
                                }

                            }
                            else if (ic.ChangeType == ChangeType.ReadFlagChange)
                            {
                                //TODO: Update the item's read flag on the client.
                            }

                            Console.WriteLine("ChangeType: " + ic.ChangeType.ToString());
                            Console.WriteLine("ItemId: " + ic.ItemId.UniqueId);
                            if (ic.Item != null)
                            {
                                Console.WriteLine("Subject: " + ic.Item.Subject);
                            }
                            Console.WriteLine("===========");
                        }
                    }

                    // Save the sync state for use in future SyncFolderHierarchy calls.
                    SyncEngine.SyncState = icc.SyncState;

                    if (!icc.MoreChangesAvailable)
                    {
                        isEndOfChanges = true;
                    }
                } while (!isEndOfChanges);
            }
            catch (Exception ex)
            {

                SyncEngine.LogExchangeSyncError(2, ex.ToString(), "ManageDeletedExchangeAppointments");
            }

        }



        #endregion


        #region CRM Events

        public int AddCrmCalendarEvent(Activity syncAppointment, DateTime? syncLastModified)
        {
            if (syncLastModified == null) { syncLastModified = DateTime.UtcNow; }

            // only add if has start datetime
            if (syncAppointment.StartDateTime != null)
            {
                var eventDescription = syncAppointment.Description;
                if (!string.IsNullOrEmpty(syncAppointment.Description) && syncAppointment.Description.Length > 3999)
                {
                    syncAppointment.Description.Substring(1, 3999);
                }

                var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == SyncEngine.User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

                var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));
                var calendarEvent = new Activity();
                calendarEvent.CreatedDate = DateTime.UtcNow;
                calendarEvent.CreatedUserId = 999;
                calendarEvent.CreatedUserName = SyncEngine.User.SyncType;
                calendarEvent.ExchangeAppointmentItemId = syncAppointment.ExchangeAppointmentItemId;

                // calendarEvent.Description = eventDescription;

                calendarEvent.ActivityType = "EVENT";
                calendarEvent.EventType = "";
                calendarEvent.Description = syncAppointment.Description;
                calendarEvent.ExchangeAppointmentId = syncAppointment.ExchangeAppointmentId;
                calendarEvent.ExchangeSyncLastModified = syncLastModified;
                calendarEvent.StartDateTime = (DateTime)syncAppointment.StartDateTime;
                calendarEvent.ActivityDate = (DateTime)syncAppointment.StartDateTime;
                calendarEvent.EndDateTime = syncAppointment.EndDateTime;
                calendarEvent.IsAllDay = syncAppointment.IsAllDay;
                calendarEvent.IsRecurring = syncAppointment.IsRecurring;
                calendarEvent.LastUpdate = calendarEvent.CreatedDate;
                calendarEvent.Location = syncAppointment.Location;
                calendarEvent.OriginSystem = syncAppointment.OriginSystem;
                calendarEvent.OwnerUserId = SyncEngine.User.UserId;
                calendarEvent.UserIdGlobal = SyncEngine.User.UserIdGlobal;
                calendarEvent.OwnerUserIdGlobal = SyncEngine.User.UserIdGlobal;
                calendarEvent.ReoccurNumberOfTimes = 0;
                calendarEvent.Subject = syncAppointment.Subject;
                calendarEvent.SubscriberId = SyncEngine.User.SubscriberId;
                calendarEvent.UpdateUserId = 999;
                calendarEvent.UpdateUserName = SyncEngine.User.SyncType;
                calendarEvent.UserTimeZone = GetUserTimeZone(SyncEngine.User.UserId, SyncEngine.User.SubscriberId);
                calendarEvent.UtcOffset = "";

                //calendarEvent.CategoryName
                //calendarEvent.CategoryColor

                // add calendar event to CRM
                try
                {
                    context.Activities.InsertOnSubmit(calendarEvent);
                    context.SubmitChanges();
                }
                catch (Exception ex)
                {

                    SyncEngine.LogExchangeSyncError(2, ex.ToString(), "AddCrmCalendarEvent");
                }

                // add the invite
                if (calendarEvent.CalendarEventId > 0)
                    AddCalendarEventInvite(calendarEvent);

                // sync log - AddCalenderEvent
                var syncMessage = calendarEvent.Subject + " | " + calendarEvent.Description + " | Start Time: " + calendarEvent.StartDateTime + " | " + calendarEvent.CompanyName;
                SyncEngine.LogExchangeSync("AddCalendarEvent", 0, calendarEvent.CalendarEventId, syncMessage);

                // return calendar event id
                return calendarEvent.CalendarEventId;
            }
            // Error
            return 0;
        }


        public bool AddCalendarEventInvite(Activity calendarEventItem)
        {
            try
            {
                var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == SyncEngine.User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

                var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));

                // check in current event invites list
                var invite = context.ActivititesMembers.FirstOrDefault(t => t.ActivitiesId == calendarEventItem.ActivityId && t.UserId == SyncEngine.User.UserId && !t.Deleted);

                if (invite == null)
                {
                    invite = new ActivititesMember
                    {
                        ActivitiesId = calendarEventItem.ActivityId,
                        InviteType = "organizer",
                        UserId = SyncEngine.User.UserId,
                        UserName = SyncEngine.User.FullName ?? "",
                        SubscriberId = calendarEventItem.SubscriberId,
                        CreatedUserId = calendarEventItem.UpdateUserId,
                        CreatedDate = DateTime.UtcNow,
                        CreatedUserName = calendarEventItem.UpdateUserName,
                        LastUpdate = DateTime.UtcNow,
                        UpdateUserId = calendarEventItem.UpdateUserId,
                        UpdateUserName = calendarEventItem.UpdateUserName,
                        AttendeeType = "Required"
                    };
                    context.ActivititesMembers.InsertOnSubmit(invite);
                    context.SubmitChanges();
                }
                return true;
            }
            catch (Exception)
            {
            }
            return false;
        }



        public List<Activity> GetCrmCalendarEventsForSyncUser(DateTime? lastmodifiedtime = null, DateTime? syncStartDateTime = null)
        {

            // TODO: make sure ALL future events included

            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == SyncEngine.User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

            var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));
            var calendarEvents = (from calendarEvent in context.Activities
                                  where !calendarEvent.Deleted
                                  select calendarEvent);

            // invited events
            var invitedEvents = context.ActivititesMembers.Where(t => t.UserId == SyncEngine.User.UserId && !t.Deleted)
                  .Select(t => t.ActivitiesId).Distinct().ToList();

            // filter for userId + subscriberId
            calendarEvents = calendarEvents.Where(c => c.SubscriberId == SyncEngine.User.SubscriberId &&
                                (c.OwnerUserId == SyncEngine.User.UserId || invitedEvents.Contains(c.CalendarEventId)));

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


        public bool SearchForCrmCalendarEventByExchangeAppointmentId(string exchangeAppointmentId, string exchangeItemId, ref DateTime? crmLastUpdate, ref int crmCalendarEventId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == SyncEngine.User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

            // Check if Appointment SyncId in Office365 / Exchange matches CRM Calendar Event
            var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));


            // find the appointment by exchange unique id   
            var found = context.Activities.FirstOrDefault(c => c.ExchangeAppointmentItemId == exchangeItemId && c.OwnerUserId == SyncEngine.UserId && !c.Deleted);
            if (found != null)
            {
                crmLastUpdate = found.LastUpdate;
                crmCalendarEventId = found.CalendarEventId;
                return true;
            }

            // not found
            // if owner user id is different, check if the sync user is in invites list
            found = (from t in context.Activities
                     join j in context.ActivititesMembers on t.ActivityId equals j.ActivitiesId
                     where t.ExchangeAppointmentItemId == exchangeItemId && !t.Deleted && j.UserId == SyncEngine.UserId && !j.Deleted
                     select t).FirstOrDefault();
            if (found != null)
            {
                crmLastUpdate = found.LastUpdate;
                crmCalendarEventId = found.CalendarEventId;
                return true;
            }

            // find the appointment by extended property 
            found = context.Activities.FirstOrDefault(c => c.ExchangeAppointmentId == exchangeAppointmentId && c.OwnerUserId == SyncEngine.UserId && !c.Deleted);
            if (found != null)
            {
                crmLastUpdate = found.LastUpdate;
                crmCalendarEventId = found.CalendarEventId;
                return true;
            }
            else
            {
                // not found
                // if owner user id is different, check if the sync user is ininvites list
                found = (from t in context.Activities
                         join j in context.ActivititesMembers on t.ActivityId equals j.ActivitiesId
                         where t.ExchangeAppointmentId == exchangeAppointmentId && !t.Deleted && j.UserId == SyncEngine.UserId && !j.Deleted
                         select t).FirstOrDefault();
                if (found != null)
                {
                    crmLastUpdate = found.LastUpdate;
                    crmCalendarEventId = found.CalendarEventId;
                    return true;
                }

                // no invite or owner user id matched, so mark as not found
                crmLastUpdate = null;
                crmCalendarEventId = 0;

                return false;
            }
        }


        public int UpdateCrmCalendarEvent(int calendarEventId, Activity syncAppointment, DateTime? syncLastModified)
        {
            var calendareEventId = 0;
            if (syncLastModified == null)
            {
                syncLastModified = DateTime.UtcNow;
            }

            // only update if has start datetime
            if (syncAppointment.StartDateTime != null)
            {
                var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == SyncEngine.User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

                var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));
                var calendarEvent = context.Activities.FirstOrDefault(c => c.CalendarEventId == calendarEventId);
                if (calendarEvent != null)
                {
                    // calendarEvent.Description = syncAppointment.Description;
                    calendarEvent.EndDateTime = syncAppointment.EndDateTime;
                    calendarEvent.ExchangeSyncLastModified = syncLastModified;
                    calendarEvent.IsAllDay = syncAppointment.IsAllDay;
                    calendarEvent.IsRecurring = syncAppointment.IsRecurring;
                    calendarEvent.LastUpdate = syncLastModified.GetValueOrDefault();
                    calendarEvent.Location = syncAppointment.Location;
                    calendarEvent.StartDateTime = (DateTime)syncAppointment.StartDateTime;
                    calendarEvent.Subject = syncAppointment.Subject;
                    calendarEvent.UpdateUserId = 999;
                    calendarEvent.UpdateUserName = SyncEngine.User.SyncType;
                    calendarEvent.ExchangeAppointmentItemId = syncAppointment.ExchangeAppointmentItemId;

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

                        SyncEngine.LogExchangeSyncError(2, ex.ToString(), "UpdateCrmCalendarEvent");
                    }

                    calendareEventId = calendarEvent.CalendarEventId;

                    // log sync calendar update
                    var syncMessage = calendarEvent.Subject + " | " + calendarEvent.Description + " | Start Time: " + calendarEvent.StartDateTime + " | " + calendarEvent.CompanyName;
                    SyncEngine.LogExchangeSync("UpdateCalendarEvent", 0, calendarEvent.CalendarEventId, syncMessage);
                }
            }

            return calendareEventId;
        }


        public string UpdateCrmCalendarEventSyncLastModified(int calendarEventId, DateTime? syncLastModified)
        {
            string response = "";
            if (syncLastModified == null)
            {
                syncLastModified = DateTime.UtcNow;
            }

            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == SyncEngine.User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

            var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));
            var calendarEvent = context.Activities.FirstOrDefault(c => c.CalendarEventId == calendarEventId);
            if (calendarEvent != null)
            {
                calendarEvent.LastUpdate = (DateTime)syncLastModified;
                calendarEvent.ExchangeSyncLastModified = syncLastModified;
                calendarEvent.UpdateUserId = 999;
                calendarEvent.UpdateUserName = SyncEngine.User.SyncType;
                context.SubmitChanges();
                response = "Updated CRM Calendar Event ExchangeSyncLastModified";
            }

            return response;
        }



        public static void AddCategory(CalendarFolder calendarFolder)
        {
            //int : Category Color Code
            ExtendedPropertyDefinition EpCategoryColorCode = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.Appointment, 33300, MapiPropertyType.Integer);
            //string: Category Name
            ExtendedPropertyDefinition epCategoryName = new ExtendedPropertyDefinition(DefaultExtendedPropertySet.PublicStrings, "Keywords", MapiPropertyType.StringArray);

            Appointment appointment = null;
            //Load Categories property of appointme\nt
            appointment.Load(new PropertySet(AppointmentSchema.Categories));
            //set Categories property,
            string[] categoryName = new string[1] { "Red Category" };
            appointment.Categories = new StringList(categoryName);
            appointment.Update(ConflictResolutionMode.AlwaysOverwrite, SendInvitationsOrCancellationsMode.SendToNone);

            SearchFilter _itemClassFilter = new SearchFilter.IsEqualTo(EmailMessageSchema.ItemClass, "IPM.Configuration.CategoryList");
            ExtendedPropertyDefinition categories = new ExtendedPropertyDefinition(0x7C08, MapiPropertyType.Binary);
            ItemView view = new ItemView(1);
            view.Traversal = ItemTraversal.Associated;

            FindItemsResults<Item> items = calendarFolder.FindItems(_itemClassFilter, view);
            if (items.TotalCount > 0)
            {
                try
                {
                    EmailMessage msg = items.First() as EmailMessage;

                    msg.Load(new PropertySet(ItemSchema.Id, categories));
                    Guid guid = Guid.NewGuid();
                    //string newCategory = Properties.Settings.Default.Category.Replace("%NAME%", category.Name).Replace("%COLOR%", category.Color.ToString()).Replace("%GUID%", "{" + guid.ToString() + "}");

                    string xmlString = Encoding.UTF8.GetString((byte[])msg.ExtendedProperties[0].Value);

                    //  xmlString = xmlString.Replace("</categories>", newCategory);
                    //  byte[] value = Encoding.UTF8.GetBytes(xmlString);
                    //  msg.SetExtendedProperty(categories, value);
                    msg.Update(ConflictResolutionMode.AlwaysOverwrite);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void ManageDeletedCrmAppointments()
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                           .GlobalSubscribers.Where(t => t.SubscriberId == SyncEngine.User.SubscriberId)
                                                           .Select(t => t.DataCenter).FirstOrDefault();

            var context = new DbSharedDataContext(LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter));
            var deletedEvents = (from t in context.Activities
                                 join j in context.ActivititesMembers on t.ActivityId equals j.ActivitiesId
                                 where t.Deleted && t.DeletedUserId != -50 && (j.UserId == SyncEngine.User.UserId || t.OwnerUserId == SyncEngine.User.UserId)
                                 && t.DeletedDate > SyncEngine.User.SyncAppointmentsLastDateTime && t.ExchangeAppointmentId != null
                                 select t).ToList();
            foreach (var crmCalendarEvent in deletedEvents)
            {
                try
                {
                    var exchangeAppointmentId = crmCalendarEvent.ExchangeAppointmentId;
                    var exchangeEntryId = GetExchangeEntryIdFromExchangeAppointmentId(exchangeAppointmentId);
                    var exchangeItemId = SyncEngine.GetExchangeIdFromEntryId(exchangeEntryId);
                    // Find Exchange Appointment Using exchangeEntryId
                    var exchangeAppointment = Appointment.Bind(SyncEngine.ExService, new ItemId(exchangeItemId));
                    if (exchangeAppointment != null)
                    {
                        exchangeAppointment.Delete(DeleteMode.MoveToDeletedItems, SendCancellationsMode.SendToNone);

                    }
                }
                catch (Exception ex)
                {

                    SyncEngine.LogExchangeSyncError(2, ex.ToString(), "108 - ManageDeletedCrmAppointments");
                }

            }
        }



        #endregion



        #region Helper Functions

        public string GetUserTimeZone(int userId, int subscriberId)
        {
            var context = new DbFirstFreightDataContext(SyncEngine.Connection);
            var timezone = context.Users.Where(u => u.UserId == userId)
                .Select(u => u.TimeZone).FirstOrDefault();
            return timezone;
        }

        #endregion


    }

}
