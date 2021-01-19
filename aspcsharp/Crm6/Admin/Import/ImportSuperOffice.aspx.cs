using System;

namespace Crm6.Admin.Import
{
    public partial class ImportSuperOffice : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblGuid.Text = Guid.NewGuid().ToString();

        }
    }
}