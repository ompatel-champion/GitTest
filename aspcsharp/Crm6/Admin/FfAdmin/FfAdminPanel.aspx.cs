using Helpers;
using System; 

namespace Crm6.FfAdmin
{
    public partial class FfAdminPanel : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Verify user is admin@firstfreight.com
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            if (!Page.IsPostBack)
            {
                if (!currentUser.User.EmailAddress.ToLower().Contains("admin@firstfreight.com"))
                {
                   // Response.Redirect("/Admin/AdminPanel.aspx");
                }
            }

        }

        protected void btnFixSpotDeals_Click(object sender, EventArgs e)
        {
            new Helpers.Deals().FixSpotDeals();

        }

        protected void btnFixDeals_Click(object sender, EventArgs e)
        {
            new DataUpdater().UpdateDealReportFields();
        }

        protected void btnUpdateActivities_Click(object sender, EventArgs e)
        {
            new DataUpdater().UpdateActivities();
        }
    }
}