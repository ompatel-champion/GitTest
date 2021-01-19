using System;
using Helpers;
using System.Web.UI.WebControls;

namespace Crm6._usercontrols.AddSalesTeamMember
{
    public partial class AddSalesTeamMember : System.Web.UI.UserControl
    {
        public string DetailType {get; set;}

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            if (currentUser != null)
            {
                PopuplateSalesTeamRoles(currentUser.User.SubscriberId);
            }
        }

        private void PopuplateSalesTeamRoles(int SubscriberId)
        {
            var subscriberId = SubscriberId;
            var salesTeamRoles = new DropdownHelper().GetSalesTeamRoles(subscriberId);
            ddlSalesTeamRole.Items.Add(new ListItem("", ""));
            foreach (var role in salesTeamRoles)
            {
                ddlSalesTeamRole.Items.Add(new ListItem(role.SelectText, role.SelectValue));
            }
        }
    }
}