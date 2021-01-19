using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Sync;
using Helpers;
using Models;
using System;
using System.Web;
using System.Web.UI;

namespace Crm6
{
    public partial class Login : Page
    { 
        protected void Page_Load(object sender, EventArgs e)
        {
            var url = Request.Url.AbsoluteUri;
            if (url.Contains("kwe"))
            {
                imgLogo.Attributes["src"] = "/_content/_img/Kwe_Login.png";
            }
            
            #if DEBUG_FAST
                var user = LoginUser.ValidateUser("devtest@firstfreight.com", "ff#1");
                //var user = LoginUser.ValidateUser("info@plutonium.dev", "ff#1");
                var loginDetails = new LoginDetailsSaveRequest
                {
                    UserId = user.User.UserId,
                    SubscriberId = user.User.SubscriberId,
                    BrowserName = HttpContext.Current.Request.Browser.Browser,
                    BrowserVersion = HttpContext.Current.Request.Browser.Version,
                    IpAddress = Utils.GetLocalIPAddress(),
                    ScreenResolution = txtScreenResolution.Text
                }; 
                new Helpers.Users().UpdateLoginDetails(loginDetails);
                // Log successful user login 
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = user.User.UserId,
                    SubscriberId = user.User.SubscriberId,
                    UserActivityMessage = "Logged In"
                });
                Response.Redirect("/Dashboards/Dashboard.aspx"); 
            #endif
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var userHelper = new Helpers.Users();

            divError.Visible = false;
            var hasError = false;

            if (string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                divError.InnerHtml = "Enter Login Details";
                hasError = true;
            }

            // validate login details
            if (!hasError)
            {
                var user = LoginUser.ValidateUser(txtUsername.Text.Trim(), txtPassword.Text);
                var redirectUrl = "/Activities/Activities.aspx";

                if (user != null)
                {
                    var userId = user.User.UserId;
                    var subscriberId = user.Subscriber.SubscriberId;
                    var subscriberName = user.Subscriber.CompanyName;
                    try
                    {
                        // create intercom event - create / update user if needed 
                        var intercomHelper = new IntercomHelper();
                        var intercomData = intercomHelper.GetIntercomData(userId, user.User.SubscriberId);
                        var eventName = "Logged In";
                        intercomHelper.IntercomAddUpdateUser(userId, user.User.SubscriberId);
                        intercomHelper.IntercomTrackEvent(userId, intercomData, eventName);
                        // save login details
                        var loginDetails = new LoginDetailsSaveRequest
                        {
                            UserId = userId,
                            SubscriberId = user.User.SubscriberId,
                            BrowserName = HttpContext.Current.Request.Browser.Browser,
                            BrowserVersion = HttpContext.Current.Request.Browser.Version,
                            IpAddress = Utils.GetLocalIPAddress(),
                            ScreenResolution = txtScreenResolution.Text
                        }; 
                        new Helpers.Users().UpdateLoginDetails(loginDetails);
                        // Log successful user login
                        new Logging().LogUserAction(new UserActivity
                        {
                            UserId = user.User.UserId,
                            SubscriberId = user.User.SubscriberId,
                            UserActivityMessage = "Logged In"
                        });
                    }
                    catch (Exception ex)
                    {
                        var error = new Crm6.App_Code.Shared.WebAppError
                        {
                            ErrorCallStack = ex.StackTrace,
                            ErrorDateTime = DateTime.UtcNow,
                            RoutineName = "Login",
                            PageCalledFrom = "Login.aspx",
                            SubscriberId = subscriberId,
                            SubscriberName = subscriberName,
                            ErrorMessage = ex.ToString(),
                            UserId = userId
                        };
                        new Logging().LogWebAppError(error);
                    }
                    if (user != null)
                    {
                        if (!string.IsNullOrWhiteSpace(user.User.UserRoles))
                        {
                            if (user.User.UserRoles.Contains("CRM Admin") || user.User.UserRoles.Contains("Location Manager") ||
                                user.User.UserRoles.Contains("Country Manager") || user.User.UserRoles.Contains("Region Manager") ||
                                user.User.UserRoles.Contains("District Manager") ||
                                user.User.UserRoles.Contains("Sales Manager") ||
                                user.User.UserRoles.Contains("Country Admin"))
                            {
                                redirectUrl = "/Dashboards/Dashboard.aspx";
                            }
                        }

                        if (user.User.SyncType == "Google")
                        {
                            GoogleSyncAppSettings.GoogleApiRefreshToken = user.User.GoogleRefreshToken;
                            GoogleSyncAppSettings.GoogleEmail = user.User.GoogleCalendarEmail;
                        }

                        new Helpers.Sync.SyncInitializer().SyncExchangeForUser(userId, subscriberId);

                        //  var successfulSync = new Helpers.Sync.SyncInitializer().SyncExchangeForUser(user.User.UserId, user.User.SubscriberId);

                        //if (!string.IsNullOrEmpty(user.User.SyncType) && (user.User.LoginFailures > 2 || !successfulSync.Result))
                        //{
                        //    redirectUrl = "/Admin/Users/UserSyncError/VerifyCredentials.aspx";
                        //}
                        //else
                        //{
                        // get the from page
                        var fromPage = Request.QueryString["from"] + "";
                            if (fromPage != "" && fromPage != "/")
                            {
                                Response.Redirect(fromPage);
                                return;
                            }

                            // FF Admin User
                            if (user.User.EmailAddress == "admin@firstfreight.com")
                                redirectUrl = "/Admin/FfAdmin/FfAdminPanel.aspx";
                        //   }

                        // redirect to start page url

                        //Start calendar sync in the background

                        Response.Redirect(redirectUrl);
                        return;
                    }
                }
                divError.InnerHtml = "Invalid Login Details";
            }

            // show error
            divError.Visible = true;
        }
    }
}
