using Models;
using System.Collections.Generic;
using System.Linq;
using System;
using Crm6.App_Code;

namespace Helpers
{
    public class AutoCompletes
    {


        public List<AutoComplete> GetAutoComplete(AutoCompleteFilter filters)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var result = new List<AutoComplete>();
            var prefix = (filters.Prefix + "").ToLower();

            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);

            var requestedDataTypes = filters.Type.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var dataType in requestedDataTypes)
            {
                var sharedConnection = LoginUser.GetSharedConnection();
                var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);

                // filter autocomplete based on type
                switch (dataType.Trim().ToLower())
                {
                    case "company":
                        result.AddRange(sharedContext.GlobalCompanies.Where(t => t.SubscriberId == filters.SusbcriberId &&
                                                       !filters.SelectedIds.Contains(t.GlobalCompanyId) &&
                                                       (filters.GlobalCompanyId == 0 || t.GlobalCompanyId == filters.GlobalCompanyId) &&
                                                       (t.CompanyName.ToLower().StartsWith(prefix) ||
                                                       (t.City != null && t.City.ToLower().StartsWith(prefix)) ||
                                                       (t.PostalCode != null && t.PostalCode.ToLower().StartsWith(prefix)) ||
                                                       (t.Address != null && t.Address.ToLower().StartsWith(prefix))
                                                       )
                                                       && !t.Deleted)
                                                        .Select(t => new AutoComplete
                                                        {
                                                            id = t.GlobalCompanyId,
                                                            name = t.CompanyName + (t.City != null ? " - " + t.City : ""),
                                                            dataObj = t,
                                                            type = "company"
                                                        }).Take(25).ToList());
                        break;
                    case "contact":

                        result.AddRange(context.Contacts.Where(t =>   !filters.SelectedIds.Contains(t.ContactId) &&
                                                     t.ContactName.ToLower().StartsWith(prefix) &&
                                                     (filters.GlobalCompanyId == 0 || t.CompanyIdGlobal == filters.GlobalCompanyId)
                                                     && !t.Deleted)
                                                     .Select(t => new AutoComplete
                                                     {
                                                         id = t.ContactId,
                                                         name = t.ContactName,
                                                         dataObj = t,
                                                         type = "contact"
                                                     }).Take(25).ToList());
                        break;
                    case "companycontacts":

                        result.AddRange(context.Contacts.Where(t => t.SubscriberId == filters.SusbcriberId &&
                                                     !filters.SelectedIds.Contains(t.ContactId) &&
                                                     t.ContactName.ToLower().StartsWith(prefix) &&
                                                     (t.CompanyIdGlobal == filters.GlobalCompanyId)
                                                     && !t.Deleted)
                                                     .Select(t => new AutoComplete
                                                     {
                                                         id = t.ContactId,
                                                         name = t.ContactName,
                                                         dataObj = t,
                                                         type = "contact"
                                                     }).Take(25).ToList());
                        break;
                    case "globalcompanycontact":
                        var linkedSubscriberIds = sharedContext.LinkGlobalSuscriberToSubscribers
                            .Where(t => t.GlobalSubscriberId == filters.SusbcriberId)
                            .Select(t => t.LinkedSubscriberId).ToList();
                        linkedSubscriberIds.Add(filters.SusbcriberId);
                        linkedSubscriberIds = linkedSubscriberIds.Distinct().ToList();
                        result.AddRange(context.Contacts.Where(t => linkedSubscriberIds.Contains(t.SubscriberId) &&
                            !t.Deleted &&
                            !filters.SelectedIds.Contains(t.ContactId) &&
                            t.ContactName.ToLower().StartsWith(prefix) &&
                            (filters.GlobalCompanyId == 0 || t.CompanyIdGlobal == filters.GlobalCompanyId))
                            .Select(t => new AutoComplete
                            {
                                id = t.ContactId,
                                name = t.ContactName,
                                dataObj = t,
                                type = "contact"
                            }).ToList());
                        break;
                    case "user":
                        result.AddRange(context.Users.Where(t => t.SubscriberId == filters.SusbcriberId &&
                                                       !filters.SelectedIds.Contains(t.UserId) &&
                                                       t.FullName.ToLower().StartsWith(prefix) && !t.Deleted)
                                                       .Select(t => new AutoComplete
                                                       {
                                                           id = t.UserId,
                                                           name = t.FullName,
                                                           dataObj = t,
                                                           type = "user"
                                                       }).Take(25).ToList());
                        break;
                    case "deal":
                        result.AddRange(context.Deals.Where(t => t.SubscriberId == filters.SusbcriberId &&
                                                    !filters.SelectedIds.Contains(t.DealId) &&
                                                    t.DealName.ToLower().StartsWith(prefix) &&
                                                    (t.CompanyIdGlobal == filters.GlobalCompanyId) &&
                                                    (filters.PrimaryContactId == 0 || t.PrimaryContactId == filters.PrimaryContactId)
                                                    && !t.Deleted)
                                                    .Select(t => new AutoComplete
                                                    {
                                                        id = t.DealId,
                                                        name = t.DealName,
                                                        dataObj = t,
                                                        type = "deal"
                                                    }).OrderBy(t => t.name).Take(25).ToList());
                        break;
                    case "companytype":
                        result.AddRange(context.CompanyTypes.Where(t => t.SubscriberId == filters.SusbcriberId &&
                                                    !filters.SelectedIds.Contains(t.CompanyTypeId) &&
                                                    (prefix == "" || t.CompanyTypeName.ToLower().StartsWith(prefix))
                                                     && !t.Deleted)
                                                    .Select(t => new AutoComplete
                                                    {
                                                        id = t.CompanyTypeId,
                                                        name = t.CompanyTypeName,
                                                        dataObj = t,
                                                        type = "companytype"
                                                    }).Take(25).ToList());
                        break;
                    case "industry":
                        result.AddRange(context.Industries.Where(t => t.SubscriberId == filters.SusbcriberId &&
                                                    !filters.SelectedIds.Contains(t.IndustryId) &&
                                                    (prefix == "" || t.IndustryName.ToLower().StartsWith(prefix))
                                                     && !t.Deleted)
                                                    .Select(t => new AutoComplete
                                                    {
                                                        id = t.IndustryId,
                                                        name = t.IndustryName,
                                                        dataObj = t,
                                                        type = "industry"
                                                    }).Take(25).ToList());
                        break;
                    case "calendarinvite":
                        // users

                        linkedSubscriberIds = sharedContext.LinkGlobalSuscriberToSubscribers
                                                 .Where(t => t.GlobalSubscriberId == filters.SusbcriberId)
                                                         .Select(t => t.LinkedSubscriberId).ToList();
                        linkedSubscriberIds.Add(filters.SusbcriberId);
                        linkedSubscriberIds = linkedSubscriberIds.Distinct().ToList();
                        result.AddRange(loginContext.GlobalUsers.Where(t => linkedSubscriberIds.Contains(t.SubscriberId) &&
                                                        !filters.SelectedIds.Contains(t.GlobalUserId) &&
                                                        t.FullName.ToLower().StartsWith(prefix))
                                                        .Select(t => new AutoComplete
                                                        {
                                                            id = t.GlobalUserId,
                                                            name = t.FullName,
                                                            dataObj = t,
                                                            type = "user"
                                                        }).Take(10).ToList());
                        // contacts
                        if (filters.GlobalCompanyId > 0)
                            result.AddRange(context.Contacts.Where(t => t.SubscriberId == filters.SusbcriberId && t.CompanyIdGlobal == filters.GlobalCompanyId &&
                                                         !filters.SelectedIds.Contains(t.ContactId) &&
                                                         t.ContactName.ToLower().StartsWith(prefix)
                                                         && !t.Deleted)
                                                         .Select(t => new AutoComplete
                                                         {
                                                             id = t.ContactId,
                                                             name = t.ContactName,
                                                             dataObj = t,
                                                             type = "contact"
                                                         }).Take(10).ToList());

                        result = result.OrderBy(t => t.name).ToList();
                        if (result.Count == 0)
                        {
                            result.Add(new AutoComplete
                            {
                                id = 0,
                                name = filters.Prefix,
                                dataObj = null,
                                type = "external"
                            });
                        }
                        break;
                    case "alluser":
                        // get all subscriber ids
                        linkedSubscriberIds = sharedContext.LinkGlobalSuscriberToSubscribers
                                                  .Where(t => t.GlobalSubscriberId == filters.SusbcriberId)
                                                          .Select(t => t.LinkedSubscriberId).ToList();
                        linkedSubscriberIds.Add(filters.SusbcriberId);
                        linkedSubscriberIds = linkedSubscriberIds.Distinct().ToList();
                        result.AddRange(loginContext.GlobalUsers.Where(t => linkedSubscriberIds.Contains(t.SubscriberId) &&
                                                     t.FullName.ToLower().StartsWith(prefix))
                                          .Select(t => new AutoComplete
                                          {
                                              id = t.UserId,
                                              name = t.FullName + " - " + t.LocationName,
                                              dataObj = t,
                                              type = "alluser"
                                          }).ToList());
                        break;
                    case "alluserwithglobaluserid":

                        // get all subscriber ids
                        linkedSubscriberIds = sharedContext.LinkGlobalSuscriberToSubscribers
                                                  .Where(t => t.GlobalSubscriberId == filters.SusbcriberId)
                                                          .Select(t => t.LinkedSubscriberId).ToList();
                        linkedSubscriberIds.Add(filters.SusbcriberId);
                        linkedSubscriberIds = linkedSubscriberIds.Distinct().ToList();
                        result.AddRange(loginContext.GlobalUsers.Where(t => linkedSubscriberIds.Contains(t.SubscriberId) &&
                                                     t.FullName.ToLower().StartsWith(prefix))
                                          .Select(t => new AutoComplete
                                          {
                                              id = t.GlobalUserId,
                                              name = t.FullName + " - " + t.LocationName,
                                              dataObj = t,
                                              type = "alluser"
                                          }).ToList());
                        break;
                    case "globalcompanywithpermission":

                        // data center user object
                        var user = context.Users.FirstOrDefault(u => u.UserId == filters.UserId
                                                                         && u.SubscriberId == filters.SusbcriberId);
                        // global user object
                        var globalUser = loginContext.GlobalUsers.FirstOrDefault(u => u.UserId == filters.UserId
                                                                                      && u.SubscriberId == filters.SusbcriberId);

                        var companies = sharedContext.GlobalCompanies.Where(c => !c.Deleted);

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
                                                                              && t.SubscriberId == filters.SusbcriberId)
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
                                                                                  && t.SubscriberId == filters.SusbcriberId)
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
                                                                                  && t.SubscriberId == filters.SusbcriberId)
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
                                                                                      && t.SubscriberId == filters.SusbcriberId)
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
                                        var location = new Locations().GetLocation(globalUser.LocationId, filters.SusbcriberId);
                                        if (location != null)
                                        {
                                            // this user is a location manager, get all the companies this user and his location users linked to 
                                            var uIds = context.Users.Where(u => u.LocationCode.Equals(location.LocationCode)
                                                                     && u.SubscriberId == globalUser.SubscriberId)
                                                                    .Select(u => u.UserId)
                                                                    .ToList();
                                            // get global user Ids
                                            userIds.AddRange(loginContext.GlobalUsers.Where(t => uIds.Contains(t.UserId)
                                                                                      && t.SubscriberId == filters.SusbcriberId)
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
                            var allCompanies = otherCompanies.Select(t => t.company).Distinct();

                            // keyword filter
                            return allCompanies.Where(t => t.CompanyName.ToLower().StartsWith(prefix)).OrderBy(t => t.CompanyName).Select(t => new AutoComplete
                            {
                                id = t.GlobalCompanyId,
                                name = t.CompanyName + (t.City != null ? " - " + t.City : ""),
                                dataObj = t,
                                type = "company"
                            }).Take(25).ToList();
                        }

                        break;
                    case "globalcompanydealswithpermission":

                        var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.GlobalCompanyId == filters.GlobalCompanyId);
                        if (globalCompany != null)
                        {
                            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                   .GlobalSubscribers.Where(t => t.SubscriberId == globalCompany.SubscriberId)
                                                                   .Select(t => t.DataCenter).FirstOrDefault();
                            connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
                            context = new DbFirstFreightDataContext(connection);

                            // get deals
                            var deals = (from deal in context.Deals
                                         join owner in context.Users on deal.DealOwnerId equals owner.UserId
                                         where !deal.Deleted && !owner.Deleted && deal.CompanyIdGlobal == filters.GlobalCompanyId
                                         select deal);

                            // linked deals
                            var dealIds = (from t in deals
                                           join j in context.LinkUserToDeals on t.DealId equals j.DealId
                                           where j.UserId == filters.UserId && !j.Deleted && t.SubscriberId == filters.SusbcriberId && t.CompanyIdGlobal == filters.GlobalCompanyId
                                           select t.DealId).ToList();

                            user = context.Users.FirstOrDefault(t => t.UserId == filters.UserId);
                            if (user != null)
                            {
                                var locationCodes = new List<string>();
                                // get sales manager user's location codes
                                if (user.UserRoles.Contains("Sales Manager"))
                                {
                                    var userIds = (from t in context.LinkUserToManagers
                                                   join j in context.Users on t.UserId equals j.UserId
                                                   where t.ManagerUserId == filters.UserId && !t.Deleted && !j.Deleted && t.SubscriberId == filters.SusbcriberId
                                                   select t.UserId).Distinct().ToList();

                                    dealIds.AddRange((from t in deals
                                                      join j in context.LinkUserToDeals on t.DealId equals j.DealId
                                                      where userIds.Contains(j.UserId) && !j.Deleted && t.SubscriberId == filters.SusbcriberId && t.CompanyIdGlobal == filters.GlobalCompanyId
                                                      select t.DealId).ToList());
                                }

                                if (!string.IsNullOrEmpty(user.UserRoles))
                                {
                                    if (user.UserRoles.Contains("CRM Admin"))
                                    {
                                        // don't do anything
                                    }
                                    else if (user.UserRoles.Contains("Region Manager"))
                                    {
                                        if (!string.IsNullOrEmpty(user.RegionName))
                                        {
                                            // this user is a region manager, get all the deals for the region
                                            locationCodes.AddRange(context.Locations
                                                                     .Where(t => t.RegionName == user.RegionName && t.LocationCode != ""
                                                                      && t.SubscriberId == filters.SusbcriberId)
                                                                     .Select(t => t.LocationCode).ToList());

                                            deals = deals.Where(t => locationCodes.Contains(t.LocationCode) || dealIds.Contains(t.DealId));
                                        }
                                    }
                                    else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                                    {
                                        if (!string.IsNullOrEmpty(user.CountryCode))
                                        {
                                            // this user is a country manager, get all the deals for the country
                                            locationCodes.AddRange(context.Locations
                                                                       .Where(t => t.CountryCode == user.CountryCode && t.LocationCode != ""
                                                                        && t.SubscriberId == filters.SusbcriberId)
                                                                           .Select(t => t.LocationCode).ToList());
                                            deals = deals.Where(t => locationCodes.Contains(t.LocationCode) || dealIds.Contains(t.DealId));
                                        }
                                    }
                                    else if (user.UserRoles.Contains("District Manager"))
                                    {
                                        if (!string.IsNullOrEmpty(user.DistrictCode))
                                        {
                                            var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                                            if (district != null)
                                            {
                                                // this user is a district manager, get all the deals for the district
                                                locationCodes.AddRange(context.Locations
                                                                           .Where(t => t.DistrictCode == district.DistrictCode && t.LocationCode != ""
                                                                           && t.SubscriberId == filters.SusbcriberId)
                                                                           .Select(t => t.LocationCode).ToList());
                                                deals = deals.Where(t => locationCodes.Contains(t.LocationCode) || dealIds.Contains(t.DealId));
                                            }
                                        }
                                    }
                                    else if (user.UserRoles.Contains("Location Manager"))
                                    {
                                        if (user.LocationId > 0)
                                        {
                                            var location = new Locations().GetLocation(user.LocationId, user.SubscriberId);
                                            if (location != null)
                                            {
                                                // this user is a location manager, get all the deals for the location
                                                locationCodes.Add(location.LocationCode);
                                                deals = deals.Where(t => locationCodes.Contains(t.LocationCode) || dealIds.Contains(t.DealId));
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // get deals for linked users
                                        deals = deals.Where(t => dealIds.Contains(t.DealId) || locationCodes.Contains(t.LocationCode));
                                    }
                                }
                            }

                            // keyword filter
                            return deals.Where(t => t.DealName.ToLower().StartsWith(prefix)).OrderBy(t => t.DealName).Select(t => new AutoComplete
                            {
                                id = t.DealId,
                                name = t.DealName,
                                dataObj = t,
                                type = "deal"
                            }).Take(25).ToList();
                        }
                        return new List<AutoComplete>();
                    default:
                        break;
                }
            }

            return result;
        }


        public AutoComplete GetDealCompany(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var company = (from t in context.Deals
                           join j in context.Companies on t.CompanyId equals j.CompanyId
                           where t.DealId == dealId
                           select new AutoComplete
                           {
                               id = j.CompanyId,
                               name = j.CompanyName + (j.City != null ? " - " + j.City : ""),
                               dataObj = j,
                               type = "company"
                           }).FirstOrDefault();
            return company;
        }


    }
}
