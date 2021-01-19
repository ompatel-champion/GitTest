using Crm6.App_Code;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Crm6.Calendar
{
    public partial class Calendar : BasePage
    {
        private int _subscriberId;

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            _subscriberId = currentUser.User.SubscriberId;
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            lblUsername.Text = currentUser.User.FirstName + " " + currentUser.User.LastName;
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString();
            if (!Page.IsPostBack)
            {
                LoadUsers();
            }
        }


        private void LoadUsers()
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var susbcriberId = int.Parse(lblSubscriberId.Text);

            var finalUserList = new List<User>();

            // current user
            var user = context.Users.FirstOrDefault(t => t.UserId == int.Parse(lblUserId.Text));
            // users
            var users = context.Users.Where(t => t.SubscriberId == susbcriberId && !t.Deleted && t.LoginEnabled);

            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.UserRoles))
                {
                    // get sales manager user's location codes
                    if (user.UserRoles.Contains("Sales Manager"))
                    {
                        var userIds = (from t in context.LinkUserToManagers
                                       where !t.Deleted && t.ManagerUserId == user.UserId
                                       select t.UserId).ToList();

                        // get users
                        finalUserList = users.Where(t => userIds.Contains(t.UserId)).ToList();
                    }

                    if (user.UserRoles.Contains("CRM Admin"))
                    {
                        finalUserList = users.ToList();
                    }
                    else if (user.UserRoles.Contains("Region Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.RegionName))
                        {
                            // this user is a region manager, get all the deals for the region
                            var locationCodes = context.Locations
                                                       .Where(t => t.RegionName == user.RegionName && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();
                            finalUserList.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                                                           t.RegionName != null && t.RegionName != "" &&
                                                           t.RegionName.Equals(user.RegionName)).ToList());
                        }
                    }
                    else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                    {
                        if (!string.IsNullOrEmpty(user.CountryCode))
                        {
                            // this user is a country manager, get all the deals for the country
                            var locationCodes = context.Locations
                                                       .Where(t => t.CountryCode == user.CountryCode && t.LocationCode != "")
                                                       .Select(t => t.LocationCode).ToList();
                            finalUserList.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                              t.CountryCode != null && t.CountryCode != "" && t.CountryCode.Equals(user.CountryCode)).ToList());
                        }
                    }
                    else if (user.UserRoles.Contains("District Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.DistrictCode))
                        {
                            var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                            if (district != null)
                            {
                                // this user is a district manager, get all the deals for the district
                                var locationCodes = context.Locations
                                                           .Where(t => t.DistrictCode == district.DistrictCode && t.LocationCode != "")
                                                           .Select(t => t.LocationCode).ToList();
                                finalUserList.AddRange(users.Where(t => locationCodes.Contains(t.LocationCode) &&
                                                  t.DistrictCode != null && t.DistrictCode != ""
                                                  && t.DistrictCode.Equals(user.DistrictCode)).ToList());
                            }
                        }
                    }
                    else if (user.UserRoles.Contains("Location Manager"))
                    {
                        if (user.LocationId > 0)
                        {
                            var location = new Helpers.Locations().GetLocation(user.LocationId, user.SubscriberId);
                            if (location != null)
                            {
                                // this user is a location manager, get all the deals for the location 
                                finalUserList.AddRange(users.Where(t => t.LocationCode == location.LocationCode
                                                        && t.CountryCode.Equals(user.CountryCode)).ToList());
                            }
                        }
                    }
                    else
                    {
                        // get deals for liked users
                        finalUserList.Add(user);
                    }
                }
            }

            // current user
            ddlUsers.Items.Add(new ListItem(user.FullName, user.UserIdGlobal.ToString()));
            finalUserList = finalUserList.OrderBy(t => t.FullName).Distinct().ToList();
            foreach (var u in finalUserList)
            {
                if (ddlUsers.Items.FindByValue(u.UserIdGlobal.ToString()) == null)
                    ddlUsers.Items.Add(new ListItem(u.FullName, u.UserIdGlobal.ToString()));
            }
        }

    }
}