using System;
using System.Collections.Generic;
using System.Web;
using ClosedXML.Excel;
using Crm6.App_Code;
using Models;
using System.Linq;
using System.IO;
using System.Text;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Login;
using Crm6.App_Code.Shared;
using GlobalUser = Crm6.App_Code.Login.GlobalUser;

namespace Crm6.App_Code.Helpers
{
    /// <summary>
    /// A kind of service class for importing companies and contacts from a well defined Excel template.
    /// Responsible for setting up Companies, contacts, globalcompanies, and linking all entities.
    /// </summary>
    public class ImportCompanies
    {
        private DbLoginDataContext _loginContext;
        private readonly DbSharedDataContext _sharedContext;
        private readonly DbFirstFreightDataContext _crmContext;

        // note: if we get a DI framework running, this constructor won't be necessary
        public ImportCompanies() : this(LoginUser.GetWritableSharedConnectionForDataCenter(""), LoginUser.GetConnection())
        {
        }

        public ImportCompanies(string sharedConnectionString, string crmConnectionString)
        {
            _sharedContext = new DbSharedDataContext(sharedConnectionString);
            _crmContext = new DbFirstFreightDataContext(crmConnectionString);
        }


        /// <summary>
        /// Get the excel workbook from Azure storage. Defined as public field to make it easier to mock in test.
        ///  This decision can be revisited.  Discussion is welcome.
        /// </summary>
        public Func<string, string, XLWorkbook> GetWorkbook = (cref, bref) =>
        {
            var docStream = new BlobStorageHelper().DownloadBlobStream(cref, bref);
            return new XLWorkbook(docStream);
        };

        /// <summary>
        /// Extract Company Import data from excel worksheet
        /// </summary>
        /// <param name="workbook">XLWorkbook</param>
        /// <param name="subscriberId">int</param>
        public Func<XLWorkbook, int, GlobalUser, IEnumerable<CompanyImportModel>> GetCompanyImports = (workbook, subscriberId, user) =>
         {
             return workbook.Worksheet(1)
                 .Rows()
                 .Where(i => !CompanyImportModel.IsHeaderRow(i))
                 //.Skip(5)
                 //.Take(5)
                 .Select(i => CompanyImportModel.FromXlRow(i, subscriberId, user));
         };

        /// <summary>
        /// Extract Contact data from excel worksheet.  
        /// </summary>
        /// <param name="workbook">XLWorkbook</param>
        /// <param name="subscriberId">int</param>
        /// <returns>IEnumerable [ContactImport] </returns>
        public Func<XLWorkbook, int, IEnumerable<ContactImport>> GetContactImports = (workbook, subscriberId) =>
        {
            return workbook.Worksheet(1)
                .Rows()
                .Where(i => !CompanyImportModel.IsHeaderRow(i))
                .Select(i => ContactImport.FromRow(i, subscriberId));
        };

        /// <summary>
        /// The main entry point for the class.  Called from the import controller.
        /// </summary>
        /// <param name="subscriberId"></param>
        /// <param name="blobReference"></param>
        /// <param name="containerReference"></param>
        /// <returns></returns>
        public bool ImportCompaniesContacts(int subscriberId, int userId, string blobReference, string containerReference)
        {
            using (_loginContext = new DbLoginDataContext(LoginUser.GetLoginConnection()))
            using (var workbook = GetWorkbook(containerReference, blobReference))
            {
                var user = _loginContext.GlobalUsers.FirstOrDefault(i => i.UserId == userId && i.SubscriberId == subscriberId);
                var dataCenter = user?.DataCenter ?? HttpContext.Current.Session["UserDataCenter"].ToString();

                //var contacts = GetContactImports(workbook, subscriberId);
                var companyImports = GetCompanyImports(workbook, subscriberId, user);

                var companies = companyImports
                    // these can be changed to for loops
                    // foreach companyImportModel in companies, add contacts from worksheet 1
                    .Select(i => CompanyImportModel.AddContacts(i, workbook.Worksheet(1)))
                    // for each companyImportModel returned from previous statement, Convert them to 
                    //  company objects to save in db
                    .Select(i => ToCompany(i, dataCenter))
                    // for each company object returned above, SaveIfNotDuplicate
                    .Select(t => SaveIfNotDuplicate(t, user))
                    // casting to array forces enumeration and causes all loops to execute
                    .ToArray();

                _crmContext.SubmitChanges();
                _sharedContext.SubmitChanges();
                _loginContext.SubmitChanges();

                // note: important to solidify the enumeration to
                //  force context save... we need company id's in next step!
                //var globalCompanies = companies
                //    .Select(i => SaveGlobalCompany(i, user))
                //    .ToArray();

                //_crmContext.SubmitChanges();
                //_sharedContext.SubmitChanges();
                //_loginContext.SubmitChanges();

                //var linkedUsers = globalCompanies
                //    .Select(i => LinkGlobalUser(i, user))
                //    .ToArray();

                _crmContext.SubmitChanges();
                _sharedContext.SubmitChanges();
                _loginContext.SubmitChanges();

                // after save, we should have company id's generated
                //  so we can now link company to contact and whatever else
                var saved = companies
                    .Select(SaveContactAndLinkCompany)
                    .ToList();

                _crmContext.SubmitChanges();
                _sharedContext.SubmitChanges();
                _loginContext.SubmitChanges();
            }
            return true;
        }

