using System;
using System.Web.UI;

namespace Crm6.Admin.EventCategories
{
    public partial class EventCategories : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                LoadCompanyEventCategories();
            }
        }

        public void LoadCompanyEventCategories()
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var companyEventCategories = new Helpers.CalendarEvents().GetEventCategories(int.Parse(currentUser.Subscriber.SubscriberId.ToString()));
            rptCompanyEventCategories.DataSource = companyEventCategories;
            rptCompanyEventCategories.DataBind();
        }
    }
}