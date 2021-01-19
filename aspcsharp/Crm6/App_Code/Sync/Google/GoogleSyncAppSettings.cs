
using Crm6.App_Code.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Helpers.Sync;
using System.Linq;

namespace Crm6.App_Code.Sync
{
    public class GoogleSyncAppSettings
    {
        public static string GoogleEmail = "";
      //  API AIzaSyA33ThltYUUozZPoF3ssNxMl8qnxmEZjls
        // public static string GoogleApiClientId = "245961378265-5ju82l7p3fru5qf646ctf9kv64an9j2c.apps.googleusercontent.com";

        public static string GoogleApiClientId = "585664513470-1jqnch9htc3dhic17rg9e3cakr11uddi.apps.googleusercontent.com";
         public static string GoogleApiClientSecret = "_ftFGHcHKdruHhwiVuAiy2xZ";
         public static string ApplicationName = "First Freight Sync";

        //public static string GoogleApiClientSecret = "ze-lpYUis-FfWUb9P6tdXoz7";
        public static string GoogleApiRefreshToken = ""; 
        //public static string ApplicationName = "First Freight CRM Google Calendar Sync";
        public static ClientSecrets GoogleClientSecrets = new ClientSecrets()
        {
            ClientId = GoogleApiClientId,
            ClientSecret = GoogleApiClientSecret
        };

        public static string[] Scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents };



        public static void SaveUserRefreshToken(int subscriberId, int userId, string refreshToken, string googleEmailAddress)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == userId);
            if (user != null)
            {
                // Save Refresh token to user table
                user.SyncType = "Google";
                user.GoogleRefreshToken = refreshToken;
                if (!string.IsNullOrEmpty(googleEmailAddress))
                    user.GoogleCalendarEmail = googleEmailAddress;
                context.SubmitChanges();
                // set static variable
                GoogleApiRefreshToken = refreshToken;
                GoogleEmail = googleEmailAddress;

                var syncMessage = user.SyncType + " sync settings saved for " + user.FirstName + " " + user.LastName;
                new SyncInitializer().LogSync(user, user.SyncType + " sync activated.", 0, 0, syncMessage);
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = user.UserId,
                    SubscriberId = user.SubscriberId,
                    UserActivityMessage = syncMessage
                });
            }
        }


        public static void GetUserRefreshToken(int subscriberId, int userId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == userId);
            if (user != null)
            {
                GoogleApiRefreshToken = user.GoogleRefreshToken;
                GoogleEmail = user.GoogleCalendarEmail;
            }
        }


        public static void UpdateSyncToken(int subscriberId, int userId, string syncToken)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(t => t.UserId == userId);
            if (user != null)
            {
                user.GoogleSyncToken = syncToken;
                context.SubmitChanges();
            }
        }
    }


    public class Token
    {
        public string access_token
        {
            get;
            set;
        }
        public string token_type
        {
            get;
            set;
        }
        public string expires_in
        {
            get;
            set;
        }
        public string refresh_token
        {
            get;
            set;
        }
    }

}