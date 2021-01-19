using System;

namespace Crm6.Emails
{
    public partial class ViewEmail : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["emailId"] != null && int.Parse(Request.QueryString["emailId"]) > 0)
                {
                    lblEmailId.Text = Request.QueryString["emailId"];
                }
                 

            }
        }
    }
}