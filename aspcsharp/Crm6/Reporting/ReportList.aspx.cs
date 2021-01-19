using System;

namespace Crm6.Reporting
{
    public partial class ReportList : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(currentUser.User.UserRoles) && currentUser.User.UserRoles.Contains("CRM Admin"))
                {
                    divCompaniesReport.Visible = true;
                    divUserActivityReport.Visible = true;
                } 
            }
        }
    }
}