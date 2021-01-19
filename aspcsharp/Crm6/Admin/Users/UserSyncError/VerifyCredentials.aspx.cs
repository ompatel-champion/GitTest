using Crm6.App_Code.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Admin.Users.UserSyncError
{
    public partial class VerifyCredentials : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblGuid.Text = Guid.NewGuid().ToString();

        }


        /// <summary>    
        ///     
        /// </summary>    
        /// <param name="data"></param>    
        /// <returns></returns>    
        private string GetAuthorizationUrl()
        {
            string Scopes = "https://www.googleapis.com/auth/userinfo.email";
            Scopes += " https://www.googleapis.com/auth/userinfo.email";
            Scopes += " https://www.googleapis.com/auth/userinfo.profile";
            Scopes += " https://www.google.com/m8/feeds";
            Scopes += " https://www.googleapis.com/auth/calendar";

            //get this value by opening your web app in browser.    
            string RedirectUrl = "http://localhost:64604/Google/GoogleCallBack.aspx";
            string Url = "https://accounts.google.com/o/oauth2/auth?";
            StringBuilder UrlBuilder = new StringBuilder(Url);
            UrlBuilder.Append("client_id=" + GoogleSyncAppSettings.GoogleApiClientId);
            UrlBuilder.Append("&redirect_uri=" + RedirectUrl);
            UrlBuilder.Append("&response_type=" + "code");
            UrlBuilder.Append("&scope=" + Scopes);
            UrlBuilder.Append("&access_type=" + "offline");
            UrlBuilder.Append("&state=" + lblUserId.Text); //setting the user id in state  
            return UrlBuilder.ToString();
        }

        protected void btnActivateGoogleSync_Click(object sender, EventArgs e)
        {
            string Url = GetAuthorizationUrl();
            HttpContext.Current.Response.Redirect(Url, false);
        }
    }
}