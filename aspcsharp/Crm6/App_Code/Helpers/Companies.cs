using System;
using System.Collections.Generic;
using System.Linq;
using Models;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Shared;
using System.Web;
using System.Net;

namespace Helpers
{
    public class Companies
    {

        #region *** Current Global Comapnies Functions ***

        // delete data center Companies table record and company contacts + deletes GlobalCompanies record in shared database
        public bool DeleteCompany(int companyId, int userId, int subscriberId)
        {
            try
            {
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                // get data center company record
                var company = context.Companies.FirstOrDefault(c => c.CompanyId == companyId);
                if (company != null)
                {
                    // set data center company to deleted
                    company.Deleted = true;
                    company.DeletedDate = DateTime.UtcNow;
                    company.DeletedUserId = userId;
                    company.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                    context.SubmitChanges();

                    // delete all company contacts in data center
                    var companyContacts = context.Contacts.Where(c => c.CompanyId == companyId).ToList();
                    foreach (var cContact in companyContacts)
                    {
                        new Contacts().DeleteContact(cContact.ContactId, userId, cContact.SubscriberId);
                    }
                    var deals = context.Deals.Where(c => c.CompanyId == companyId).ToList();
                    foreach (var deal in deals)
                    {
                        new Deals().DeleteDeal(deal.DealId, userId, subscriberId);
                    }

                    // Delete Company in Shared database GlobalCompanies
                    DeleteGlobalCompany(companyId, userId, subscriberId);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    ErrorMessage = ex.ToString(),
                    PageCalledFrom = "Helper/DeleteCompany",
                    RoutineName = "SaveGlobalCompany",
                    SubscriberName = "",
                    UserId = userId
                };
                new Logging().LogWebAppError(error);
            }
            return false;
        }


        public bool DeleteGlobalCompany(int companyId, int userId, int subscriberId)
        {
            // TODO: CompanyIdGlobal, UserIdGlobal
            var sharedConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(c => c.CompanyId == companyId && c.SubscriberId == subscriberId);
            if (globalCompany != null)
            {
                globalCompany.Deleted = true;
                globalCompany.DeletedDate = DateTime.UtcNow;
                globalCompany.DeletedUserId = userId;
                globalCompany.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                sharedContext.SubmitChanges();
                return true;
            }
            return false;
        }


        // used to populate CompanyList.aspx - api/Company/GetCompaniesGlobal
        public GlobalCompanyListResponse GetCompaniesGlobal(CompanyFilters filters)
        {
            var response = new GlobalCompanyListResponse
            {
                Companies = new List<GlobalCompany>()
            };

            var subscriberId = filters.SubscriberId;

            // data center db context
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection) { CommandTimeout = 0 };

            // shared db context
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            // login db context
            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);

            if (!string.IsNullOrEmpty(filters.FilterType))
            {
                // get global companies
                // TODO: filter for subscriberId
                var companies = sharedContext.GlobalCompanies.Where(c => !c.Deleted);

                // filter - country code
                if (!string.IsNullOrEmpty(filters.CountryCode))
                {
                    var countryName = new Countries().GetCountryNameFromCountryCode(filters.CountryCode).ToLower();
                    companies = companies.Where(t => t.CountryName.ToLower() == countryName);
                }

                // filter - userId
                if (filters.SalesRep > 0)
                {
                    // TODO: use GlobalUserId??
                    var filterUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == filters.SalesRep
                                                                                  && u.SubscriberId == filters.SubscriberId);
                    if (filterUser != null)
                    {
                        companies = companies.Where(t => t.SalesTeam.IndexOf(filterUser.FullName) >= 0);
                    }
                    else
                    {
                        return response;
                    }
                }

                // filter - is customer
                if (filters.IsCustomer)
                {
                    companies = companies.Where(t => t.IsCustomer);
                }

                IQueryable<GlobalCompany> allCompanies = null;

