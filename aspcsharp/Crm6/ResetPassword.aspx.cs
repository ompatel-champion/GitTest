using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Login;
using System;
using System.Linq;

namespace Crm6
{
    public partial class ResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.QueryString["token"] != null)
            {
                var token = Request.QueryString["token"];
                var loginConnection = SetupLoginConnectionForForgotPassword(token);
                var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
                var forgotPasswordRequest = loginContext.ForgotPasswordRequests.FirstOrDefault(t => t.Guid.Equals(token));
                if (forgotPasswordRequest != null && !forgotPasswordRequest.Processed)
                {
                    // validate days - link only valid for 1 day
                    var hours = (DateTime.UtcNow - forgotPasswordRequest.RequestedDate).TotalHours;
                    if (hours <= 24)
                    {
                        // success
                        return;
                    }
                }
            }
            // invalid token
            divInvalid.Visible = true;
            // divContent.Visible = false;
        }

        private string SetupLoginConnectionForForgotPassword(string token)
        {
            var loginConnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Security;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            var loginContext = new DbLoginDataContext(loginConnection);

            ForgotPasswordRequest request = null;
            try
            {
                // check for the token
                request = loginContext.ForgotPasswordRequests.FirstOrDefault(t => t.Guid.Equals(token));

            }
            catch (System.Exception)
            {
            }

            if (request == null)
            {
                // not found in the live database - look in the dev test
                loginConnection = "Data Source=ffcrm-test.database.windows.net;Initial Catalog=CRM_Test_Security;Persist Security Info=True;User ID=ffcrmTest;Password=Test#9605";
                loginContext = new DbLoginDataContext(loginConnection);
                request = loginContext.ForgotPasswordRequests.FirstOrDefault(t => t.Guid.Equals(token));
            }

            if (request != null)
            {
                return loginConnection;
            }

            return "";
        }

        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            divError.Visible = false;
            var token = Request.QueryString["token"];
            var loginConnection = SetupLoginConnectionForForgotPassword(token);
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
            var forgotPasswordRequest = loginContext.ForgotPasswordRequests.FirstOrDefault(t => t.Guid.Equals(token));
            if (forgotPasswordRequest != null)
            {
                var connection = LoginUser.GetConnectionForDataCenter(forgotPasswordRequest.DataCenter);
                var context = new DbFirstFreightDataContext(connection);
                var user = context.Users.FirstOrDefault(u => u.EmailAddress.ToLower().Equals(forgotPasswordRequest.EmailAddress.ToLower()) && !u.Deleted);
                if (user != null)
                {
                    user.Password = txtPassword.Text;
                    user.PasswordHashed = PasswordEncryptor.CreateHash(txtPassword.Text.Trim());
                    user.LastUpdate = DateTime.UtcNow;
                    context.SubmitChanges();

                    // update forgot password
                    forgotPasswordRequest.Processed = true;
                    loginContext.SubmitChanges();

                    // divContent.Visible = false;
                    divSuccess.Visible = true;
                    return;
                }
            }
            // show error
            divError.Visible = true;
        }
    }
}
