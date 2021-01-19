using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Helpers;
using Models;

namespace Crm6.Admin.Users.SwitchUserCrmAdmin
{
    public partial class SwitchUserCrmAdmin : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            // verify user is crmadmin
            if (currentUser.User.UserRoles.Contains("CRM Admin") || currentUser.User.UserRoles.Contains("Country Admin"))
            {
                if (!Page.IsPostBack)
                {
                    LoadUsers();
                }
            }
            else
            {
                Response.Redirect("/login.aspx");
            }
        }


        private void LoadUsers()
        {
            var users = new List<SelectList>();
            var currentUser = LoginUser.GetLoggedInUser();

            if (currentUser.User.UserRoles.Contains("CRM Admin"))
                users = new DropdownHelper().GetUsers(int.Parse(lblSubscriberId.Text));

            else if (currentUser.User.UserRoles.Contains("Country Admin"))
            {
                var countryname = new Countries().GetCountryNameFromCountryCode(currentUser.User.CountryCode);
                if (!string.IsNullOrEmpty(countryname))
                {
                    users = new DropdownHelper().GetUsersByCountry(int.Parse(lblSubscriberId.Text), countryname);
                }
                else
                {
                    Response.Redirect("/login.aspx");
                    return;
                }
            }


            ddlUsers.Items.Add(new ListItem("Select User", ""));
            foreach (var user in users)
            {
                ddlUsers.Items.Add(new ListItem(user.SelectText, user.SelectValue.ToString()));
            }
        }

        protected void btnSwitchUser_Click(object sender, EventArgs e)

        {
            var userId = int.Parse(ddlUsers.SelectedValue);
            if (userId > 0)
            {
                PerformSwitch(userId);
            }
        }


        private void PerformSwitch(int userId)
        {
            LoginUser.PerformSwitch(userId, int.Parse(lblSubscriberId.Text));
            var redirectUrl = "/Activities/Activities.aspx";
            // get new user
            var currentUser = LoginUser.GetLoggedInUser();  
            if (currentUser != null && !string.IsNullOrWhiteSpace(currentUser.User.UserRoles))
            {
                if (currentUser.User.UserRoles.Contains("CRM Admin") || currentUser.User.UserRoles.Contains("Location Manager") ||
                    currentUser.User.UserRoles.Contains("Country Manager") || 
                    currentUser.User.UserRoles.Contains("Region Manager") || 
                    currentUser.User.UserRoles.Contains("District Manager") ||
                    currentUser.User.UserRoles.Contains("Sales Manager") ||
                    currentUser.User.UserRoles.Contains("Country Admin"))
                {
                    redirectUrl = "/Dashboards/Dashboard.aspx";
                }
            } 

            Response.Redirect(redirectUrl);

        }
    }
}
