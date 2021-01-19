using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Helpers;
using System; 
using System.Globalization;
using System.Linq; 
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Crm6.Reporting.WeeklyActivity
{
    public partial class WeeklyActivity : BasePage
    { 
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set Defaults - get from the session
            var currentUser = LoginUser.GetLoggedInUser();  
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblUserIdGlobal.Text = currentUser.User.UserIdGlobal.ToString(); 
            lblSubscriberId.Text = currentUser.User.SubscriberId.ToString();
            lblUsername.Text = currentUser.User.FirstName + " " + currentUser.User.LastName; 
            // Set Jquery DatePicker Date Format
            if (!string.IsNullOrEmpty(currentUser.User.DateFormatReports))
            {
                lblDateFormat.Text = currentUser.User.DateFormatReports;
            } 
            if (!Page.IsPostBack)
            {
                LoadAccessibleGlobalUsers();
                LoadYears();
                LoadCategories(); 
                LoadCountries();
                LoadCampaigns();
            }
        }

        /// <summary>
        /// load users dropdown
        /// </summary>
        private void LoadAccessibleGlobalUsers()
        {
            var susbcriberId = int.Parse(lblSubscriberId.Text);
            var globalUserId = int.Parse(lblUserIdGlobal.Text);
            // get users
            var users = new ActitivtyByDateRangeReport().GetAccessibleGlobalUserIdsForUser(globalUserId, susbcriberId);
            // bind to the user list
            foreach (var u in users)
            {
                ddlUsers.Items.Add(new ListItem(u.FullName, u.UserIdGlobal.ToString()));
            }
        }


        private void LoadCampaigns()
        {
            var subscriberId = int.Parse(lblSubscriberId.Text);
            var campaigns = new DropdownHelper().GetCampaigns(subscriberId);
            ddlCampaigns.Items.Add("");
            foreach (var campaign in campaigns)
            {
                ddlCampaigns.Items.Add(new ListItem(campaign.SelectText, campaign.SelectValue));
            }
        }


        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }


        /// <summary>
        /// load years
        /// </summary>
        private void LoadYears()
        {
            var startYear = 2015;
            for (int i = 0; i < 30; i++)
            {
                ddlYear.Items.Add(new ListItem(startYear.ToString(), startYear.ToString()));
                startYear += 1;
            }
        }


        private void LoadCountries()
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            ddlCountry.Items.Clear();
            ddlCountry.Items.Add("");
            var currentUser = LoginUser.GetLoggedInUser();

            if (currentUser.User.UserRoles.Contains("CRM Admin"))
            {
                var countries = context.Users.Where(x => x.SubscriberId == int.Parse(lblSubscriberId.Text)).Select(t => new { t.CountryCode, t.CountryName }).Distinct().ToList();
                foreach (var item in countries)
                {
                    ddlCountry.Items.Add(new ListItem(item.CountryName, item.CountryCode));
                }
            }
            else if (currentUser.User.UserRoles.Contains("District Manager"))
            {
                var countries = context.Users.Where(x => x.SubscriberId == int.Parse(lblSubscriberId.Text)
                                    && x.DistrictCode == currentUser.User.DistrictCode).Select(t => new { t.CountryCode, t.CountryName }).Distinct().ToList();
                foreach (var item in countries)
                {
                    ddlCountry.Items.Add(new ListItem(item.CountryName, item.CountryCode));
                }
            }
            else if (currentUser.User.UserRoles.Contains("Region Manager"))
            {
                var countries = context.Users.Where(x => x.SubscriberId == int.Parse(lblSubscriberId.Text)
                                  && x.RegionName == currentUser.User.RegionName).Select(t => new { t.CountryCode, t.CountryName }).Distinct().ToList();
                foreach (var item in countries)
                {
                    ddlCountry.Items.Add(new ListItem(item.CountryName, item.CountryCode));
                }
            }

        }

        private void LoadCategories()
        {
            var connection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(connection);
            var categories = context.Activities.Where(x => x.SubscriberId == int.Parse(lblSubscriberId.Text) && (x.CalendarEventId > 0 || x.ActivityType == "EVENT")).Select(x => x.CategoryName).Distinct()?.ToList();

            foreach (var item in categories)
            {
                ddlCategories.Items.Add(item);
            }
        }


    }
}