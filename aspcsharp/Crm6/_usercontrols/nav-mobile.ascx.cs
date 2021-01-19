using System;

namespace Crm6._usercontrols
{
    public partial class navmobile : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            liAdmin.Visible = false;
            liLocations.Visible = false;
            liLanguages.Visible = false;
            liSettings.Visible = false;
            liUsers.Visible = false;
            liCampaigns.Visible = false;
            SetActiveNavItem();
            ManageNavItemsForUser();
        }

        public void SetActiveNavItem()
        {
            var dir = (System.IO.Path.GetDirectoryName(Request.RawUrl) + "").ToLower().Replace("\\", "");
            var file = System.IO.Path.GetFileNameWithoutExtension(Request.RawUrl);

            liActivities.Attributes.Remove("class");
            liDashboard.Attributes.Remove("class");
            liQuotes.Attributes.Remove("class");
            liReports.Attributes.Remove("class");
            liCompanies.Attributes.Remove("class");
            liContacts.Attributes.Remove("class");
            liDeals.Attributes.Remove("class");
            liCalendar.Attributes.Remove("class");
            liUsers.Attributes.Remove("class");
            liSettings.Attributes.Remove("class");
            liLocations.Attributes.Remove("class");
            liCampaigns.Attributes.Remove("class");
            liLanguages.Attributes.Remove("class");

            if (dir.Contains("activities"))
            {
                liActivities.Attributes.Add("class", "active");
            }
            else if (dir.Contains("dashboard"))
            {
                liDashboard.Attributes.Add("class", "active");
            }
            else if (dir.Contains("reporting"))
            {
                liReports.Attributes.Add("class", "active");
            }
            else if (dir.Contains("companies"))
            {
                liCompanies.Attributes.Add("class", "active");
            }
            else if (dir.Contains("contacts"))
            {
                liContacts.Attributes.Add("class", "active");
            }
            else if (dir.Contains("deals"))
            {
                liDeals.Attributes.Add("class", "active");
            }
            else if (dir.Contains("calendar"))
            {
                liCalendar.Attributes.Add("class", "active");
            }
            else if (dir.Contains("users"))
            {
                liUsers.Attributes.Add("class", "active");
            }
            else if (dir.Contains("settings"))
            {
                liSettings.Attributes.Add("class", "active");
            }
            else if (dir.Contains("locations"))
            {
                liLocations.Attributes.Add("class", "active");
            }
            else if (dir.Contains("quotes"))
            {
                liQuotes.Attributes.Add("class", "active");
            }
            else if (dir.Contains("campaigns"))
            {
                liCampaigns.Attributes.Add("class", "active");
            }
            else if (dir.Contains("languages"))
            {
                liLanguages.Attributes.Add("class", "active");
            }
            else
            {
                liDashboard.Attributes.Add("class", "active");
            }
        }

        public void ManageNavItemsForUser()
        {
            var currentUser = LoginUser.GetLoggedInUser();
            var userRoles = currentUser.User.UserRoles.ToLower();

            // Subscriber CRM Admin
            if (userRoles.ToLower().Contains("crm admin"))
            {
                liAdmin.Visible = true;
                liUsers.Visible = true;
                liSettings.Visible = true;
                liLocations.Visible = true;
                liLanguages.Visible = true;
                liCampaigns.Visible = true;
            }

            // Country CRM Admin
            if (userRoles.ToLower().Contains("country admin"))
            {
                liAdmin.Visible = true;
                liUsers.Visible = true;
                liLocations.Visible = true;
                liLanguages.Visible = true;
                liCampaigns.Visible = true;
            }
        }

    }
}