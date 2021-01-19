using Crm6.App_Code;
using Crm6.App_Code.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpers.Sync
{
    public class SyncInitializer
    {
        public async Task<bool> SyncExchangeForUser(int userId, int subscriberId)
        {
            var user = new Users().GetUser(userId, subscriberId);
            if (user != null)
            {
                // get user connection string
                var connection = LoginUser.GetConnection();

                if (user.User.SyncType == "Office365")
                {
                    return await InitO365Sync(user.User, connection);
                }
                else if (user.User.SyncType == "Exchange")
                {
                    return await InitExchangeSync(user.User, connection);
                }
                else if (user.User.SyncType == "Google")
                {
                    //if (GoogleSyncTimings.LastGoogleSync.HasValue && GoogleSyncTimings.LastGoogleSync.Value.AddMinutes(GoogleSyncTimings.GoogleSyncIntervalMinutes) > DateTime.UtcNow)
                    //{
                    //    return false;
                    //}
                    //else
                    //{
                    //    GoogleSyncTimings.LastGoogleSync = DateTime.UtcNow;
                    //}

                    return await InitGoogleSync(user.User, connection);
                }
            }

            return false;
        }


        public async Task<bool> InitO365Sync(User user, string connection)
        {

            Console.WriteLine("Connecting to Office 365 for " + user.FullName + " ...");

            // Login to Office 365 and Authenticate User / Password
            var loginStartTime = DateTime.UtcNow;
            var syncEngine = new ExchangeSyncEngine();
            await syncEngine.InitEws(new SyncUser
            {
                UserId = user.UserId,
                SubscriberId = user.SubscriberId,
                SyncEmail = user.SyncEmail,
                SyncPassword = Utils.Decrypt(user.SyncPasswordHashed),
                SyncType = user.SyncType,
                Connection = connection,
                SyncState = user.SyncState
            });
            var isValidSyncUser = syncEngine.ValidateSyncUser();



            if (isValidSyncUser == true)
            {
                // check how long it took for the exchange login
                var elapsedTime = Utils.GetElapsedTime(loginStartTime, DateTime.UtcNow);
                Console.WriteLine(syncEngine.EmailAddress + " Logged-in to Office 365: " + elapsedTime + Environment.NewLine);

                // calendar sync
                var aps = new ExchangeAppointmentSync(syncEngine);
                aps.RunAppointmentsSync();

                Console.WriteLine(syncEngine.EmailAddress + " Calendar sync completed." + Environment.NewLine);
                return true;
            }
            else
            {
                // Exchange Login Error
                syncEngine.LogExchangeSyncError(2, "Failed to Validate Office 365 User", "999.1 - InitO365Sync: " + syncEngine.User.FullName);
                // Increment LoginFailures 
                syncEngine.IncrementExchangeLoginFailures();
                return false;
            }
        }


        public async Task<bool> InitExchangeSync(User user, string connection)
        {
            Console.WriteLine("Connecting to Exchange for " + user.FullName + " ...");

            // Login to Exchange and Authenticate User / Password
            var loginStartTime = DateTime.UtcNow;
            var syncEngine = new ExchangeSyncEngine();
            syncEngine.InitEws(new SyncUser
            {
                UserId = user.UserId,
                SubscriberId = user.SubscriberId,
                SyncEmail = user.SyncEmail,
                SyncPassword = Utils.Decrypt(user.SyncPasswordHashed),
                SyncUsername = user.SyncUserName,
                SyncType = user.SyncType,
                Connection = connection
            });
            var isValidSyncUser = syncEngine.ValidateSyncUser();

            if (isValidSyncUser == true)
            {
                // check how long it took for the exchange login
                var elapsedTime = Utils.GetElapsedTime(loginStartTime, DateTime.UtcNow);
                Console.WriteLine(syncEngine.EmailAddress + " Logged-in to Exchange: " + elapsedTime + Environment.NewLine);

                // calendar sync
                var aps = new ExchangeAppointmentSync(syncEngine);
                aps.RunAppointmentsSync();

                Console.WriteLine(syncEngine.EmailAddress + " Calendar sync completed." + Environment.NewLine);
                return true;
            }
            else
            {
                // Exchange Login Error
                syncEngine.LogExchangeSyncError(2, "Failed to Validate Exchange User", "999.1 - InitExchangeSync: " + syncEngine.User.FullName);
                // Increment LoginFailures 
                syncEngine.IncrementExchangeLoginFailures();
                return false;
            }
        }


        public async Task<bool> InitGoogleSync(User user, string connection)
        {
            Console.WriteLine("Connecting to Google for " + user.FullName + " ...");

            var loginStartTime = DateTime.UtcNow;
            var syncEngine = new GoogleSyncEngine();

            syncEngine.InitGoogleSync(new SyncUser
            {
                UserId = user.UserId,
                SyncType = user.SyncType,
                Connection = connection
            });

            // it is better to validate by checking accessing email

            if (!string.IsNullOrEmpty(GoogleSyncAppSettings.GoogleApiRefreshToken))
            {
                // reset login failures
                await ResetLoginFailures(user.UserId, connection);
                // run appointment sync
                syncEngine.RunGoogleAppointmentsSync();

                return true;
            }
            else
            {
                //  Log Error
                LogSyncError(user, 2, "Failed to Validate Google User", "999.1 - InitGooleSync: " + syncEngine.User.FullName);
                // Increment LoginFailures 
                IncrementLoginFailures(user.UserId, connection);
                return false;
            }
        }


        public SyncErrorItemsResponse GetSyncErrors(SyncErrorItemsRequest request)
        {
            var response = new SyncErrorItemsResponse
            {
                Items = new List<ExchangeSyncErrorLog>()
            };
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var errros = context.ExchangeSyncErrorLogs.Where(t => t.UserId == request.UserId).OrderByDescending(t => t.ErrorDateTime);
            response.RecordsCount = errros.Count();
            response.Items = errros.Skip((request.CurrentPage - 1) * request.RecordsPerPage)
                                   .Take(request.RecordsPerPage).ToList();
            return response;
        }


        public SyncHistoryResponse GetSyncHistory(SyncHistoryRequest request)
        {
            var response = new SyncHistoryResponse
            {
                Items = new List<ExchangeSyncLog>()
            };
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var errros = context.ExchangeSyncLogs.Where(t => t.UserId == request.UserId).OrderByDescending(t => t.SyncDateTime);
            response.RecordsCount = errros.Count();
            response.Items = errros.Skip((request.CurrentPage - 1) * request.RecordsPerPage).Take(request.RecordsPerPage).ToList();
            return response;
        }


        public int LogSyncError(User user, int errorCode, string errorMessage, string routineName)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var error = new ExchangeSyncErrorLog();
            error.ErrorCode = errorCode.ToString();
            error.ErrorDateTime = DateTime.UtcNow;
            error.ErrorMessage = errorMessage;
            error.RoutineName = routineName;
            error.SubscriberId = user.SubscriberId;
            error.UserId = user.UserId;
            error.UserName = user.FullName;
            context.ExchangeSyncErrorLogs.InsertOnSubmit(error);
            context.SubmitChanges();

            return error.ExchangeSyncErrorLogId;
        }


        public int LogSync(User user, string syncType, int contactId, int calendarEventId, string syncMessage)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var log = new ExchangeSyncLog();

            log.CalendarEventId = calendarEventId;
            log.ContactId = contactId;
            log.SubscriberId = user.SubscriberId;
            log.SyncMessage = syncMessage;          // details of data
            log.SyncDateTime = DateTime.UtcNow;
            log.SyncType = syncType;                // CrmContactAdd / CrmContactUpdate / ExchangeContactAdd / ExchangeContactUpdate
            // CrmCalendarEventAdd / CrmCalendarEventUpdate / ExchangeAppointmentAdd / ExchangeAppointmentUpdate
            log.UserId = user.UserId;
            log.UserName = user.FullName;

            // Add sync log record
            context.ExchangeSyncLogs.InsertOnSubmit(log);
            context.SubmitChanges();
            return log.ExchangeSyncLogId;
        }


        public void UpdateAppointmentsLastSyncdate(int subscriberId, int userId, DateTime dt)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == userId);
            if (user != null)
            {
                user.SyncAppointmentsLastDateTime = dt;
                context.SubmitChanges();
            }
        }


        public void IncrementLoginFailures(int userId, string connection)
        {
            var context = new DbFirstFreightDataContext(connection);
            var users = context.Users.Where(u => u.UserId == userId).Select(u => u).FirstOrDefault();
            if (users != null)
            {
                users.LoginFailures = users.LoginFailures + 1;
                context.SubmitChanges();
            }
        }


        public async Task ResetLoginFailures(int userId, string connection)
        {
            var context = new DbFirstFreightDataContext(connection);
            var users = context.Users.Where(u => u.UserId == userId).Select(u => u).FirstOrDefault();
            if (users != null)
            {
                users.LoginFailures = 0;
                context.SubmitChanges();
            }
        }

    }
}
