using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Companies.Reassign
{
    public partial class ReassignCompany : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString(); 
            if (!Page.IsPostBack)
            {
                int companyId = 0;

                if (Request.QueryString["companyId"] != null && int.TryParse(Request.QueryString["companyId"], out companyId))
                {
                    lblCompanyId.Text = Request.QueryString["companyId"];


                    var connection = LoginUser.GetConnection( );
                    var context = new DbFirstFreightDataContext(connection);
                    var company = (from t in context.Companies where t.CompanyId == companyId select t).FirstOrDefault();

                    lblBreadcrumbHeader.Text = $"Reassign Company - {company?.CompanyName}";
                }
            }
        }
    }
}