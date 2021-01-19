using System;
using Helpers;
using Helpers.Sync;
using Models;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using ClosedXML.Excel;
using Crm6.App_Code;
using Microsoft.WindowsAzure.Storage;

namespace API
{
    public class UserController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<SelectList> GetUsersForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetUsers(subscriberId);
        }


        [AcceptVerbs("POST")]
        public List<User> GetUsers([FromBody]UserFilter filters)
        {
            var filterdUsers = new Users().GetUsers(filters);
            return filterdUsers;
        }


        [AcceptVerbs("POST")]
        public UserSaveResponse SaveUser([FromBody]UserSaveRequest request)
        {
            return new Users().SaveUser(request);
        }


        // user save profile
        [AcceptVerbs("POST")]
        public int SaveProfile([FromBody]UserSaveRequest request)
        {
            return new Users().SaveProfile(request);
        }


        [AcceptVerbs("GET")]
        public UserModel GetUser([FromUri]int userId, int subscriberId)
        {
            return new Users().GetUser(userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteUser([FromUri]int userId, int loggedInUserId, int subscriberId)
        {
            return new Users().DeleteUser(userId, loggedInUserId, subscriberId);
        }
        
        [AcceptVerbs("GET")]
        public string GetUserProfilePic([FromUri]int userId, int subscriberId=0, string type=null)
        {
            return new Users().GetUserProfilePicUrl(userId, subscriberId, type);
        }
        
        [AcceptVerbs("POST")]
        public string UpdatePassword([FromBody]PasswordChangeRequest request)
        {
            return new Users().UpdatePassword(request.UserId, request.SubscriberId, request.OldPassword, request.NewPassword);
        }

        [AcceptVerbs("GET")]
        public List<Crm6.App_Code.Login.GlobalUser> GetGlobaUsers([FromUri]int subscriberId)
        {
            return new Users().GetGlobaUsers(subscriberId);
        }



        [AcceptVerbs("GET")]
        public bool UpdateGlobalUsers()
        {
            return new Users().UpdateGlobalUsers();
        }


        [AcceptVerbs("POST")]
        public bool ActivateSync([FromBody] SyncUser syncUser)
        {
            return new Users().SaveSyncSettings(syncUser).Result;
        }


        [AcceptVerbs("GET")]
        public bool DisableSync([FromUri] int userId, int subscriberId)
        {
            return new Users().DisableSync(userId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public bool ReassignUser([FromBody] ReassignUserRequest request)
        {
            return new Users().ReassignUser(request);
        }


        [AcceptVerbs("GET")]
        public bool IsUserFoundWithSameEmail([FromUri] string emailAddress, int subscriberId)
        {
            return new Users().IsUserFoundWithSameEmail(emailAddress, subscriberId);
        }


        [AcceptVerbs("POST")]
        public IHttpActionResult ExportToExcel([FromBody]UserFilter filters)
        {
            var usermgr = new Users();
            var subscriberMgr = new Subscribers();
            var filteredUsers = usermgr.GetUsers(filters);
            var subscriber = subscriberMgr.GetSubscriber(filters.SubscriberId);

            var xlBook = new ClosedXML.Excel.XLWorkbook();
            var ws = xlBook.AddWorksheet($"User Export {System.DateTime.UtcNow:yyyy-MM-dd}");

            ws.Cell("A4").Value = "";
            ws.Cell("B4").Value = "User";
            ws.Cell("C4").Value = "Email";
            ws.Cell("D4").Value = "Location";
            ws.Cell("E4").Value = "Country";
            ws.Cell("F4").Value = "Job Title";
            ws.Cell("G4").Value = "Last Login";
            ws.Cell("H4").Value = "Created Date";

            var headerRange = ws.Range("A4:H4");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
            headerRange.Style.Font.Bold = true;
            //headerRange.Style.Font.FontColor = XLColor.DarkBlue;
            headerRange.Style.Fill.BackgroundColor = XLColor.Gray;


            var infoDisplay = $"as of {DateTime.Today:DD-MMM-YY}";
            ws.Cell("H2").Value = infoDisplay;
            ws.Cell("H2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("H2").Style.Font.Bold = true;

            var loginDateRange = ws.Range($"G4:G{filteredUsers.Count + 10}");
            loginDateRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            loginDateRange.Style.NumberFormat.Format = "dd/MMM/yyyy";

            var createdDateRange = ws.Range($"H4:H{filteredUsers.Count + 10}");
            createdDateRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            createdDateRange.Style.NumberFormat.Format = "dd/MMM/yyyy";

            var dt = filteredUsers.Select((i, counter) => new
            {
                counter = counter + 1,
                i.FullName,
                i.EmailAddress,
                i.LocationName,
                i.CountryName,
                i.Title,
                i.LastLoginDate,
                i.CreatedDate
            }).AsEnumerable();

            ws.Cell("A5").InsertData(dt);
            ws.Column(1).AdjustToContents();
            ws.Column(2).AdjustToContents();
            ws.Column(3).AdjustToContents();
            ws.Column(4).AdjustToContents();
            ws.Column(5).AdjustToContents();
            ws.Column(6).AdjustToContents();


            // set the info cell, now the widths should be taken care of... 
            ws.Cell("A2").Style.Font.FontSize = ws.Cell("B4").Style.Font.FontSize + 2;
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Value = $"{filteredUsers.Count} {subscriber.CompanyName} Active Users";


            // try to auto width the columns

            var st = new MemoryStream();
            xlBook.SaveAs(st);

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

            var link = new Crm6.App_Code.Helpers.BlobStorageHelper().GetBlob(containerReference, fileName);

            return Ok(link);
        }
    }

}
