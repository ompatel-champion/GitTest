using System;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Routing;
using Crm6.App_Code.Helpers;
using Sentry;
using Sentry.EntityFramework;
using System.Web;
using Models;
using Crm6.App_Code.Shared;
using System.Threading.Tasks;
using Helpers.Sync;
#if DEBUG_FAST
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.ApplicationInsights.Extensibility.Implementation;
#endif

namespace Crm6
{
    public class Global : System.Web.HttpApplication
    {
        // getsentry.io global variable
        private IDisposable _sentry;
        private string sentryDsn = ConfigurationManager.AppSettings["SentryDsn"];

        protected void Application_Start(object sender, EventArgs e)
        {
            #if DEBUG_FAST
                TelemetryConfiguration.Active.DisableTelemetry = true;
                TelemetryDebugWriter.IsTracingDisabled = true;
            #endif

            RouteTable.Routes.MapHttpRoute("DefaultApiWithId", "api/{controller}/{id}", new { id = System.Web.Http.RouteParameter.Optional }, new { id = @"\d+" });
            RouteTable.Routes.MapHttpRoute("ControllerAndAction", "api/{controller}/{action}");

            //(string keyword)
            RouteTable.Routes.MapHttpRoute("keyword", "api/{controller}/{keyword}", new { keyword = System.Web.Http.RouteParameter.Optional });

            GlobalConfiguration.Configuration.Formatters.Clear();
            var jsonMediatypeFormatter = new JsonMediaTypeFormatter();
            jsonMediatypeFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters.Add(jsonMediatypeFormatter);

            // segment project's write key
            Segment.Analytics.Initialize("5LKpSaWD5onrNeheeXfWmMpvUeSReKAr");

            // We add the query logging here so multiple DbContexts in the same project are supported
            SentryDatabaseLogging.UseBreadcrumbs();

            // Set up the getsentry.io SDK
            if (!string.IsNullOrEmpty(sentryDsn))
            {
                try
                {
                    // Set up the sentry SDK
                    _sentry = SentrySdk.Init(o =>
                    {
                        // We store the DSN inside Web.config; make sure to use your own DSN!
                        o.Dsn = new Dsn(ConfigurationManager.AppSettings["SentryDsn"]);
                    });
                }
                catch (Exception ex)
                {

                var error = new WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    ErrorMessage = ex.ToString(),
                    PageCalledFrom = "Helper/DeleteCompany",
                    RoutineName = "Application_Start",
                    SubscriberName = "",
                    //UserId = request.UserId,
                    //SubscriberId = request.CompanySubscriberId,
                };
                new Logging().LogWebAppError(error);            
                }

            }
        }


        //  Code that runs on application shutdown
        void Application_End(object sender, EventArgs e)
        {
            // Close the Sentry SDK (flushes queued events to Sentry)
            _sentry?.Dispose();
        }


        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }


        // Global error catcher
        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs
            var webAppError = new App_Code.Shared.WebAppError();
            var errorDescription = Server.GetLastError();

            // get current user
            var user = LoginUser.GetLoggedInUser();
            if (user != null)
            {
                webAppError.SubscriberId = user.User.SubscriberId;
                webAppError.UserId = user.User.UserId;
                webAppError.UserName = user.User.FullName;
            }

            // Code that runs when an unhandled error occurs
            if (HttpContext.Current != null)
            {
                var url = HttpContext.Current.Request.Url;
                if (url != null)
                    webAppError.PageCalledFrom = url.AbsoluteUri;

            }

            webAppError.ErrorMessage = errorDescription.ToString();
            webAppError.ErrorDateTime = DateTime.UtcNow;
            // log to WebAppErros trable in Shared database
            new Logging().LogWebAppError(webAppError);


            SentrySdk.CaptureException(errorDescription);

            // getSentry.io - capture unhandled exceptions 
        }


        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
#if FAKEAUTH
            var userHelper = new Helpers.Users();

            var user = userHelper.ValidateDevUser("devtest@firstfreight.com", "ff#1");
            LoginUser.CreateUserSession(user);
            HttpContext.Current.Session["UserDataCenter"] = user.User.DataCenter;
#endif
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }

    }
}
