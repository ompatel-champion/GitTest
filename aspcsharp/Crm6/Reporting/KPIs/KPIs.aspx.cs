using Crm6.App_Code;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Crm6.Reporting.Kpis
{
    public partial class Kpis : BasePage
    {
        private readonly DropdownHelper _dropDownHelper = new DropdownHelper(); 
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set Defaults - get from the session
            var currentUser = LoginUser.GetLoggedInUser(); 
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString(); 
            lblSubscriberId.Text = currentUser.User.SubscriberId.ToString();

            // Set Jquery DatePicker Date Format
            if (!string.IsNullOrEmpty(currentUser.User.DateFormatReports))
            {
                lblDateFormat.Text = currentUser.User.DateFormatReports;
            }
            // if not manager, hide all active button
            var managerUserRoles = new List<string> {
                    "CRM Admin",
                    "Station Manager",
                    "Location Manager",
                    "District Manager",
                    "Country Manager",
                    "Country Admin",
                    "Region Manager"
                };

            var isManager = false;
            var userRoles = currentUser.User.UserRoles.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var us in managerUserRoles)
            {
                var found = userRoles.FirstOrDefault(t => t.Equals(us));
                isManager = (found != null);
                if (isManager)
                {
                    break;
                }
            }
            if (!isManager)
            {
                locationContainer.Attributes["class"] = locationContainer.Attributes["class"] + " hide";
                countryContainer.Attributes["class"] = countryContainer.Attributes["class"] + " hide";
            }
            if (!Page.IsPostBack)
            {
                LoadCountries();
                LoadUsers(); 
            }
        }

        
        private void LoadCountries()
        {
            var susbcriberId = int.Parse(lblSubscriberId.Text);
            var countries = _dropDownHelper.GetCountriesForSubscriberUsers(susbcriberId);
            ddlCountry.Items.Add(new ListItem("", ""));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue));
            }
        }



        /// <summary>
        /// load users dropdown
        /// </summary>
        private void LoadUsers()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var userId = int.Parse(lblUserId.Text); 
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            var finalUserList = new List<User>();

            App_Code.User user = context.Users.FirstOrDefault(t => t.UserId == userId);

            // users
            var users = context.Users.Where(t => t.SubscriberId == subscriberId && !t.Deleted);

            if (user != null)
            {

                if (!string.IsNullOrEmpty(user.UserRoles))
                {
                    var userIds = new List<int>();
                    // get sales manager user's location codes
                    if (user.UserRoles.Contains("Sales Manager"))
                    {
                        userIds = (from t in context.LinkUserToManagers
                                   where t.ManagerUserId == user.UserId && !t.Deleted
                                   select t.UserId).Distinct().ToList();
                    }

                    if (user.UserRoles.Contains("CRM Admin"))
                    {
                        finalUserList = users.ToList();
                    }
                    else if (user.UserRoles.Contains("Region Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.RegionName))
                        {
                            // this user is a region manager, get all the companies this user and his district users linked to 
                            userIds.AddRange(context.Users.Where(u => u.RegionName.Equals(user.RegionName))
                                .Select(u => u.UserId).ToList());
                            finalUserList = users.Where(t => userIds.Contains(t.UserId)).ToList();
                        }
                    }
                    else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                    {
                        if (!string.IsNullOrEmpty(user.CountryCode))
                        {
                            // this user is a country manager, get all the companies this user and his district users linked to 
                            userIds.AddRange(context.Users.Where(c => c.CountryCode.Equals(user.CountryCode))
                                .Select(c => c.UserId).ToList());
                            finalUserList = users.Where(t => userIds.Contains(t.UserId)).ToList();
                        }
                    }
                    else if (user.UserRoles.Contains("District Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.DistrictCode))
                        {
                            var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                            if (district != null)
                            {
                                // this user is a district manager, get all the companies this user and his district users linked to 
                                userIds.AddRange(context.Users.Where(u => u.DistrictCode.Equals(district.DistrictCode))
                                        .Select(u => u.UserId).ToList());
                                finalUserList = users.Where(t => userIds.Contains(t.UserId)).ToList();
                            }
                        }
                    }
                    else if (user.UserRoles.Contains("Location Manager"))
                    {
                        if (user.LocationId > 0)
                        {
                            var location = new Helpers.Locations().GetLocation(user.LocationId, int.Parse(lblSubscriberId.Text));
                            if (location != null)
                            {
                                // this user is a location manager, get all the companies this user and his location users linked to 
                                userIds.AddRange(context.Users.Where(u => u.LocationCode.Equals(location.LocationCode)).Select(u => u.UserId)
                                    .ToList());
                                finalUserList = users.Where(t => userIds.Contains(t.UserId)).ToList();
                            }
                        }
                    }
                    else
                    {
                        userIds.Add(user.UserId);
                        finalUserList = users.Where(t => userIds.Contains(t.UserId)).ToList();
                    }
                }
            }


            // current user
            finalUserList = finalUserList.OrderBy(t => t.FullName).ToList();


            ddlSalesReps.Items.Add(new ListItem("", ""));
            ddlSalesReps.Items.Add(new ListItem(user.FullName, user.UserId.ToString()));
            foreach (var u in finalUserList)
            {
                if (ddlSalesReps.Items.FindByValue(u.UserId.ToString()) == null)
                    ddlSalesReps.Items.Add(new ListItem(u.FullName, u.UserId.ToString()));
            }
        }

    }
}
