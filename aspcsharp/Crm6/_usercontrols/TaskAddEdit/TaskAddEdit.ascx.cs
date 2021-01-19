using Helpers;
using System; 
using System.Linq; 
using System.Web.UI.WebControls;

namespace Crm6._usercontrols.TaskAddEdit
{
    public partial class WebUserControl1 : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PopulateSalesRepCombo();
        }

        private void PopulateSalesRepCombo()
        {
            var subscriberId = LoginUser.GetLoggedInUser().User.SubscriberId;
            var users = new DropdownHelper().GetLinkedSubsciberSalesRepGlobalUserIds(subscriberId).OrderBy(t => t.SelectText);
            ddlSalesRep.Items.Add(new ListItem("", "0"));
            foreach (var u in users)
            {
                ddlSalesRep.Items.Add(new ListItem(u.SelectText, u.SelectValue));
            }
        } 

    }
}