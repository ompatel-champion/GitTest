using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Companies.CompanyAddEdit
{
    public partial class CompanyAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // set page title
            Title = "Add Company";

            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                var companySubscriberId = currentUser.User.SubscriberId;
                if (Request.QueryString["subscriberid"] != null && Utils.IsNumeric(Request.QueryString["subscriberid"])
                    && int.Parse(Request.QueryString["subscriberid"]) > 0)
                {
                    companySubscriberId = int.Parse(Request.QueryString["subscriberid"]);
                }
                lblCompanySubscriberId.Text = companySubscriberId.ToString();
                lblIsAdmin.Text = (!string.IsNullOrEmpty(currentUser.User.UserRoles) &&
                                    currentUser.User.UserRoles.Contains("CRM Admin")) ? "1" : "0";

                lblGuid.Text = Guid.NewGuid().ToString();

                // load drop downs
                LoadCampaigns(currentUser.Subscriber.SubscriberId);
                LoadCompanyTypes();
                LoadCountries();
                LoadIndustries();
                LoadSources();
                LoadUsers();

                if (Request.QueryString["companyId"] != null && int.Parse(Request.QueryString["companyId"]) > 0)
                {
                    // TODO: GlobalCompany
                    lblCompanyId.Text = Request.QueryString["companyId"];
                    LoadCompany(int.Parse(Request.QueryString["companyId"]));
                }
                else
                {
                    // add company
                    lblBreadcrumbHeader.Text = "Add Company ";
                    chkActive.Enabled = true;
                    chkCustomer.Enabled = false;
                }
            }
        }

        private void LoadCompany(int companyId)
        {
            var subscriberId = int.Parse(lblCompanySubscriberId.Text);
            var company = new Helpers.Companies().GetCompany(companyId, subscriberId);
            if (company != null)
            {
                // set page title
                Title = "Edit Company - " + company.CompanyName;
                // populate form controls
                lblBreadcrumbHeader.Text = "Edit Company ";
                txtCompanyName.Text = company.CompanyName;
                txtCity.Text = company.City;
                txtAddress.Text = company.Address;
                txtPostalCode.Text = company.PostalCode;
                txtPhone.Text = company.Phone;
                txtStateProvince.Text = company.StateProvince;
                ddlCountry.SelectedValue = company.CountryName;
                ddlIndustry.SelectedValue = company.Industry;
                ddlCompanyType.SelectedValue = company.CompanyTypes;
                ddlSource.SelectedValue = company.Source;
                txtFax.Text = company.Fax;
                txtWebsite.Text = company.Website;
                txtDivision.Text = company.Division;
                txtCompanyCode.Text = company.CompanyCode;
                chkActive.Checked = company.Active;
                chkCustomer.Checked = company.IsCustomer;
                ddlCampaign.SelectedValue = company.CampaignName;
                Notes.Text = company.Comments;
                if (ddlOwner.Items.FindByValue(company.CompanyOwnerUserId.ToString()) != null)
                {
                    ddlOwner.SelectedValue = company.CompanyOwnerUserId.ToString();
                }
                else
                {
                    ddlOwner.SelectedValue = company.CreatedUserId.ToString();
                }
                // company types
                var customerTypes = (company.CompanyTypes + "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var customerTypesInputs = new List<AutoComplete>();
                var cTypes = new CompanyTypes().GetCompanyTypes(company.SubscriberId);
                foreach (var cType in customerTypes)
                {
                    var found = cTypes.FirstOrDefault(t => t.CompanyTypeName.ToLower() == cType.ToLower());
                    if (found != null)
                    {
                        customerTypesInputs.Add(new AutoComplete { id = found.CompanyTypeId, name = found.CompanyTypeName });
                    }
                }
                
                //company logo / svg initials
                var logoUrl = new Helpers.Companies().GetCompanyLogoUrl(companyId, subscriberId);
                imgCompanyLogo.Attributes["src"] = logoUrl;
            }
        }

        private void LoadCountries()
        {
            var countries = new DropdownHelper().GetCountries();
            ddlCountry.Items.Add(new ListItem("", "0"));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue.ToString()));
            }
            // load user country
            var currentUser = LoginUser.GetLoggedInUser();
            if (!string.IsNullOrEmpty(currentUser.User.CountryName))
            {
                ddlCountry.SelectedValue = currentUser.User.CountryName;
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

        private void LoadIndustries()
        {
            var industries = new Industries().GetIndustriesForDropdown(int.Parse(lblCompanySubscriberId.Text));
            ddlIndustry.Items.Add(new ListItem("", "0"));
            foreach (var industry in industries)
            {
                ddlIndustry.Items.Add(new ListItem(industry.SelectValue, industry.SelectText.ToString()));
            }
        }

        private void LoadSources()
        {
            var sources = new Sources().GetSources(int.Parse(lblCompanySubscriberId.Text));
            ddlSource.Items.Add(new ListItem("", "0"));
            foreach (var source in sources)
            {
                ddlSource.Items.Add(new ListItem(source.SourceName, source.SourceName.ToString()));
            }
        }

        private void LoadCompanyTypes()
        {
            var companyTypes = new CompanyTypes().GetCompanyTypesForDropdown(int.Parse(lblCompanySubscriberId.Text));
            ddlCompanyType.Items.Add(new ListItem("", "0"));
            foreach (var type in companyTypes)
            {
                ddlCompanyType.Items.Add(new ListItem(type.SelectValue, type.SelectText.ToString()));
            }
        }

        private void LoadUsers()
        {
            // TODO: GlobalUsers
            var users = new DropdownHelper().GetUsers(int.Parse(lblCompanySubscriberId.Text));
            ddlOwner.Items.Add(new ListItem("", "0"));
            foreach (var user in users)
            {
                ddlOwner.Items.Add(new ListItem(user.SelectText, user.SelectValue.ToString()));
            }
            // select the logged in user as the sales rep
            var loggedinUserId = lblUserId.Text;
            if (ddlOwner.Items.FindByValue(loggedinUserId) != null)
            {
                ddlOwner.SelectedValue = loggedinUserId;
            }
        }

    }
}
