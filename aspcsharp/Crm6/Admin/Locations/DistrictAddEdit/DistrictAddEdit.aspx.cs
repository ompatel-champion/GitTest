using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Admin.Districts.DistrictAddEdit
{
    public partial class DistrictAddEdit : BasePage
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

                if (Request.QueryString["districtid"] != null && int.Parse(Request.QueryString["districtid"]) > 0)
                {
                    lblDistrictId.Text = Request.QueryString["districtid"];
                    LoadDistrict();
                }
            }
        }

        private void LoadCountries()
        {
            var countries = new Helpers.DropdownHelper().GetCountries();
            ddlCountry.Items.Add(new ListItem("Select Country...", "0"));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue.ToString()));
            }
        }

        private void LoadDistrict()
        {
            int districtId = int.Parse(lblDistrictId.Text);
            int subscriberId = int.Parse(lblSubscriberId.Text);
            var district = new Helpers.Districts().GetDistrict(districtId, subscriberId);
            if (district != null)
            {
                lblBreadcrumbHeader.Text = "Edit District";
                txtDistrictName.Text = district.DistrictName;
                txtDistrictCode.Text = district.DistrictCode;
                ddlCountry.SelectedValue = district.CountryName;
            }
        }
    }
}
