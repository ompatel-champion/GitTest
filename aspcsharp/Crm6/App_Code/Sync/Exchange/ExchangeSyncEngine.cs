using Crm6.App_Code;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Linq;
using System.Net;

namespace Helpers.Sync
{
    public class ExchangeSyncEngine
    {
        public ExchangeService ExService { get; set; }
        public int UserId { get; set; }
        public int SubscriberId { get; set; }
        public string EmailAddress { get; set; }
        public bool IsValidated { get; set; }
        public string FullName { get; set; }
        public User User { get; set; }
        public ServerVersion ExchangeServerVersion;
        public string Connection { get; set; }
        public string SyncState { get; set; }

        public async System.Threading.Tasks.Task InitEws(SyncUser syncUser)
        {
            Connection = syncUser.Connection;
            var context = new DbFirstFreightDataContext(Connection);
            UserId = syncUser.UserId;
            SyncState = string.IsNullOrEmpty(syncUser.SyncState) ? null : syncUser.SyncState;
            var user = context.Users.FirstOrDefault(t => t.UserId == syncUser.UserId);
            if (user != null)
            {
                var subscriber = context.Subscribers.FirstOrDefault(t => t.SubscriberId == user.SubscriberId);
                SubscriberId = user.SubscriberId;
                FullName = user.FullName;
                EmailAddress = syncUser.SyncEmail;

                if (syncUser.SyncType == "Office365")
                {
                    ExService = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
                    ExService.Credentials = new WebCredentials(syncUser.SyncEmail, syncUser.SyncPassword);
                    ExService.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");
                }
                else if (syncUser.SyncType == "Exchange")
                {
                    // Create the binding.
                    ExService = new ExchangeService(ExchangeVersion.Exchange2013_SP1);
                    //   ExService.Credentials = new WebCredentials("lurun", "Qwer1234", "center.intranet.sinotrans");
                    ExService.Credentials = new WebCredentials(syncUser.SyncUsername, syncUser.SyncPassword, subscriber.SyncServiceDomain);
                    // Set the URL.
                    // https://owa.sinotrans.com/EWS/Exchange.asmx"
                    ExService.Url = new Uri(subscriber.SyncServiceUrl);
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                }
                User = user;
            }
        }


        public bool ValidateSyncUser()
        {
            var isValidExchangeSyncUser = false;
            try
            {
                var view = new ItemView(1)
                {
                    PropertySet = new PropertySet(BasePropertySet.IdOnly, ItemSchema.LastModifiedTime)
                };
                if (EmailAddress.Length > 0)
                {
                    ExService.FindItems(new FolderId(WellKnownFolderName.Contacts), view);
                    isValidExchangeSyncUser = true;
                    // Update LoginFailues to 0
                    ResetExchangeLoginFailures();
                }
                else
                {
                    // Increment LoginFailures field in Users
                    IncrementExchangeLoginFailures();
                }
            }
            catch (Exception ex)
            {
                // Log Error
                LogExchangeSyncError(2, ex.Message, "999 - ValidateSyncUser - Invalid Exchange Sync User: " + EmailAddress);
                isValidExchangeSyncUser = false;
                // Increment LoginFailures field in Users
                IncrementExchangeLoginFailures();
            }
            return isValidExchangeSyncUser;
        }


        public void UpdateAppointmentsLastSyncdate(DateTime dt)
        {
            var context = new DbFirstFreightDataContext(Connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == UserId);
            if (user != null)
            {
                user.SyncAppointmentsLastDateTime = dt;
                context.SubmitChanges();
            }
        }

        public void UpdateSyncState()
        {
            var context = new DbFirstFreightDataContext(Connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == UserId);
            if (user != null)
            {
                user.SyncState = SyncState;
                context.SubmitChanges();
            }
        }


        public void IncrementExchangeLoginFailures()
        {
            var context = new DbFirstFreightDataContext(Connection);
            var users = context.Users.Where(u => u.UserId == UserId).Select(u => u).FirstOrDefault();
            if (users != null)
            {
                users.LoginFailures = users.LoginFailures + 1;
                context.SubmitChanges();
            }
        }


        public void ResetExchangeLoginFailures()
        {
            var context = new DbFirstFreightDataContext(Connection);
            var users = context.Users.Where(u => u.UserId == UserId).Select(u => u).FirstOrDefault();
            if (users != null)
            {
                users.LoginFailures = 0;
                context.SubmitChanges();
            }
        }


