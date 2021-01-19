using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Admin.Users.UserDetail
{
    public partial class UserDetail : BasePage
    {
        private bool _isAdmin; 

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser(); 
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            _isAdmin = LoginUser.GetLoggedInUser().User.UserRoles.Contains("CRM Admin");


            if (!Page.IsPostBack)
            {
                if (Request.QueryString["userId"] != null && int.Parse(Request.QueryString["userId"]) > 0)
                {
                    lblLoadedUserId.Text = Request.QueryString["userId"]; 

                    // load User
                    LoadUser();
                }
            }
        }

        private void LoadUser()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);  
            var userId = int.Parse(lblLoadedUserId.Text);
            var objUser = new Helpers.Users().GetUser(userId, subscriberId);
            if (objUser != null)
            {
                var user = objUser.User;
                // validate user
                if (user.SubscriberId != subscriberId)
                {
                    // TODO: Go to invalid request
                }

                lblBreadcrumbUserName.Text = user.FullName;

                var subscriber = new Helpers.Subscribers().GetSubscriber(user.SubscriberId);

                lblUserId.Text = userId.ToString();
                lblName.Text = user.FullName;
                lblJobTitle.Text = user.Title;
                lblEmail.Text = user.EmailAddress;
                aEmail.Attributes["href"] = "mailto:" + user.EmailAddress;
                lblMobile.Text = user.MobilePhone;
                lblFax.Text = user.Fax;
                lblLocationName.Text = $"{user.LocationName}, {user.CountryName}";
                lblAddress.Text = user.Address;
                lblSpokenLanguages.Text = user.LanguagesSpoken;
                lblCurrencyName.Text = user.CurrencyCode;
                lblTimezone.Text = user.TimeZoneOffset + " " + user.TimeZoneCityNames;
                lblUserRoles.Text = user.UserRoles;
                lblDisplayLanguage.Text = user.DisplayLanguage;
                lblBillingCode.Text = user.BillingCode;
                lblIpAddress.Text = user.IpAddress;
                lblBrowser.Text = user.BrowserName;
                lblDataCenter.Text = user.DataCenter;
                lblLoginStatus.Text = user.LoginEnabled ? "Enabled" : "Disabled";
                lblScreenResolution.Text = user.ScreenResolution;

                if (user.LastLoginDate.HasValue)
                { 
                    lblLastLogin.Text = string.Format("{0:ddd, dd MMM, yyyy hh:mm:ss tt}", user.LastLoginDate.Value);
                }
                else
                {
                    lblLastLogin.Text = "-";
                }

                if (_isAdmin)
                {
                    lblPassword.Text = user.Password;
                }
                else
                {
                    lblPassword.Text = "";
                }

                lblRegion.Text = user.RegionName;

                //profile pic / svg initials
                var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(user.UserId, subscriberId);
                imgProfile.Attributes["src"] = profilePicUrl;
            }
        }
    }
}