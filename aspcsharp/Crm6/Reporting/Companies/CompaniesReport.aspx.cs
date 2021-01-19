using Helpers;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Reporting.Companies
{

    public partial class CompaniesReport : BasePage
    {
        private readonly DropdownHelper _dropDownHelper = new DropdownHelper();
        private int _subscriberId;
        private int _userId;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Set Defaults - get from the session
            var currentUser = LoginUser.GetLoggedInUser();

            if (currentUser != null)
            {
                _userId = currentUser.User.UserId;
                lblUserId.Text = currentUser.User.UserId.ToString();
                _subscriberId = currentUser.User.SubscriberId;
                lblSubscriberId.Text = _subscriberId.ToString();
                lblUsername.Text = currentUser.User.FirstName + " " + currentUser.User.LastName;

                if (!Page.IsPostBack)
                {
                    LoadStatuses();
                    LoadCountries(_subscriberId);
                    LoadCompetitors(_subscriberId);
                    LoadCampaigns(_subscriberId);
                    LoadSources(_subscriberId);
                    LoadIndustries(_subscriberId);
                }
            }
            else
            {
                Response.Redirect("/Login.aspx");
            }
        }

        private void LoadStatuses()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem("All Companies", "All Companies"));
            ddlStatus.Items.Add(new ListItem("Active Companies", "Active Companies"));
            ddlStatus.Items.Add(new ListItem("Inactive Companies", "Inactive Companies"));
            ddlStatus.Items.Add(new ListItem("All Customers", "All Customers"));
            ddlStatus.Items.Add(new ListItem("Active Customers", "Active Customers"));
            ddlStatus.Items.Add(new ListItem("Inactive Customers", "Inactive Customers"));
        }

        private void LoadCountries(int subscriberId)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var companyCountries = new DropdownHelper().GetCountries();

            ddlCountry.Items.Add(new ListItem("", ""));

            foreach (var c in companyCountries)
            {
                ddlCountry.Items.Add(new ListItem(c.SelectText, c.SelectText));
            }
        }

        private void LoadCompetitors(int subscriberId)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var competitors = new DropdownHelper().GetCompetitors(subscriberId);

            ddlCompetitor.Items.Add(new ListItem("", ""));
            foreach (var c in competitors)
            {
                ddlCompetitor.Items.Add(new ListItem(c.SelectText, c.SelectText));
            }
        }

        private void LoadCampaigns(int subscriberId)
        {
            var campaigns = new DropdownHelper().GetCampaigns(subscriberId);

            ddlCampaign.Items.Add(new ListItem("", ""));
            foreach (var campaign in campaigns)
            {
                ddlCampaign.Items.Add(new ListItem(campaign.SelectText, campaign.SelectValue));
            }
        }

        private void LoadSources(int subscriberId)
        {
            var sources = new DropdownHelper().GetSources(subscriberId);

            ddlSource.Items.Add(new ListItem("", ""));
            foreach (var source in sources)
            {
                ddlSource.Items.Add(new ListItem(source.SelectText, source.SelectValue));
            }
        }

        private void LoadIndustries(int subscriberId)
        {
            var industries = new DropdownHelper().GetIndustries(subscriberId);

            ddlIndustry.Items.Add(new ListItem("", ""));
            foreach (var industry in industries)
            {
                ddlIndustry.Items.Add(new ListItem(industry.SelectText, industry.SelectValue));
            }
        }

    }
}