        /// <summary>
        /// Convert the intermediate CompanyImportModel type to a db Company
        /// </summary>
        /// <param name="model"></param>
        /// <param name="dataCenter"></param>
        /// <returns></returns>
        private CompanyContactModel ToCompany(CompanyImportModel model, string dataCenter)
        {
            var userId = GetUserIdByFullName(model.SalesRepName);
            var result = new CompanyContactModel
            {
                Company = new Company
                {
                    SubscriberId = model.SubscriberId,
                    Address = model.Address,
                    City = model.City,
                    StateProvince = model.StateProvince,
                    PostalCode = model.PostalCode,
                    CountryName = model.CountryName,
                    CountryCode = GetCountryCode(model.CountryName),
                    CompanyName = model.CompanyName,
                    //CompanyTypes = "",
                    CreatedDate = DateTime.UtcNow,
                    CreatedUserId = userId,
                    CreatedUserName = model.SalesRepName,
                    Fax = model.Fax,
                    Website = model.Website,
                    Industry = model.Industry,
                    LastUpdate = DateTime.UtcNow,
                    OriginSystem = "Data Import",
                    Phone = model.Phone,
                    Phone2 = model.Phone2,
                    Source = model.Source ?? "Data Import",
                    Comments = model.Comments,
                    CampaignName = model.CampaignName,
                    UpdateUserId = userId,
                    UpdateUserName = model.SalesRepName,
                    CompanyOwnerUserId = userId,
                    SourceDataCenter = dataCenter,
                    Active = true,

                }
            };

            var contacts = model
                .ContactImports
                .Select(i => ContactImport.ToContact(i, result.Company))
                .ToList();

            result.Contacts = contacts;
            return result;
        }

        private int GetUserIdByFullName(string fullName)
        {
            var userId = _crmContext
                .Users
                .FirstOrDefault(u => u.FullName.ToLower() == fullName.ToLower())?.UserId;

            return userId ?? 0;
        }

        private string GetCountryCode(string countryName)
        {
            if (countryName.ToLower().Equals("uae"))
                countryName = "United Arab Emirates";
            if (countryName.ToLower().Equals("usa"))
                countryName = "United States";

            var country = _sharedContext.Countries.FirstOrDefault(c => c.CountryName.ToLower() == countryName.ToLower());
            return country?.CountryCode ?? "US";
        }

        private CompanyContactModel SaveContactAndLinkCompany(CompanyContactModel model)
        {
            foreach (var co in model.Contacts)
            {
                co.CompanyId = model.Company.CompanyId;
                co.CompanyIdGlobal = model.Company.CompanyIdGlobal;
                if (CheckDuplicateContactGetId(co) == 0)
                    _crmContext.Contacts.InsertOnSubmit(co);
            }

            // refresh for Id's 
            _crmContext.SubmitChanges();
            //get the companyContactLinks
            foreach (var co in model.Contacts)
            {
                var link = _crmContext
                    .LinkContactToCompanies
                    .FirstOrDefault(i => i.CompanyId == model.Company.CompanyId
                                         && i.ContactId == co.ContactId
                                         && !i.Deleted);
                if (link != null) continue;

                link = new LinkContactToCompany
                {
                    ContactId = co.ContactId,
                    ContactName = co.ContactName,
                    CompanyId = co.CompanyId,
                    CompanyName = co.CompanyName,
                    SubscriberId = co.SubscriberId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow,
                    UpdateUserName = co.UpdateUserName,
                    UpdateUserId = co.UpdateUserId,
                    CreatedUserId = co.UpdateUserId,
                    CreatedUserName = co.UpdateUserName,
                    LinkType = "Company Contact",
                    CompanyIdGlobal = co.CompanyIdGlobal
                };
                _crmContext.LinkContactToCompanies.InsertOnSubmit(link);
            }
            _crmContext.SubmitChanges();

            return model;
        }

