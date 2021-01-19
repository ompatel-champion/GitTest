using System;
using Crm6.App_Code.Shared;
using Crm6.Emails;
using Segment;
using Segment.Model;

namespace Crm6.App_Code.Helpers
{
    public class Logging
    {

        // Log User Activity
        public void LogUserAction(UserActivity userAction)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = new global::Helpers.Users().GetUser(userAction.UserId, userAction.SubscriberId);
            if (user != null)
            {
                var newUserAction = new UserActivity
                {
                    CalendarEventId = userAction.CalendarEventId,
                    CalendarEventSubject = userAction.CalendarEventSubject,
                    CompanyId = userAction.CompanyId,
                    CompanyName = userAction.CompanyName,
                    ContactId = userAction.ContactId,
                    ContactName = userAction.ContactName,
                    DealId = userAction.DealId,
                    DealName = userAction.DealName,
                    NoteContent = userAction.NoteContent,
                    NoteId = userAction.NoteId,
                    SubscriberId = user.User.SubscriberId,
                    SubscriberName = user.Subscriber.CompanyName,
                    TaskId = userAction.TaskId,
                    TaskName = userAction.TaskName,
                    UserActivityMessage = userAction.UserActivityMessage,
                    UserId = user.User.UserId,
                    UserName = user.User.FullName,
                    UserActivityTimestamp = DateTime.UtcNow
                };
                context.UserActivities.InsertOnSubmit(newUserAction);
                context.SubmitChanges(); 

                // SEGMENT
                //initialize the project #{source.owner.login}/#{source.slug}...
                Analytics.Initialize("5LKpSaWD5onrNeheeXfWmMpvUeSReKAr");
                // Segment Source ID - qIf9kRbUkm

                Analytics.Client.Identify(userAction.UserId.ToString(), new Traits
                {
                    {"name", "#{ " + userAction.UserName + "}"},
                    {"email", "#{ " + user.User.EmailAddress + "}"},
                    {"subscriberid", user.User.SubscriberId},
                    {"subscriberName", user.Subscriber.CompanyName},
                    {"userid", user.User.UserId}
                });

                Analytics.Client.Track(userAction.UserId.ToString(), "UserActionProperties", new Segment.Model.Properties
                {
                    {"contact", userAction.ContactName},
                    {"company", userAction.CompanyName},
                    {"deal", userAction.DealName},
                    {"eventsubject", userAction.CalendarEventSubject},
                    {"message", userAction.UserActivityMessage},
                    {"task", userAction.TaskName}
                });

                //CRM6 - Production(2420880320) - Heap??
                // TODO: Heap Analytics - Server Side???

                // TODO: Application Insights Logging
                var telemetry = new Microsoft.ApplicationInsights.TelemetryClient();

                //var subscriber = new Dictionary<,> String, String;
                //{
                //    { "subscriberid", userAction.SubscriberId},
                //};

                //{ "message", userAction.UserActivityMessage },
                //{ "contact", userAction.ContactName},
                //{ "company", userAction.CompanyName},
                //{ "deal", userAction.DealName},
                //{ "eventsubject", userAction.CalendarEventSubject},
                //{ "name", userAction.UserName },

                //{ "subscriberName", userAction.SubscriberName},
                //{ "task", userAction.TaskName },
                //{ "userid", userAction.UserId}

                // Send the event:
                telemetry.TrackEvent(userAction.UserActivityMessage);
            }
            //https://msftplayground.com/2017/02/trace-listeners-logging-azure-application-insights/
            //https://dzimchuk.net/tracing-and-logging-with-application-insights/
            //https://stackify.com/application-insights-things-to-know/
        }


        // Log Errors
        public int LogWebAppError(Shared.WebAppError webAppError)
        {
            try
            { 
                var sharedWriteableConnnection =LoginUser.GetWritableSharedConnectionForSubscriberId(webAppError.SubscriberId);
                var sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnnection);

                 var webAppErrorDetail = new Shared.WebAppError()
                {
                    ErrorCallStack = webAppError.ErrorCallStack ?? "",
                    ErrorCode ="",
                    ErrorMessage = webAppError.ErrorMessage ?? "", 
                    PageCalledFrom = webAppError.PageCalledFrom ?? "",
                    RoutineName = webAppError.RoutineName ?? "",
                    UserId = webAppError.UserId, 
                    ErrorDateTime = DateTime.UtcNow
                };
                var user = new global::Helpers.Users().GetUser(webAppError.UserId, webAppError.SubscriberId);
                if (user != null)
                {
                    webAppErrorDetail.SubscriberId = user.Subscriber.SubscriberId;
                    webAppErrorDetail.SubscriberName = user.Subscriber.CompanyName;
                    webAppErrorDetail.UserName = user.User.FullName;
                }
                else { 
                    webAppErrorDetail.SubscriberName = "";
                    webAppErrorDetail.UserName = "";
                }

                sharedWriteableContext.WebAppErrors.InsertOnSubmit(webAppErrorDetail);
                sharedWriteableContext.SubmitChanges();

                // return the webAppError id
                return webAppErrorDetail.WebAppErrorId;
            }
            catch (Exception e)
            {
                // send an email
                var email = new Email();
                email.Subject = "Logging Error";
                email.SubscriberId = webAppError.SubscriberId;
                email.HtmlBody = e.ToString();
                email.SentByUserId = webAppError.UserId;
                email.ToEmail = "sendgrid@firstfreight.com";
                var response = new SendEmail();
                return 0;
            }

        }

    }
}