using System;
using System.Web.Http;
using Helpers;
using Models;
using Crm6.App_Code;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Microsoft.WindowsAzure.Storage;

namespace API
{
    public class CompanyController : ApiController
    {

        [AcceptVerbs("POST")]
        public GlobalCompanyListResponse GetCompaniesGlobal([FromBody]CompanyFilters filters)
        {
            return new Companies().GetCompaniesGlobal(filters);
        }


        [AcceptVerbs("POST")]
        public GlobalCompanyListResponse SearchGlobalCompanies(CompanyFilters filters) 
        {
            return new Companies().SearchGlobalCompanies(filters);
        }


        [AcceptVerbs("POST")]
        public int SaveCompany([FromBody]CompanyModel companyModel)
        {
            return new Companies().SaveCompany(companyModel);
        }


        [AcceptVerbs("POST")]
        public List<CompanyExtended> HasDuplicateCompanies([FromBody]CompanyModel companyModel)
        {
            return new Companies().CheckDuplicateCompany(companyModel);
        }


        [AcceptVerbs("GET")]
        public Company GetCompany([FromUri]int companyId, int subscriberId)
        {
            return new Companies().GetCompany(companyId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteCompany([FromUri]int companyId, int userId, int subscriberId)
        {
            return new Companies().DeleteCompany(companyId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<ContactModel> GetCompanyContacts(int companyId, int subscriberid)
        {
            return new Companies().GetCompanyContacts(companyId, subscriberid);
        }


        [AcceptVerbs("GET")]
        public RevenueResponse GetCompanyDealsRevenue([FromUri] int companyId, int userId, int subscriberId)
        {
            return new Companies().GetCompanyDealsRevenue(companyId, userId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public bool LinkCompany([FromBody] LinkCompanyRequest request)
        {
            return new Companies().LinkCompany(request);
        }


        [AcceptVerbs("GET")]
        public List<LinkCompanyToCompany> GetLinkedCompanies([FromUri] int companyId, int subscriberId)
        {
            return new Companies().GetLinkedCompanies(companyId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteLinkedCompany([FromUri]int linkedCompanyId, int userId, int subscriberId)
        {
            return new Companies().DeleteLinkedCompany(linkedCompanyId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<CompanySalesTeamMember> GetCompanyUsers([FromUri]int companyId, int subscriberId)
        {
            return new Companies().GetCompanyUsers(companyId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public bool AddCompanyUser([FromBody]AddCompanyUserRequest companyUser)
        {
            return new Companies().AddCompanyUser(companyUser);
        }


        [AcceptVerbs("GET")]
        public bool DeleteCompanyUser([FromUri]int companyId, int companySubscriberId, int userId, int deleteUserId, int deleteUserSubscriberId) 
        {
            return new Companies().DeleteCompanyUser(companyId, companySubscriberId, userId, deleteUserId, deleteUserSubscriberId);
        }


        [AcceptVerbs("GET")]
        public bool FixCompanyData()
        {
            return new Companies().FixCompanyData();
        }


        [AcceptVerbs("POST")]
        public GlobalCompanyListResponse GetGlobalCompanies([FromBody]CompanyFilters filters)
        {
            return new Companies().GetGlobalCompanies(filters);
        }


        [AcceptVerbs("GET")]
        public List<User> GetGlobalCompanyUsers([FromUri]int companyId, int subscriberId)
        {
            return new Companies().GetGlobalCompanyUsers(companyId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public string GetCompanyLogo([FromUri]int companyId)
        {
            return new Companies().GetCompanyLogoUrl(companyId);
        }


        [AcceptVerbs("GET")]
        public bool ClaimCompany([FromUri]int companyId, int userId, int subscriberId)
        {
            return new Companies().ClaimCompany(companyId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ReassignCompany([FromUri]int companyId, int userId,int subscriberId, int assignedBy)
        {
            return new Companies().ReassignCompany(companyId, userId, subscriberId, assignedBy);
        }


        [AcceptVerbs("GET")]
        public bool RequestAccess([FromUri]int companyId, int userId, int companySubscriberId, int userSubscriberId)
        {
            return new Companies().RequestAccess(companyId, userId, companySubscriberId, userSubscriberId);
        }

        [AcceptVerbs("GET")]
        public bool RequestAccess([FromUri]int companyId, int userId)
        {
            return new Companies().RequestAccess(companyId, userId);
        }


        [AcceptVerbs("GET")]
        public UserModel GetCompanyOwner([FromUri]int companyId, int subscriberId)
        {
            return new Companies().GetCompanyOwner(companyId, subscriberId);
        }

          [AcceptVerbs("POST")]
        public IHttpActionResult ExportToExcel([FromBody]CompanyFilters filters)
        {
            var filteredCompanies = new Companies().GetCompaniesGlobal(filters);

            var xlBook = new ClosedXML.Excel.XLWorkbook();
            var ws = xlBook.AddWorksheet($"Company Export {System.DateTime.UtcNow:yyyy-MMM-dd}");

            // Columns to Export

            // CompanyName
            // Division
            // Address
            // City
            // StateProvince
            // PostalCode
            // Country
            // Website
            // Phone
            // Fax
            // Industry
            // Source
            // CompanyType
            // Owner
            // SalesTeam
            // Campaigns
            // Customer (populate with 'x' if true)

            // TODO: modify for specified columns - make all columns proper width
            ws.Cell("A1").Value = "User";
            ws.Cell("B1").Value = "Email";
            ws.Cell("C1").Value = "Location";
            ws.Cell("D1").Value = "Country";
            ws.Cell("E1").Value = "Job Title";
            ws.Cell("F1").Value = "Last Login";

            var headerRange = ws.Range("A1:F1");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Bottom;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Font.FontColor = XLColor.DarkBlue;
            headerRange.Style.Fill.BackgroundColor = XLColor.Aqua;
 
            //var dt = filteredCompanies.Companies.Select(i => new
 
            //{
            //    i.FullName,
            //    i.EmailAddress,
            //    i.LocationName,
            //    i.CountryName,
            //    i.Title,
            //    i.LastLoginDate
            //}).AsEnumerable();

            // TODO:  figure out what data type I can use here to add the table. 
           // ws.Cell("A2").InsertData(dt); 
            var st = new MemoryStream();
            xlBook.SaveAs(st);

            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);

            // Create the blob client
            var blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container
            var containerReference = "temp";
            var container = blobClient.GetContainerReference(containerReference);

            var companyName = LoginUser.GetLoggedInUser()?.Subscriber?.CompanyName ?? "";

            var fileName = $"{companyName}_CRM_CompanyList_{DateTime.Now.ToString("dd-MMM-yy")}_{Guid.NewGuid()}.xlsx";

            var blockBlob = container.GetBlockBlobReference(fileName);
            blockBlob.Properties.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (st)
            {
                long streamlen = st.Length;
                st.Position = 0;
                blockBlob.UploadFromStream(st);
            }

            new Crm6.App_Code.Helpers.BlobStorageHelper().GetBlob(containerReference, fileName);

            return Ok();
        }
    }

}