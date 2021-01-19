using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Reporting.KPIs.DetailViews
{
    public partial class Logins : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                LoadLogins();
            }
        }


        private void LoadLogins()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var userId = 0;
            if (Request.QueryString["userId"] != null && int.Parse(Request.QueryString["userId"]) > 0)
            {
                userId = int.Parse(Request.QueryString["userId"]);
            }

            var locationId = 0;
            if (Request.QueryString["locationId"] != null && int.Parse(Request.QueryString["locationId"]) > 0)
            {
                locationId = int.Parse(Request.QueryString["locationId"]);
            }
            var countryCode = "";
            if (Request.QueryString["countrycode"] != null && !string.IsNullOrEmpty(Request.QueryString["countrycode"]))
            {
                countryCode = Request.QueryString["countrycode"];
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

            var logins = new SalesRepKPIReport().GetLogins(subscriberId, userId, datefrom, dateTo); 
            rptLogins.DataSource = logins;
            rptLogins.DataBind();
        }
    }
}