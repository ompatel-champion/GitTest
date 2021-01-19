using System;
using Crm6.App_Code;
using Models;
using System.Linq;
using Crm6.App_Code.Helpers;
using ClosedXML.Excel;
using Crm6.App_Code.Shared;

namespace Helpers
{
    public class Admin
    {

        #region Last Activity Dates

        public bool UpdateLastActivityDates(int subscriberId)
        {
            // companies
            UpdateCompanyLastActivityDates(subscriberId);

            // contacts
            UpdateContactsLastActivityDates(subscriberId);

            // deals
            UpdateDealsLastActivityDates(subscriberId);

            return true;
        }


        private void UpdateCompanyLastActivityDates(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection); 
            var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());


            // comanies
            var comanies = context.Companies.Where(t => t.SubscriberId == subscriberId && !t.Deleted).ToList();
            foreach (var company in comanies)
            {
                var lastActivityDate = company.CreatedDate;

                // check for events
                var calendarEventsDates = (from t in sharedContext.Activities 
                                           where t.CompanyId == company.CompanyId && !t.Deleted && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                                           select t.CreatedDate).OrderByDescending(t => t).ToList();
                if (calendarEventsDates.Count > 0 && calendarEventsDates[0] > lastActivityDate)
                {
                    lastActivityDate = calendarEventsDates[0];
                }

                // check for deals
                var dealsDates = (from t in context.Deals
                                  where t.CompanyId == company.CompanyId && !t.Deleted && t.LastUpdate.HasValue
                                  select t.LastUpdate.Value).OrderByDescending(t => t).ToList();

                if (dealsDates.Count > 0 && dealsDates[0] > lastActivityDate)
                {
                    lastActivityDate = dealsDates[0];
                }

                // check for lanes
                var lanesDates = (from t in context.Deals
                                  join j in context.Lanes on t.DealId equals j.DealId
                                  where t.CompanyId == company.CompanyId && !t.Deleted
                                  select t.CreatedDate).OrderByDescending(t => t).ToList();
                if (lanesDates.Count > 0 && lanesDates[0] > lastActivityDate)
                {
                    lastActivityDate = lanesDates[0];
                }


                // check for contacts
                var contactDates = (from t in context.Contacts
                                    where t.CompanyId == company.CompanyId && !t.Deleted
                                    select t.LastUpdate).OrderByDescending(t => t).ToList();

                if (contactDates.Count > 0 && contactDates[0] > lastActivityDate)
                {
                    lastActivityDate = contactDates[0];
                }

                company.LastActivityDate = lastActivityDate;
                context.SubmitChanges();


            }
        }


        private void UpdateContactsLastActivityDates(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());
            // contacts
            var contacts = context.Contacts.Where(t => t.SubscriberId == subscriberId && !t.Deleted).ToList();
            foreach (var contact in contacts)
            {
                var lastActivityDate = contact.CreatedDate;

                // check for events
                var calendarEventsDates = (from t in sharedContext.Activities
                                           join j in sharedContext.ActivititesMembers on t.ActivityId equals j.ActivitiesId
                                           where j.ContactId == contact.ContactId && j.ContactSubscriberId == subscriberId && !t.Deleted && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                                           select t.CreatedDate).OrderByDescending(t => t).ToList();
                if (calendarEventsDates.Count > 0 && calendarEventsDates[0] > lastActivityDate)
                {
                    lastActivityDate = calendarEventsDates[0];
                }


                // check for deals
                var dealsContactsDates = (from t in context.Deals
                                          join j in context.LinkContactToDeals on t.DealId equals j.DealId
                                          where j.ContactId == contact.ContactId && !t.Deleted
                                          select j.CreatedDate).OrderByDescending(t => t).ToList();

                if (dealsContactsDates.Count > 0 && dealsContactsDates[0] > lastActivityDate)
                {
                    lastActivityDate = dealsContactsDates[0];
                }


                contact.LastActivityDate = lastActivityDate;
                context.SubmitChanges();

            }
        }


        private void UpdateDealsLastActivityDates(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            // deals
            var deals = context.Deals.Where(t => t.SubscriberId == subscriberId && !t.Deleted).ToList();
            foreach (var deal in deals)
            {

                var lastActivityDate = deal.CreatedDate;
                if (deal.LastActivityDate.HasValue)
                {
                    lastActivityDate = deal.LastActivityDate.Value;
                }

                // check for lanes
                var lanesDates = (from t in context.Deals
                                  join j in context.Lanes on t.DealId equals j.DealId
                                  where t.DealId == deal.DealId && !t.Deleted
                                  select t.CreatedDate).OrderByDescending(t => t).ToList();
                if (lanesDates.Count > 0 && lanesDates[0] > lastActivityDate)
                {
                    lastActivityDate = lanesDates[0];
                }

                //deal contact
                var dealsContactsDates = (from t in context.Deals
                                          join j in context.LinkContactToDeals on t.DealId equals j.DealId
                                          where j.DealId == deal.DealId && !t.Deleted
                                          select j.CreatedDate).OrderByDescending(t => t).ToList();

                if (dealsContactsDates.Count > 0 && dealsContactsDates[0] > lastActivityDate)
                {
                    lastActivityDate = dealsContactsDates[0];
                }

                //deal user
                var dealsUserDates = (from t in context.Deals
                                      join j in context.LinkUserToDeals on t.DealId equals j.DealId
                                      where j.DealId == deal.DealId && !t.Deleted
                                      select j.CreatedDate).OrderByDescending(t => t).ToList();

                if (dealsUserDates.Count > 0 && dealsUserDates[0] > lastActivityDate)
                {
                    lastActivityDate = dealsUserDates[0];
                }

                deal.LastActivityDate = lastActivityDate;
                context.SubmitChanges();

            }
        }


        #endregion


        #region Hkg Company Import

        public bool HkgCompanyImport(int subscriberId, string blobReference, string containerReference)
        {
            //var connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG_Staging;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //var sharedCconnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared_Staging;Persist Security Info=True;User ID=crm;Password=Ak#1350!";

            var connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            var sharedCconnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";


            var docStream = new BlobStorageHelper().DownloadBlobStream(containerReference, blobReference);

            //   return false;
            var rowNumber = 0;

            if (subscriberId > 0)
            {
                var userId = 0;
                var userFullName = "";
                var companyName = "";
                try
                {
                    // Open Excel File
                    using (XLWorkbook workBook = new XLWorkbook(docStream))
                    {
                        // Read the first Sheet from Excel file
                        IXLWorksheet workSheet = workBook.Worksheet("Sheet1");
                        //Loop through the Worksheet rows
                        foreach (IXLRow row in workSheet.Rows())
                        {

                            var rn = row.RowNumber();
                            if (rn == 1)
                            {
                                // First Row - Column Headers (Ignore)
                                rowNumber = 1;
                            }
                            else
                            {
                                try
                                {
                                    // USER
                                    userFullName = row.Cell(1).Value.ToString();

                                    if (string.IsNullOrEmpty(userFullName))
                                    {
                                        rowNumber += 1;
                                        continue;
                                    }
                                    userId = GetUserIdByFullName(connection, userFullName);
                                    if (userId > 0)
                                    {
                                        // COMPANY NAME
                                        companyName = row.Cell(2).Value.ToString();

                                        // Populate Company Object
                                        var co = new Company();
                                        {
                                            co.SubscriberId = subscriberId;
                                            co.Address = "";
                                            co.City = "";
                                            co.CompanyName = companyName;
                                            co.CompanyTypes = "";
                                            co.CountryName = "";
                                            co.CreatedDate = DateTime.UtcNow;
                                            co.CreatedUserId = 0;
                                            co.CreatedUserName = "HKG Data Import";
                                            co.Fax = "";
                                            co.Industry = "";
                                            co.LastUpdate = DateTime.UtcNow;
                                            co.OriginSystem = "HKG Data Import";
                                            co.Phone = "";
                                            co.Phone2 = "";
                                            co.Source = "HKG Data Import";
                                            co.StateProvince = "";
                                            co.UpdateUserId = 0;
                                            co.UpdateUserName = "HKG Data Import";
                                            co.CompanyOwnerUserId = userId;
                                        }

                                        // Check for Duplicate Company
                                        var companyFound = CheckDuplicateCompanyGetId(connection, co);
                                        var username = GetUserFullNameById(connection, userId);
                                        if (companyFound == null)
                                        {
                                            // Save Add Company and Link Company To userId
                                            var company = SaveCompany(connection, sharedCconnection, username, new CompanyModel { Company = co });
                                            if (company != null)
                                            {
                                                co.CompanyId = company.CompanyId;
                                                co.CompanyIdGlobal = company.CompanyIdGlobal;
                                            }
                                           
                                        }
                                        else
                                        {
                                            co.CompanyId = companyFound.CompanyId;
                                        }

                                        if (co.CompanyId > 0)
                                        {
                                            var context = new DbFirstFreightDataContext(connection);
                                            var linkedUser = context.LinkUserToCompanies.FirstOrDefault(c => c.CompanyId == co.CompanyId && c.UserId == userId && !c.Deleted);

                                            if (linkedUser == null)
                                            {
                                                context.LinkUserToCompanies.InsertOnSubmit(new LinkUserToCompany
                                                {
                                                    UserId = userId,
                                                    UserName = username,
                                                    CompanyId = co.CompanyId,
                                                    CompanyName = co.CompanyName,
                                                    SubscriberId = co.SubscriberId,
                                                    CreatedDate = DateTime.UtcNow,
                                                    LastUpdate = DateTime.UtcNow,
                                                    UpdateUserName = username,
                                                    UpdateUserId = userId,
                                                    CreatedUserId = userId,
                                                    CreatedUserName = username,
                                                    LinkType = ""
                                                });
                                                context.SubmitChanges();

                                                UpdateCompanySalesTeam(connection, co.CompanyId);
                                            }

                                            //  SaveGlobalCompany(sharedCconnection, username, co, "HKG");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var error = new Crm6.App_Code.Shared.WebAppError
                                    {
                                        ErrorCallStack = ex.StackTrace,
                                        ErrorDateTime = DateTime.UtcNow,
                                        RoutineName = "Import",
                                        PageCalledFrom = "ImportCompanies",
                                        SubscriberId = subscriberId,
                                        SubscriberName = "",
                                        ErrorMessage = ex.ToString(),
                                        UserId = userId
                                    };
                                    new Logging().LogWebAppError(error);
                                }
                            }
                            rowNumber += 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                }
            }

            return true;
        }

        #endregion


        #region Visa Global Company Import


        // aka: The Great Cyclomatic Complexity Nightmare
        public bool VisaGlobalCompanyImport(int subscriberId, string blobReference, string containerReference)
        {
            var connection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            var sharedCconnection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";

            var docStream = new BlobStorageHelper().DownloadBlobStream(containerReference, blobReference);

            //   return false;
            var rowNumber = 0;

            if (subscriberId > 0)
            {
                var userId = 0;
                var userFullName = "";
                var companyName = "";
                var contactFirstName = "";
                var contactLastName = "";
                var emailaddress = "";
                var address = "";
                var city = "";
                var province = "";
                var postalcode = "";
                var country = "";
                var telephone = "";
                var fax = "";
                var website = "";
                var source = "";
                var campaignName = "";
                var comments = "";
                var industry = "";
                try
                {
                    // Open Excel File
                    using (XLWorkbook workBook = new XLWorkbook(docStream))
                    {
                        // Read the first Sheet from Excel file
                        IXLWorksheet workSheet = workBook.Worksheet("Sheet1");
                        //Loop through the Worksheet rows
                        foreach (IXLRow row in workSheet.Rows())
                        {

                            var rn = row.RowNumber();
                            if (rn == 1)
                            {
                                // First Row - Column Headers (Ignore)
                                rowNumber = 1;
                            }
                            else
                            {
                                try
                                {
                                    // USER
                                    userFullName = "Adriana Rego";  //row.Cell(1).Value.ToString();

                                    if (string.IsNullOrEmpty(userFullName))
                                    {
                                        rowNumber += 1;
                                        continue;
                                    }
                                    // userId = GetUserIdByFullName(connection, userFullName);
                                    userId = 14571;
                                    if (userId > 0)
                                    {
                                        // COMPANY NAME
                                        companyName = row.Cell(1).Value.ToString();
                                        if (string.IsNullOrEmpty(companyName.Trim()))
                                        {
                                            continue;
                                        }

                                        contactFirstName = row.Cell(2).Value.ToString();
                                        contactLastName = row.Cell(3).Value.ToString();
                                        emailaddress = row.Cell(4).Value.ToString();

                                        address = row.Cell(5).Value.ToString();
                                        city = row.Cell(6).Value.ToString();
                                        province = row.Cell(7).Value.ToString();
                                        postalcode = row.Cell(8).Value.ToString();
                                        country = row.Cell(9).Value.ToString();
                                        telephone = row.Cell(10).Value.ToString();
                                        fax = row.Cell(11).Value.ToString();
                                        website = row.Cell(12).Value.ToString();
                                        industry = row.Cell(13).Value.ToString();
                                        source = row.Cell(14).Value.ToString();
                                        campaignName = row.Cell(15).Value.ToString();
                                        comments = row.Cell(16).Value.ToString();

                                        // Populate Company Object
                                        var co = new Company();
                                        {
                                            co.SubscriberId = subscriberId;
                                            co.Address = address;
                                            co.City = city;
                                            co.StateProvince = province;
                                            co.PostalCode = postalcode;
                                            co.CountryName = country;
                                            if (!string.IsNullOrEmpty(country))
                                                co.CountryCode = GetCountryCodeFromCountryName(sharedCconnection, country);
                                            co.CompanyName = companyName;
                                            co.CompanyTypes = "";
                                            co.CreatedDate = DateTime.UtcNow;
                                            co.CreatedUserId = userId;
                                            co.CreatedUserName = userFullName;
                                            co.Fax = fax;
                                            co.Website = website;
                                            co.Industry = industry;
                                            co.LastUpdate = DateTime.UtcNow;
                                            co.OriginSystem = "Data Import";
                                            co.Phone = telephone;
                                            co.Phone2 = "";
                                            co.Source = string.IsNullOrEmpty(source) ? "Data Import" : source;
                                            co.Comments = comments;
                                            co.CampaignName = campaignName;
                                            co.UpdateUserId = userId;
                                            co.UpdateUserName = userFullName;
                                            co.CompanyOwnerUserId = userId;
                                        }


                                        // Check for Duplicate Company
                                        var companyFound = CheckDuplicateCompanyGetId(connection, co);
                                        var username = GetUserFullNameById(connection, userId);
                                        if (companyFound == null)
                                        {
                                            // Save Add Company and Link Company To userId
                                            var newCompany = SaveCompany(connection, sharedCconnection, username, new CompanyModel { Company = co });
                                            if (newCompany != null)
                                            {
                                                co.CompanyId = newCompany.CompanyId;
                                                co.CompanyIdGlobal = newCompany.CompanyIdGlobal;
                                            }
                                        }
                                        else
                                        {
                                            co.CompanyId = companyFound.CompanyId;
                                            co.CompanyIdGlobal = companyFound.CompanyIdGlobal;

                                            // todo:  try to understand what the hell this block is actually doing
                                            var contextC = new DbFirstFreightDataContext(connection);
                                            var cc = contextC.Companies.Where(c => c.SubscriberId == co.SubscriberId
                                                             && c.CompanyId == co.CompanyId )
                                                                .Select(c => c).FirstOrDefault();
                                            if (cc != null)
                                            {
                                                cc.City = city;
                                                cc.Address = address;
                                                contextC.SubmitChanges();
                                            }
                                        }

                                        if (co.CompanyId > 0)
                                        {

                                            connection = LoginUser.GetConnection();
                                            var context = new DbFirstFreightDataContext(connection);
                                            // global company
                                            var sharedConnection = LoginUser.GetSharedConnection( );
                                            var sharedContext = new DbSharedDataContext(sharedConnection);
                                            // get the company by id or create new global company object
                                            var globalCompany = sharedContext
                                                        .GlobalCompanies
                                                        .FirstOrDefault(c => c.CompanyId == co.CompanyId && c.SubscriberId == co.SubscriberId) ?? new GlobalCompany();

                                            if (globalCompany != null)
                                            {
                                                globalCompany.City = city;
                                                globalCompany.Address = address;
                                                sharedContext.SubmitChanges();

                                                var loginConnection = LoginUser.GetLoginConnection();
                                                var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
                                                var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.UserId == userId &&  t.SubscriberId == co.SubscriberId);
                                                if (globalUser != null)
                                                {
                                                    // add link company user 
                                                    var linkedUser = sharedContext.LinkGlobalCompanyGlobalUsers
                                                        .FirstOrDefault(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId &&
                                                                             c.GlobalUserId == globalUser.GlobalUserId && !c.Deleted);
                                                    if (linkedUser == null)
                                                    {
                                                        var companyUser = new LinkGlobalCompanyGlobalUser();
                                                        companyUser.GlobalUserName = globalUser.FullName;
                                                        companyUser.UserSubscriberId = globalUser.SubscriberId;
                                                        companyUser.GlobalUserId = globalUser.GlobalUserId;
                                                        companyUser.CreatedBy = globalUser.GlobalUserId;
                                                        companyUser.CreatedByName = globalUser.FullName;
                                                        companyUser.CreatedDate = DateTime.UtcNow;
                                                        companyUser.LinkType = "";
                                                        companyUser.GlobalCompanyName = globalCompany.CompanyName;
                                                        companyUser.GlobalCompanyId = globalCompany.GlobalCompanyId;
                                                        companyUser.CompanySubscriberId = globalCompany.SubscriberId;
                                                        sharedContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);
                                                        sharedContext.SubmitChanges();

                                                        var usernames = sharedContext.LinkGlobalCompanyGlobalUsers
                                              .Where(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId)
                                              .Select(t => t.GlobalUserName).ToList();

                                                        var c = context.Companies.FirstOrDefault(t => t.CompanyId == co.CompanyId);
                                                        if (c != null)
                                                        {
                                                            c.SalesTeam = string.Join(",", usernames);
                                                            c.LastActivityDate = DateTime.UtcNow;
                                                            context.SubmitChanges();


                                                            // update global company sales team  
                                                            if (globalCompany != null)
                                                            {
                                                                globalCompany.SalesTeam = c.SalesTeam;
                                                                globalCompany.LastActivityDate = DateTime.UtcNow;
                                                                sharedContext.SubmitChanges();
                                                            }
                                                        }
                                                    }
                                                }
                                            }

                                            // add contact
                                            var contact = new Contact();
                                            contact.FirstName = contactFirstName;
                                            contact.LastName = contactLastName;
                                            contact.SubscriberId = co.SubscriberId;
                                            contact.BusinessAddress = address;
                                            contact.BusinessCity = city;
                                            contact.BusinessCountry = country;
                                            contact.Fax = fax;
                                            contact.BusinessPhone = telephone;
                                            contact.BusinessPostalCode = postalcode;
                                            contact.BusinessStateProvince = province;
                                            contact.CompanyName = companyName;
                                            contact.CompanyId = co.CompanyId;
                                            contact.CompanyIdGlobal = co.CompanyIdGlobal;
                                            contact.ContactName = contactFirstName + " " + contactLastName;
                                            contact.ContactSource = co.Source;
                                            contact.CreatedDate = DateTime.UtcNow;
                                            contact.CreatedUserId = userId;
                                            contact.CreatedUserName = userFullName;
                                            contact.Email = emailaddress;
                                            contact.LastUpdate = DateTime.UtcNow;
                                            contact.UpdateUserId = userId;
                                            contact.UpdateUserName = userFullName;

                                            var contactId = CheckDuplicateContactGetId(connection, contact);
                                            if (contactId < 1)
                                            {
                                                // Save Contact and Link Contact to Company and Contact to userId
                                                var contactSaveRequest = new ContactSaveRequest { Contact = contact };
                                                contactId = SaveContact(connection, contactSaveRequest);
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    var error = new Crm6.App_Code.Shared.WebAppError
                                    {
                                        ErrorCallStack = ex.StackTrace,
                                        ErrorDateTime = DateTime.UtcNow,
                                        RoutineName = "Import",
                                        PageCalledFrom = "ImportCompanies",
                                        SubscriberId = subscriberId,
                                        SubscriberName = "",
                                        ErrorMessage = ex.ToString(),
                                        UserId = userId
                                    };
                                    new Logging().LogWebAppError(error);
                                }
                            }
                            rowNumber += 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    var error = ex.ToString();
                }
            }

            return true;
        }

        #endregion


        #region Company

        private Company CheckDuplicateCompanyGetId(string connection, Company co)
        {
            // First 20 characters of the CompanyName
            var companyName = co.CompanyName;
            if (co.CompanyName.Length > 50)
                companyName = co.CompanyName.Substring(0, 50);
            var context = new DbFirstFreightDataContext(connection);
            var company = context.Companies.Where(c => c.SubscriberId == co.SubscriberId
                                 && c.Deleted == false
                                 && c.CompanyName.Contains(companyName))
                                .Select(c => c).FirstOrDefault();
            return company;
        }



        private Company SaveCompany(string connection, string sharedCconnection, string username, CompanyModel request)
        {
            try
            {
                var context = new DbFirstFreightDataContext(connection);

                var companyDetails = request.Company;

                // get the company by id or create new company object
                var company = context.Companies.FirstOrDefault(c => c.CompanyId == companyDetails.CompanyId) ?? new Company();
                var oldCompanyOwnerId = company.CompanyOwnerUserId;

                // fill company details
                company.Address = companyDetails.Address;
                company.AnnualRevenue = companyDetails.AnnualRevenue;
                company.AnnualShipments = companyDetails.AnnualShipments;
                company.AnnualVolumes = companyDetails.AnnualVolumes;
                company.CompanyName = companyDetails.CompanyName;
                company.City = companyDetails.City;
                company.CampaignName = companyDetails.CampaignName;
                company.Comments = companyDetails.Comments;
                company.Commodities = companyDetails.Commodities;
                company.CompanyTypes = companyDetails.CompanyTypes;
                company.Competitors = companyDetails.Competitors;
                company.CountryName = companyDetails.CountryName;
                company.CountryCode = !string.IsNullOrEmpty(company.CountryName) ? new Countries().GetCountryCodeFromCountryName(company.CountryName) : "";
                company.Destinations = companyDetails.Destinations;
                company.Fax = companyDetails.Fax;
                company.FreightServices = companyDetails.FreightServices;
                company.Industry = companyDetails.Industry;
                company.LastUpdate = DateTime.UtcNow;
                company.Origins = companyDetails.Origins;
                company.Phone = companyDetails.Phone;
                company.PostalCode = companyDetails.PostalCode;
                company.Source = companyDetails.Source;
                company.Website = companyDetails.Website;
                company.UpdateUserId = 0;
                company.UpdateUserName = username;
                company.LastActivityDate = DateTime.UtcNow;


                company.Division = companyDetails.Division;
                company.CompanyCode = companyDetails.CompanyCode;

                if (company.CompanyId < 1)
                {
                    // new company - insert
                    company.SubscriberId = companyDetails.SubscriberId;
                    company.CreatedUserId = 0;
                    company.CreatedDate = DateTime.UtcNow;
                    company.CreatedUserName = username;
                    company.IsCustomer = true;
                    company.Active = true;
                    company.CompanyOwnerUserId = 0;
                    company.OriginatingUserId = 0;
                    company.LastActivityDate = DateTime.UtcNow;
                    context.Companies.InsertOnSubmit(company);
                }
                // add/update verify method
                context.SubmitChanges();


                // Save Global Company in Shared Database 
                var gcid = SaveGlobalCompany(sharedCconnection, username, company, "USA");
                company.CompanyIdGlobal = gcid;
                context.SubmitChanges();

                // Return Company ID if Add
                return company;
            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "SaveCompany",
                    PageCalledFrom = "Helper/Companies",
                    SubscriberId = request.Company.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = request.Company.UpdateUserId
                };
                new Logging().LogWebAppError(error);
                return null;
            };
        }


        private int GetUserIdByFullName(string connection, string fullName)
        {
            var context = new DbFirstFreightDataContext(connection);
            var userId = context.Users.Where(u => (u.FirstName.Trim() + " " + u.LastName.Trim()).ToLower().Contains(fullName.ToLower()))
                .Select(u => u.UserId).FirstOrDefault();
            return userId;
        }

        private string GetUserFullNameById(string connection, int userId)
        {
            var context = new DbFirstFreightDataContext(connection);
            return context.Users.Where(u => u.UserId == userId)
                .Select(u => u.FullName).FirstOrDefault();
        }


        private void UpdateCompanySalesTeam(string connection, int companyId)
        {
            var context = new DbFirstFreightDataContext(connection);
            var salesTeamUsers = context.LinkUserToCompanies.Where(t => t.CompanyId == companyId && !t.Deleted)
                                    .Select(t => t.UserName).ToList();

            var company = context.Companies.FirstOrDefault(u => u.CompanyId == companyId);
            if (company != null)
            {
                company.SalesTeam = string.Join(", ", salesTeamUsers);
                context.SubmitChanges();
            }




        }

        private string GetCountryCodeFromCountryName(string connection, string countryName)
        {
            var context = new DbSharedDataContext(connection);
            return context.Countries.Where(c => c.CountryName.ToLower() == countryName.ToLower()).Select(c => c.CountryCode).FirstOrDefault();
        }


        private int SaveGlobalCompany(string connection, string username, Company company, string dataCenter)
        {
            try
            {
                var sharedContext = new DbSharedDataContext(connection);

                // get the company by id or create new global company object
                var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(c => c.CompanyId == company.CompanyId && c.SubscriberId == company.SubscriberId) ?? new GlobalCompany();

                // fill company details
                globalCompany.Address = string.IsNullOrEmpty(company.Address) ? "" : company.Address;
                globalCompany.City = string.IsNullOrEmpty(company.City) ? "" : company.City;
                globalCompany.EmailAddress = "";
                globalCompany.CompanyId = company.CompanyId;
                globalCompany.CompanyName = string.IsNullOrEmpty(company.CompanyName) ? "" : company.CompanyName;
                globalCompany.CountryName = string.IsNullOrEmpty(company.CountryName) ? "" : company.CountryName;
                globalCompany.DataCenter = dataCenter;
                globalCompany.LastUpdate = DateTime.UtcNow;
                globalCompany.UpdateUserId = company.UpdateUserId;
                globalCompany.LastUpdate = DateTime.UtcNow;
                globalCompany.Phone = string.IsNullOrEmpty(company.Phone) ? "" : company.Phone;
                globalCompany.PostalCode = string.IsNullOrEmpty(company.PostalCode) ? "" : company.PostalCode;
                globalCompany.UpdateUserId = company.UpdateUserId;
                globalCompany.StateProvince = string.IsNullOrEmpty(company.StateProvince) ? "" : company.StateProvince;
                globalCompany.LinkedUserNames = "";
                globalCompany.LinkedUserEmails = "";
                globalCompany.IpAddress = "";
                globalCompany.SalesTeam = company.SalesTeam;
                globalCompany.UpdateUserName = username;

                globalCompany.StateProvince = string.IsNullOrEmpty(company.StateProvince) ? "" : company.StateProvince;
                globalCompany.UpdateUserId = company.UpdateUserId;
                globalCompany.IsCustomer = company.IsCustomer;
                globalCompany.Active = company.Active;
                globalCompany.CompanyTypes = company.CompanyTypes;
                globalCompany.LastActivityDate = company.LastActivityDate.HasValue ? company.LastActivityDate.Value : DateTime.UtcNow;
                globalCompany.Division = company.Division;
                globalCompany.CompanyCode = company.CompanyCode;


                if (globalCompany.GlobalCompanyId < 1)
                {
                    // new company - insert
                    globalCompany.SubscriberId = company.SubscriberId;
                    globalCompany.CreatedUserId = company.UpdateUserId;
                    globalCompany.CreatedDate = DateTime.UtcNow;
                    globalCompany.CreatedUserName = username;
                    sharedContext.GlobalCompanies.InsertOnSubmit(globalCompany);
                }
                // add/update verify method
                sharedContext.SubmitChanges();
                return globalCompany.GlobalCompanyId;
            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "SaveGlobalCompany",
                    PageCalledFrom = "Helper/Companies",
                    SubscriberId = company.SubscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = company.UpdateUserId
                };
                new Logging().LogWebAppError(error);
            }
            return 0;

        }

        public int CheckDuplicateContactGetId(string connection, Contact co)
        {
            var duplicateContactId = 0;
            // First 20 characters of the CompanyName
            var companyName = co.CompanyName;
            if (co.CompanyName.Length > 50)
                companyName = co.CompanyName.Substring(0, 50);
            string contactName = co.ContactName + "";
            string email = co.Email + "";
            var context = new DbFirstFreightDataContext(connection);
            duplicateContactId = context.Contacts.Where(c => c.SubscriberId == co.SubscriberId
                                 && c.Deleted == false
                                 && c.CompanyName.Contains(companyName)
                                 && c.Email.ToLower().Equals(email.ToLower())
                                 && c.ContactName.ToLower().Equals(contactName.ToLower()))
                                .Select(c => c.ContactId).FirstOrDefault();
            return duplicateContactId;
        }


        public int SaveContact(string connection, ContactSaveRequest request)
        {
            try
            {
                var context = new DbFirstFreightDataContext(connection);
                // save contact
                var contact = request.Contact;
                // check for contact
                var objContact = context.Contacts.FirstOrDefault(d => d.ContactId == contact.ContactId) ?? new Contact();
                // populate fields
                objContact.FirstName = contact.FirstName;
                objContact.LastName = contact.LastName;
                objContact.MiddleName = contact.MiddleName;
                objContact.ContactName = contact.FirstName + (string.IsNullOrEmpty(contact.MiddleName) ? "" : (" " + contact.MiddleName)) + " " + contact.LastName;
                objContact.Title = contact.Title;
                objContact.Hobbies = contact.Hobbies;
                objContact.Email = contact.Email;
                objContact.MobilePhone = contact.MobilePhone;
                objContact.BusinessPhone = contact.BusinessPhone;
                objContact.BusinessAddress = contact.BusinessAddress;
                objContact.BusinessCity = contact.BusinessCity;
                objContact.BusinessCountry = contact.BusinessCountry;
                objContact.BusinessPostalCode = contact.BusinessPostalCode;
                objContact.Website = contact.Website;
                objContact.ContactType = contact.ContactType;
                objContact.CompanyId = contact.CompanyId;
                objContact.CompanyName = contact.CompanyName;
                objContact.LastUpdate = DateTime.UtcNow;
                objContact.UpdateUserId = contact.UpdateUserId;
                // user name 
                objContact.UpdateUserName = contact.UpdateUserName;

                // insert new contact
                if (objContact.ContactId < 1)
                {
                    objContact.SubscriberId = contact.SubscriberId;
                    objContact.ContactOwnerUserId = contact.UpdateUserId;
                    objContact.CreatedUserId = contact.UpdateUserId;
                    objContact.CreatedDate = DateTime.UtcNow;
                    objContact.CreatedUserName = contact.UpdateUserName;

                    // insert contact
                    context.Contacts.InsertOnSubmit(objContact);
                }
                context.SubmitChanges();


                // add link contact company
                var contactLinkCompany = context.LinkContactToCompanies
                        .FirstOrDefault(t => t.CompanyId == objContact.CompanyId && t.ContactId == objContact.ContactId && !t.Deleted);
                if (contactLinkCompany == null)
                {
                    context.LinkContactToCompanies.InsertOnSubmit(new LinkContactToCompany
                    {
                        ContactId = objContact.ContactId,
                        ContactName = objContact.ContactName,
                        CompanyId = objContact.CompanyId,
                        CompanyName = objContact.CompanyName,
                        SubscriberId = objContact.SubscriberId,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdate = DateTime.UtcNow,
                        UpdateUserName = objContact.UpdateUserName,
                        UpdateUserId = objContact.UpdateUserId,
                        CreatedUserId = objContact.UpdateUserId,
                        CreatedUserName = objContact.UpdateUserName,
                        LinkType = "Company Contact"
                    });
                    context.SubmitChanges();
                }

                // add link contact user - created user
                var createdContactUser = context.LinkUserToContacts
                        .FirstOrDefault(t => t.ContactId == objContact.ContactId && t.UserId == objContact.CreatedUserId && !t.Deleted);
                if (createdContactUser == null)
                {
                    context.LinkUserToContacts.InsertOnSubmit(new LinkUserToContact
                    {
                        UserId = objContact.UpdateUserId,
                        UserName = objContact.UpdateUserName,
                        ContactId = objContact.ContactId,
                        ContactName = objContact.ContactName,
                        SubscriberId = objContact.SubscriberId,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdate = DateTime.UtcNow,
                        UpdateUserName = objContact.UpdateUserName,
                        UpdateUserId = objContact.UpdateUserId,
                        CreatedUserId = objContact.UpdateUserId,
                        CreatedUserName = objContact.UpdateUserName,
                        LinkType = "",
                    });
                    context.SubmitChanges();
                }

                // add link company user - updated user
                var updatedContactUser = context.LinkUserToContacts
                       .FirstOrDefault(t => t.ContactId == objContact.ContactId && t.UserId == objContact.UpdateUserId && !t.Deleted);
                if (updatedContactUser == null)
                {
                    context.LinkUserToContacts.InsertOnSubmit(new LinkUserToContact
                    {
                        UserId = objContact.UpdateUserId,
                        UserName = objContact.UpdateUserName,
                        ContactId = objContact.ContactId,
                        ContactName = objContact.ContactName,
                        SubscriberId = objContact.SubscriberId,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdate = DateTime.UtcNow,
                        UpdateUserName = objContact.UpdateUserName,
                        UpdateUserId = objContact.UpdateUserId,
                        CreatedUserId = objContact.UpdateUserId,
                        CreatedUserName = objContact.UpdateUserName,
                        LinkType = ""
                    });
                    context.SubmitChanges();
                }

                // sales team
                objContact.SalesTeam = GetContactSalesTeam(connection, objContact.ContactId);
                context.SubmitChanges();


                // return contact Id
                return objContact.ContactId;

            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "SaveContact",
                    PageCalledFrom = "Helper/Contacts",
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = request.Contact.UpdateUserId
                };
                new Logging().LogWebAppError(error);
            }
            return 0;
        }

        private string GetContactSalesTeam(string connection, int contactId)
        {
            var context = new DbFirstFreightDataContext(connection);
            var salesTeamUsers = context.LinkUserToContacts.Where(t => t.ContactId == contactId && !t.Deleted)
                                    .Select(t => t.UserName).ToList();

            return string.Join(", ", salesTeamUsers);
        }

        #endregion


        #region Hash Passwords

        public bool HashPasswords()
        {
            // "USA":
            //  var connection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //  HashPassword(connection);

            //connection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US_Staging;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //HashPassword(connection);

            // "EMEA":
            //connection = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //HashPassword(connection);

            //connection = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA_Staging;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //HashPassword(connection);

            // "HKG":
            //connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //HashPassword(connection);

            //connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG_Staging;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            //HashPassword(connection);

            var connection = "Data Source=sqlsinotrans.database.chinacloudapi.cn;Initial Catalog=CRM_Sinotrans;Persist Security Info=True;User ID=crmffsino;Password=sinoff#1359Ak!";
            HashPassword(connection);

            return true;
        }

        private void HashPassword(string connection)
        {
            var context = new DbFirstFreightDataContext(connection);
            var users = context.Users.Where(t => !t.Deleted && t.LoginEnabled).ToList();
            foreach (var user in users)
            {
                if (!string.IsNullOrEmpty(user.Password))
                {
                    user.PasswordHashed = PasswordEncryptor.CreateHash(user.Password);
                    context.SubmitChanges();
                }
            }
        }

        #endregion


        #region Update Sales Team

        // NOT Used - One time run to update sales teams
        public bool UpdateSalesTeam()
        {

            //var liveShared = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_Shared;Persist Security Info=True;User ID=crm;Password=Ak#1350!";

            // "USA":
            var connection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRM_US;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            // UpdateDealsSalesTeam(connection);
            // UpdateCompaniesSalesTeam(connection, liveShared);
            // UpdateContactSalesteam(connection);

            // "EMEA":
            connection = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            // UpdateDealsSalesTeam(connection);
            // UpdateCompaniesSalesTeam(connection, liveShared);
            // UpdateContactSalesteam(connection);

            // "HKG":
            connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            // UpdateDealsSalesTeam(connection);
            // UpdateCompaniesSalesTeam(connection, liveShared);
            // UpdateContactSalesteam(connection);

            return true;
        }


        private void UpdateDealsSalesTeam(string connection)
        {
            var context = new DbFirstFreightDataContext(connection);
            var deals = context.Deals.Where(t => !t.Deleted);
            foreach (var deal in deals)
            {
                // get deal users
                var salesTeamUsers = context.LinkUserToDeals.Where(t => t.DealId == deal.DealId && !t.Deleted)
                                   .Select(t => t.UserName).ToList();
                deal.SalesTeam = string.Join(", ", salesTeamUsers);
                context.SubmitChanges();
            }
        }


        private void UpdateCompaniesSalesTeam(string connection, string sharedConnection)
        {
            var context = new DbFirstFreightDataContext(connection);
            var companies = context.Companies.Where(t => !t.Deleted);
            foreach (var company in companies)
            {
                // get company users
                var salesTeamUsers = context.LinkUserToCompanies
                                            .Where(t => t.CompanyId == company.CompanyId && !t.Deleted)
                                   .Select(t => t.UserName).ToList();
                company.SalesTeam = string.Join(", ", salesTeamUsers);
                context.SubmitChanges();

                // update global company
                var globalContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);
                // get the user by id or create new user object
                var gCompany = globalContext.GlobalCompanies.FirstOrDefault(l => l.CompanyId == company.CompanyId
                                && l.SubscriberId == company.SubscriberId);
                if (gCompany != null)
                {
                    gCompany.SalesTeam = company.SalesTeam;
                    globalContext.SubmitChanges();
                }
            }
        }


        private void UpdateContactSalesteam(string connection)
        {
            var context = new DbFirstFreightDataContext(connection);
            var contacts = context.Contacts.Where(t => !t.Deleted);
            foreach (var contact in contacts)
            {
                // get contacts users
                var salesTeamUsers = context.LinkUserToContacts.Where(t => t.ContactId == contact.ContactId && !t.Deleted)
                                   .Select(t => t.UserName).ToList();
                contact.SalesTeam = string.Join(", ", salesTeamUsers);
                context.SubmitChanges();
            }
        }

        #endregion

    }
}
