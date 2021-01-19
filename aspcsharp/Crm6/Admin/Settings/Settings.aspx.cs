using System;
using System.Web.UI;

namespace Crm6.Admin
{
    public partial class Settings : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                LoadCommodities();
                LoadCompanySegments();
                LoadCompanyTypes();
                LoadCompetitors();
                LoadContactTypes();
                LoadDealTypes();
                LoadIndustries();
                LoadLostReasons();
                LoadSalesStages();
                LoadSources();
                LoadTags();
                LoadWonReasons();
                LoadSalesTeamRoles();
            }
        }

        void LoadCommodities()
        {
            var commodities = new Helpers.Commodities().GetCommodities(int.Parse(lblSubscriberId.Text));
            rptCommodities.DataSource = commodities;
            rptCommodities.DataBind();
        }

        void LoadSalesTeamRoles()
        {
            var salesTeamRoles = new Helpers.SalesReps().GetSalesTeamRoles(int.Parse(lblSubscriberId.Text));
            rptSalesTeamRoles.DataSource = salesTeamRoles;
            rptSalesTeamRoles.DataBind();
        }

        void LoadCompanySegments()
        {
            var companySegments = new Helpers.CompanySegments().GetCompanySegments(int.Parse(lblSubscriberId.Text));
            rptCompanySegments.DataSource = companySegments;
            rptCompanySegments.DataBind();
        }

         void LoadCompanyTypes()
        {
            var companyTypes = new Helpers.CompanyTypes().GetCompanyTypes(int.Parse(lblSubscriberId.Text));
            rptCompanyTypes.DataSource = companyTypes;
            rptCompanyTypes.DataBind(); ;
        }

        void LoadCompetitors()
        {
            var competitors = new Helpers.Competitors().GetCompetitors(int.Parse(lblSubscriberId.Text));
            rptCompetitors.DataSource = competitors;
            rptCompetitors.DataBind(); ;
        }

        void LoadContactTypes()
        {
            var contactTypes = new Helpers.ContactTypes().GetContactTypes(int.Parse(lblSubscriberId.Text));
            rptContactTypes.DataSource = contactTypes;
            rptContactTypes.DataBind(); ;
        }

        void LoadDealTypes()
        {
            var dealTypes = new Helpers.DealTypes().GetDealTypes(int.Parse(lblSubscriberId.Text));
            rptDealTypes.DataSource = dealTypes;
            rptDealTypes.DataBind();
        }

        void LoadIndustries()
        {
            var industries = new Helpers.Industries().GetIndustries(int.Parse(lblSubscriberId.Text));
            rptIndustries.DataSource = industries;
            rptIndustries.DataBind();
        }

        void LoadLostReasons()
        {
            var lostReasons = new Helpers.LostReasons().GetLostReasons(int.Parse(lblSubscriberId.Text));
            rptLostReasons.DataSource = lostReasons;
            rptLostReasons.DataBind();
        }

        void LoadSalesStages()
        {
            var salesStages = new Helpers.SalesStages().GetSalesStages(int.Parse(lblSubscriberId.Text));
            rptSalesStages.DataSource = salesStages;
            rptSalesStages.DataBind();
        }

        void LoadSources()
        {
            var sources = new Helpers.Sources().GetSources(int.Parse(lblSubscriberId.Text));
            rptSources.DataSource = sources;
            rptSources.DataBind();
        }

        void LoadTags()
        {
            var tags = new Helpers.Tags().GetTags(int.Parse(lblSubscriberId.Text));
            rptTags.DataSource = tags;
            rptTags.DataBind();
        }

        void LoadWonReasons()
        {
            var wonReasons = new Helpers.WonReasons().GetWonReasons(int.Parse(lblSubscriberId.Text));
            rptWonReasons.DataSource = wonReasons;
            rptWonReasons.DataBind();
        }

    }
}
