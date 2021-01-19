using System;
using Models;

namespace Crm6
{

    public partial class ExchangeSyncErrorLog : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            if (!Page.IsPostBack)
            {
                LoadSyncLog();
            }
        }

        private void LoadSyncLog()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var userId = int.Parse(lblUserId.Text);
            var response = new Helpers.ExchangeSyncErrorLogs().GetExchangeSyncErrorLog(new ExchangeSyncErrorLogFilter
            {
                CurrentPage = 1,
                RecordsPerPage = 50,
                UserId = userId,
                SubscriberId = subscriberId
            });
            rptSyncLog.DataSource = response.SyncErrorLogEntries;
            rptSyncLog.DataBind();

        }
    }
}