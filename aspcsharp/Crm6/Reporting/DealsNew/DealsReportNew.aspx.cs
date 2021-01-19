using Crm6.App_Code;
using Helpers;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;

namespace Crm6.Reporting.Deals
{
    public partial class DealsReportNew : BasePage
    {
        private readonly DropdownHelper _dropDownHelper = new DropdownHelper();
        private int _subscriberId;
        private int _userId;
        protected string CurrencyText = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set Defaults - get from the session
            var currentUser = LoginUser.GetLoggedInUser();
            _userId = currentUser.User.UserId;
            lblUserId.Text = currentUser.User.UserId.ToString();
            _subscriberId = currentUser.User.SubscriberId;
            lblSubscriberId.Text = _subscriberId.ToString();
            lblUserRole.Text = currentUser.User.UserRoles;

            //TODO: fill Hidden labels ...
            lblCurrencyCode.Text = currentUser.User.CurrencyCode;
            // Set Jquery DatePicker Date Format
            if (!string.IsNullOrEmpty(currentUser.User.DateFormatReports))
            {
                lblDateFormat.Text = currentUser.User.DateFormatReports;
            }
            lblCurrencyCode.Text = currentUser.User.CurrencyCode;
            var currencySymbol = new Currencies().GetCurrencySymbolFromCode(currentUser.User.CurrencyCode);
            lblCurrencySymbol.Text = currencySymbol;
            lblShipmentFrequency.Text = currentUser.Subscriber.DefaultShippingFrequency;
            CurrencyText = currentUser.User.CurrencyCode + currencySymbol;
            //DateFormatMask = GetDatePickerFormatMaskFromUserId(UserId);
            //LanguageCode

            if (!Page.IsPostBack)
            {
                // load drop downs 
                LoadCurrencies();
                LoadDateTypes();
                LoadSalesStages();
                LoadServices();
                LoadDealTypes();
                LoadIndustries();
                LoadManagingUsers();
                LoadCountries();
                LoadCampaigns(); 
              //  LoadCompetitors(); 
                if (Request.QueryString["locationcode"] != null)
                {
                    LoadLocation(Request.QueryString["locationcode"]);
                }
                // set header
                // lblReportHeader.Text = Request.QueryString["spot"] != null ? "Spot Deals" : "Deals";
                lblIsSpotDealReports.Text = Request.QueryString["spot"] != null ? "1" : "0";
            }

        }

        #region Dropdowns


        /// <summary>
        /// load deal date types dropdown
        /// </summary>
        private void LoadDateTypes()
        {
            var dateTypes = new Helpers.DealsReport().GetDealDateTypes();
            foreach (var dateType in dateTypes)
            {
                ddlDateType.Items.Add(dateType);
            }
        }
        /// <summary>
        /// load sales stage dropdown
        /// </summary>
        private void LoadSalesStages()
        {
            var salesStages = _dropDownHelper.GetSalesStages(_subscriberId);
            foreach (var stage in salesStages)
            {
                ddlSalesStage.Items.Add(new ListItem(stage.SelectText.Trim(), stage.SelectText.Trim()));
            }
        }

        /* Advanced Filter dropdowns */
        /// <summary>
        /// load services dropdown
        /// </summary>
        public void LoadServices()
        {
            var services = _dropDownHelper.GetServices();
            foreach (var service in services)
            {
                ddlService.Items.Add(new ListItem(service.SelectText, service.SelectValue));
            }
        }

        /// <summary>
        /// load deal types dropdown
        /// </summary>
        public void LoadDealTypes()
        {
            var dealTypes = _dropDownHelper.GetDealTypes(_subscriberId);
            foreach (var dealType in dealTypes)
            {
                ddlDealType.Items.Add(new ListItem(dealType.SelectText, dealType.SelectValue));
            }
        }

        /// <summary>
        /// load industries dropdown
        /// </summary>
        public void LoadIndustries()
        {
            var industries = _dropDownHelper.GetIndustries(_subscriberId);
            foreach (var industry in industries)
            {
                ddlIndustry.Items.Add(new ListItem(industry.SelectText, industry.SelectValue));
            }
        }



        /// <summary>
        /// load all the users managed by the logged in user
        /// </summary>
        private void LoadManagingUsers()
        {
            var users = new DropdownHelper().GetManagingUsersByUser(LoginUser.GetLoggedInUserId(), int.Parse(lblSubscriberId.Text));
            ddlUser.Items.Add(new ListItem("Select User...", "0"));
            foreach (var user in users)
            {
                ddlUser.Items.Add(new ListItem(user.SelectText, user.SelectValue.ToString()));
            }
        }


        /// <summary>
        /// load countries dropdowns
        /// </summary>
        private void LoadCountries()
        {
            var countries = _dropDownHelper.GetCountriesWithCode(int.Parse(lblSubscriberId.Text)).OrderBy(t=>t.SelectText);
            foreach (var country in countries)
            {
                ddlOriginCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue));
                ddlDestinationCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue));
            }

            var userCountries = _dropDownHelper.GetCountriesForSubscriberUsers(_subscriberId).OrderBy(t => t.SelectText).ToList();
            foreach (var country in userCountries)
            {
                ddlUserCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue));
            }
        }


        private void LoadCurrencies()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var currencies = new Helpers.DealsReport().GetCurrencies(subscriberId);
            foreach (var currency in currencies)
            {
                ddlCurrency.Items.Add(new ListItem(currency.SelectText, currency.SelectValue));
            }
            ddlCurrency.SelectedValue = lblCurrencyCode.Text + "|" + lblCurrencySymbol.Text;
        }


        private void LoadLocation(string locationCode)
        {
            var location = new Helpers.Locations().GetLocationByCode(locationCode, int.Parse(lblSubscriberId.Text));
            if (location != null)
            {
                ddlLocations.Items.Add(new ListItem(location.LocationName, location.LocationCode));
            }
        }


        private void LoadCampaigns()
        {
            var campaigns = new DropdownHelper().GetCampaigns(int.Parse(lblSubscriberId.Text));
            foreach (var campaign in campaigns)
            {
                ddlCampaigns.Items.Add(new ListItem(campaign.SelectText, campaign.SelectValue));
            }

        }


        //private void LoadCompetitors()
        //{
        //    var subscriberId = int.Parse(lblSubscriberId.Text);
        //    var currentUser = LoginUser.GetLoggedInUser();
        //    var competitors = new DropdownHelper().GetCompetitors(subscriberId);

        //    foreach (var c in competitors)
        //    {
        //        ddlCompetitors.Items.Add(new ListItem(c.SelectText, c.SelectText));
        //    }
        //}

        #endregion

    }
}