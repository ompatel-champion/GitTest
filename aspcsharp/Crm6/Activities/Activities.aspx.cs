using System;

namespace Crm6.Activities
{
    public partial class Activities : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();

            var userId = currentUser.User.UserId;
            lblUserId.Text = userId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblUsername.Text = currentUser.User.FirstName + " " + currentUser.User.LastName;
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();

            if (!IsPostBack)  {  }
        }
        
    }
}