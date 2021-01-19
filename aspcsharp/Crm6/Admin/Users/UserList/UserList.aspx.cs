using ClosedXML.Excel;
using Crm6.App_Code.Helpers;
using Helpers;
using Microsoft.WindowsAzure.Storage;
using Models;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Web.UI.WebControls;

namespace Crm6.Users
{
    public partial class UserList : BasePage
    {
        private static readonly Func<string, bool> IsCountryAdmin = userRole =>
            !userRole.Contains("CRM Admin") && userRole.Contains("Country Admin");

        protected void Page_Load(object sender, EventArgs e)
        {
            var currentUser = LoginUser.GetLoggedInUser();
            lblUserId.Text = currentUser.User.UserId.ToString();
            lblSubscriberId.Text = currentUser.Subscriber.SubscriberId.ToString();
            LoadCountries();
        }

        private void LoadCountries()
        {
            var countries = new DropdownHelper().GetCountriesForSubscriberUsers(int.Parse(lblSubscriberId.Text));
            ddlCountry.Items.Add(new ListItem("Country", "Country"));
            foreach (var country in countries)
            {
                ddlCountry.Items.Add(new ListItem(country.SelectText, country.SelectValue));
            }
        }

        protected void btnUsersExcel_Click(object sender, EventArgs e)
        {
            // todo: keyword filter / login enabled
            UserFilter filter = new UserFilter()
            {
                //keyword = 
                //LoginEnabled = 
            };

            var uri = CreateUserListExcel(filter);

            using (var client = new WebClient())
            {
                System.Diagnostics.Process.Start(uri);
            }
        }

        private string CreateUserListExcel(UserFilter filter)
        {
            try
            {
                var users = new Helpers.Users().GetUsers(filter);

                if (users != null)
                {
                    var dt = new DataTable("User List");
                    dt.Clear();
                    dt.Columns.Add("Full Name");
                    dt.Columns.Add("Email");
                    dt.Columns.Add("Location");
                    dt.Columns.Add("Country");
                    dt.Columns.Add("Title");
                    dt.Columns.Add("Last Login");

                    foreach (var user in users)
                    {
                        DataRow dr = dt.NewRow();
                        dr["Full Name"] = user.FullName;
                        dr["Email"] = user.EmailAddress;
                        dr["Location"] = user.LocationName;
                        dr["Country"] = user.CountryName;
                        dr["Title"] = user.Title;
                        dr["Last Login"] = user.LastLoginDate;

                        dt.Rows.Add(dr);
                    }

                    var wb = new XLWorkbook();
                    var dataTable = dt;

                    // Add a DataTable as a worksheet
                    wb.Worksheets.Add(dataTable);
                    var st = new MemoryStream();
                    wb.SaveAs(st);

                    var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

                    // Create the blob client
                    var blobClient = storageAccount.CreateCloudBlobClient();
                    // Retrieve a reference to a container
                    var containerReference = "temp";
                    var container = blobClient.GetContainerReference(containerReference);

                    var companyName = LoginUser.GetLoggedInUser()?.Subscriber?.CompanyName ?? "";

                    var fileName = $"{companyName}_CRM_UserList_{DateTime.Now.ToString("dd-MMM-yy")}_{Guid.NewGuid()}.xlsx";

                    var blockBlob = container.GetBlockBlobReference(fileName);
                    blockBlob.Properties.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                    using (st)
                    {
                        long streamlen = st.Length;
                        st.Position = 0;
                        blockBlob.UploadFromStream(st);
                    }

                    return new BlobStorageHelper().GetBlob(containerReference, fileName);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "CreateExcel",
                    PageCalledFrom = "UserList",
                    SubscriberId = filter.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = filter.UserId
                };

                return null;
            }
        }

    }
}
