using Crm6.App_Code;
using Crm6.App_Code.Sync;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Calendar.v3;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
namespace Crm6
{
    public partial class GoogleConnector : Page
    {
        static string[] Scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents };


        protected void Page_Load(object sender, EventArgs e)
        {
            var userId = 0;
            var subscriberId = 0;
            if (Request.QueryString["userId"] != null && int.Parse(Request.QueryString["userId"]) > 0)
            {
                userId = int.Parse(Request.QueryString["userId"]);
            }
            if (Request.QueryString["subscriberId"] != null && int.Parse(Request.QueryString["subscriberId"]) > 0)
            {
                subscriberId = int.Parse(Request.QueryString["subscriberId"]);
            }
            if (userId > 0 && subscriberId > 0)
            {

                if (IsAuthorized(userId, subscriberId)) //check whether user is already authorized  
                {
                }
                else
                {
                    var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
                    var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == userId && u.SubscriberId == subscriberId);
                    if (globalUser != null)
                    {
                        // call the Google to authenticate the user
                        string Url = GetAuthorizationUrl(globalUser.GlobalUserId);
                        HttpContext.Current.Response.Redirect(Url, false);
                    }
                }


                //var credntialsError = string.Empty;

                //UserCredential credential = GetUserCredential(out credntialsError);

                //if (credential != null && string.IsNullOrEmpty(credntialsError))
                //{
                //    var refereshToken = credential.Token.RefreshToken;
                //    GoogleSyncAppSettings.SaveUserRefreshToken(subscriberId, userId, refereshToken, "");
                //}

                //return;



                //// get global user id
                //var loginContext = new App_Code.Login.DbLoginDataContext();
                //// check for the global user
                //var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == userId && u.SubscriberId == subscriberId);
                //if (globalUser != null)
                //{
                //    // call the Google to authenticate the user
                //    string Url = GetAuthorizationUrl(globalUser.GlobalUserId);
                //    HttpContext.Current.Response.Redirect(Url, false);
                //}


            }



            //var initializer = new GoogleAuthorizationCodeFlow.Initializer
            //{
            //    ClientSecrets = new ClientSecrets
            //    {
            //        ClientId = GoogleSyncAppSettings.GoogleApiClientId,
            //        ClientSecret = GoogleSyncAppSettings.ClientSecret,
            //    },
            //    Scopes = new List<string> {
            //        "https://www.googleapis.com/auth/userinfo.email",
            //        "https://www.googleapis.com/auth/userinfo.profile",
            //        "https://www.googleapis.com/auth/calendar",
            //        "https://www.google.com/m8/feeds"
            //    },
            //};
            //var flow = new GoogleAuthorizationCodeFlow(initializer);

            //var identity = await HttpContext.GetOwinContext().Authentication.GetExternalIdentityAsync(
            //    DefaultAuthenticationTypes.ApplicationCookie);
            //var userId = identity.FindFirstValue(MyClaimTypes.GoogleUserId);

            //var token = await dataStore.GetAsync<TokenResponse>(userId);
            //// return new UserCredential(flow, userId, token);


        }


        private string GetAuthorizationUrl(int globalUserId)
        {

            var uri = Request.Url;
            var baseUri = uri.GetLeftPart(UriPartial.Authority);
            var subdomian = GetSubDomain(uri);
            var Scopes = "https://www.googleapis.com/auth/userinfo.email";
            Scopes += " https://www.googleapis.com/auth/userinfo.profile";
            Scopes += " https://www.google.com/m8/feeds";
            Scopes += " https://www.googleapis.com/auth/calendar";
            //get this value by opening your web APP in browser.    
            var isLocalhost = uri.ToString().Contains("localhost");
            var redirectUrl = "https://crm.firstfreight.com/Google/GoogleCallBack.aspx";
            if (isLocalhost)
            {
                redirectUrl = "https://localhost:44391/Google/GoogleCallBack.aspx";
                subdomian = "localhost";
            }
            var url = "https://accounts.google.com/o/oauth2/auth?";
            var urlBuilder = new StringBuilder(url);
            urlBuilder.Append("client_id=" + GoogleSyncAppSettings.GoogleApiClientId);
            urlBuilder.Append("&redirect_uri=" + redirectUrl);
            urlBuilder.Append("&response_type=" + "code");
            urlBuilder.Append("&scope=" + Scopes);
            urlBuilder.Append("&access_type=" + "offline");
            urlBuilder.Append("&state=" + globalUserId + "|" + subdomian); //setting the user id and state  
            return urlBuilder.ToString();



        }


        private string GetSubDomain(Uri url)
        {
            if (url.HostNameType == UriHostNameType.Dns)
            {
                try
                {
                    var host = url.Host;
                    var subDomains = host.Split('.');
                    return subDomains[0];
                }
                catch (Exception)
                {
                    return "";
                }
            }
            return null;
        }



        public UserCredential GetUserCredential(out string error)
        {
            UserCredential credential = null;
            error = string.Empty;

            var secrets = new ClientSecrets
            {
                ClientId = GoogleSyncAppSettings.GoogleApiClientId,
                ClientSecret = GoogleSyncAppSettings.GoogleApiClientSecret,
            };

            try
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, Scopes, Environment.UserName,
                    CancellationToken.None, new FileDataStore("Google Sync")).Result;
            }
            catch (Exception ex)
            {
                credential = null;
                error = "Failed to initialize user credentials " + ex.ToString();
            }
            return credential;
        }






        /// <summary>    
        /// Return gmail id from database. it will saved in the database after successful authentication.    
        /// </summary>    
        /// <param name="userId"></param>    
        /// <returns></returns>    
        private string GetGmailId(int userId, int subscriberId)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            // check for the   user
            var user = context.Users.FirstOrDefault(u => u.UserId == userId && u.SubscriberId == subscriberId);
            if (user != null)
            {
                return user.GoogleCalendarEmail;
            }
            return "";
        }

        private bool IsAuthorized(int userId, int subscriberId)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            // check for the   user
            var user = context.Users.FirstOrDefault(u => u.UserId == userId && u.SubscriberId == subscriberId);
            if (user != null)
            {
                return !string.IsNullOrEmpty(user.GoogleRefreshToken);
            }
            return false;
        }





    }
}