using System;

namespace Crm6.Admin
{
    public partial class Campaigns : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                LoadCampaigns();
            }
        }

        private void LoadCampaigns()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var campaigns = new Helpers.Campaigns().GetCampaigns(subscriberId);
            rptCampaigns.DataSource = campaigns;
            rptCampaigns.DataBind();

        }

    }
}
