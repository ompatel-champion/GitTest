using System;
using System.Web.UI.WebControls;

namespace Crm6.Locations
{
    public partial class LocationAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                // load dropdowns
                LoadCountries();
                LoadDistricts(currentUser.Subscriber.SubscriberId);
                LoadRegions(currentUser.Subscriber.SubscriberId);
                LoadLocationTypes(); 

                if (Request.QueryString["locationId"] != null && int.Parse(Request.QueryString["locationId"]) > 0)
                {
                    lblLocationId.Text = Request.QueryString["locationId"];
                    LoadLocation(currentUser.Subscriber.SubscriberId);
                }
            }
        }


        private void LoadRegions(int subscriberId)
        {
            var regions = new Helpers.DropdownHelper().GetRegions(subscriberId);
            ddlRegion.Items.Add(new ListItem("Region...", "0"));
            foreach (var region in regions)
            {
                ddlRegion.Items.Add(new ListItem(region.SelectText, region.SelectValue.ToString()));
            }
        }


        private void LoadCountries()
        {
            var countries = new Helpers.DropdownHelper().GetCountries();
            ddlCountry.Items.Add(new ListItem("Country...", "0"));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue.ToString()));
            }
        }


        private void LoadDistricts(int subscriberId)
        {
            var districts = new Helpers.DropdownHelper().GetDistricts(subscriberId);
            ddlDistrict.Items.Add(new ListItem("District...", "0"));
            foreach (var district in districts)
            {
                ddlDistrict.Items.Add(new ListItem(district.SelectText, district.SelectValue.ToString()));
            }
        }


        private void LoadLocationTypes()
        {
            var locationTypes = new Helpers.DropdownHelper().GetLocationTypes();
            ddlLocationType.Items.Add(new ListItem("Location Type...", ""));
            foreach (var locationType in locationTypes)
            {
                ddlLocationType.Items.Add(new ListItem(locationType.SelectText, locationType.SelectValue.ToString()));
            }
        }


        private void LoadLocation(int subscriberId)
        {
            int locationId = int.Parse(lblLocationId.Text);
            var location = new Helpers.Locations().GetLocation(locationId, subscriberId);
            if (location != null)
            {
                lblBreadcrumbHeader.Text = "Edit Location";
                txtLocationName.Text = location.LocationName;
                txtLocationCode.Text = location.LocationCode;
                if (ddlLocationType.Items.FindByValue(location.LocationType) != null)
                    ddlLocationType.SelectedValue = location.LocationType;
                txtAddress.Text = location.Address;
                txtCity.Text = location.City;
                txtStateProvince.Text = location.StateProvince;
                txtPostalCode.Text = location.PostalCode;
                ddlCountry.SelectedValue = location.CountryName;
                if (ddlDistrict.Items.FindByValue(location.DistrictCode) != null)
                    ddlDistrict.SelectedValue = location.DistrictCode;
                if (ddlRegion.Items.FindByValue(location.RegionName) != null)
                    ddlRegion.SelectedValue = location.RegionName;
                txtPhone.Text = location.Phone;
                txtFax.Text = location.Fax; 
                txtComments.Text = location.Comments; 
            }
        }
    }

}
