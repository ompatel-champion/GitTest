using System;
using System.Linq;

namespace Crm6.Admin
{
    public partial class Locations : BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();

            if (!Page.IsPostBack)
            {
                LoadLocations();
                LoadDistricts(); 
                LoadRegions();
            }
        }


        private void LoadLocations(string keyword = "")
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var response = new Helpers.Locations().GetLocations(new Models.LocationFilter
            {
                CurrentPage = 1,
                RecordsPerPage = 30,
                SubscriberId = subscriberId,
                SortBy = "locationname asc"
            });

            if (response.Locations != null && response.Locations.Count > 0 && string.IsNullOrWhiteSpace(keyword) == false)
            {
                response.Locations = response.Locations.Where(x => x.LocationName.ToLower().Contains(keyword.ToLower())
                                                                || x.LocationCode.ToLower().Contains(keyword.ToLower())
                                                                || x.LocationType.ToLower().Contains(keyword.ToLower())
                                                                || x.Address.ToLower().Contains(keyword.ToLower())
                                                                || x.CountryName.ToLower().Contains(keyword.ToLower()))?.ToList();
            }

            rptLocations.DataSource = response.Locations;
            rptLocations.DataBind();

        }

        private void LoadDistricts()
        {
            var districts = new Helpers.Districts().GetDistricts(int.Parse(lblSubscriberId.Text));
            rptDistricts.DataSource = districts;
            rptDistricts.DataBind();
        } 

        void LoadRegions()
        {
            var regions = new Helpers.Regions().GetRegions(int.Parse(lblSubscriberId.Text));
            rptRegions.DataSource = regions;
            rptRegions.DataBind(); ;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadLocations(txtKeyword.Text);
        }
         

    }
}
