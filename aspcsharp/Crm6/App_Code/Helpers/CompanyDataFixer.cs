using Crm6.App_Code;
using Crm6.App_Code.Shared;
using System;
using System.Linq;

namespace Helpers
{
    public class CompanyDataFixer
    {

        public bool FixCompanyData()
        {
            // "USA":
            var connection = "Data Source=ffcrm.database.windows.net;Initial Catalog=CRMv6;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            UpdateGlobalCompaniesForDataCenter(connection, "USA");
            // "EMEA":
            connection = "Data Source=ffemea.database.windows.net;Initial Catalog=CRM_EMEA;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            UpdateGlobalCompaniesForDataCenter(connection, "EMEA");
            // "HKG":
            connection = "Data Source=ffhkg.database.windows.net;Initial Catalog=CRM_HKG;Persist Security Info=True;User ID=crm;Password=Ak#1350!";
            UpdateGlobalCompaniesForDataCenter(connection, "HKG");
            return true;
        }


        private void UpdateGlobalCompaniesForDataCenter(string connection, string dataCenter)
        {
            var dbContext = new DbFirstFreightDataContext(connection);
            if (dbContext != null)
            {
                var companies = dbContext.Companies.Where(c => !c.Deleted).ToList();
                foreach (var company in companies)
                {
                    SaveGlobalCompany(company);
                }
            }
        }




        // shared DB
        public int SaveGlobalCompany(Company company)
        {
            var globalCompanyId = 0;
            try
            {
                var subscriberId = company.SubscriberId;
                var sharedWriteableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
                var sharedContext = new DbSharedDataContext(sharedWriteableConnection);
                // get the company by id or create new global company object
                var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(c => c.CompanyId == company.CompanyId && c.SubscriberId == company.SubscriberId) ?? new GlobalCompany();

                // fill company details
                globalCompany.Address = string.IsNullOrEmpty(company.Address) ? "" : company.Address;
                globalCompany.City = string.IsNullOrEmpty(company.City) ? "" : company.City;
                globalCompany.StateProvince = string.IsNullOrEmpty(company.StateProvince) ? "" : company.StateProvince;
                globalCompany.EmailAddress = "";
                globalCompany.CompanyId = company.CompanyId;
                globalCompany.CompanyName = string.IsNullOrEmpty(company.CompanyName) ? "" : company.CompanyName;
                globalCompany.CountryName = string.IsNullOrEmpty(company.CountryName) ? "" : company.CountryName;
                globalCompany.DataCenter = new Subscribers().GetDataCenter(company.SubscriberId); ;
                globalCompany.IpAddress = "";
                globalCompany.LastUpdate = DateTime.UtcNow;
                globalCompany.LastUpdate = DateTime.UtcNow;
                globalCompany.LinkedUserNames = "";
                globalCompany.LinkedUserEmails = "";
                globalCompany.Phone = string.IsNullOrEmpty(company.Phone) ? "" : company.Phone;
                globalCompany.PostalCode = string.IsNullOrEmpty(company.PostalCode) ? "" : company.PostalCode;
                globalCompany.SalesTeam = company.SalesTeam;
                globalCompany.StateProvince = string.IsNullOrEmpty(company.StateProvince) ? "" : company.StateProvince;
                globalCompany.UpdateUserId = company.UpdateUserId;
                globalCompany.IsCustomer = company.IsCustomer;
                globalCompany.Active = company.Active;
                globalCompany.CompanyTypes = company.CompanyTypes;
                globalCompany.LastActivityDate = company.LastActivityDate.HasValue ? company.LastActivityDate.Value : DateTime.UtcNow;
                globalCompany.Division = company.Division;
                globalCompany.CompanyCode = company.CompanyCode;

                var username = new Users().GetUserFullNameById(company.UpdateUserId, subscriberId) ?? "";
                globalCompany.UpdateUserName = username;

                if (globalCompany.GlobalCompanyId < 1)
                {
                    // new company - insert
                    globalCompany.CreatedDate = DateTime.UtcNow;
                    globalCompany.CreatedUserId = company.UpdateUserId;
                    globalCompany.CreatedUserName = username;
                    sharedContext.GlobalCompanies.InsertOnSubmit(globalCompany);
                    globalCompany.SubscriberId = company.SubscriberId;
                }
                // add/update verify method
                sharedContext.SubmitChanges();

                globalCompanyId = globalCompany.GlobalCompanyId;

                if (company.CreatedUserId > 0)
                {
                    var loginConnection = LoginUser.GetLoginConnection();
                    var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
                    var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.UserId == company.CreatedUserId &&
                                                                               t.SubscriberId == company.SubscriberId);
                    if (globalUser != null)
                    {

                        // add link company user - created user
                        var createdCompanyUser = sharedContext.LinkGlobalCompanyGlobalUsers
                                .FirstOrDefault(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId &&
                                                     c.GlobalUserId == globalUser.GlobalUserId && !c.Deleted);
                        if (createdCompanyUser == null)
                        {
                            var companyUser = new LinkGlobalCompanyGlobalUser();
                            companyUser.GlobalUserName = globalUser.FullName;
                            companyUser.UserSubscriberId = globalUser.SubscriberId;
                            companyUser.GlobalUserId = globalUser.GlobalUserId;
                            companyUser.CreatedBy = globalUser.GlobalUserId;
                            companyUser.CreatedByName = globalUser.FullName;
                            companyUser.CreatedDate = DateTime.UtcNow;
                            companyUser.LastUpdate = DateTime.UtcNow;
                            companyUser.UpdateUserId = globalUser.GlobalUserId;
                            companyUser.UpdateUserName = globalUser.FullName;
                            companyUser.LinkType = "";
                            companyUser.GlobalCompanyName = globalCompany.CompanyName;
                            companyUser.GlobalCompanyId = globalCompany.GlobalCompanyId;
                            companyUser.CompanySubscriberId = globalCompany.SubscriberId;
                            sharedContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);
                            sharedContext.SubmitChanges();
                        }
                    }
                }

                // update sales team
                var usernames = sharedContext.LinkGlobalCompanyGlobalUsers
                                       .Where(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId && !t.Deleted)
                                       .Select(t => t.GlobalUserName).ToList();

                var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
                var co = context.Companies.FirstOrDefault(t => t.CompanyId == company.CompanyId);
                if (co != null)
                {
                    co.SalesTeam = string.Join(",", usernames);
                    co.LastActivityDate = DateTime.UtcNow;
                    co.CompanyIdGlobal = globalCompanyId;
                    context.SubmitChanges();

                    // update global company sales team  
                    if (globalCompany != null)
                    {
                        globalCompany.SalesTeam = co.SalesTeam;
                        globalCompany.LastActivityDate = DateTime.UtcNow;
                        sharedContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            { 
            }

            return globalCompanyId;
        }



    }
}