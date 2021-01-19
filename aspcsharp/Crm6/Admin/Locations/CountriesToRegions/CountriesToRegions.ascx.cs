using System;
using System.Linq;
using Helpers;

namespace Crm6.Admin.CountriesToRegions
{
    public partial class CountriesToRegionsControl : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        { 
            LoadRegions(); 
        }

        private void LoadRegions() {
            var currentUser = LoginUser.GetLoggedInUser();
            var subscriberId = currentUser.Subscriber.SubscriberId;
            var regions = new Regions().GetRegions(subscriberId).Select(r => r.RegionName);
            ddlRegions.DataSource = regions;
            ddlRegions.DataBind();
        }
    }
}