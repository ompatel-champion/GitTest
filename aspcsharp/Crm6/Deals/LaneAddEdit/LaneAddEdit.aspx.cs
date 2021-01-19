using Helpers;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Deals
{
    public partial class LaneAddEdit : BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var subscriberId = currentUser.Subscriber.SubscriberId;
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = subscriberId.ToString();
            int dealId = 0;

            if (!Page.IsPostBack)
            {
                if (Request.QueryString["dealId"] != null && int.Parse(Request.QueryString["dealId"]) > 0)
                {
                    lblDealId.Text = Request.QueryString["dealId"];

                    int.TryParse(lblDealId.Text, out dealId);
                }

                if (Request.QueryString["laneId"] != null && int.Parse(Request.QueryString["laneId"]) > 0)
                {
                    lblLaneId.Text = Request.QueryString["laneId"];
                }
                else if (dealId > 0)
                {
                    //If a new lane (and there is a deal ID), default shipper textbox to current company name.
                    var deal = new Helpers.Deals().GetDeal(dealId, subscriberId);

                    var company = new Helpers.Companies().GetCompany(deal.CompanyId, subscriberId);

                    lblCompanyName.Text = company.CompanyName;
                }

                // load drop downs
                LoadCountries();
                LoadRegions();
                LoadServiceLocations();
                LoadCurrencies();
            }
        }

        private void LoadCountries()
        {
            var countries = new DropdownHelper().GetCountriesWithCode(int.Parse(lblSubscriberId.Text));
            ddlOriginCountry.Items.Add(new ListItem("", ""));
            ddlDestinationCountry.Items.Add(new ListItem("", ""));
            foreach (var country in countries)
            {
                ddlOriginCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue.ToString()));
                ddlDestinationCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue.ToString()));
            }
        }

        private void LoadRegions()
        {
            var regions = new DropdownHelper().GetRegionsShared(int.Parse(lblSubscriberId.Text));
            ddlOriginRegion.Items.Add(new ListItem("", ""));
            ddlDestinationRegion.Items.Add(new ListItem("", ""));
            foreach (var region in regions)
            {
                ddlOriginRegion.Items.Add(new ListItem(region.SelectText, region.SelectValue?.ToString()));
                ddlDestinationRegion.Items.Add(new ListItem(region.SelectText, region.SelectValue?.ToString()));
            }
        }

        private void LoadServiceLocations()
        {
            var locations = new DropdownHelper().GetLocations(int.Parse(lblSubscriberId.Text));
            ddlServiceLocation.Items.Add(new ListItem("", ""));
            foreach (var location in locations)
            {
                ddlServiceLocation.Items.Add(new ListItem(location.SelectText, location.SelectValue.ToString()));
            }
        }

        private void LoadCurrencies()
        {
            var defaultCurrency = "";
            var currencies = new DropdownHelper().GetCurrencies(defaultCurrency);
            ddlCurrency.Items.Add(new ListItem("", ""));
            foreach (var currency in currencies)
            {
                ddlCurrency.Items.Add(new ListItem(currency.SelectText, currency.SelectValue));
            }

            // set logged in user default currency
            var user = LoginUser.GetLoggedInUser();
            if (user != null && !string.IsNullOrEmpty(user.User.CurrencyCode))
            {
                ddlCurrency.SelectedValue = user.User.CurrencyCode;
            }
            else
            {
                ddlCurrency.SelectedValue = "USD";
            }
        }

    }
}
