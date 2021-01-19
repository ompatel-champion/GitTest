using System;
using System.Web.UI.WebControls;

namespace Crm6.Campaigns
{
    public partial class CampaignAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();
            lblUserName.Text = currentUser.User.FullName;
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {  
                // load users and default to logged-in user
                LoadUsers();

                if (Request.QueryString["campaignId"] != null && int.Parse(Request.QueryString["campaignId"]) > 0)
                {
                    lblCampaignId.Text = Request.QueryString["campaignId"];
                    // load campaign
                    LoadCampaign(int.Parse(lblCampaignId.Text), currentUser.Subscriber.SubscriberId);
                } 
            }
        }


        private void LoadCampaign(int campaignId, int subscriberId)
        {
            var campaign = new Helpers.Campaigns().GetCampaign(campaignId, subscriberId);
            if (campaign != null)
            {
                lblBreadcrumbHeader.Text = "Edit Campaign";
                txtCampaignName.Text = campaign.CampaignName;
                txtCampaignNumber.Text = campaign.CampaignNumber;
                txtComments.Text = campaign.Comments;
                ddlCampaignType.SelectedValue = campaign.CampaignType;
                if (ddlCampaignOwner.Items.FindByValue(campaign.CampaignOwnerUserIdGlobal.ToString()) != null)
                {
                    ddlCampaignOwner.SelectedValue = campaign.CampaignOwnerUserIdGlobal.ToString();
                }
                if (campaign.StartDate.HasValue)
                    txtStartDate.Text = campaign.StartDate.Value.ToString("dd MMM, yyyy");
                if (campaign.EndDate.HasValue)
                    txtEndDate.Text = campaign.EndDate.Value.ToString("dd MMM, yyyy");
                rbtActive.Checked = campaign.CampaignStatus == "Active";
                rbtInactive.Checked = campaign.CampaignStatus != "Active";
            }
        }

        private void LoadUsers()
        {
            var users = new Helpers.DropdownHelper().GetSalesRepGlobalUserIds(int.Parse(lblSubscriberId.Text)); 
            foreach (var user in users)
            {
                ddlCampaignOwner.Items.Add(new ListItem(user.SelectText, user.SelectValue.ToString()));
            }
            // select the logged in user as the campaign owner
            var loggedinUserId = lblUserIdGlobal.Text;
            if (ddlCampaignOwner.Items.FindByValue(loggedinUserId) != null)
            {
                ddlCampaignOwner.SelectedValue = loggedinUserId;
            }
        }
    }

}
