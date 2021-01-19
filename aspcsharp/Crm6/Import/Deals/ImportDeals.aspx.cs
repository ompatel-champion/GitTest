using Crm6.App_Code;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Helpers;

namespace Crm6.Import
{
    public partial class ImportDeals : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblGuid.Text = Guid.NewGuid().ToString();
            if (!Page.IsPostBack)
            {
                LoadUsers();
            }
        }


        private void LoadUsers()
        {
            var salesReps = new Helpers.DropdownHelper().GetUsers(int.Parse(lblSubscriberId.Text));
            ddlSalesTeam.Items.Add(new ListItem("", "0"));
            salesReps = salesReps.OrderBy(t => t.SelectText).Distinct().ToList();
            foreach (var u in salesReps)
            {
                if (ddlSalesTeam.Items.FindByValue(u.SelectValue.ToString()) == null)
                    ddlSalesTeam.Items.Add(new ListItem(u.SelectText, u.SelectValue.ToString()));
            }

            if (ddlSalesTeam.Items.FindByValue(lblUserId.Text) == null)
            {
                ddlSalesTeam.SelectedValue = lblUserId.Text;
            }
        }


    }
}