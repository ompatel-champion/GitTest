using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Reporting.KPIs.DetailViews
{
    public partial class Deals : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                LoadDeals();
            }
        }


        private void LoadDeals()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var userId = 0;
            if (Request.QueryString["userId"] != null && int.Parse(Request.QueryString["userId"]) > 0)
            {
                userId = int.Parse(Request.QueryString["userId"]);
            }


            var datefrom = "";
            if (Request.QueryString["datefrom"] != null && !string.IsNullOrEmpty(Request.QueryString["datefrom"]))
            {
                datefrom = Request.QueryString["datefrom"];
            }
            var dateTo = "";
            if (Request.QueryString["dateto"] != null && !string.IsNullOrEmpty(Request.QueryString["dateto"]))
            {
                dateTo = Request.QueryString["dateto"];
            }
            var status = "";
            if (Request.QueryString["status"] != null && !string.IsNullOrEmpty(Request.QueryString["status"]))
            {
                status = Request.QueryString["status"];
            }

            var deals = new SalesRepKPIReport().GetDeals(subscriberId, userId, int.Parse(lblUserId.Text), status, datefrom, dateTo);
            rptDeals.DataSource = deals;
            rptDeals.DataBind();
        }
    }
}