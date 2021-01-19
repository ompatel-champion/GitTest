using Crm6.App_Code;
using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI.WebControls;

namespace Crm6.Deals
{
    public partial class MarkAsWonLost : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var subscriberId = currentUser.Subscriber.SubscriberId;
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["dealId"] != null && int.Parse(Request.QueryString["dealId"]) > 0)
                {
                    lblDealId.Text = Request.QueryString["dealId"];
                }
                if (Request.QueryString["won"] != null && int.Parse(Request.QueryString["won"]) > 0)
                {
                    lblIsWon.Text = "1";
                }

                // load reasons
                LoadWonLostReasons(subscriberId);
            }
        }


        private void LoadWonLostReasons(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var isWon = lblIsWon.Text == "1";
            var reasons = new List<SelectList>();
            reasons.Add(new SelectList { SelectText = "Select Reason", SelectValue = "0" });
            if (isWon)
                reasons.AddRange(new DropdownHelper().GetWonReasons(int.Parse(lblSubscriberId.Text)));
            else
                reasons.AddRange(new DropdownHelper().GetLostReasons(int.Parse(lblSubscriberId.Text)));
            foreach (var reason in reasons)
            {
                ddlWonLostReason.Items.Add(new ListItem(reason.SelectText, reason.SelectValue));
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            var subscriberId = HttpContext.Current.Session["subscriberId"] as int? ?? 0;
            // mark won or lost
            new Helpers.Deals().MarkAsWonLost(new Deal
            {
                DealId = int.Parse(lblDealId.Text),
                Won = lblIsWon.Text == "1",
                Lost = lblIsWon.Text != "1",
                UpdateUserId = int.Parse(lblUserId.Text),
                ReasonWonLost = ddlWonLostReason.SelectedItem.Text
            }, subscriberId);
            // parent
            ClientScript.RegisterStartupScript(GetType(), "Success", "parent.MarkWonLostSuccess('" + lblIsWon.Text + "');", true);
        }
    }
}
