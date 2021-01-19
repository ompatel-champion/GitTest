using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Login;
using Models;
using System;
using System.Linq;
using System.Web;

namespace Helpers
{
    public class ResetPassword
    {

        private string SetupLoginConnectionForForgotPassword(string emailAddress)
        {
            var loginConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Security;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            var loginContext = new DbLoginDataContext(loginConnection);

            GlobalUser globalUser = null;
            try
            {
                // check for the global user
                globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.EmailAddress.ToLower().Equals(emailAddress.ToLower()));
            }
            catch (System.Exception)
            {
            }

            if (globalUser == null)
            {
                // not found in the live database - look in the dev test
                loginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                loginContext = new DbLoginDataContext(loginConnection);
                globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.EmailAddress.ToLower().Equals(emailAddress.ToLower()));
            }

            if (globalUser != null)
            {
                // get data center connection string based on global user 
                return loginConnection;
            }

            return "";
        }


        public ForgotPasswordResponse SendForgotPasswordEmail(string emailAddress)
        {
            var response = new ForgotPasswordResponse { IsError = false };
            var subscriberId = 0;
            var userId = 0;

            try
            {
                var loginConnection = SetupLoginConnectionForForgotPassword(emailAddress);
                var loginContext = new DbLoginDataContext(loginConnection);
                var guid = Guid.NewGuid().ToString();
                var currentDate = DateTime.Now;
                var currentUTCDate = DateTime.UtcNow;
                var ipAddress = Utils.GetLocalIPAddress();

                var globalUser = loginContext.GlobalUsers.Where(t => t.EmailAddress.ToLower() == emailAddress.ToLower()).FirstOrDefault();
                if (globalUser != null)
                {
                    var forgotPasswordRequest = new ForgotPasswordRequest
                    {
                        DataCenter = globalUser.DataCenter,
                        EmailAddress = emailAddress,
                        Guid = guid,
                        IpAddress = ipAddress,
                        RequestedDate = currentUTCDate
                    };
                    var connection = LoginUser.GetConnectionForDataCenter(globalUser.DataCenter);
                    var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(globalUser.DataCenter);
                    if (!string.IsNullOrEmpty(connection))
                    {
                        var context = new DbFirstFreightDataContext(connection);
                        // check if the user exists
                        var user = context.Users.FirstOrDefault(u => u.EmailAddress.ToLower().Equals(emailAddress.ToLower()) && !u.Deleted);
                        if (user != null)
                        {
                            response.IsUserFound = true;
                            forgotPasswordRequest.UserId = user.UserId;
                            forgotPasswordRequest.SubscriberId = user.SubscriberId;
                            subscriberId = user.SubscriberId;
                            userId = user.UserId;

                            var userIpAddress = string.IsNullOrEmpty(forgotPasswordRequest.IpAddress) ? "unknown" : forgotPasswordRequest.IpAddress;
                            var uriAddress = HttpContext.Current.Request.Url;
                            var forgotPasswordLink = uriAddress.GetLeftPart(UriPartial.Authority) + "/ResetPassword.aspx?token=" + guid;

                            if (user.DataCenter != null)
                                forgotPasswordRequest.DataCenter = user.DataCenter;

                            // get forgot password email body
                            var emailBody = "Dear " + user.FirstName +",";
                            emailBody += "<br />";
                            emailBody += "To reset the password to your First Freight account, click the link below:";
                            emailBody += "<br />";
                            emailBody += forgotPasswordLink;
                            emailBody += "<br /><br />";
                            emailBody += "If the above link is not clickable, please copy the whole link and paste it in your browser.";
                            emailBody += "<br /><br />";
                            emailBody += "Time of Request : " + currentDate.ToString("dd MMM, yyyy HH:mm");
                            emailBody += "<br /><br />";
                            emailBody += "IP Address: " + userIpAddress;
                            emailBody += "<br /><br />";
                            emailBody += "Should you have any questions or concerns, please contact us at help@firstfreight.com | (866) 683 - 2239";
                            emailBody += "<br /><br />";
                            emailBody += "Sincerely, the First Freight Team";
                            emailBody += "<br />";
                            emailBody += "https://www.firstfreight.com";

                            var CRM_AdminEmailSender =
                            new Recipient
                            {
                                EmailAddress = "admin@firstfreight.com",
                                Name = "First Freight CRM"
                            };

                            var request = new SendEmailRequest
                            {
                                Sender = CRM_AdminEmailSender,
                                Subject = "Reset CRM Password Request",
                                HtmlBody = emailBody,
                                OtherRecipients = new System.Collections.Generic.List<Recipient>
                                                    {
                                                            new Recipient{EmailAddress =user.EmailAddress, Name = user.FullName,UserId = user.UserId },
                                                            new Recipient{EmailAddress = "charles@firstfreight.com" },
                                                            new Recipient{EmailAddress = "sendgrid@firstfreight.com" }
                                                    }
                            };
                            var result = new SendGridHelper().SendEmail(request);
                            if (result)
                            {
                                response.IsEmailSent = true;
                                // add email record
                                var email = new Email
                                {
                                    DateSent = currentDate,
                                    Subject = request.Subject,
                                    HtmlBody = request.HtmlBody,
                                    FromName = request.Sender.Name,
                                    FromEmailAddress = request.Sender.EmailAddress,
                                    ToEmail = emailAddress,
                                    UtcSentDateTime = currentUTCDate
                                };
                                context.Emails.InsertOnSubmit(email);
                                context.SubmitChanges();
                                // set email Id
                                forgotPasswordRequest.EmailId = email.EmailId;
                            }
                            }
                    }
                    // add forgot password record
                    loginContext.ForgotPasswordRequests.InsertOnSubmit(forgotPasswordRequest);
                    loginContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;

                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    ErrorMessage = ex.ToString(),
                    PageCalledFrom = "ForgotPassword.aspx",
                    RoutineName = "SendForgotPassword",
                    SubscriberId = subscriberId,
                    SubscriberName = "",
                    UserId = userId
                };
                new Logging().LogWebAppError(error);

            }
            return response;
        }

    }


    public class ForgotPasswordResponse
    {
        public bool IsError { get; set; }
        public bool IsUserFound { get; set; }
        public bool IsEmailSent { get; set; }
    }
}