        private bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // Default for the validation callback is to reject the Url
            var result = false;
            var redirectionUri = new Uri(redirectionUrl);
            // Redirection Url is considered valid if it is using HTTPS to encrypt authentication credentials
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            }
            return result;
        }


        public string CrmOrExchangeIsLastestVersion(DateTime? crmLastModified, DateTime? exchangeLastModified, DateTime? syncLastModified)
        {
            var updateType = "None";
            if (crmLastModified == null)
            {
                // =================================================================
                // CRM NO LastModified - Compare with ExchangeLastModified
                // =================================================================
                if (exchangeLastModified != null)
                {
                    updateType = "Exchange";
                }
            }
            else
            {
                if (exchangeLastModified == null)
                {
                    // CRM HAS LastModified - Exchange NULL LastModified
                    updateType = "CRM";
                }
                else
                {
                    // =================================================================
                    // Compare Exchange with crmLastModified (CRM)
                    // =================================================================
                    if (crmLastModified > syncLastModified)
                    {
                        updateType = "CRM";
                    }
                    else if (crmLastModified == exchangeLastModified)
                    {
                        // -----------------------------------------------------------------
                        // CRM and Exchange Match
                        // -----------------------------------------------------------------
                        updateType = "None";
                    }
                    else if (crmLastModified < exchangeLastModified)
                    {
                        // -----------------------------------------------------------------
                        // Exchange LastModified > syncLastModified from CRM
                        // -----------------------------------------------------------------
                        updateType = "Exchange";
                    }
                }
            }
            return updateType;
        }


        public string GetExchangeServerTimeZone()
        {
            var exchangeServerTimezone = "";
            var context = new DbFirstFreightDataContext(Connection);
            var subscriber = context.Subscribers.FirstOrDefault(s => s.SubscriberId == SubscriberId);
            if (subscriber != null)
            {
                exchangeServerTimezone = subscriber.ExchangeServerTimeZone;
            }
            return exchangeServerTimezone;
        }


        public string GetEntryIdFromExchangeId(string exchangeId)
        {
            var contactEntryId = "";
            var altExchangeId = new AlternateId(IdFormat.EwsId, exchangeId, EmailAddress);
            var entryId = ExService.ConvertId(altExchangeId, IdFormat.EntryId) as AlternateId;
            if (entryId != null)
            {
                contactEntryId = Utils.Base64StringToHexString(entryId.UniqueId);
            }
            return contactEntryId;
        }


        public string GetExchangeIdFromEntryId(string entryId)
        {
            var basestring = Convert.ToBase64String(Utils.StringToByteArray(entryId.Trim()));
            var entry = new AlternateId(IdFormat.EntryId, basestring, EmailAddress);
            var exchangeId = ExService.ConvertId(entry, IdFormat.EwsId) as AlternateId;
            return exchangeId != null ? exchangeId.UniqueId : string.Empty;
        }


        public int LogExchangeSyncError(int errorCode, string errorMessage, string routineName)
        {
            var context = new DbFirstFreightDataContext(Connection);
            var error = new ExchangeSyncErrorLog();
            error.ErrorCode = errorCode.ToString();
            error.ErrorDateTime = DateTime.UtcNow;
            error.ErrorMessage = errorMessage;
            error.RoutineName = routineName;
            error.SubscriberId = User.SubscriberId;
            error.UserId = User.UserId;
            error.UserName = User.FullName;

            context.ExchangeSyncErrorLogs.InsertOnSubmit(error);
            context.SubmitChanges();

            return error.ExchangeSyncErrorLogId;
        }


        public int LogExchangeSync(string syncType, int contactId, int calendarEventId, string syncMessage)
        {
            var context = new DbFirstFreightDataContext(Connection);
            var log = new ExchangeSyncLog();

            log.CalendarEventId = calendarEventId;
            log.ContactId = contactId;
            log.SubscriberId = User.SubscriberId;
            log.SyncMessage = syncMessage;          // details of data
            log.SyncDateTime = DateTime.UtcNow;
            log.SyncType = syncType;                
            // CrmContactAdd / CrmContactUpdate / ExchangeContactAdd / ExchangeContactUpdate
            // CrmCalendarEventAdd / CrmCalendarEventUpdate / ExchangeAppointmentAdd / ExchangeAppointmentUpdate
            log.UserId = User.UserId;
            log.UserName = User.FullName;

            // Add sync log record
            context.ExchangeSyncLogs.InsertOnSubmit(log);
            context.SubmitChanges();
            return log.ExchangeSyncLogId;
        }

    }
}
