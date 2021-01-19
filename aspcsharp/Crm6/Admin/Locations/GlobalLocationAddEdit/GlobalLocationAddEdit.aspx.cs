using System;
using System.Web.UI.WebControls;

namespace Crm6.Locations
{
    public partial class GlobalLocationAddEdit : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();

            if (!Page.IsPostBack)
            {
                // load country dropdown
                LoadCountries();

                if (Request.QueryString["globalLocationId"] != null && int.Parse(Request.QueryString["globalLocationId"]) > 0)
                {
                    lblGlobalLocationId.Text = Request.QueryString["globalLocationId"];
                    LoadGlobalLocation(currentUser.Subscriber.SubscriberId);
                }
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


        private void LoadGlobalLocation(int subscriberId)
        {
            int globalLocationId = int.Parse(lblGlobalLocationId.Text);
            var globalLocation = new Helpers.GlobalLocations().GetGlobalLocation(globalLocationId);
            if (globalLocation != null)
            {
                lblBreadcrumbHeader.Text = "Edit Global Location";
                chkAirport.Checked = globalLocation.Airport;
                chkInlandPort.Checked = globalLocation.InlandPort;
                chkMultiModal.Checked = globalLocation.MultiModal;
                chkRailTerminal.Checked = globalLocation.RailTerminal;
                chkRoadTerminal.Checked = globalLocation.RoadTerminal;
                chkSeaport.Checked = globalLocation.SeaPort;
                ddlCountry.SelectedValue = globalLocation.CountryName;
                txtLocationName.Text = globalLocation.LocationName;
                txtLocationCode.Text = globalLocation.LocationCode;
            }
        }
    }

}
