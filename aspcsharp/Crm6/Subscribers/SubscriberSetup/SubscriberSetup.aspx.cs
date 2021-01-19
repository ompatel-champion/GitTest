using Helpers;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Models;

namespace Crm6.Subscribers
{
    public partial class SubscriberSetup : Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadTimeZones();
                Loadlanguages();
                LoadCountries();
                LoadDataCenters();
            }
        }

        #region Load Dropdowns

        private void LoadTimeZones()
        {
            var timeZones = new Helpers.DropdownHelper().GetTimeZones();
            ddlTimeZone.Items.Add(new ListItem("Select Timezone...", "0"));
            foreach (var timeZone in timeZones)
            {
                ddlTimeZone.Items.Add(new ListItem(timeZone.SelectText, timeZone.SelectValue));
            }
        }


        private void Loadlanguages()
        {
            var languages = new Languages().PopulateLanguagesDropdown();
            ddlLanguage.Items.Add(new ListItem("Select Language...", ""));
            foreach (var language in languages)
            {
                ddlLanguage.Items.Add(new ListItem(language.SelectText, language.SelectValue));
            }
        }


        private void LoadCountries()
        {
            var countries = new DropdownHelper().GetCountries();
            ddlCountry.Items.Add(new ListItem("Select country...", ""));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue));
            }
        }


        private void LoadDataCenters()
        {
            ddlDataCenter.Items.Add(new ListItem("Select Data Center...", ""));
            ddlDataCenter.Items.Add(new ListItem("EMEA", "EMEA"));
            ddlDataCenter.Items.Add(new ListItem("HKG", "HKG"));
            ddlDataCenter.Items.Add(new ListItem("USA", "USA"));
        }

        #endregion

        #region Setup Subscriber

        protected void btnSetupSubscriber_Click(object sender, EventArgs e)
        {
            if (ValidateSave())
            {
                SetupSubscriber();
                divErrors.Visible = false;
                divSuccess.Visible = true;
            }
            else
            {
                divErrors.Visible = true;
                divSuccess.Visible = false;
            }
        }


        private bool ValidateSave()
        {
            // clear the errors
            plhldrErrors.Controls.Clear();
            divErrors.Visible = false;
            divSuccess.Visible = false;

            // check the mandatory inputs
            if (string.IsNullOrWhiteSpace(txtAddress.Text))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Address</strong> can not be left blank.</li>"));
            if (string.IsNullOrWhiteSpace(txtCity.Text))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>City</strong> can not be left blank.</li>"));
            if (string.IsNullOrWhiteSpace(ddlCountry.SelectedValue))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Country</strong> has to be selected.</li>"));
            if (string.IsNullOrWhiteSpace(txtCompanyName.Text))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Company Name</strong> can not be left blank.</li>"));
            if (string.IsNullOrWhiteSpace(ddlDataCenter.SelectedValue))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Data Center</strong> has to be selected.</li>"));
            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>First Name</strong> can not be left blank.</li>"));
            if (string.IsNullOrWhiteSpace(txtLastName.Text))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Last Name</strong> can not be left blank.</li>"));
            if (string.IsNullOrWhiteSpace(ddlLanguage.SelectedValue))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Language</strong> has to be selected.</li>"));
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Password</strong> can not be left blank.</li>"));
            if (string.IsNullOrWhiteSpace(txtPostalCode.Text))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Postal Code</strong> can not be left blank.</li>"));
            //if (string.IsNullOrWhiteSpace(txtStateProvince.Text))
            //    plhldrErrors.Controls.Add(new LiteralControl("<li><strong>State / Province</strong> can not be left blank.</li>"));
            if (string.IsNullOrWhiteSpace(ddlTimeZone.SelectedValue))
                plhldrErrors.Controls.Add(new LiteralControl("<li><strong>Timezone</strong> has to be selected.</li>"));
            return !(plhldrErrors.Controls.Count > 0);
        }


        private void SetupSubscriber()
        {
            var subscriberInfo = new SubscriberSetupModel
            {
                Address = txtAddress.Text,
                City = txtCity.Text,
                CompanyName = txtCompanyName.Text,
                ContactName = txtFirstName.Text + " " + txtLastName.Text,
                CountryName = ddlCountry.Text,
                DataCenter = ddlDataCenter.Text,
                Email = txtEmail.Text,
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Language = ddlLanguage.Text,
                Password = txtPassword.Text,
                PostalCode = txtPostalCode.Text,
                StateProvince = txtStateProvince.Text,
                Subdomain = txtSubDomain.Text,
                Timezone = ddlTimeZone.Text
            };

            new SetupSubscriber().NewSubscriber(subscriberInfo);
        }

        #endregion

    }
}