using Crm6.App_Code.Sync;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Google.Apis.Auth.OAuth2;
using System.Threading;
using Google.Apis.Calendar.v3;
using Helpers.Sync;
using Crm6.App_Code;
using Google.Apis.Services;
using Google.Apis.Auth.OAuth2.Flows;

using Microsoft.Owin.Security;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Security.Claims;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.Owin;


namespace Crm6
{


    public partial class GoogleCallBack : System.Web.UI.Page
    {
        static string[] Scopes = { CalendarService.Scope.Calendar, CalendarService.Scope.CalendarEvents };
        private readonly IDataStore dataStore = new FileDataStore(GoogleWebAuthorizationBroker.Folder);


        protected void Page_Load(object sender, EventArgs e)
        {
            string Error = Request.QueryString["error"];
            // authorization code after successful authorization    
            string Code = Request.QueryString["code"];
            var url = "";
            var uri = Request.Url;
            var baseUri = uri.GetLeftPart(UriPartial.Authority);
            var subDomain = GetSubDomain(uri);
            var isLocalhost = uri.ToString().Contains("localhost");
            if (isLocalhost)
            {
                subDomain = "localhost";
            }
            if (Code != null)
            {
                // remember, we have set user id in State    
                string state = Request.QueryString["state"];
                // get access token    
                var globalUserId = 0;

                var stateArr = state.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (stateArr.Length > 1)
                {
                    globalUserId = int.Parse(stateArr[0]);
                    subDomain = stateArr[1];
                }

                if (globalUserId > 0)
                {
                    var loginConnection = LoginUser.GetLoginConnection();
                    var loginContext = new App_Code.Login.DbLoginDataContext(loginConnection);
                    var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.GlobalUserId == globalUserId);
                    if (globalUser != null)
                    {
                        string AccessToken = string.Empty;

                        var refereshToken = ExchangeAuthorizationCode(globalUser.UserId, Code, out AccessToken);
                        if (!string.IsNullOrEmpty(refereshToken))
                        {
                            //saving refresh token in database 
                            GoogleSyncAppSettings.SaveUserRefreshToken(globalUser.SubscriberId, globalUser.UserId, refereshToken, "");

                            //Get Email Id of the authorized user    
                            //string EmailId = FetchEmailId(AccessToken);

                            //Redirect the user to Authorize.aspx with user id    
                            url = "https://" + subDomain + ".firstfreight.com/Admin/Users/UserProfile/UserProfile.aspx?UserId=" + globalUser.UserId + "&syncState=success";
                            if (subDomain == "localhost")
                                url = "https://localhost:44391/Admin/Users/UserProfile/UserProfile.aspx?UserId=" + globalUser.UserId + "&syncState=success";

                            Response.Redirect(url, true);
                            return;
                        }
                    }
                }


            }

            //Redirect the user to Authorize.aspx with user id
            url = "https://" + subDomain + ".firstfreight.com/Admin/Users/UserProfile/UserProfile.aspx?syncState=error";
            if (subDomain == "localhost")
                url = "https://localhost:44391/Admin/Users/UserProfile/UserProfile.aspx?syncState=error";

            Response.Redirect(url, true);
        }




