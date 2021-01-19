using Crm6.App_Code;
using Helpers;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Crm6.Reporting
{
    public partial class ActivitiesByDateRangeReport : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // set defaults - get from the session
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();
            lblSubscriberId.Text = currentUser.User.SubscriberId.ToString();
            lblUsername.Text = currentUser.User.FirstName + " " + currentUser.User.LastName;
            // Set jquery DatePicker Date Format
            if (!string.IsNullOrEmpty(currentUser.User.DateFormatReports))
            {
                lblDateFormat.Text = currentUser.User.DateFormatReports;
            }

            if (!Page.IsPostBack)
            {
                LoadAccessibleGlobalUsers();
                LoadCountries();
                LoadCompetitors();
                LoadCampaigns();
                LoadDealTypes();
            }

            // show/hide country, location and user drop doan
            if (currentUser.User.UserRoles.Contains("CRM Admin") || currentUser.User.UserRoles.Contains("District Manager") || currentUser.User.UserRoles.Contains("Region Manager"))
            {
                divCountries.Visible = true;
                divLocations.Visible = true;
                divUsers.Visible = true;
            }
            else if (currentUser.User.UserRoles.Contains("Country Manager") || currentUser.User.UserRoles.Contains("Country Admin"))
            {
                divLocations.Attributes["class"] = "col-md-12";
                divCountries.Attributes["class"] += divCountries.Attributes["class"] + " hide";
                divLocations.Visible = true;
                divUsers.Visible = true;
            }
            else if (currentUser.User.UserRoles.Contains("Sales Manager") || currentUser.User.UserRoles.Contains("Location Manager"))
            {
                divCountries.Visible = false;
                divLocations.Visible = false;
                divUsers.Visible = true;
            }
            else if (currentUser.User.UserRoles.Contains("Sales Rep"))
            {
                divCountries.Visible = false;
                divLocations.Visible = false;
                divUsers.Visible = false;
            }
        }


        private void LoadCampaigns()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var campaigns = new DropdownHelper().GetCampaigns(subscriberId);
            ddlCampaigns.Items.Add("");
            foreach (var campaign in campaigns)
            {
                ddlCampaigns.Items.Add(new ListItem(campaign.SelectText, campaign.SelectValue));
            }
        }

        private void LoadDealTypes()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var dealTypes = new DropdownHelper().GetDealTypes(subscriberId);
            ddlDealType.Items.Add("");
            foreach (var dealType in dealTypes)
            {
                ddlDealType.Items.Add(new ListItem(dealType.SelectText, dealType.SelectValue.ToString()));
            }
        }

        private void LoadCompetitors()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var currentUser = LoginUser.GetLoggedInUser();
            var competitors = new DropdownHelper().GetCompetitors(subscriberId);
            foreach (var c in competitors)
            {
                ddlCompetitors.Items.Add(new ListItem(c.SelectText, c.SelectValue));
            }
        }


        private void LoadCountries()
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            ddlCountry.Items.Clear();
            ddlCountry.Items.Add("");
            var currentUser = LoginUser.GetLoggedInUser();

            if (currentUser.User.UserRoles.Contains("CRM Admin"))
            {
                var countries = context.Users.Where(x => x.SubscriberId == int.Parse(lblSubscriberId.Text)).Select(t => new { t.CountryCode, t.CountryName }).Distinct().ToList();
                foreach (var item in countries)
                {
                    ddlCountry.Items.Add(new ListItem(item.CountryName, item.CountryCode));
                }
            }
            else if (currentUser.User.UserRoles.Contains("District Manager"))
            {
                var countries = context.Users.Where(x => x.SubscriberId == int.Parse(lblSubscriberId.Text)
                                    && x.DistrictCode == currentUser.User.DistrictCode).Select(t => new { t.CountryCode, t.CountryName }).Distinct().ToList();
                foreach (var item in countries)
                {
                    ddlCountry.Items.Add(new ListItem(item.CountryName, item.CountryCode));
                }
            }
            else if (currentUser.User.UserRoles.Contains("Region Manager"))
            {
                var countries = context.Users.Where(x => x.SubscriberId == int.Parse(lblSubscriberId.Text)
                                  && x.RegionName == currentUser.User.RegionName).Select(t => new { t.CountryCode, t.CountryName }).Distinct().ToList();
                foreach (var item in countries)
                {
                    ddlCountry.Items.Add(new ListItem(item.CountryName, item.CountryCode));
                }
            }
            else if (currentUser.User.UserRoles.Contains("Country Manager") || currentUser.User.UserRoles.Contains("Country Admin"))
            {
                var countries = context.Users.Where(x => x.SubscriberId == int.Parse(lblSubscriberId.Text)
                                  && x.CountryName == currentUser.User.CountryName).Select(t => new { t.CountryCode, t.CountryName }).Distinct().ToList();
                if (countries.Count > 0)
                {
                    var locations = new DropdownHelper().GetUserLocations(new UserLocationRequest
                    {
                        UserId = int.Parse(lblUserId.Text),
                        SubscriberId = int.Parse(lblSubscriberId.Text),
                        CountryCodes = new System.Collections.Generic.List<string> { countries[0].CountryCode }
                    });
                    ddlLocations.Items.Add(new ListItem("", ""));
                    foreach (var location in locations)
                    {
                        ddlLocations.Items.Add(new ListItem(location.SelectText, location.SelectValue));
                    }
                }
                foreach (var item in countries)
                {
                    ddlCountry.Items.Add(new ListItem(item.CountryName, item.CountryCode));
                }
            }
        }

        private void LoadAccessibleGlobalUsers()
        {
            var susbcriberId = int.Parse(lblSubscriberId.Text);
            var globalUserId = int.Parse(lblUserIdGlobal.Text);
            // get users
            var users = new ActitivtyByDateRangeReport().GetAccessibleGlobalUserIdsForUser(globalUserId, susbcriberId);
            // bind to the user list
            foreach (var u in users)
            {
                ddlUsers.Items.Add(new ListItem(u.FullName, u.UserIdGlobal.ToString()));
            }
        }


    }
}
