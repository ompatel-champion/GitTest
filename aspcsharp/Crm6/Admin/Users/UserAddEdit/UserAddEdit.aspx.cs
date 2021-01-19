using System;
using System.Web.UI.WebControls;
using System.Web.UI;
using Helpers;
using System.Linq;

namespace Crm6.Users
{
    public partial class UserAddEdit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                // load dropdowns
                LoadCurrencies();
                LoadTimeZones();
                LoadUserRoles();
                LoadLocations();
                LoadLanguages();
                LoadCountries();
                LoadReportDateFormats();

                // load user if id passed
                if (Request.QueryString["userid"] != null && int.Parse(Request.QueryString["userid"]) > 0)
                {
                    lblEditingUserId.Text = Request.QueryString["userid"];
                    LoadUser(int.Parse(Request.QueryString["userid"]));

                } 
            }
        }


        #region Load Dropdowns

        private void LoadCurrencies()
        {
            var defaultCurrency = "";
            var currencies = new Helpers.DropdownHelper().GetCurrencies(defaultCurrency);
            foreach (var currency in currencies)
            {
                ddlCurrency.Items.Add(new ListItem(currency.SelectText, currency.SelectValue));
            }

            ddlCurrency.SelectedValue = "USD";
        }

        private void LoadTimeZones()
        {
            var timeZones = new Helpers.DropdownHelper().GetTimeZones();
            foreach (var timeZone in timeZones)
            {
                ddlTimezone.Items.Add(new ListItem(timeZone.SelectText, timeZone.SelectValue));
            }
        }

        private void LoadUserRoles()
        {
            var userRoles = new Helpers.DropdownHelper().GetUserRoles();

            ddlUserRoles.DataSource = userRoles;
            ddlUserRoles.DataTextField = "SelectText";
            ddlUserRoles.DataValueField = "SelectValue";
            ddlUserRoles.DataBind();

        }

        private void LoadLocations()
        {
            var locations = new Helpers.DropdownHelper().GetLocations(int.Parse(lblSubscriberId.Text));
            foreach (var location in locations)
            {
                ddlLocation.Items.Add(new ListItem(location.SelectText, location.SelectValue.ToString()));
            }
        }

        private void LoadCountries()
        {
            var countries = new Helpers.DropdownHelper().GetCountriesWithCode(int.Parse(lblSubscriberId.Text));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue.ToString()));
            }
        }

        private void LoadLanguages()
        {
            var languages = new Helpers.DropdownHelper().GetLanguages(int.Parse(lblSubscriberId.Text));
            foreach (var language in languages)
            {
                ddlDisplayLanguage.Items.Add(new ListItem(language.SelectText, language.SelectValue.ToString()));
            }

            ddlDisplayLanguage.SelectedValue = "en-US";
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

        #endregion


        private void LoadUser(int userId)
        {
            var user = new Helpers.Users().GetUser(userId, int.Parse(lblSubscriberId.Text));
            if (user == null)
            {
                // invalid request, go to the list
                Response.Redirect("/Admin/Users/UserList/UserList.aspx");
            }

            // bind the user
            lblEditingUserId.Text = userId.ToString();
            lblHeaderUsername.Text = user.User.FullName;
            txtFirstName.Text = user.User.FirstName;
            txtLastName.Text = user.User.LastName;
            txtEmailAddress.Text = user.User.EmailAddress;
            txtJobTitle.Text = user.User.Title;
            txtMobile.Text = user.User.MobilePhone;
            txtPhone.Text = user.User.Phone;
            txtFax.Text = user.User.Fax;
            txtSpokenLanguage.Text = user.User.LanguagesSpoken;
            txtPassword.Text = user.User.Password;
            txtBillingCode.Text = user.User.BillingCode;
            ddlCountry.SelectedValue = user.User.CountryCode;

            // timezone
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


                ddlCurrency.SelectedValue = "USD";


            if (ddlReportDateFormat.Items.FindByValue(user.User.ReportDateFormat) != null)
            {
                ddlReportDateFormat.SelectedValue = user.User.ReportDateFormat;
            }

            if (ddlDisplayLanguage.Items.FindByValue(user.User.LanguageCode) != null)
                ddlDisplayLanguage.SelectedValue = "en-US";

            ddlLocation.SelectedValue = user.User.LocationId.ToString();

            // address
            txtAddress.Text = user.User.Address;
            txtCity.Text = user.User.City;
            txtStateProvince.Text = user.User.StateProvince;
            txtPostcode.Text = user.User.PostalCode;
            chkLoginEnabled.Checked = user.User.LoginEnabled;

            //profile pic / svg initials
            var profilePicUrl = new Helpers.Users().GetUserProfilePicUrl(userId, int.Parse(lblSubscriberId.Text));
            imgProfile.Attributes["src"] = profilePicUrl;
            
            // get selected manager sales reps
            var managerSalesreps = new Helpers.Users().GetManagerSalesRepIds(user.User.UserId, int.Parse(lblSubscriberId.Text));
            lblSelectedManagerSalesReps.Text = string.Join(",", managerSalesreps);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            // go to user list
            Response.Redirect("/Admin/Users/UserList/UserList.aspx");
        }

    }
}