                if (filters.FilterType.Equals("ALL"))
                {
                    if (filters.UserId > 0)
                    {
                        // data center user object
                        var user = context.Users.FirstOrDefault(u => u.UserId == filters.UserId
                                                                     && u.SubscriberId == filters.SubscriberId);
                        // global user object
                        var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == filters.UserId
                                                                                      && u.SubscriberId == filters.SubscriberId);

                        if (globalUser != null && user != null)
                        {
                            var userRole = user.UserRoles;
                            // get all the linked global companies for globalUserId
                            var linkedCompanyIds = (from t in companies
                                                    join l in sharedContext.LinkGlobalCompanyGlobalUsers on t.GlobalCompanyId equals l.GlobalCompanyId
                                                    where !l.Deleted
                                                    && l.GlobalUserId == globalUser.GlobalUserId
                                                    && t.Active
                                                    select t.GlobalCompanyId);

                            // get all accessible companies | this can be as a manager
                            var otherCompanies = (from t in companies
                                                  join l in sharedContext.LinkGlobalCompanyGlobalUsers on t.GlobalCompanyId equals l.GlobalCompanyId
                                                  where !l.Deleted
                                                        && t.Active
                                                  select new { company = t, link = l });

                            var filterLinkedCompaniesOnly = false;

                            if (!string.IsNullOrEmpty(userRole))
                            {
                                var userIds = new List<int>();
                                // get sales manager user's location codes
                                if (user.UserRoles.Contains("Sales Manager"))
                                {
                                    var uIds = (from t in context.LinkUserToManagers
                                                where t.ManagerUserId == filters.UserId && !t.Deleted
                                                select t.UserId).Distinct().ToList();

                                    userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                              && t.SubscriberId == filters.SubscriberId)
                                                                              .Select(t => t.GlobalUserId));
                                }

                                if (user.UserRoles.Contains("CRM Admin"))
                                {
                                    otherCompanies = (from c in otherCompanies
                                                      where c.company.SubscriberId == globalUser.SubscriberId
                                                      || linkedCompanyIds.Contains(c.company.GlobalCompanyId)
                                                      select c);
                                }
                                else if (user.UserRoles.Contains("Region Manager"))
                                {
                                    if (!string.IsNullOrEmpty(globalUser.RegionName))
                                    {
                                        // this user is a region manager, get all the companies this user and his district users linked to 
                                        var uIds = context.Users.Where(u => u.RegionName.Equals(globalUser.RegionName)
                                                                 && u.SubscriberId == globalUser.SubscriberId)
                                                                .Select(u => u.UserId).ToList();
                                        // get global user Ids
                                        userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                                  && t.SubscriberId == filters.SubscriberId)
                                                                                 .Select(t => t.GlobalUserId));

                                        otherCompanies = (from c in otherCompanies
                                                          where (c.company.SubscriberId == globalUser.SubscriberId
                                                          && userIds.Contains(c.link.GlobalUserId))
                                                          || linkedCompanyIds.Contains(c.company.GlobalCompanyId)
                                                          select c);
                                    }
                                    else
                                    {
                                        filterLinkedCompaniesOnly = true;
                                    }
                                }
                                else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                                {
                                    // var countryCode = user.CountryCode;
                                    if (!string.IsNullOrEmpty(user.CountryCode))
                                    {
                                        // this user is a country manager, get all the companies this user and his district users linked to 
                                        var uIds = context.Users.Where(c => c.CountryCode.Equals(user.CountryCode)
                                                                 && c.SubscriberId == globalUser.SubscriberId)
                                                                .Select(c => c.UserId).ToList();
                                        // get global user Ids
                                        userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                                  && t.SubscriberId == filters.SubscriberId)
                                                                                 .Select(t => t.GlobalUserId));
                                        otherCompanies = (from c in otherCompanies
                                                          where (c.company.SubscriberId == globalUser.SubscriberId
                                                          && userIds.Contains(c.link.GlobalUserId))
                                                          || linkedCompanyIds.Contains(c.company.GlobalCompanyId)
                                                          select c);
                                    }
                                    else
                                    {
                                        filterLinkedCompaniesOnly = true;
                                    }
                                }
                                else if (user.UserRoles.Contains("District Manager"))
                                {
                                    var districtCode = user.DistrictCode;

                                    if (!string.IsNullOrEmpty(districtCode))
                                    {
                                        var district = new Districts().GetDistrictFromCode(districtCode, globalUser.SubscriberId);
                                        if (district != null)
                                        {
                                            // this user is a district manager, get all the companies this user and his district users linked to 
                                            var uIds = context.Users.Where(u => u.DistrictCode.Equals(district.DistrictCode)
                                                                     && u.SubscriberId == globalUser.SubscriberId)
                                                                    .Select(u => u.UserId).ToList();
                                            // get global user Ids
                                            userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                                      && t.SubscriberId == filters.SubscriberId)
                                                                                     .Select(t => t.GlobalUserId));
                                            otherCompanies = (from c in otherCompanies
                                                              where (c.company.SubscriberId == globalUser.SubscriberId
                                                              && userIds.Contains(c.link.GlobalUserId))
                                                              || linkedCompanyIds.Contains(c.company.GlobalCompanyId)
                                                              select c);
                                        }
                                        else
                                        {
                                            filterLinkedCompaniesOnly = true;
                                        }
                                    }
                                    else
                                    {
                                        filterLinkedCompaniesOnly = true;
                                    }
                                }
                                else if (user.UserRoles.Contains("Location Manager"))
                                {
                                    if (globalUser.LocationId > 0)
                                    {
                                        var location = new Locations().GetLocation(globalUser.LocationId, subscriberId);
                                        if (location != null)
                                        {
                                            // this user is a location manager, get all the companies this user and his location users linked to 
                                            var uIds = context.Users.Where(u => u.LocationCode.Equals(location.LocationCode)
                                                                     && u.SubscriberId == globalUser.SubscriberId)
                                                                    .Select(u => u.UserId)
                                                                    .ToList();
                                            // get global user Ids
                                            userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                                      && t.SubscriberId == filters.SubscriberId)
                                                                                     .Select(t => t.GlobalUserId));
                                            otherCompanies = (from c in otherCompanies
                                                              where (c.company.SubscriberId == globalUser.SubscriberId
                                                              && userIds.Contains(c.link.GlobalUserId))
                                                              || linkedCompanyIds.Contains(c.company.GlobalCompanyId)
                                                              select c);
                                        }
                                        else
                                        {
                                            filterLinkedCompaniesOnly = true;
                                        }
                                    }
                                    else
                                    {
                                        filterLinkedCompaniesOnly = true;
                                    }
                                }
                                else
                                {
                                    filterLinkedCompaniesOnly = true;
                                }

                                if (filterLinkedCompaniesOnly)
                                {
                                    otherCompanies = (from c in otherCompanies
                                                      where userIds.Contains(c.link.GlobalUserId) || linkedCompanyIds.Contains(c.company.GlobalCompanyId)
                                                      select c);
                                }
                            }
                            else
                            {
                                otherCompanies = (from c in otherCompanies
                                                  where linkedCompanyIds.Contains(c.company.GlobalCompanyId)
                                                  select c);
                            }

                            // final list
                            allCompanies = otherCompanies.Select(t => t.company).Distinct();

                            // keyword filter
                            allCompanies = allCompanies.Where(t => filters.Keyword == null ||
                                                          t.CompanyName.ToLower().Contains(filters.Keyword) ||
                                                          t.SalesTeam.ToLower().Contains(filters.Keyword) ||
                                                          (t.CompanyTypes != null && t.CompanyTypes.ToLower().Contains(filters.Keyword)) ||
                                                          (t.StateProvince != null && t.StateProvince.ToLower().Contains(filters.Keyword)) ||
                                                          (t.PostalCode != null && t.PostalCode.ToLower().Contains(filters.Keyword)) ||
                                                          (t.Address != null && t.Address.ToLower().Contains(filters.Keyword)))
                                                         .Distinct();

                            if (!string.IsNullOrEmpty(filters.CountryName))
                            {
                                allCompanies = allCompanies.Where(t => t.CountryName.ToLower().Contains(filters.CountryName.ToLower()));
                            }
                            if (!string.IsNullOrEmpty(filters.City))
                            {
                                allCompanies = allCompanies.Where(t => t.City.ToLower().Contains(filters.City.ToLower()));
                            }
                            if (!string.IsNullOrEmpty(filters.PostalCode))
                            {
                                allCompanies = allCompanies.Where(t => t.PostalCode.ToLower().Contains(filters.PostalCode.ToLower()));
                            }
                        }
                    }
                }
                else if (filters.FilterType.Equals("INACTIVE"))
                {
                    // get companies
                    allCompanies = companies.Where(c => c.SubscriberId == filters.SubscriberId && !c.Active && !c.Deleted);

                    // filter by last activity date
                    //  allCompanies = allCompanies.Where(c => c.LastActivityDate <= DateTime.Now.AddMonths(-6));

                    if (!string.IsNullOrEmpty(filters.Keyword))
                    {
                        allCompanies = allCompanies.Where(t => filters.Keyword == null ||
                                                         t.SalesTeam.ToLower().Contains(filters.Keyword) ||
                                                         t.CompanyName.ToLower().Contains(filters.Keyword)
                                                         || (t.CompanyTypes != null && t.CompanyTypes.ToLower().Contains(filters.Keyword)) ||
                                                         (t.StateProvince != null && t.StateProvince.ToLower().Contains(filters.Keyword)) ||
                                                         (t.PostalCode != null && t.PostalCode.ToLower().Contains(filters.Keyword)) ||
                                                        (t.Address != null && t.Address.ToLower().Contains(filters.Keyword))
                                                        ).Distinct();
                    }

                    if (!string.IsNullOrEmpty(filters.CountryName))
                    {
                        allCompanies = allCompanies.Where(t => t.CountryName.ToLower().Contains(filters.CountryName.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filters.City))
                    {
                        allCompanies = allCompanies.Where(t => t.City.ToLower().Contains(filters.City.ToLower()));
                    }
                    if (!string.IsNullOrEmpty(filters.PostalCode))
                    {
                        allCompanies = allCompanies.Where(t => t.PostalCode.ToLower().Contains(filters.PostalCode.ToLower()));
                    }

                    // company type
                    if (!string.IsNullOrEmpty(filters.CompanyType))
                    {
                        allCompanies = allCompanies.Where(t => t.CompanyTypes.ToLower().Contains(filters.CompanyType.ToLower()));
                    }
                    allCompanies = allCompanies.Distinct();
                }

                if (!string.IsNullOrEmpty(filters.SortBy) && allCompanies != null)
                {
                    switch (filters.SortBy.ToLower())
                    {
                        case "createddate asc":
                            allCompanies = allCompanies.OrderBy(t => t.CreatedDate);
                            break;
                        case "createddate desc":
                            allCompanies = allCompanies.OrderByDescending(t => t.CreatedDate);
                            break;
                        case "companyname asc":
                            allCompanies = allCompanies.OrderBy(t => t.CompanyName);
                            break;
                        case "companyname desc":
                            allCompanies = allCompanies.OrderByDescending(t => t.CompanyName);
                            break;
                        case "phone asc":
                            allCompanies = allCompanies.OrderBy(t => t.Phone);
                            break;
                        case "phone desc":
                            allCompanies = allCompanies.OrderByDescending(t => t.Phone);
                            break;
                        case "lastactivity asc":
                            allCompanies = allCompanies.OrderBy(t => t.LastActivityDate);
                            break;
                        case "lastactivity desc":
                            allCompanies = allCompanies.OrderByDescending(t => t.LastActivityDate);
                            break;
                        case "nextactivity asc":
                            allCompanies = allCompanies.OrderBy(t => t.NextActivityDate);
                            break;
                        case "nextactivity desc":
                            allCompanies = allCompanies.OrderByDescending(t => t.NextActivityDate);
                            break;
                        case "countryname asc":
                            allCompanies = allCompanies.OrderBy(t => t.CountryName);
                            break;
                        case "countryname desc":
                            allCompanies = allCompanies.OrderByDescending(t => t.CountryName);
                            break;
                        case "city asc":
                            allCompanies = allCompanies.OrderBy(t => t.City);
                            break;
                        case "city desc":
                            allCompanies = allCompanies.OrderByDescending(t => t.City);
                            break;
                        case "postalcode asc":
                            allCompanies = allCompanies.OrderBy(t => t.PostalCode);
                            break;
                        case "postalcode desc":
                            allCompanies = allCompanies.OrderByDescending(t => t.PostalCode);
                            break;
                        default:
                            break;
                    }
                }

                // record count / total pages
                var recordCount = allCompanies.Count();
                var totalPages = 0;
                // apply paging
                if (filters.RecordsPerPage > 0 && filters.CurrentPage > 0)
                {
                    allCompanies = allCompanies.Skip((filters.CurrentPage - 1) * filters.RecordsPerPage)
                                 .Take(filters.RecordsPerPage);
                    totalPages = recordCount % filters.RecordsPerPage == 0 ?
                                        (recordCount / filters.RecordsPerPage) :
                                      ((recordCount / filters.RecordsPerPage) + 1);
                }

                var listCompanies = allCompanies.ToList();

                response.Companies = listCompanies;

                if (!string.IsNullOrEmpty(filters.Keyword))
                {
                    //        response.AccessibleCompanyIds = GetAccessibleCompanyIds(filters.UserId, filters.SubscriberId);
                }

                if (response.Companies.Count > 0)
                {
                    response.TotalPages = totalPages;
                    response.Records = recordCount;
                }
            }

            // set the return companies list
            return response;
        }


        private string GetCompanyAccessRequestUrl(int subscriberId, string guid)
        {
            var url = "http://";
            // TODO: always https
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                url = "https://";
            }

            var uriAddress = HttpContext.Current.Request.Url.ToString();
            if (uriAddress.Contains("localhost"))
            {
                // dev TODO: port 44391
                url += "localhost:64604/Companies/SalesTeamAccessByEmail/SalesTeamAccessByEmail.aspx?guid=" + guid;
            }
            else
            {
                // live
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                var subscriberDomain = context.Subscribers.Where(t => t.SubscriberId == subscriberId).Select(t => t.SubDomain).FirstOrDefault();
                if (string.IsNullOrEmpty(subscriberDomain))
                {
                    subscriberDomain = "crm6";
                }
                url += subscriberDomain + ".firstfreight.com/Companies/SalesTeamAccessByEmail/SalesTeamAccessByEmail.aspx?guid=" + guid;
            }
            return url;
        }


        private string GetCompanyDetailUrl(int subscriberId, int companyId)
        {
            var url = "http://";
            // TODO: always https
            if (HttpContext.Current.Request.IsSecureConnection)
            {
                url = "https://";
            }

            var uriAddress = HttpContext.Current.Request.Url.ToString();
            if (uriAddress.Contains("localhost"))
            {
                // dev TODO: port 44391
                url += "localhost:64604/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + companyId + "&subscriberId=" + subscriberId;
            }
            else
            {
                var connection = LoginUser.GetConnection();
                var userContext = new DbFirstFreightDataContext(connection);
                var subscriberDomain = userContext.Subscribers.Where(t => t.SubscriberId == subscriberId).Select(t => t.SubDomain).FirstOrDefault();
                if (string.IsNullOrEmpty(subscriberDomain))
                    subscriberDomain = "crm6";

                url += subscriberDomain + ".firstfreight.com/Companies/CompanyDetail/CompanyDetail.aspx?companyId=" + companyId + "&subscriberId=" + subscriberId;
            }
            return url;
        }


        public string GetCompanyLogoUrl(int companyId)
        {
            var logo = new Documents().GetDocumentsByDocType(Convert.ToInt32(DocumentTypeEnum.CompanyLogo), companyId).ToList();
            if (logo.Count > 0)
                return logo[0].DocumentUrl;
            return "";
        }


        public string GetCompanyNameFromGlobalCompanyId(int globalcompanyId, int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(sharedConnection);
            return context.GlobalCompanies.Where(c => c.GlobalCompanyId == globalcompanyId).Select(c => c.CompanyName).FirstOrDefault() ?? "";
        }


        public int GetGlobalCompanyIdForCompanyId(int companyId, int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var globalCompanyId = sharedContext.GlobalCompanies.Where(t => t.CompanyId == companyId
                                                    && t.SubscriberId == subscriberId)
                                                    .Select(t => t.GlobalCompanyId)
                                                    .FirstOrDefault();
            return globalCompanyId;
        }


        public GlobalCompanyListResponse SearchGlobalCompanies(CompanyFilters filters)
        {
            // Global Company Search Lookup
            var response = new GlobalCompanyListResponse
            {
                Companies = new List<GlobalCompany>()
            };

            // shared db context
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var linkedSubscribers = sharedContext.LinkGlobalSuscriberToSubscribers
                                    .Where(s => s.GlobalSubscriberId == filters.SubscriberId)
                                    .Select(s => s.LinkedSubscriberId)
                                    .ToList();

            // get global companies TODO: why do this ???
            var globalCompanies = sharedContext.GlobalCompanies.Where(c => !c.Deleted);

            // get companies for linked subscribers
            var allGlobalCompanies = globalCompanies.Where(c => linkedSubscribers.Contains(c.SubscriberId) && !c.Deleted);

            // keyword search
            allGlobalCompanies = allGlobalCompanies.Where(t => filters.Keyword == null || t.CompanyName.ToLower()
                                                    .Contains(filters.Keyword))
                                                    .Distinct();

            response.AccessibleCompanyIds = GetAccessibleCompanyIds(filters.UserId, filters.SubscriberId);
            response.Companies = allGlobalCompanies.ToList();

            return response;
        }


        #endregion


        #region *** DataCenter Companies Functions ***

        public Company GetCompany(int companyId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                            .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                            .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Companies.FirstOrDefault(c => c.CompanyId == companyId);
        }

        public List<CompanyExtended> CheckDuplicateCompany(CompanyModel co)
        {
            List<CompanyExtended> finalList = new List<CompanyExtended>();

            var response = new List<Company>();
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                              .GlobalSubscribers.Where(t => t.SubscriberId == co.Company.SubscriberId)
                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);

            // First 10 characters of the CompanyName
            var companyName = co.Company.CompanyName != null && co.Company.CompanyName.Length > 10 ? co.Company.CompanyName.Substring(0, 10) : co.Company.CompanyName;
            //var addressMatch = co.Company.Address != null && co.Company.Address.Length > 8 ? co.Company.Address.Substring(0, 8) : co.Company.Address;
            var cityMatch = co.Company.City;
            var phoneMatch = co.Company.Phone;
            //var postalCodeMatch = co.Company.PostalCode;

            // check the company match for name,address and city or postcode
            var matchingCompanies = context.Companies.Where(t => !t.Deleted && t.SubscriberId == co.Company.SubscriberId && t.CompanyName.ToLower().Contains(companyName.ToLower())
                                                                    //&& t.Address.ToLower().Contains(addressMatch.ToLower())
                                                                    && ((cityMatch != null && cityMatch != "" && t.City.ToLower().Contains(cityMatch.ToLower()))
                                                                    /*|| (postalCodeMatch != null && postalCodeMatch != "" && t.PostalCode.ToLower().Contains(postalCodeMatch.ToLower()))*/)
                                                                    ).ToList();
            var alreadyFoundIds = new List<int>();
            if (matchingCompanies.Count > 0)
            {
                response.AddRange(matchingCompanies);
                alreadyFoundIds.AddRange(response.Select(t => t.CompanyId).ToList());
            }

            // phone matching companies
            if (!string.IsNullOrWhiteSpace(phoneMatch))
            {
                var phoneMatchingCompanies = context.Companies.Where(t => !t.Deleted && t.SubscriberId == co.Company.SubscriberId && t.Phone != null && t.Phone != "" && (t.Phone.Contains(phoneMatch)) && !alreadyFoundIds.Contains(t.CompanyId)).ToList();
                if (phoneMatchingCompanies.Count > 0)
                {
                    matchingCompanies.AddRange(phoneMatchingCompanies);
                }
            }

            foreach (var company in matchingCompanies)
            {
                var salesOwnerName = "";
                var user = (from t in context.Users where t.UserId == company.CompanyOwnerUserId select t).FirstOrDefault();

                if (user != null)
                {
                    salesOwnerName = user.FullName;
                }

                finalList.Add(new CompanyExtended() { Company = company, SalesOwnerName = salesOwnerName });
            }

            return finalList;
        }

        // TODO: need to check for company duplicate on ADD COMPANY
        public int CheckDuplicateCompanyGetId(Company co)
        {
            var duplicateCompanyId = 0;
            // First 20 characters of the CompanyName
            string companyName = co.CompanyName.Substring(1, 20);
            // First 10 characters of Address
            string address = co.Address.Substring(1, 10);
            // City - exact match
            string city = co.City;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            duplicateCompanyId = context.Companies.Where(c => c.SubscriberId == co.SubscriberId
                                                              && c.Deleted == false
                                                              && c.CompanyName.Contains(companyName)
                                                              && c.Address.Contains(address)
                                                              && c.City.Contains(city))
                .Select(c => c.CompanyId).FirstOrDefault();
            return duplicateCompanyId;
        }


        public string GetCompanyCity(int companyId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                             .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                             .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            //var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Companies.Where(t => t.CompanyId == companyId).Select(t => t.City).FirstOrDefault() ?? "";
        }


        public List<ContactModel> GetCompanyContacts(int companyId, int subscriberId)
        {
            //var connection = LoginUser.GetConnection();

            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                          .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                          .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);

            var contacts = (from t in context.LinkContactToCompanies
                            join j in context.Contacts on t.ContactId equals j.ContactId
                            where t.CompanyId == companyId && !t.Deleted && !j.Deleted
                            select new Contacts().GetContact(t.ContactId, subscriberId)).ToList();
            return contacts;
        }


        public RevenueResponse GetCompanyDealsRevenue(int companyId, int userId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                           .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                           .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);


            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var revenueResponses = context.Deals.Where(c => c.CompanyId == companyId && !c.Deleted)
                .Select(t => new Deals().GetDealRevenue(t.DealId, userId, subscriberId)).ToList();

            if (revenueResponses.Count > 0)
            {
                return new RevenueResponse
                {
                    CurrencySymbol = revenueResponses[0].CurrencySymbol,
                    Revenue = revenueResponses.Sum(c => c.Revenue),
                    Profit = revenueResponses.Sum(c => c.Profit),
                };
            }

            // zero revenue
            return new RevenueResponse
            {
                CurrencySymbol = new Users().GetUserCurrencySymbol(userId, subscriberId),
                Revenue = 0,
                Profit = 0,
            };
        }

        public string GetCompanyIndustry(int companyId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            //  var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Companies.Where(c => c.CompanyId == companyId).Select(c => c.Industry).FirstOrDefault();
        }

        //get the company logo URL
        //note: this can return a user uploaded image url if available, an svg data url displaying the company initials, or a default placeholder image
        public string GetCompanyLogoUrl(int companyId, int subscriberId)
        {
            var defImgUrl = "_content/_img/image-placeholder.png";
            var logos = new Documents().GetDocumentsByDocType(Convert.ToInt32(DocumentTypeEnum.CompanyLogo), companyId, subscriberId).ToList();
            var imageUrl = logos.Count == 0 ? null : logos[0].DocumentUrl;
            var companyModel = new Helpers.Companies().GetCompany(companyId, subscriberId);
            return Utils.GetProfileImageUrl(imageUrl, companyModel.CompanyName, defImgUrl);
        }

        public DocumentModel GetCompanyLogo(int companyId, int subscriberId)
        {
            var logo = new Documents().GetDocumentsByDocType(Convert.ToInt32(DocumentTypeEnum.CompanyLogo), companyId, subscriberId).ToList();
            if (logo.Count > 0)
                return logo[0];
            return null;
        }

        public int SaveCompany(CompanyModel request)
        {
            try
            {
                var subscriberId = request.Company.SubscriberId;
                var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                              .Select(t => t.DataCenter).FirstOrDefault();
                var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
                var context = new DbFirstFreightDataContext(connection);

                var companyDetails = request.Company;

                // get the company by id or create new company object
                var company = context.Companies.FirstOrDefault(c => c.CompanyId == companyDetails.CompanyId) ?? new Company();

                // company owner id
                var oldCompanyOwnerId = company.CompanyOwnerUserId;

                // fill company details
                company.Address = companyDetails.Address;
                company.AnnualRevenue = companyDetails.AnnualRevenue;
                company.AnnualShipments = companyDetails.AnnualShipments;
                company.AnnualVolumes = companyDetails.AnnualVolumes;
                company.City = companyDetails.City;
                company.CampaignName = companyDetails.CampaignName;
                company.Comments = companyDetails.Comments;
                company.Commodities = companyDetails.Commodities;
                company.CompanyCode = companyDetails.CompanyCode;
                company.CompanyOwnerUserId = companyDetails.CompanyOwnerUserId;
                company.CompanyTypes = companyDetails.CompanyTypes;
                company.Competitors = companyDetails.Competitors;
                company.CountryName = companyDetails.CountryName;
                company.CountryCode = !string.IsNullOrEmpty(company.CountryName) ? new Countries().GetCountryCodeFromCountryName(company.CountryName) : "";
                company.Destinations = companyDetails.Destinations;
                company.Division = companyDetails.Division;
                company.Fax = companyDetails.Fax;
                company.FreightServices = companyDetails.FreightServices;
                company.Industry = companyDetails.Industry;
                company.IsCustomer = companyDetails.IsCustomer;
                company.Active = companyDetails.Active;
                company.LastUpdate = DateTime.UtcNow;
                company.Origins = companyDetails.Origins;
                company.Phone = companyDetails.Phone;
                company.PostalCode = companyDetails.PostalCode;
                company.StateProvince = companyDetails.StateProvince;
                company.Source = companyDetails.Source;
                company.Website = companyDetails.Website;
                company.UpdateUserId = companyDetails.UpdateUserId;
                company.UpdateUserName = new Users().GetUserFullNameById(companyDetails.UpdateUserId, subscriberId);

                if (company.CompanyId < 1)
                {

                    // new company - insert
                    company.SubscriberId = companyDetails.SubscriberId;
                    company.CreatedUserId = companyDetails.UpdateUserId;
                    company.CreatedDate = DateTime.UtcNow;
                    company.CreatedUserName = new Users().GetUserFullNameById(companyDetails.UpdateUserId, subscriberId);
                    company.Active = true;
                    company.OriginatingUserId = companyDetails.UpdateUserId;
                    company.LastActivityDate = DateTime.UtcNow;
                    company.CompanyName = companyDetails.CompanyName;

                    // if (!CheckDuplicateCompany(new CompanyModel { Company = company }, companyDetails.SubscriberId))
                    context.Companies.InsertOnSubmit(company);
                    // else
                    //     return 0;
                }
                else
                {
                    if (!company.Active && companyDetails.Active)
                    {
                        // company was inactive and marked by an admin as active
                        company.LastActivityDate = DateTime.UtcNow;
                        company.Active = true;
                        company.AdminOverideActive = true;
                        company.AdminOverideActiveDate = DateTime.UtcNow;
                    }
                    else
                    {
                        company.Active = companyDetails.Active;
                    }
                                        
                    if (company.CompanyName != companyDetails.CompanyName) {
                        //update company name in link and contacts tables
                        var contactsLinks = (from i in context.LinkContactToCompanies
                            where i.CompanyId == company.CompanyId select i);
                        foreach (var contactLink in contactsLinks) {
                            contactLink.CompanyName = companyDetails.CompanyName;
                            var contact = context.Contacts.FirstOrDefault(i => i.ContactId == contactLink.ContactId);
                            contact.CompanyName = companyDetails.CompanyName;
                        }
                        //update company name in activities table
                        var sharedConnection = LoginUser.GetSharedConnection();
                        var sharedContext = new DbSharedDataContext(sharedConnection);
                        var activites = sharedContext.Activities.Where(i => i.CompanyIdGlobal == company.CompanyIdGlobal).ToList();
                        foreach (var activity in activites) activity.CompanyName = companyDetails.CompanyName;
                        sharedContext.SubmitChanges();
                        //update company name                        
                        company.CompanyName = companyDetails.CompanyName;
                    }
                }
                // add/update verify method
                context.SubmitChanges();

                // Save Company Logo
                var companyLogo = request.CompanyLogo;
                if (companyLogo != null)
                {
                    companyLogo.CompanyId = company.CompanyId;
                    companyLogo.SubscriberId = company.SubscriberId;
                    companyLogo.DocumentTypeId = Convert.ToInt32(DocumentTypeEnum.CompanyLogo);
                    companyLogo.UploadedBy = company.UpdateUserId;
                    companyLogo.UploadedByName = new Users().GetUserFullNameById(companyDetails.UpdateUserId, subscriberId);
                    new Documents().SaveDocument(companyLogo);
                }

                if (oldCompanyOwnerId != 0 && oldCompanyOwnerId != company.CompanyOwnerUserId)
                {
                    company.LastActivityDate = DateTime.UtcNow;
                    company.Active = true;
                    context.SubmitChanges();
                }

                // Save Global Company in Shared Database
                var gCompanyId = SaveGlobalCompany(company);
                try
                {
                    if (company.CompanyIdGlobal == 0)
                    {
                        company.CompanyIdGlobal = gCompanyId;
                        context.SubmitChanges();
                    }

                }
                catch (Exception) { }


                // Log Event 
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = company.CreatedUserId,
                    CompanyId = company.CompanyId,
                    CompanyName = company.CompanyName,
                    UserActivityMessage = "Saved Company"
                });

                //DONE: intercom Journey Step event
                // log event to intercom
                var eventName = "Saved company";
                var intercomHelper = new IntercomHelper();
                intercomHelper.IntercomTrackEvent(company.CreatedUserId, subscriberId, eventName);

                if (request.CreateSession)
                {
                    // LoginUser.CreateQuickAddCompanySession(company);
                }

                // Return Company ID if Add
                return company.CompanyId;
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
                return 0;
            };
        }



        #endregion

        // TODO: global company keyword search - for action button "request access"
        public List<int> GetAccessibleCompanyIds(int userId, int subscriberId)
        {
            // data center context
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection) { CommandTimeout = 0 };

            // shared context
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection) { CommandTimeout = 0 };

            // login context
            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection) { CommandTimeout = 0 };

            var companyIds = new List<int>();
            // global user object
            var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == userId
                                                                          && u.SubscriberId == subscriberId);
            // data center user object
            var user = context.Users.FirstOrDefault(u => u.UserId == userId
                                                         && u.SubscriberId == subscriberId);

            var userIds = new List<int>();

            if (globalUser != null && user != null)
            {
                userIds.Add(globalUser.GlobalUserId);

                // get all the linked global companies for globalUserId
                var linkedGlobalCompanyIds = (from t in sharedContext.GlobalCompanies
                                              join l in sharedContext.LinkGlobalCompanyGlobalUsers on t.GlobalCompanyId equals l.GlobalCompanyId
                                              where !l.Deleted
                                                      && l.GlobalUserId == globalUser.GlobalUserId
                                                      && t.Active
                                                      && !t.Deleted
                                              select t.GlobalCompanyId);


                var otherCompanies = (from t in sharedContext.GlobalCompanies
                                      join l in sharedContext.LinkGlobalCompanyGlobalUsers on t.GlobalCompanyId equals l.GlobalCompanyId
                                      where !l.Deleted
                                            && t.Active
                                            && !t.Deleted
                                      select new { company = t, link = l });

                // get sales manager user's location codes
                if (user.UserRoles.Contains("Sales Manager"))
                {
                    var uIds = (from t in context.LinkUserToManagers
                                where t.ManagerUserId == userId
                                      && !t.Deleted
                                select t.UserId).Distinct().ToList();

                    userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                         && t.SubscriberId == subscriberId)
                                                                        .Select(t => t.GlobalUserId));
                }

                if (user.UserRoles.Contains("CRM Admin"))
                {
                    companyIds = (from c in otherCompanies
                                  where c.company.SubscriberId == globalUser.SubscriberId
                                        || linkedGlobalCompanyIds.Contains(c.company.GlobalCompanyId)
                                  select c.company.GlobalCompanyId).ToList();
                }
                else if (user.UserRoles.Contains("Region Manager"))
                {
                    if (!string.IsNullOrEmpty(globalUser.RegionName))
                    {
                        // this user is a region manager, get all the companies this user and his district users linked to 
                        var uIds = context.Users.Where(u => u.RegionName.Equals(globalUser.RegionName)
                                                            && u.SubscriberId == globalUser.SubscriberId)
                                                            .Select(u => u.UserId).ToList();
                        // get global user Ids
                        userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                             && t.SubscriberId == subscriberId)
                                                                            .Select(t => t.GlobalUserId));
                        companyIds = (from c in otherCompanies
                                      where (c.company.SubscriberId == globalUser.SubscriberId
                                            && userIds.Contains(c.link.GlobalUserId))
                                            || linkedGlobalCompanyIds.Contains(c.company.GlobalCompanyId)
                                      select c.company.GlobalCompanyId).ToList();
                    }
                }
                else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                {
                    var countryCode = context.Users.Where(u => u.UserId == userId
                                                                && u.SubscriberId == subscriberId)
                                                                .Select(t => t.CountryCode).FirstOrDefault();
                    if (!string.IsNullOrEmpty(countryCode))
                    {
                        // this user is a country manager, get all the companies this user and his district users linked to 
                        var uIds = context.Users.Where(c => c.CountryCode.Equals(countryCode)
                                                            && c.SubscriberId == globalUser.SubscriberId)
                                                            .Select(c => c.UserId).ToList();
                        // get global user Ids
                        userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                             && t.SubscriberId == subscriberId)
                                                                            .Select(t => t.GlobalUserId));
                        companyIds = (from c in otherCompanies
                                      where (c.company.SubscriberId == globalUser.SubscriberId && userIds.Contains(c.link.GlobalUserId))
                                      || linkedGlobalCompanyIds.Contains(c.company.GlobalCompanyId)
                                      select c.company.GlobalCompanyId).ToList();
                    }
                }
                else if (user.UserRoles.Contains("District Manager"))
                {
                    var districtCode = context.Users.Where(u => u.UserId == userId
                                                && u.SubscriberId == subscriberId)
                                                .Select(t => t.DistrictCode).FirstOrDefault();

                    if (!string.IsNullOrEmpty(districtCode))
                    {
                        var district = new Districts().GetDistrictFromCode(districtCode, globalUser.SubscriberId);
                        if (district != null)
                        {
                            // this user is a district manager, get all the companies this user and his district users linked to
                            var uIds = context.Users.Where(u => u.DistrictCode.Equals(district.DistrictCode)
                                                                && u.SubscriberId == globalUser.SubscriberId)
                                                                .Select(u => u.UserId).ToList();
                            // get global user Ids
                            userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                                 && t.SubscriberId == subscriberId)
                                                                                .Select(t => t.GlobalUserId));
                            companyIds = (from c in otherCompanies
                                          where (c.company.SubscriberId == globalUser.SubscriberId
                                                 && userIds.Contains(c.link.GlobalUserId))
                                          || linkedGlobalCompanyIds.Contains(c.company.GlobalCompanyId)
                                          select c.company.GlobalCompanyId).ToList();
                        }
                    }
                }
                else if (user.UserRoles.Contains("Location Manager"))
                {
                    if (globalUser.LocationId > 0)
                    {
                        var location = new Locations().GetLocation(globalUser.LocationId, subscriberId);
                        if (location != null)
                        {
                            // this user is a location manager, get all the companies this user and his location users linked to 
                            var uIds = context.Users.Where(u => u.LocationCode.Equals(location.LocationCode)
                                                                && u.SubscriberId == globalUser.SubscriberId)
                                                                .Select(u => u.UserId)
                                                                .ToList();
                            // get global user Ids
                            userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                                 && t.SubscriberId == subscriberId)
                                                                                .Select(t => t.GlobalUserId));
                            companyIds = (from c in otherCompanies
                                          where (c.company.SubscriberId == globalUser.SubscriberId
                                                && userIds.Contains(c.link.GlobalUserId))
                                                || linkedGlobalCompanyIds.Contains(c.company.GlobalCompanyId)
                                          select c.company.GlobalCompanyId).ToList();
                        }
                    }
                }
                else
                {
                    companyIds = (from c in otherCompanies
                                  where linkedGlobalCompanyIds.Contains(c.company.GlobalCompanyId)
                                  select c.company.GlobalCompanyId).ToList();
                }
            }

            return companyIds;
        }


        // datacenter DB
        public string GetCompanyNameFromId(int companyId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Companies.Where(c => c.CompanyId == companyId).Select(c => c.CompanyName).FirstOrDefault() ?? "";
        }





        // shared DB
        public GlobalCompanyListResponse GetGlobalCompanies(CompanyFilters filters)
        {
            var response = new GlobalCompanyListResponse
            {
                Companies = new List<GlobalCompany>()
            };
            var sharedConnection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(sharedConnection);
            var suscriberCompanies = context.LinkGlobalSuscriberToSubscribers
                                            .Where(s => s.GlobalSubscriberId == filters.SubscriberId)
                                            .Select(s => s.LinkedSubscriberId)
                                            .ToList();

            // get global companies
            var companies = context.GlobalCompanies.Where(c => suscriberCompanies.Contains(c.SubscriberId) && !c.Deleted);

            // apply filters
            if (!string.IsNullOrEmpty(filters.Keyword))
            {
                filters.Keyword = filters.Keyword.ToLower();
                companies = companies.Where(c => c.CompanyName.ToLower().Contains(filters.Keyword));
            }

            // set sort order
            companies = companies.OrderBy(c => c.CompanyName);

            // record count
            var recordCount = companies.Count();

            // TODO: remove paging
            var totalPages = 0;
            // apply paging
            if (filters.RecordsPerPage > 0 && filters.CurrentPage > 0)
            {
                companies = companies.Skip((filters.CurrentPage - 1) * filters.RecordsPerPage)
                             .Take(filters.RecordsPerPage);
                totalPages = recordCount % filters.RecordsPerPage == 0 ?
                                    (recordCount / filters.RecordsPerPage) :
                                  ((recordCount / filters.RecordsPerPage) + 1);
            }
            response.Companies = companies.ToList();

            // TODO: remove paging
            if (response.Companies.Count > 0)
            {
                response.TotalPages = totalPages;
                response.Records = recordCount;
            }
            // set the return deal list
            return response;
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
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    ErrorMessage = ex.ToString(),
                    PageCalledFrom = "Helper/Companies",
                    RoutineName = "SaveGlobalCompany",
                    SubscriberId = company.SubscriberId,
                    SubscriberName = "",
                    UserId = company.UpdateUserId
                };
                new Logging().LogWebAppError(error);
            }

            return globalCompanyId;
        }


        // datacenter DB
        public void UpdateCompanyLastActivityDate(int companyId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var company = context.Companies.FirstOrDefault(t => t.CompanyId == companyId);
            if (company != null)
            {
                company.LastActivityDate = DateTime.UtcNow;
                context.SubmitChanges();

                // get the global company
                var sharedWriteableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
                // var sharedConnection = LoginUser.GetSharedConnection();
                var sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnection);
                var globalCompany = sharedWriteableContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == company.CompanyId &&
                                                                                      t.SubscriberId == company.SubscriberId);
                if (globalCompany != null)
                {
                    globalCompany.LastActivityDate = DateTime.UtcNow;
                    sharedWriteableContext.SubmitChanges();
                }
            }
        }


        // datacenter DB
        public void UpdateCompanyLastUpdateDate(int companyId, int userId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var company = context.Companies.FirstOrDefault(t => t.CompanyId == companyId);
            if (company != null)
            {
                company.LastUpdate = DateTime.UtcNow;
                company.UpdateUserId = userId;
                company.UpdateUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
            }
        }


        #region Company Users


        // shared DB
        public bool AddCompanyUser(AddCompanyUserRequest request)
        {
            try
            {
                var loginConnection = LoginUser.GetLoginConnection();
                var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);

                var subscriberId = request.CompanySubscriberId;
                var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                              .Select(t => t.DataCenter).FirstOrDefault();
                var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

                // var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                var updatedUserName = new Users().GetUserFullNameById(request.UpdatedBy, subscriberId);

                // global company Id
                var sharedWriteableConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
                var sharedContext = new DbSharedDataContext(sharedWriteableConnection);
                var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == request.CompanyId
                                                                                && t.SubscriberId == request.CompanySubscriberId);

                // global user Id 
                var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.GlobalUserId == request.GlobalUserId);

                if (globalCompany != null && globalUser != null)
                {
                    // check if link global company to global user record exists
                    var found = sharedContext.LinkGlobalCompanyGlobalUsers.FirstOrDefault(i => i.GlobalCompanyId == globalCompany.GlobalCompanyId
                        && i.GlobalUserId == globalUser.GlobalUserId
                        && !i.Deleted);
                    if (found == null)
                    {
                        var companyUser = new LinkGlobalCompanyGlobalUser();
                        companyUser.CompanySubscriberId = globalCompany.SubscriberId;
                        companyUser.CreatedBy = request.UpdatedBy;
                        companyUser.CreatedByName = updatedUserName;
                        companyUser.CreatedDate = DateTime.UtcNow;
                        companyUser.GlobalCompanyId = globalCompany.GlobalCompanyId;
                        companyUser.GlobalCompanyName = globalCompany.CompanyName;
                        companyUser.GlobalUserId = globalUser.GlobalUserId;
                        companyUser.GlobalUserName = globalUser.FullName;
                        companyUser.UpdateUserId = request.UpdatedBy;
                        companyUser.UpdateUserName = updatedUserName;
                        companyUser.LastUpdate = DateTime.UtcNow;
                        companyUser.SalesTeamRole = request.SalesTeamRole;
                        // TODO: populate LinkType
                        companyUser.LinkType = request.LinkType;
                        sharedContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);
                        companyUser.UserSubscriberId = globalUser.SubscriberId;
                    }
                    else
                    {
                        found.SalesTeamRole = request.SalesTeamRole;
                        found.LastUpdate = DateTime.UtcNow;
                        found.UpdateUserName = updatedUserName;
                        found.UpdateUserId = request.UpdatedBy;
                        found.LinkType = request.LinkType;
                    }
                    sharedContext.SubmitChanges();

                    // update company last activity date
                    var companyConnection = LoginUser.GetConnection();
                    var companyContext = new DbFirstFreightDataContext(companyConnection);
                    var company = companyContext.Companies.FirstOrDefault(t => t.CompanyId == request.CompanyId);
                    if (company != null)
                    {
                        // update sales team
                        var usernames = sharedContext.LinkGlobalCompanyGlobalUsers
                                                 .Where(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId && !t.Deleted)
                                                 .Select(t => t.GlobalUserName).ToList();

                        company.SalesTeam = string.Join(",", usernames);
                        company.LastActivityDate = DateTime.UtcNow;
                        context.SubmitChanges();

                        // update global company sales team and last activity date
                        globalCompany.SalesTeam = company.SalesTeam;
                        globalCompany.LastActivityDate = DateTime.UtcNow;
                        sharedContext.SubmitChanges();

                        // if new user send company invite email
                        if (found == null)
                        {
                            // send email
                            // TODO: check if invite has already been sent
                            // TODO: UNcomment following line when go live
                            // SendCompanyInvitation(company, companyUser);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var error = new WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    ErrorMessage = ex.ToString(),
                    PageCalledFrom = "Helper/DeleteCompany",
                    RoutineName = "SaveGlobalCompany",
                    SubscriberName = "",
                    UserId = request.UserId,
                    SubscriberId = request.CompanySubscriberId,
                };
                new Logging().LogWebAppError(error);
            }
            return false;
        }


        // shared DB
        public bool DeleteCompanyUser(int companyId, int companySubscriberId, int userId, int deleteUserId, int deleteUserSubscriberId)
        {
            // CRM_Shared database
            var sharedWriteableConnnection = LoginUser.GetWritableSharedConnectionForSubscriberId(deleteUserSubscriberId);
            var sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnnection);
            var globalCompany = sharedWriteableContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == companyId &&
                                                                        t.SubscriberId == companySubscriberId);

            // global user Id
            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
            var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.UserId == deleteUserId
                                                                          && t.SubscriberId == deleteUserSubscriberId);

            if (globalCompany != null && globalUser != null)
            {
                var companyUsers = sharedWriteableContext.LinkGlobalCompanyGlobalUsers.Where(l => l.GlobalUserId == globalUser.GlobalUserId
                                                                                         && l.GlobalCompanyId == globalCompany.GlobalCompanyId
                                                                                         ).ToList();

                foreach (var companyUser in companyUsers)
                {
                    companyUser.DeletedBy = userId;
                    companyUser.Deleted = true;
                    companyUser.DeletedDate = DateTime.UtcNow;
                    sharedWriteableContext.SubmitChanges();

                    // TODO: sales team - data center database???
                    var companyConnection = LoginUser.GetConnection();
                    var companyContext = new DbFirstFreightDataContext(companyConnection);
                    var company = companyContext.Companies.FirstOrDefault(t => t.CompanyId == globalCompany.CompanyId);
                    if (company != null)
                    {
                        // get global link company to users list
                        var usernames = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                                                 .Where(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId
                                                             && !t.Deleted)
                                                 .Select(t => t.GlobalUserName).ToList();
                        // update data center company sales team
                        company.SalesTeam = string.Join(",", usernames);
                        companyContext.SubmitChanges();

                        // TODO: update last activity date
                        // update global company sales team
                        globalCompany.SalesTeam = company.SalesTeam;
                        sharedWriteableContext.SubmitChanges();
                    }
                }
            }
            return true;
        }


        // shared DB
        public List<CompanySalesTeamMember> GetCompanyUsers(int companyId, int subscriberId)
        {
            var linkedUsers = new List<CompanySalesTeamMember>();

            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == companyId && t.SubscriberId == subscriberId);

            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);

            if (globalCompany != null)
            {
                var users = sharedContext.LinkGlobalCompanyGlobalUsers.Where(i => i.GlobalCompanyId == globalCompany.GlobalCompanyId
                      && !i.Deleted).OrderBy(i => i.Id);
                foreach (var linkUser in users)
                {
                    var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.GlobalUserId == linkUser.GlobalUserId && t.SubscriberId == linkUser.UserSubscriberId);
                    if (globalUser != null)
                    {
                        var user = new Users().GetUser(globalUser.UserId, globalUser.SubscriberId);
                        if (user != null)
                        {
                            linkedUsers.Add(new CompanySalesTeamMember
                            {
                                LinkGlobalCompanyGlobalUser = linkUser,
                                User = user.User,
                                ProfilePicture = user.ProfilePicture
                            });
                        }
                    }
                }
            }
            return linkedUsers;
        }


        public List<User> GetGlobalCompanyUsers(int companyId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companylUsers = (from t in context.LinkUserToCompanies
                                 join j in context.Users on t.UserId equals j.UserId
                                 where t.CompanyId == companyId
                                       && !t.Deleted
                                       && !j.Deleted
                                 select new Users().GetUserByConnection(connection, t.UserId)
                            ).ToList();
            return companylUsers.Where(t => t != null).Select(t => t).ToList();
        }

        #endregion


        // datacenter DB
        public bool ClaimCompany(int companyId, int userId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var company = context.Companies.Where(t => t.CompanyId == companyId).FirstOrDefault();
            if (company != null)
            {
                company.CompanyOwnerUserId = userId;
                company.Active = true;
                company.LastActivityDate = DateTime.UtcNow;
                company.LastUpdate = DateTime.UtcNow;
                company.UpdateUserId = userId;
                company.UpdateUserName = new Users().GetUserFullNameById(userId, subscriberId);
                company.Claimed = 1;
                company.ClaimedDate = DateTime.UtcNow;
                company.ClaimedUserId = userId;
                company.ClaimedUserName = company.UpdateUserName;
                context.SubmitChanges();

                // remove all the old linked users

                // add company user
                var sharedWriteableConnnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
                var sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnnection);

                var globalCompany = sharedWriteableContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == company.CompanyId && t.SubscriberId == company.SubscriberId);
                if (globalCompany != null)
                {
                    // update global company
                    globalCompany.Active = company.Active;
                    globalCompany.LastActivityDate = company.LastActivityDate.Value;
                    globalCompany.LastUpdate = company.LastUpdate;
                    globalCompany.UpdateUserId = userId;
                    globalCompany.UpdateUserName = company.UpdateUserName;
                    sharedWriteableContext.SubmitChanges();

                    var oldlinkUsers = sharedWriteableContext.LinkGlobalCompanyGlobalUsers.Where(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId && !c.Deleted).ToList();
                    var noteContent = "";
                    if (oldlinkUsers.Count > 0)
                    {
                        // add a note to the company with old sales team
                        noteContent = "Claimed on " + DateTime.Now.ToString("dd-MMM-yy") + ". Old Linked sales team: " + string.Join(",", oldlinkUsers.Select(t => t.GlobalUserName).ToArray());

                        foreach (var oldlinkUser in oldlinkUsers)
                        {
                            oldlinkUser.Deleted = true;
                            oldlinkUser.DeletedBy = userId;
                            oldlinkUser.DeletedDate = DateTime.Now;
                            sharedWriteableContext.SubmitChanges();
                        }
                    }

                    var loginConnection = LoginUser.GetLoginConnection();
                    var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
                    var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.UserId == userId
                                                                                  && t.SubscriberId == company.SubscriberId);

                    // add link company user - created user
                    var linkUser = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                            .FirstOrDefault(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId
                                                 && c.GlobalUserId == globalUser.GlobalUserId
                                                 && !c.Deleted);
                    if (linkUser == null && globalUser != null)
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
                        companyUser.LastUpdate = DateTime.UtcNow;
                        companyUser.UpdateUserId = globalUser.UpdateUserId;
                        companyUser.UpdateUserName = globalUser.UpdateUserName;

                        sharedWriteableContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);

                        sharedWriteableContext.SubmitChanges();

                        // update sales team
                        var usernames = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                                                    .Where(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId && !t.Deleted)
                                                    .Select(t => t.GlobalUserName).ToList();
                        globalCompany.SalesTeam = string.Join(",", usernames);
                        sharedWriteableContext.SubmitChanges();

                        company.SalesTeam = string.Join(",", usernames);
                        context.SubmitChanges();

                        // create note with old company users
                        if (!string.IsNullOrEmpty(noteContent))
                        {
                            //new Notes().SaveNote(new Note
                            //{
                            //    CompanyId = company.CompanyId,
                            //    CompanyName = company.CompanyName,
                            //    UpdateUserId = userId,
                            //    SubscriberId = company.SubscriberId,
                            //    NoteContent = noteContent
                            //});
                        }
                    }
                }

                // deals
                var deals = context.Deals.Where(t => !t.Deleted && !t.Won && !t.Lost
                                        && !t.SalesStageName.ToLower().Equals("stalled")
                                        && !t.SalesStageName.ToLower().Equals("lost")
                                        && !t.SalesStageName.ToLower().Equals("won")
                                        && t.SubscriberId == company.SubscriberId
                                        && t.CompanyId == company.CompanyId).ToList();
                foreach (var deal in deals)
                {
                    deal.DealOwnerId = userId;
                    deal.SalesRepId = userId;
                    context.SubmitChanges();

                    new Deals().AddDealUser(
                            new LinkUserToDeal
                            {
                                DealId = deal.DealId,
                                DealName = deal.DealName,
                                SubscriberId = deal.SubscriberId,
                                UserId = deal.DealOwnerId,
                                UpdateUserId = deal.UpdateUserId
                            }, deal.SubscriberId);
                }

                // calendar events
                var events = (from t in sharedWriteableContext.Activities
                              where t.CompanyId == company.CompanyId && t.StartDateTime > DateTime.Now && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                              select t).ToList();

                foreach (var e in events)
                {
                    e.OwnerUserId = userId;
                    context.SubmitChanges();
                }

                // tasks
                var tasks = (from t in sharedWriteableContext.Activities
                             where t.CompanyId == company.CompanyId && t.DueDate > DateTime.Now && (t.TaskId > 0 || t.ActivityType == "TASK")
                             select t).ToList();

                foreach (var t in tasks)
                {
                    t.UserId = userId;
                    context.SubmitChanges();
                }
                return true;
            }
            return false;
        }


        // datacenter DB and shared DB
        public bool ReassignCompany(int companyId, int userId, int subscriberId, int assignedBy)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var company = context.Companies.Where(t => t.CompanyId == companyId).FirstOrDefault();
            if (company != null)
            {
                company.CompanyOwnerUserId = userId;
                company.Active = true;
                company.LastActivityDate = DateTime.UtcNow;
                company.LastUpdate = DateTime.UtcNow;
                company.UpdateUserId = assignedBy;
                company.UpdateUserName = new Users().GetUserFullNameById(assignedBy, subscriberId);
                company.AdminOverideActive = true;
                company.AdminOverideActiveDate = DateTime.UtcNow;


                // change company ownerId and admin override, etc...
                context.SubmitChanges();

                var oldlinkUsersCompany = context.LinkUserToCompanies
                        .Where(c => c.CompanyId == company.CompanyId && !c.Deleted).ToList();
                var noteContentCompany = "";
                if (oldlinkUsersCompany.Count > 0)
                {
                    // add a note to the company with old sales team
                    noteContentCompany = "Reassigned on " + DateTime.Now.ToString("dd-MMM-yy")
                                  + ". Old Linked sales team: "
                                  + string.Join(",", oldlinkUsersCompany.Select(t => t.UserName).ToArray());
                    foreach (var oldlinkUser in oldlinkUsersCompany)
                    {
                        oldlinkUser.Deleted = true;
                        oldlinkUser.DeletedUserId = userId;
                        oldlinkUser.DeletedDate = DateTime.UtcNow;
                        context.SubmitChanges();
                    }
                }

                // add link company user - created user
                var linkUserCompany = context.LinkUserToCompanies
                    .FirstOrDefault(c => c.CompanyId == company.CompanyId &&
                                         c.UserId == userId && !c.Deleted);
                if (linkUserCompany == null)
                {
                    var user = context.Users.FirstOrDefault(t => t.UserId == userId &&
                                                                                  t.SubscriberId ==
                                                                                  company.SubscriberId);

                    var companyUser = new LinkUserToCompany();
                    companyUser.UserName = user.FullName;
                    companyUser.SubscriberId = user.SubscriberId;
                    companyUser.UserIdGlobal = user.UserId;
                    companyUser.CreatedUserId = userId;
                    companyUser.CreatedUserName = user.CreatedUserName;
                    companyUser.UpdateUserName = user.UpdateUserName;
                    companyUser.CreatedDate = DateTime.UtcNow;
                    companyUser.LastUpdate = DateTime.UtcNow;
                    companyUser.LinkType = "";
                    companyUser.CompanyName = company.CompanyName;
                    companyUser.CompanyId = company.CompanyId;
                    companyUser.CompanyIdGlobal = company.CompanyIdGlobal;
                    companyUser.UserId = user.UserId;
                    context.LinkUserToCompanies.InsertOnSubmit(companyUser);
                    context.SubmitChanges();
                }

                // add company user
                var sharedWriteableConnnection = LoginUser.GetWritableSharedConnectionForSubscriberId(subscriberId);
                var sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnnection);

                var globalCompany = sharedWriteableContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == company.CompanyId && t.SubscriberId == company.SubscriberId);

                if (globalCompany != null)
                {
                    // update global company
                    globalCompany.Active = company.Active;
                    globalCompany.LastActivityDate = company.LastActivityDate.Value;
                    globalCompany.LastUpdate = company.LastUpdate;
                    globalCompany.UpdateUserId = userId;
                    globalCompany.UpdateUserName = company.UpdateUserName;
                    sharedWriteableContext.SubmitChanges();

                    var oldlinkUsers = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                        .Where(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId && !c.Deleted).ToList();
                    var noteContent = "";
                    if (oldlinkUsers.Count > 0)
                    {
                        // add a note to the company with old sales team
                        noteContent = "Reassigned on " + DateTime.Now.ToString("dd-MMM-yy")
                                      + ". Old Linked sales team: "
                                      + string.Join(",", oldlinkUsers.Select(t => t.GlobalUserName).ToArray());
                        foreach (var oldlinkUser in oldlinkUsers)
                        {
                            oldlinkUser.Deleted = true;
                            oldlinkUser.DeletedBy = userId;
                            oldlinkUser.DeletedDate = DateTime.UtcNow;
                            sharedWriteableContext.SubmitChanges();
                        }
                    }

                    var loginConnnection = LoginUser.GetLoginConnection();
                    var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnnection);
                    var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.UserId == userId && t.SubscriberId == company.SubscriberId);
                    if (globalUser != null)
                    {
                        // update global company owner
                        globalCompany.GlobalCompanyOwnerGlobalUserId = globalUser.GlobalUserId;
                        globalCompany.GlobalCompanyOwnerName = globalUser.FullName;
                        sharedWriteableContext.SubmitChanges();

                        // add link company user - created user
                        var linkUser = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                                                                .FirstOrDefault(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId &&
                                                                                     c.GlobalUserId == globalUser.GlobalUserId && !c.Deleted);
                        if (linkUser == null)
                        {
                            var companyUser = new LinkGlobalCompanyGlobalUser();
                            companyUser.GlobalUserName = globalUser.FullName;
                            companyUser.UserSubscriberId = globalUser.SubscriberId;
                            companyUser.GlobalUserId = globalUser.GlobalUserId;
                            companyUser.CreatedBy = globalUser.GlobalUserId;
                            companyUser.CreatedByName = globalUser.FullName;
                            companyUser.CreatedDate = DateTime.UtcNow;
                            companyUser.LastUpdate = DateTime.UtcNow;
                            companyUser.LinkType = "";
                            companyUser.GlobalCompanyName = globalCompany.CompanyName;
                            companyUser.GlobalCompanyId = globalCompany.GlobalCompanyId;
                            companyUser.CompanySubscriberId = globalCompany.SubscriberId;
                            sharedWriteableContext.LinkGlobalCompanyGlobalUsers.InsertOnSubmit(companyUser);
                            sharedWriteableContext.SubmitChanges();
                        }
                    }

                    // update sales team
                    var usernames = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                                            .Where(t => t.GlobalCompanyId == globalCompany.GlobalCompanyId && !t.Deleted)
                                            .Select(t => t.GlobalUserName).ToList();
                    globalCompany.SalesTeam = string.Join(",", usernames);
                    sharedWriteableContext.SubmitChanges();

                    company.SalesTeam = string.Join(",", usernames);
                    context.SubmitChanges();

                    // create note with old company users
                    if (!string.IsNullOrEmpty(noteContent))
                    {
                        //new Notes().SaveNote(new Note
                        //{
                        //    CompanyId = company.CompanyId,
                        //    CompanyName = company.CompanyName,
                        //    UpdateUserId = assignedBy,
                        //    SubscriberId = company.SubscriberId,
                        //    NoteContent = noteContent
                        //});
                    }
                }

                // re-assign deals
                var deals = context.Deals.Where(t => !t.Deleted && !t.Won && !t.Lost
                                                     && !t.SalesStageName.ToLower().Equals("stalled")
                                                     && !t.SalesStageName.ToLower().Equals("lost")
                                                     && !t.SalesStageName.ToLower().Equals("won")
                                                     && t.SubscriberId == company.SubscriberId
                                                     && t.CompanyId == company.CompanyId).ToList();
                foreach (var deal in deals)
                {
                    deal.DealOwnerId = userId;
                    deal.SalesRepId = userId;
                    context.SubmitChanges();

                    new Deals().AddDealUser(
                        new LinkUserToDeal
                        {
                            DealId = deal.DealId,
                            DealName = deal.DealName,
                            SubscriberId = deal.SubscriberId,
                            UserId = deal.DealOwnerId,
                            UpdateUserId = deal.UpdateUserId
                        }, deal.SubscriberId);

                }

                // re-assign calendar events
                var events = (from t in sharedWriteableContext.Activities
                              where t.CompanyIdGlobal == company.CompanyIdGlobal && t.StartDateTime > DateTime.Now && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                              select t).ToList();

                foreach (var e in events)
                {
                    e.OwnerUserId = userId;
                    context.SubmitChanges();
                }

                // re-assign tasks

                var tasks = (from t in sharedWriteableContext.Activities
                             where t.CompanyId == company.CompanyId && t.DueDate > DateTime.Now && (t.TaskId > 0 || t.ActivityType == "TASK")
                             select t).ToList();


                foreach (var t in tasks)
                {
                    t.UserId = userId;
                    context.SubmitChanges();
                }

                return true;
            }
            return true;
        }

        public bool RequestAccess(int companyId, int userId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var company = context.Companies.Where(t => t.CompanyId == companyId).FirstOrDefault();
            var user = context.Users.Where(x => x.UserId == userId).FirstOrDefault();

            if (user == null)
                return false;

            if (company == null)
                return false;

            return RequestAccess(companyId, userId, company.SubscriberId, user.SubscriberId);
        }

        // datacenter DB and shared DB
        public bool RequestAccess(int companyId, int userId, int companySubscriberId, int userSubscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var company = context.Companies.Where(t => t.CompanyId == companyId).FirstOrDefault();
            if (company != null)
            {
                var sharedWriteableConnnection = LoginUser.GetWritableSharedConnectionForSubscriberId(userSubscriberId);
                var sharedWriteableContext = new DbSharedDataContext(sharedWriteableConnnection);

                var globalCompany = sharedWriteableContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == company.CompanyId
                                                    && t.SubscriberId == company.SubscriberId);
                if (globalCompany != null)
                {
                    // company owner user id
                    var loginConnection = LoginUser.GetConnection();
                    var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);
                    var globalUser = sharedWriteableContext.GlobalUsers.FirstOrDefault(t => t.UserId == company.CompanyOwnerUserId &&
                                                                               t.SubscriberId == company.SubscriberId);

                    var requestedGlobalUser = sharedWriteableContext.GlobalUsers.FirstOrDefault(t => t.UserId == userId &&
                                                                               t.SubscriberId == userSubscriberId);

                    // if null, get the created user id
                    if (globalUser == null)
                    {
                        globalUser = sharedWriteableContext.GlobalUsers.FirstOrDefault(t => t.UserId == company.CreatedUserId &&
                                                                                  t.SubscriberId == company.SubscriberId);
                    }

                    if (globalUser != null)
                    {
                        var linkGlobalUser = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                                            .FirstOrDefault(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId &&
                                                 c.GlobalUserId == globalUser.GlobalUserId && !c.Deleted);
                        if (linkGlobalUser == null)
                        {
                            linkGlobalUser = sharedWriteableContext.LinkGlobalCompanyGlobalUsers
                                           .Where(c => c.GlobalCompanyId == globalCompany.GlobalCompanyId && !c.Deleted)
                                           .OrderBy(t => t.CreatedBy).FirstOrDefault();
                        }
                        if (linkGlobalUser != null && requestedGlobalUser != null)
                        {
                            // send request
                            var req = new GlobalCompanyAccessRequest
                            {
                                Guid = Guid.NewGuid().ToString(),
                                CompanyOwnerId = linkGlobalUser.GlobalUserId,
                                GlobalCompanyId = globalCompany.GlobalCompanyId,
                                RequestedGlobalUserId = requestedGlobalUser.GlobalUserId
                            };

                            // set the deal detail URL
                            var companyAccessRequestUrl = GetCompanyAccessRequestUrl(globalUser.SubscriberId, req.Guid);

                            var emailTemplate = "";
                            using (WebClient client = new WebClient())
                            {
                                string path = HttpContext.Current.Server.MapPath("~/_email_templates/company-access-request.html");
                                emailTemplate = client.DownloadString(path);
                            }

                            // to first name - User in CRM LinkUserToDeal table
                            var nameArray = (globalUser.FullName + "").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            var firstName = "";
                            if (nameArray.Length > 0)
                                firstName = nameArray[0];

                            emailTemplate = emailTemplate.Replace("#companyowner#", firstName);

                            // set the body
                            emailTemplate = emailTemplate.Replace("#RequestedUser#", requestedGlobalUser.FullName);
                            emailTemplate = emailTemplate.Replace("#companyname#", globalCompany.CompanyName);
                            emailTemplate = emailTemplate.Replace("#AddToSalesTeamUrl#", companyAccessRequestUrl);

                            // set the  content
                            var content = "";
                            if (!string.IsNullOrEmpty(requestedGlobalUser.FullName))
                                content += "<p style=\"margin: 0; font-size: 14px;\">Requested By: <strong>" + requestedGlobalUser.FullName + "</strong></p>";

                            if (!string.IsNullOrEmpty(requestedGlobalUser.Title))
                                content += "<p style=\"margin: 0; font-size: 14px;\">Job Title: <strong>" + requestedGlobalUser.Title + "</strong></p>";

                            if (!string.IsNullOrEmpty(requestedGlobalUser.EmailAddress))
                                content += "<p style=\"margin: 0; font-size: 14px;\">Email: <strong>" + requestedGlobalUser.EmailAddress + "</strong></p>";

                            if (!string.IsNullOrEmpty(requestedGlobalUser.MobilePhone))
                                content += "<p style=\"margin: 0; font-size: 14px;\">Mobile: <strong>" + requestedGlobalUser.MobilePhone + "</strong></p>";

                            emailTemplate = emailTemplate.Replace("#content#", content);

                            var body = emailTemplate;
                            var CRM_AdminEmailSender =
                                new Recipient
                                {
                                    EmailAddress = "admin@firstfreight.com",
                                    Name = "First Freight CRM"
                                };
                            var request = new SendEmailRequest
                            {
                                Sender = CRM_AdminEmailSender,
                                Subject = "Company Access Request - " + globalCompany.CompanyName,
                                HtmlBody = body,
                                OtherRecipients = new List<Recipient>
                                                        {
                                                            new Recipient{EmailAddress =globalUser.EmailAddress, Name = globalUser.FullName,UserId = globalUser.UserId },
                                                            new Recipient{EmailAddress = "sendgrid@firstfreight.com" }
                                                        }
                            };

                            // set the reply to email address
                            if (requestedGlobalUser != null)
                            {
                                request.ReplyToEmail = requestedGlobalUser.EmailAddress;
                            }

                            new SendGridHelper().SendEmail(request);

                            // TODO: log email sent record

                            // add global company access request record
                            req.RequestedDate = DateTime.Now;
                            sharedWriteableContext.GlobalCompanyAccessRequests.InsertOnSubmit(req);
                            sharedWriteableContext.SubmitChanges();
                        }
                    }
                }
                return true;
            }
            return false;
        }


        // TODO: log email sent record
        public bool SendCompanyInvitation(Company company, LinkGlobalCompanyGlobalUser companyUser)
        {
            var subscriberId = company.SubscriberId;
            if (companyUser.UserSubscriberId > 0)
            {
                subscriberId = companyUser.UserSubscriberId;
            }
            var connection = LoginUser.GetConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(connection);
            var globalUser = loginContext.GlobalUsers.FirstOrDefault(t => t.GlobalUserId == companyUser.GlobalUserId);
            if (globalUser != null)
            {
                // set the company detail URL
                var companyDetailUrl = GetCompanyDetailUrl(company.SubscriberId, company.CompanyId);

                var emailTemplate = "";
                using (WebClient client = new WebClient())
                {
                    string path = HttpContext.Current.Server.MapPath("~/_email_templates/company-invite.html");
                    emailTemplate = client.DownloadString(path);
                }

                // to first name - User in CRM
                var nameArray = (companyUser.GlobalUserName + "").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var firstName = "";
                if (nameArray.Length > 0)
                    firstName = nameArray[0];

                emailTemplate = emailTemplate.Replace("#inviteduser#", firstName);

                // set the body 
                emailTemplate = emailTemplate.Replace("#subscriberlogo#", new Subscribers().GetLogo(company.SubscriberId));
                emailTemplate = emailTemplate.Replace("#CompanyDetailUrl#", companyDetailUrl);
                emailTemplate = emailTemplate.Replace("#companyname#", company.CompanyName);
                emailTemplate = emailTemplate.Replace("#invitedbyuser#", companyUser.CreatedByName);

                // set the  content
                var content = "";
                if (!string.IsNullOrEmpty(company.CompanyName))
                    content += "<p style=\"margin: 0; font-size: 14px;\">Company: <strong>" + company.CompanyName + "</strong></p>";

                if (!string.IsNullOrEmpty(company.City))
                    content += "<p style=\"margin: 0; font-size: 14px;\">City: <strong>" + company.City + "</strong></p>";

                if (!string.IsNullOrEmpty(company.Phone))
                    content += "<p style=\"margin: 0; font-size: 14px;\">Phone: <strong>" + company.Phone + "</strong></p>";

                emailTemplate = emailTemplate.Replace("#content#", content);

                var body = emailTemplate;
                var CRM_AdminEmailSender =
                               new Recipient
                               {
                                   EmailAddress = "admin@firstfreight.com",
                                   Name = "First Freight CRM"
                               };

                var request = new SendEmailRequest
                {
                    Sender = CRM_AdminEmailSender,
                    Subject = "Added to Company Sales Team - " + company.CompanyName,
                    HtmlBody = body,
                    OtherRecipients = new List<Recipient>
                                                        {
                        new Recipient{EmailAddress = globalUser.EmailAddress, Name = globalUser.FullName,UserId = globalUser.UserId },
                        new Recipient{EmailAddress = "charles@firstfreight.com" },
                        new Recipient{EmailAddress = "sendgrid@firstfreight.com" }
                    }
                };

                // set the reply to email address
                var sendByEmail = new Users().GetUserEmailById(companyUser.CreatedBy, subscriberId);
                if (!string.IsNullOrEmpty(sendByEmail))
                {
                    request.ReplyToEmail = sendByEmail;
                }

                new SendGridHelper().SendEmail(request);
                return true;
            }

            return false;
        }


        #region Link Company to Company

        public bool DeleteLinkedCompany(int linkedCompanyId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var company = context.LinkCompanyToCompanies.FirstOrDefault(c => c.LinkCompanyToCompanyId == linkedCompanyId);
            if (company != null)
            {
                company.Deleted = true;
                company.DeletedUserId = userId;
                company.DeletedDate = DateTime.UtcNow;
                company.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public List<LinkCompanyToCompany> GetLinkedCompanies(int companyId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            // get linked compnaies linked both ways
            var companieSet01 = context.LinkCompanyToCompanies.Where(c => (c.CompanyId == companyId)
                                                                          && !c.Deleted).ToList();

            var companieSet02 = context.LinkCompanyToCompanies.Where(c => (c.LinkedCompanyId == companyId)
                                                                          && !c.Deleted).ToList();

            var finalCompanyList = companieSet01;
            foreach (var company in companieSet02)
            {
                finalCompanyList.Add(new LinkCompanyToCompany
                {
                    LinkedCompanyId = company.CompanyId,
                    LinkedCompanyName = company.CompanyName,
                    LinkCompanyToCompanyId = company.LinkCompanyToCompanyId
                });
            }
            return finalCompanyList;
        }


        public bool LinkCompany(LinkCompanyRequest request)
        {
            var subscriberId = request.SubscriberId;
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            // get link company
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var gCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == request.LinkedCompanyId);
            if (gCompany != null)
            {

                var linkedCompany = context.LinkCompanyToCompanies
                                             .FirstOrDefault(c =>
                                            ((c.CompanyId == request.CompanyId && c.LinkedCompanyId == gCompany.CompanyId) ||
                                            (c.CompanyId == gCompany.CompanyId && c.LinkedCompanyId == request.CompanyId))
                                             && c.SubscriberId == subscriberId);
                if (linkedCompany == null)
                {
                    linkedCompany = new LinkCompanyToCompany
                    {
                        SubscriberId = request.SubscriberId,
                        CompanyId = request.CompanyId,
                        CompanyName = GetCompanyNameFromId(request.CompanyId, subscriberId),
                        LinkedCompanyId = gCompany.CompanyId,
                        LinkedCompanyName = gCompany.CompanyName,
                        LastUpdate = DateTime.UtcNow,
                        UpdateUserId = request.UpdateUserId,
                        CreatedUserId = request.UpdateUserId,
                        CreatedDate = DateTime.UtcNow,
                        LinkType = request.LinkType,
                        CreatedUserName = new Users().GetUserFullNameById(request.UpdateUserId, subscriberId),
                        UpdateUserName = new Users().GetUserFullNameById(request.UpdateUserId, subscriberId),
                        UserId = 0,
                        UserName = ""
                    };
                    context.LinkCompanyToCompanies.InsertOnSubmit(linkedCompany);
                    context.SubmitChanges();
                }
                else
                {
                    linkedCompany.UpdateUserId = request.UpdateUserId;
                    linkedCompany.UpdateUserName = new Users().GetUserFullNameById(request.UpdateUserId, subscriberId);
                    linkedCompany.Deleted = false;
                    linkedCompany.DeletedUserId = 0;
                    linkedCompany.DeletedUserName = "";
                    linkedCompany.DeletedDate = null;
                    context.SubmitChanges();
                }

                // update company last activity date
                UpdateCompanyLastActivityDate(request.CompanyId, subscriberId);
                UpdateCompanyLastActivityDate(request.LinkedCompanyId, subscriberId);

                // Log Event 
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = linkedCompany.CreatedUserId,
                    CompanyId = linkedCompany.CompanyId,
                    CompanyName = linkedCompany.CompanyName,
                    UserActivityMessage = "Linked Company: " + linkedCompany.CompanyName + " to " + linkedCompany.LinkedCompanyName
                });
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion


        public UserModel GetCompanyOwner(int companyId, int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var company = context.Companies.Where(t => t.CompanyId == companyId
                                                       && t.SubscriberId == subscriberId).FirstOrDefault();
            if (company != null && company.CompanyOwnerUserId > 0)
            {
                var user = context.Users.Where(u => u.UserId == company.CompanyOwnerUserId)
                                           .Select(u => new UserModel
                                           {
                                               User = u,
                                           }).FirstOrDefault();
                if (user != null)
                {
                    var docs = new Documents().GetDocumentsByDocType(1, user.User.UserId);
                    if (docs.Count > 0)
                    {
                        user.ProfilePicture = docs.FirstOrDefault();
                    }
                }
                return user;
            }
            return null;

        }


        #region Admin Update Routines

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

        #endregion


    }


    public class AddCompanyUserRequest
    {
        public int CompanyId { get; set; }
        public int CompanyIdGlobal { get; set; }
        public int CompanySubscriberId { get; set; }
        public int GlobalUserId { get; set; }
        public string LinkType { get; set; }
        public string SalesTeamRole { get; set; }
        public int UpdatedBy { get; set; }
        public int UserId { get; set; }
        public int UserSubscriberId { get; set; }
    }

    public class CompanySalesTeamMember
    {
        public User User { get; set; }
        public DocumentModel ProfilePicture { get; set; }
        public LinkGlobalCompanyGlobalUser LinkGlobalCompanyGlobalUser { get; set; }
    }

}
