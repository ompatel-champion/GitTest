using Helpers;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Reporting.UserActivity
{

    public partial class UserActivityReport : BasePage
    {
        private readonly DropdownHelper _dropDownHelper = new DropdownHelper();
        private int _subscriberId;
        private int _userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set Defaults - get from the session
            var currentUser = LoginUser.GetLoggedInUser();

            if (currentUser != null)
            {
                _userId = currentUser.User.UserId;
                lblUserId.Text = currentUser.User.UserId.ToString();
                _subscriberId = currentUser.User.SubscriberId;
                lblSubscriberId.Text = _subscriberId.ToString();
                lblUsername.Text = currentUser.User.FirstName + " " + currentUser.User.LastName;

                if (!Page.IsPostBack)
                {
                    LoadUsers(_subscriberId); 
                }
            }
            else
            {
                Response.Redirect("/Login.aspx");
            }
        }

        private void LoadUsers(int subscriberId)
        {
            var users = new DropdownHelper().GetUsers(subscriberId);
            foreach (var user in users)
            {
                ddlUsers.Items.Add(new ListItem(user.SelectText, user.SelectValue));
            }
        } 

    }


}