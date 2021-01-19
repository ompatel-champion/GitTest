using System;

namespace Crm6.Contacts
{
    public partial class ContactList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            if (!Page.IsPostBack)
            {
            }
        }
    }
}