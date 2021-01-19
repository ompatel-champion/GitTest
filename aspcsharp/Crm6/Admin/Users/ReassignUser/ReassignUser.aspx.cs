using Helpers;
using System;
using System.Web.UI.WebControls;
using Models;
using System.Collections.Generic;

namespace Crm6.Admin
{
    public partial class ReassignUser : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                LoadUserDropdowns();
            }
        }


        void LoadUserDropdowns()
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

            // departing user
            ddlDepartingUser.Items.Add(new ListItem("", ""));
            foreach (var user in users)
            {
                ddlDepartingUser.Items.Add(new ListItem(user.SelectText, user.SelectValue));
            }

            // new user
            ddlNewUser.Items.Add(new ListItem("", ""));
            foreach (var user in users)
            {
                ddlNewUser.Items.Add(new ListItem(user.SelectText, user.SelectValue));
            }
        }
    }

}