        private string ExchangeAuthorizationCode(int userId, string code, out string accessToken)
        {
            accessToken = string.Empty;
            string ClientSecret = GoogleSyncAppSettings.GoogleApiClientSecret;
            string ClientId = GoogleSyncAppSettings.GoogleApiClientId;
            //get this value by opening your web app in browser.    
            var uri = Request.Url;
            var baseUri = uri.GetLeftPart(UriPartial.Authority);
            var subdomian = GetSubDomain(uri);
            var isLocalhost = uri.ToString().Contains("localhost");
            var redirectUrl = "https://crm.firstfreight.com/Google/GoogleCallBack.aspx";
            if (isLocalhost)
            {
                redirectUrl = "https://localhost:44391/Google/GoogleCallBack.aspx";
                subdomian = "localhost";
            }
            var Content = "code=" + code + "&client_id=" + ClientId + "&client_secret=" + ClientSecret + "&redirect_uri=" + redirectUrl + "&grant_type=authorization_code";
            var request = WebRequest.Create("https://accounts.google.com/o/oauth2/token");
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(Content);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }
            var Response = (HttpWebResponse)request.GetResponse();
            Stream responseDataStream = Response.GetResponseStream();
            StreamReader reader = new StreamReader(responseDataStream);
            string ResponseData = reader.ReadToEnd();
            reader.Close();
            responseDataStream.Close();
            Response.Close();
            if (Response.StatusCode == HttpStatusCode.OK)
            {
                var ReturnedToken = JsonConvert.DeserializeObject<Token>(ResponseData);
                if (ReturnedToken.refresh_token != null)
                {
                    accessToken = ReturnedToken.access_token;
                    return ReturnedToken.refresh_token;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return string.Empty;
            }
        }


