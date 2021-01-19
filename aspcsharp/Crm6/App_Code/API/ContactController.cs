using ClosedXML.Excel;
using Crm6.App_Code;
using Helpers;
using Microsoft.WindowsAzure.Storage;
using Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web.Http;

namespace API
{
    public class ContactController : ApiController
    {

        [AcceptVerbs("POST")]
        public ContactListResponse GetContacts([FromBody]ContactFilter filters)
        {
            return new Contacts().GetContacts(filters);
        }


        // contact save
        [AcceptVerbs("POST")]
        public int SaveContact([FromBody]ContactSaveRequest request)
        {
            return new Contacts().SaveContact(request);
        }


        [AcceptVerbs("GET")]
        public ContactModel GetContact([FromUri]int contactId, int subscriberId)
        {
            return new Contacts().GetContact(contactId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteContact([FromUri]int contactId, int userId, int subscriberId)
        {
            return new Contacts().DeleteContact(contactId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public int GetCompanyContactCount([FromUri]int companyId, int subscriberId)
        {
            return new Contacts().GetCompanyContactCount(companyId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public string GetContactProfilePic([FromUri]int contactId, int subscriberId)
        {
            return new Contacts().GetContactProfilePicUrl(contactId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<ContactSalesTeamMember> GetContactUsers([FromUri]int contactId, int subscriberId)
        {
            return new Contacts().GetContactUsers(contactId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public bool AddContactUser([FromBody]AddContactUserRequest contactUser)
        {
            return new Contacts().AddContactUser(contactUser);
        }


        [AcceptVerbs("GET")]
        public bool DeleteContactUser([FromUri]int contactId, int userId, int deleteUserId, int subscriberId)
        {
            return new Contacts().DeleteContactUser(contactId, userId, deleteUserId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public IHttpActionResult ExportToExcel([FromBody]ContactFilter filters)
        {
            var contactmgr = new Contacts();
            var subscriberMgr = new Subscribers();
            var filteredContacts = contactmgr.GetContactLists(filters);
            var subscriber = subscriberMgr.GetSubscriber(filters.SubscriberId);

            var xlBook = new ClosedXML.Excel.XLWorkbook();
            var ws = xlBook.AddWorksheet($"Contact Export {System.DateTime.UtcNow:yyyy-MMM-dd}");


            // set the info cell, now the widths should be taken care of... 
            ws.Cell("A2").Style.Font.FontSize = ws.Cell("B4").Style.Font.FontSize + 2;
            ws.Cell("A2").Style.Font.Bold = true;
            ws.Cell("A2").Value = $"{filteredContacts.Count} {subscriber.CompanyName} Companies";

            var infoDisplay = $"as of {DateTime.Today:dd-MMM-yyyy}";
            ws.Cell("H2").Value = infoDisplay;
            ws.Cell("H2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell("H2").Style.Font.Bold = true;

            // TODO: modify for specified columns - make all columns proper width
            ws.Cell("A4").Value = "";
            ws.Cell("B4").Value = "Name";
            ws.Cell("C4").Value = "Phone";
            ws.Cell("D4").Value = "Email";
            ws.Cell("E4").Value = "Company";
            ws.Cell("F4").Value = "Location";
            ws.Cell("G4").Value = "Sales Team";
            ws.Cell("H4").Value = "Last Activity";

            var headerRange = ws.Range("A4:R4");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
            headerRange.Style.Font.Bold = true;
            //headerRange.Style.Font.FontColor = XLColor.DarkBlue;
            headerRange.Style.Fill.BackgroundColor = XLColor.Gray;

            int counter = 5;
            foreach (var item in filteredContacts)
            {
                ws.Cell(counter, 1).Value = counter - 4;
                ws.Cell(counter, 2).Value = item.ContactName;
                ws.Cell(counter, 3).Value = item.BusinessPhone;
                ws.Cell(counter, 4).Value = item.Email;
                ws.Cell(counter, 5).Value = item.CompanyName;
                ws.Cell(counter, 6).Value = item.BusinessCity + ", " + item.BusinessCountry;
                ws.Cell(counter, 7).Value = item.SalesTeam;
                ws.Cell(counter, 8).Value = item.LastActivityDate;
                counter++;
            }
          
            ws.Columns(1, 8).AdjustToContents();
            var st = new MemoryStream();
            xlBook.SaveAs(st);

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container
            var containerReference = "temp";
            var container = blobClient.GetContainerReference(containerReference);

            var companyName = LoginUser.GetLoggedInUser()?.Subscriber?.CompanyName ?? "";

            var fileName = $"{companyName}_CRM_ContactList_{DateTime.Now.ToString("dd-MMM-yy")}_{Guid.NewGuid()}.xlsx";

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