        private GlobalCompany SaveGlobalCompany(CompanyContactModel model, Crm6.App_Code.Login.GlobalUser globalUser)
        {
            var co = model.Company;
            var globalCompany = _sharedContext.GlobalCompanies
                                    .FirstOrDefault(c => c.CompanyId == co.CompanyId && c.SubscriberId == co.SubscriberId);

            if (globalCompany == null)
            {
                globalCompany = new GlobalCompany
                {
                    SubscriberId = co.SubscriberId,
                    CompanyId = co.CompanyId,
                    CompanyName = co.CompanyName,

                    CreatedUserId = co.CreatedUserId,
                    CreatedUserName = co.CreatedUserName,
                    UpdateUserName = co.UpdateUserName,
                    Active = co.Active,
                    IsCustomer = co.IsCustomer,
                    GlobalCompanyOwnerGlobalUserId = globalUser.UserId, 
                    GlobalCompanyOwnerName = globalUser.UpdateUserName, 
                    DataCenter = co.SourceDataCenter,
                    EmailAddress = globalUser.EmailAddress,
                    IpAddress = "0.0.0.0",
                };
                _sharedContext.GlobalCompanies.InsertOnSubmit(globalCompany);
            }
            globalCompany.City = co.City;
            globalCompany.Address = co.Address;
            globalCompany.CountryName = co.CountryName;
            globalCompany.CreatedDate = DateTime.UtcNow;
            globalCompany.LastUpdate = DateTime.UtcNow;
            globalCompany.LastActivityDate = DateTime.UtcNow;

            _sharedContext.SubmitChanges();


            LinkGlobalUser(globalCompany, globalUser);

            co.CompanyIdGlobal = globalCompany.GlobalCompanyId;

            return globalCompany;
        }

        private GlobalCompany LinkGlobalUser(GlobalCompany gloco, GlobalUser globu)
        {
            var linkedUser = _sharedContext
                .LinkGlobalCompanyGlobalUsers
                .Where(i => i.GlobalCompanyId == gloco.GlobalCompanyId)
                .Where(i => i.GlobalUserId == globu.GlobalUserId)
                .Where(i => !i.Deleted)
                .FirstOrDefault();

            if (linkedUser == null)
            {
                linkedUser = new LinkGlobalCompanyGlobalUser
                {
                    GlobalUserName = globu.FullName,
                    UserSubscriberId = globu.SubscriberId,
                    GlobalUserId = globu.GlobalUserId,
                    CreatedBy = globu.GlobalUserId,
                    CreatedByName = globu.FullName,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdate = DateTime.UtcNow,
                    LinkType = string.Empty,
                    GlobalCompanyName = gloco.CompanyName,
                    GlobalCompanyId = gloco.GlobalCompanyId,
                    CompanySubscriberId = gloco.SubscriberId,
                };
                _sharedContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(linkedUser);
                _sharedContext.SubmitChanges();

            }

            // set the sales team
            var usernames = _sharedContext.LinkGlobalCompanyGlobalUsers
                                     .Where(t => t.GlobalCompanyId == gloco.GlobalCompanyId && !t.Deleted)
                                     .Select(t => t.GlobalUserName).ToList();

            var co = _crmContext.Companies.FirstOrDefault(t => t.CompanyId == gloco.CompanyId);
            if (co != null)
            {
                co.SalesTeam = string.Join(",", usernames);
                co.LastActivityDate = DateTime.UtcNow;
                co.CompanyIdGlobal = gloco.GlobalCompanyId;
                _crmContext.SubmitChanges();

                // update global company sales team  
                gloco.SalesTeam = co.SalesTeam;
                gloco.LastActivityDate = DateTime.UtcNow;
                _sharedContext.SubmitChanges();
            }

            return gloco;
        }

        private CompanyContactModel SaveIfNotDuplicate(CompanyContactModel model, GlobalUser user)
        {
            var company = model.Company;

            // First 20 characters of the CompanyName
            var companyName = company.CompanyName;
            if (company.CompanyName.Length > 20)
                companyName = company.CompanyName.Substring(0, 19);

            // First 10 characters of Address
            var address = "";
            if (company.Address.Length > 10)
                address = company.Address.Substring(0, 10);

            // City - exact match
            var city = company.City;
            var found = _crmContext.Companies
                .Where(i => i.SubscriberId == company.SubscriberId)
                .Where(i => i.Deleted == false)
                .Where(i => i.City == city)
                .Where(i => i.CompanyName.StartsWith(companyName))
                .Where(i => i.Address.StartsWith(address))
                .FirstOrDefault();

            if (found != null)
            {
                company.CompanyId = found.CompanyId;
                company.CompanyIdGlobal = found.CompanyIdGlobal;
                found.City = city;
                found.Address = company.Address + "";
            }
            else
            {
                _crmContext.Companies.InsertOnSubmit(company);
            }
            _crmContext.SubmitChanges();


            SaveGlobalCompany(model, user);

            return model;

        }

        private int CheckDuplicateContactGetId(Contact co)
        {
            var duplicateContactId = 0;

            // First 20 characters of the CompanyName
            var companyName = co.CompanyName;
            if (co.CompanyName.Length > 20)
                companyName = co.CompanyName.Substring(0, 19);

            // First 10 characters of contactName
            var contactName = co.ContactName + "";
            var email = co.Email + "";

            //var context = new DbFirstFreightDataContext();
            duplicateContactId = _crmContext.Contacts
                .Where(c => c.SubscriberId == co.SubscriberId
                     && c.Deleted == false
                     && c.CompanyName.StartsWith(companyName)
                     && c.Email.ToLower().Equals(email.ToLower())
                     && c.ContactName.ToLower().Equals(contactName.ToLower()))
                .Select(c => c.ContactId)
                .FirstOrDefault();

            return duplicateContactId;
        }

    }
}
