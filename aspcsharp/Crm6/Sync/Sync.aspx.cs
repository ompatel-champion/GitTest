using System; 

namespace Crm6
{
    public partial class Sync : BasePage
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

        protected void btnSync_Click(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser(); 
            if (!new Helpers.Sync.SyncInitializer().SyncExchangeForUser(currentUser.User.UserId, currentUser.User.SubscriberId).Result)
            {
                Response.Redirect("/Admin/Users/UserSyncError/VerifyCredentials.aspx");
            }
        }

        protected void btnSyncLog_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Sync/ExchangeSyncLog/ExchangeSyncLog.aspx");
        }

        protected void btnSyncErrorLog_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Sync/ExchangeSyncErrorLog/ExchangeSyncErrorLog.aspx");
        }
    }
}