        private string FetchEmailId(string accessToken)
        {
            var EmailRequest = "https://www.googleapis.com/userinfo/email?alt=json&access_token=" + accessToken;
            // Create a request for the URL.    
            var Request = WebRequest.Create(EmailRequest);
            // Get the response.    
            var Response = (HttpWebResponse)Request.GetResponse();
            // Get the stream containing content returned by the server.    
            var DataStream = Response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.    
            var Reader = new StreamReader(DataStream);
            // Read the content.    
            var JsonString = Reader.ReadToEnd();
            // Cleanup the streams and the response.    
            Reader.Close();
            DataStream.Close();
            Response.Close();
            dynamic json = JValue.Parse(JsonString);
            return json.data.email;
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




        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    var userId = 0;
        //    var subscriberId = 0;
        //    if (Request.QueryString["userId"] != null && int.Parse(Request.QueryString["userId"]) > 0)
        //    {
        //        userId = int.Parse(Request.QueryString["userId"]);
        //    }
        //    if (Request.QueryString["subscriberId"] != null && int.Parse(Request.QueryString["subscriberId"]) > 0)
        //    {
        //        subscriberId = int.Parse(Request.QueryString["subscriberId"]);
        //    }

        //    // get base domain
        //    var uri = Request.Url;
        //    var baseUri = uri.GetLeftPart(UriPartial.Authority);
        //    var subdomian = GetSubDomain(uri);
        //    if (uri.ToString().Contains("localhost"))
        //    {
        //        subdomian = "localhost";
        //    }

        //    if (userId > 0 && subscriberId > 0 && !string.IsNullOrEmpty(subdomian))
        //    {
        //        var credntialsError = string.Empty;
        //        var loginContext = new App_Code.Login.DbLoginDataContext();
        //        var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == userId && u.SubscriberId == subscriberId);
        //        if (globalUser != null)
        //        {
        //            //UserCredential credential = GetUserCredential(out credntialsError, globalUser.GlobalUserId);

        //            var tokenResponse = GetUserCredential(out credntialsError, globalUser.GlobalUserId);

        //            //   if (credential != null && string.IsNullOrEmpty(credntialsError))
        //            if (tokenResponse != null && string.IsNullOrEmpty(credntialsError))
        //            {
        //               // var refereshToken = credential.Token.RefreshToken;
        //                var refereshToken = tokenResponse.RefreshToken;
        //                //saving refresh token in database 
        //                GoogleSyncAppSettings.SaveUserRefreshToken(subscriberId, userId, refereshToken, "");
        //                //Redirect the user to Authorize.aspx with user id    
        //                var url = "https://" + subdomian + ".firstfreight.com/Admin/Users/UserProfile/UserProfile.aspx?UserId=" + userId + "&syncState=success";
        //                if (subdomian == "localhost")
        //                    url = "https://localhost:44391/Admin/Users/UserProfile/UserProfile.aspx?UserId=" + userId + "&syncState=success";

        //                Response.Redirect(url, true);

        //            }
        //            else
        //            {
        //                var user = new Helpers.Users().GetUser(userId, subscriberId);
        //                if (user != null)
        //                {
        //                    // var user = 
        //                    new SyncInitializer().LogSyncError(user.User, 2, "Google Initialization Error", "Error authorizing the FirstFreight App with Google - " + credntialsError);

        //                }

        //                var url = "https://" + subdomian + ".firstfreight.com/Admin/Users/UserProfile/UserProfile.aspx?UserId=" + userId + "&syncState=error";
        //                if (subdomian == "localhost")
        //                    url = "https://localhost:44391/Admin/Users/UserProfile/UserProfile.aspx?UserId=" + userId + "&syncState=error";

        //                Response.Redirect(url, true);
        //            }
        //        }
        //        return;
        //    }
        //    return;
        //}





        //public TokenResponse GetUserCredential(out string error, int globalUserId)
        //{
        //    UserCredential credential = null;
        //    error = string.Empty;

        //    var secrets = new ClientSecrets
        //    {
        //        ClientId = GoogleSyncAppSettings.GoogleApiClientId,
        //        ClientSecret = GoogleSyncAppSettings.GoogleApiClientSecret,
        //    };

        //    var initializer = new GoogleAuthorizationCodeFlow.Initializer
        //    {
        //        ClientSecrets = secrets,
        //        Scopes = Scopes,
        //    };
        //    var flow = new GoogleAuthorizationCodeFlow(initializer);


        //        // YOUR CODE SHOULD BE HERE..
        //        // SAMPLE CODE:
        //        var list = await service.Files.List().ExecuteAsync();
        //        ViewBag.Message = "FILE COUNT IS: " + list.Items.Count();
        //        return View();


        //    var identity = HttpContext.Current.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ApplicationCookie).Result;
        //    var displayName = identity.Name;
        //    var email = identity.FindFirstValue(ClaimTypes.Email);
        //    var token = dataStore.GetAsync<TokenResponse>(email).Result;
        //    return token;
        //    //return new UserCredential(flow, userId, token);


        //    //  var externalIdentity = HttpContext.Current.GetOwinContext().Authentication.GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie).Result;
        //    //  var displayName = externalIdentity.Name;
        //    // var email = externalIdentity.FindFirstValue(ClaimTypes.Email);


        //    //try
        //    //{
        //    //    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(secrets, Scopes, globalUserId.ToString(),
        //    //        CancellationToken.None).Result;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    credential = null;
        //    //    error = "Failed to initialize user credentials " + ex.ToString();
        //    //}
        //    // return credential;
        //}


        //private string GetSubDomain(Uri url)
        //{
        //    if (url.HostNameType == UriHostNameType.Dns)
        //    {
        //        try
        //        {
        //            var host = url.Host;
        //            var subDomains = host.Split('.');
        //            return subDomains[0];
        //        }
        //        catch (Exception)
        //        {
        //            return "";
        //        }
        //    }
        //    return null;
        //}


        //private string FetchEmailId(string accessToken)
        //{
        //    var emailRequest = "https://www.googleapis.com/userinfo/email?alt=json&access_token=" + accessToken;
        //    // Create a request for the URL.    
        //    var request = WebRequest.Create(emailRequest);
        //    // Get the response.    
        //    var response = (HttpWebResponse)request.GetResponse();
        //    // Get the stream containing content returned by the server.    
        //    var dataStream = response.GetResponseStream();
        //    // Open the stream using a StreamReader for easy access.    
        //    var reader = new StreamReader(dataStream);
        //    // Read the content.    
        //    var JsonString = reader.ReadToEnd();
        //    // Cleanup the streams and the response.    
        //    reader.Close();
        //    dataStream.Close();
        //    Response.Close();
        //    dynamic json = JValue.Parse(JsonString);
        //    return json.data.email;
        //}









    }


}
