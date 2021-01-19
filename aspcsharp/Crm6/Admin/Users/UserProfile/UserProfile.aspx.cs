using Helpers;
using System;
using System.Web.UI.WebControls;

namespace Crm6.Users
{
    public partial class UserProfile : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblGuid.Text = Guid.NewGuid().ToString();

            if (!Page.IsPostBack)
            {
                // load dropdowns
                LoadCurrencies();
                LoadTimeZones();
                LoadLocations();
                LoadLanguages();
                LoadCountries();
                LoadDistricts();
                LoadRegions();
                LoadDateFormats();
                LoadReportDateFormats();
                LoadUserProfile(currentUser.User.UserId, currentUser.Subscriber.SubscriberId);
            }
        }


        #region Load Dropdowns

        /// <summary>
        /// load countries 
        /// </summary>
        private void LoadCountries()
        {
            var countries = new DropdownHelper().GetCountries();
            ddlCountry.Items.Add(new ListItem("Select Country...", "0"));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue.ToString()));
            }
        }

        /// <summary>
        /// load currencies
        /// </summary>
        private void LoadCurrencies()
        {
            var defaultCurrency = "";
            var currencies = new DropdownHelper().GetCurrencies(defaultCurrency);
            ddlCurrency.Items.Add(new ListItem("Select Currency...", "0"));
            foreach (var currency in currencies)
            {
                ddlCurrency.Items.Add(new ListItem(currency.SelectText, currency.SelectValue));
            }
        }


        /// <summary>
        /// load time zones
        /// </summary>
        private void LoadTimeZones()
        {
            var timeZones = new DropdownHelper().GetTimeZones();
            ddlTimezone.Items.Add(new ListItem("Select Timezone...", "0"));
            foreach (var timeZone in timeZones)
            {
                ddlTimezone.Items.Add(new ListItem(timeZone.SelectText, timeZone.SelectValue));
            }
        }


        /// <summary>
        /// load locations
        /// </summary>
        private void LoadLocations()
        {
            var locations = new DropdownHelper().GetLocations(int.Parse(lblSubscriberId.Text));
            ddlLocation.Items.Add(new ListItem("Select Location...", "0"));
            foreach (var location in locations)
            {
                ddlLocation.Items.Add(new ListItem(location.SelectText, location.SelectValue.ToString()));
            }
        }



        private void LoadDateFormats()
        {
            var dateFormats = new DropdownHelper().GetDateFormats();
            ddlDateFormat.Items.Add(new ListItem("Select Date Format...", ""));
            foreach (var dateFormat in dateFormats)
            {
                ddlDateFormat.Items.Add(new ListItem(dateFormat.SelectText, dateFormat.SelectValue.ToString()));
            }
        }

        private void LoadReportDateFormats()
        {
            var dateFormats = new DropdownHelper().GetDateFormats();
            ddlReportDateFormat.Items.Add(new ListItem("Select Date Format...", ""));
            foreach (var dateFormat in dateFormats)
            {
                ddlReportDateFormat.Items.Add(new ListItem(dateFormat.SelectText, dateFormat.SelectValue.ToString()));
            }
        }

        /// <summary>
        /// load languages
        /// </summary>
        private void LoadLanguages()
        {
            var languages = new DropdownHelper().GetLanguages(int.Parse(lblSubscriberId.Text));
            ddlDisplayLanguage.Items.Add(new ListItem("Select Language...", ""));
            foreach (var language in languages)
            {
                ddlDisplayLanguage.Items.Add(new ListItem(language.SelectText, language.SelectValue.ToString()));
            }
        }

        /// <summary>
        /// load districts 
        /// </summary>
        private void LoadDistricts()
        {
            var districts = new Helpers.DropdownHelper().GetDistricts(int.Parse(lblSubscriberId.Text));
            ddlDistrict.Items.Add(new ListItem("Select District...", ""));
            foreach (var district in districts)
            {
                ddlDistrict.Items.Add(new ListItem(district.SelectText, district.SelectValue.ToString()));
            }
        }

        /// <summary>
        /// load regions 
        /// </summary>
        private void LoadRegions()
        {
            var regions = new  DropdownHelper().GetRegions(int.Parse(lblSubscriberId.Text));
            ddlRegions.Items.Add(new ListItem("Select Region...", ""));
            foreach (var region in regions)
            {
                ddlRegions.Items.Add(new ListItem(region.SelectText, region.SelectValue.ToString()));
            }
        }


        #endregion


        private void LoadUserProfile(int userId, int subscriberId)
        {
            var user = new Helpers.Users().GetUser(userId, subscriberId);
            // bind the user profile
            lblUserFullName.Text = user.User.FullName;
            lblHeaderUsername.Text = user.User.FullName;
            lblJobTitle.Text = user.User.Title;
            lblLocationAndCompany.Text = user.User.LocationId > 0 ? user.Subscriber.CompanyName : "";
            txtFirstName.Text = user.User.FirstName;
            txtLastName.Text = user.User.LastName;
            txtEmailAddress.Text = user.User.EmailAddress.Trim();
            txtJobTitle.Text = user.User.Title;
            txtLanguagesSpoken.Text = user.User.LanguagesSpoken;
            txtMobile.Text = user.User.MobilePhone;
            txtPhone.Text = user.User.Phone;
            txtFax.Text = user.User.Fax;
            ddlCountry.SelectedValue = user.User.CountryName;
            // timezones
            var timezone = new Timezones().GetTimeZoneIdByTimeZoneNameOffcetAndName(user.User.TimeZone,
               user.User.TimeZoneCityNames,
               user.User.TimeZoneOffset);
            if (timezone != null)
            {
                if (ddlTimezone.Items.FindByValue(timezone.TimeZoneId.ToString()) != null)
                {
                    ddlTimezone.SelectedValue = timezone.TimeZoneId.ToString();
                }
            }

            ddlCurrency.SelectedValue = user.User.CurrencyCode;
            if (ddlDisplayLanguage.Items.FindByValue(user.User.LanguageCode) != null)
                ddlDisplayLanguage.SelectedValue = user.User.LanguageCode;

            ddlLocation.SelectedValue = user.User.LocationId.ToString();
            ddlDistrict.SelectedValue = user.User.DistrictCode;
            if (ddlRegions.Items.FindByValue(user.User.RegionName) != null)
                ddlRegions.SelectedValue = user.User.RegionName;
            ddlCountry.SelectedValue = user.User.CountryName;

            // date formats
            if (ddlDateFormat.Items.FindByValue(user.User.DateFormat) != null)
            {
                ddlDateFormat.SelectedValue = user.User.DateFormat;
            }
            if (ddlReportDateFormat.Items.FindByValue(user.User.ReportDateFormat) != null)
            {
                ddlReportDateFormat.SelectedValue = user.User.ReportDateFormat;
            }

            //profile pic / svg initials
            var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(userId, subscriberId);
            imgProfile.Attributes["src"] = profilePicUrl;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            // delete user session
            LoginUser.DeleteUserSession();
            // redirect to login page
            Response.Redirect("/login.aspx");
        }

        protected void btnActivateGoogleSync_Click(object sender, EventArgs e)
        {
            Response.Redirect("/Google/GoogleConnector.aspx?userId=" + lblUserId.Text + "&subscriberId=" + lblSubscriberId.Text);
        }
    }
}
