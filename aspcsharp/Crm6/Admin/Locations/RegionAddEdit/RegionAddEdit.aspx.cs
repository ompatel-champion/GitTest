using System;
using System.Web.UI;

namespace Crm6.Admin
{
    public partial class RegionAddEdit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString(); 
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();

            if (!Page.IsPostBack)
            {  
                if (Request.QueryString["regionId"] != null && int.Parse(Request.QueryString["regionId"]) > 0)
                {
                    lblRegionId.Text = Request.QueryString["regionId"];
                    LoadRegion(currentUser.Subscriber.SubscriberId);
                }
            }
        }

        private void LoadRegion(int subscriberId)
        {
            int regionId = int.Parse(lblRegionId.Text);
            var region = new Helpers.Regions().GetRegion(regionId, subscriberId);
            if (region != null)
            {
                lblBreadcrumbHeader.Text = "Edit Region";
                txtRegionName.Text = region.RegionName; 
            }
        }
    }
}
