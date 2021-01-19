using System;

namespace Crm6.Emails
{
    public partial class SendEmail : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["dealId"] != null && int.Parse(Request.QueryString["dealId"]) > 0)
                {
                    lblDealId.Text = Request.QueryString["dealId"];
                    // LoadDeal(int.Parse(Request.QueryString["dealId"]));
                }
            }
        }
    }
}