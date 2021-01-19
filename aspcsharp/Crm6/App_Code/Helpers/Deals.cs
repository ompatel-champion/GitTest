using Crm6.App_Code;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Login;
using Crm6.App_Code.Shared;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Helpers
{
    public class Deals
    {

        public Crm6.App_Code.Deal GetDeal(int dealId, int dealSubscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == dealSubscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Deals.FirstOrDefault(t => t.DealId == dealId);
        }


        public DealListResponse GetDeals(DealFilters filters)
        {
            var response = new DealListResponse
            {
                Deals = new List<DealExtended>()
            };

            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == filters.SubscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            // var connection = LoginUser.GetConnection();

            var context = new DbFirstFreightDataContext(connection);

            // get deals
            var deals = (from deal in context.Deals
                         join owner in context.Users on deal.DealOwnerId equals owner.UserId
                         where !deal.Deleted && !owner.Deleted
                         select deal);

            // apply filters
            if (filters.SubscriberId > 0)
                deals = deals.Where(t => t.SubscriberId == filters.SubscriberId && t.CompanyId > 0);
            if (!string.IsNullOrWhiteSpace(filters.CountryName))
                deals = deals.Where(t => t.CountryName.ToLower() == filters.CountryName.ToLower());
            if (filters.CompanyId > 0)
                deals = deals.Where(t => t.CompanyId == filters.CompanyId);
            if (!string.IsNullOrWhiteSpace(filters.Location))
                deals = deals.Where(t => t.LocationName.ToLower() == filters.Location.ToLower());

            if (filters.ContactId > 0)
            {
                var contactDealIds = context.LinkContactToDeals
                                            .Where(t => t.ContactId == filters.ContactId && t.SubscriberId == filters.SubscriberId && !t.Deleted)
                                            .Select(t => t.DealId).ToList();
                deals = deals.Where(t => contactDealIds.Contains(t.DealId));
            }


            if (filters.UserId > 0)
            {
                // linked deals
                var dealIds = (from t in deals
                               join j in context.LinkUserToDeals on t.DealId equals j.DealId
                               where j.UserId == filters.UserId && !j.Deleted && t.SubscriberId == filters.SubscriberId
                               select t.DealId).ToList();

                var user = context.Users.FirstOrDefault(t => t.UserId == filters.UserId);
                if (user != null)
                {
                    var locationCodes = new List<string>();
                    // get sales manager user's location codes
                    if (user.UserRoles.Contains("Sales Manager"))
                    {
                        //locationCodes = (from t in context.LinkUserToManagers
                        //                 join j in context.Users on t.UserId equals j.UserId
                        //                 where t.ManagerUserId == filters.UserId && !t.Deleted && !j.Deleted && t.SubscriberId == filters.SubscriberId
                        //                 select j.LocationCode).Distinct().ToList();

                        var userIds = (from t in context.LinkUserToManagers
                                       join j in context.Users on t.UserId equals j.UserId
                                       where t.ManagerUserId == filters.UserId && !t.Deleted && !j.Deleted && t.SubscriberId == filters.SubscriberId
                                       select t.UserId).Distinct().ToList();

                        dealIds.AddRange((from t in deals
                                          join j in context.LinkUserToDeals on t.DealId equals j.DealId
                                          where userIds.Contains(j.UserId) && !j.Deleted && t.SubscriberId == filters.SubscriberId
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
                                                          && t.SubscriberId == filters.SubscriberId)
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
                                                            && t.SubscriberId == filters.SubscriberId)
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
                                                               && t.SubscriberId == filters.SubscriberId)
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
            }


            // sales stages
            if (filters.SalesStages != null && filters.SalesStages.Count > 0)
            {
                if (filters.SalesStages[0] == "All")
                {
                    // don't filter for stage
                }
                else
                {
                    deals = deals.Where(t => filters.SalesStages.Contains(t.SalesStageName.ToString()));
                }
            }
            if (filters.SalesRepId > 0)
                deals = deals.Where(t => filters.SalesRepId == t.SalesRepId);
            if (!string.IsNullOrEmpty(filters.Keyword))
            {
                filters.Keyword = filters.Keyword.ToLower();

                deals = (from t in deals
                         join j in context.LinkUserToDeals on t.DealId equals j.DealId
                         where (j.UserName != null && j.UserName.ToLower().Contains(filters.Keyword)) ||
                                         t.DealName.ToLower().Contains(filters.Keyword)
                                      || t.DealDescription.ToLower().Contains(filters.Keyword)
                                      || t.CompanyName.ToLower().Contains(filters.Keyword)
                                      || t.PrimaryContactName.ToLower().Contains(filters.Keyword)
                                      || t.CountryName.ToLower().Contains(filters.Keyword)
                                      || t.LocationName.ToLower().Contains(filters.Keyword)
                         select t).Distinct();
            }

            // sort
            if (!string.IsNullOrEmpty(filters.SortBy))
            {
                switch (filters.SortBy.ToLower())
                {
                    case "createddate asc":
                        deals = deals.OrderBy(t => t.CreatedDate);
                        break;
                    case "createddate desc":
                        deals = deals.OrderByDescending(t => t.CreatedDate);
                        break;
                    case "dealname asc":
                        deals = deals.OrderBy(t => t.DealName);
                        break;
                    case "dealname desc":
                        deals = deals.OrderByDescending(t => t.DealName);
                        break;
                    case "location asc":
                        deals = deals.OrderBy(t => t.LocationName);
                        break;
                    case "location desc":
                        deals = deals.OrderByDescending(t => t.LocationName);
                        break;
                    case "salesteam asc":
                        deals = deals.OrderBy(t => t.SalesTeam);
                        break;
                    case "salesteam desc":
                        deals = deals.OrderByDescending(t => t.SalesTeam);
                        break;
                    case "companyname asc":
                        deals = deals.OrderBy(t => t.CompanyName);
                        break;
                    case "companyname desc":
                        deals = deals.OrderByDescending(t => t.CompanyName);
                        break;
                    case "salesstagename asc":
                        deals = deals.OrderBy(t => t.SalesStageName);
                        break;
                    case "salesstagename desc":
                        deals = deals.OrderByDescending(t => t.SalesStageName);
                        break;
                    case "decisiondate asc":
                        deals = deals.OrderBy(t => t.DecisionDate);
                        break;
                    case "decisiondate desc":
                        deals = deals.OrderByDescending(t => t.DecisionDate);
                        break;
                    case "lastactivity asc":
                        deals = deals.OrderBy(t => t.LastActivityDate);
                        break;
                    case "lastactivity desc":
                        deals = deals.OrderByDescending(t => t.LastActivityDate);
                        break;
                    case "primarycontactname asc":
                        deals = deals.OrderBy(t => t.PrimaryContactName);
                        break;
                    case "primarycontactname desc":
                        deals = deals.OrderByDescending(t => t.PrimaryContactName);
                        break;
                    case "lastactivitydate asc":
                        deals = deals.OrderBy(t => t.LastActivityDate);
                        break;
                    case "lastactivitydate desc":
                        deals = deals.OrderByDescending(t => t.LastActivityDate);
                        break;
                    default:
                        break;
                }
            }

            // record count/ total pages 
            var recordCount = deals.Count();
            var totalPages = 0;

            // apply paging
            if (filters.RecordsPerPage > 0 && filters.CurrentPage > 0)
            {
                deals = deals.Skip((filters.CurrentPage - 1) * filters.RecordsPerPage)
                             .Take(filters.RecordsPerPage);
                totalPages = recordCount % filters.RecordsPerPage == 0 ?
                                    (recordCount / filters.RecordsPerPage) :
                                  ((recordCount / filters.RecordsPerPage) + 1);
            }

            var finalList = new List<DealExtended>();

            foreach (var deal in deals.ToList())
            {
                finalList.Add(new DealExtended()
                {
                    DealId = deal.DealId,
                    SubscriberId = deal.SubscriberId,
                    Campaign = deal.Campaign,
                    CompanyId = deal.CompanyId,
                    CompanyName = deal.CompanyName,
                    Comments = deal.Comments,
                    Commodities = deal.Commodities,
                    Competitors = deal.Competitors,
                    ContractEndDate = deal.ContractEndDate,
                    ContractStartDate = deal.ContractStartDate,
                    CountryCode = deal.CountryCode,
                    CountryName = deal.CountryName,
                    CreatedDate = deal.CreatedDate,
                    CreatedUserId = deal.CreatedUserId,
                    CreatedUserName = deal.CreatedUserName,
                    CurrencyCode = deal.CurrencyCode,
                    DateLost = deal.DateLost,
                    DateProposalDue = deal.DateProposalDue,
                    DateWon = deal.DateWon,
                    DealCountryName = deal.DealCountryName,
                    DealDescription = deal.DealDescription,
                    DealName = deal.DealName,
                    DealNumber = deal.DealNumber,
                    DealOwnerId = deal.DealOwnerId,
                    DealRequestType = deal.DealRequestType,
                    DealType = deal.DealType,
                    DecisionDate = deal.DecisionDate,
                    Deleted = deal.Deleted,
                    DeletedDate = deal.DeletedDate,
                    DeletedUserId = deal.DeletedUserId,
                    DeletedUserName = deal.DeletedUserName,
                    DistrictCode = deal.DistrictCode,
                    DistrictName = deal.DistrictName,
                    DealSource = deal.DealSource,
                    EstimatedStartDate = deal.EstimatedStartDate,
                    Incoterms = deal.Incoterms,
                    Industry = deal.Industry,
                    LastActivityDate = deal.LastActivityDate,
                    LastUpdate = deal.LastUpdate,
                    LocationCode = deal.LocationCode,
                    LocationName = deal.LocationName,
                    Lost = deal.Lost,
                    NextActionDate = deal.NextActionDate,
                    NextActionStep = deal.NextActionStep,
                    PrimaryContactId = deal.PrimaryContactId,
                    PrimaryContactName = deal.PrimaryContactName,
                    Products = deal.Products,
                    ProfitPercentage = deal.ProfitPercentage,
                    Ranking = deal.Ranking,
                    ReasonWonLost = deal.ReasonWonLost, 
                    Revenue = deal.Revenue,
                    RegionName = deal.RegionName,
                    SalesRepId = deal.SalesRepId,
                    SalesRepName = deal.SalesRepName,
                    SalesStageId = deal.SalesStageId,
                    SalesStageName = deal.SalesStageName,
                    SalesTeam = deal.SalesTeam,
                    SourceDataCenter = deal.SourceDataCenter,
                    SourceDataCenterDealId = deal.SourceDataCenterDealId,
                    SourceSubscriberId = deal.SourceSubscriberId,
                    Tags = deal.Tags,
                    UpdateUserId = deal.UpdateUserId,
                    UpdateUserName = deal.UpdateUserName,
                    Won = deal.Won,
                    WonExchangeRate = deal.WonExchangeRate,
                    ConversionAccountId = deal.ConversionAccountId,
                    ConversionEntityId = deal.ConversionEntityId,
                    ConversionEntityName = deal.ConversionEntityName,
                    ConversionEntityType = deal.ConversionEntityType,
                    ConversionLeadId = deal.ConversionLeadId,
                    ConversionOpportunityId = deal.ConversionOpportunityId,
                    ConversionOwnerUserId = deal.ConversionOwnerUserId,
                    ConversionPrimaryContactId = deal.ConversionPrimaryContactId,
                    ConversionSalesRepCode = deal.ConversionSalesRepCode,
                    ConversionSalesRepName = deal.ConversionSalesRepName,
                    CBMs = deal.CBMs,
                    Kgs = deal.Kgs,
                    Lbs = deal.Lbs,
                    ProfitUSD = deal.ProfitUSD,
                    RevenueUSD = deal.RevenueUSD,
                    Services = deal.Services,
                    TEUs = deal.TEUs,
                    Tonnes = deal.Tonnes,
                    SpotDeal = deal.SpotDeal,
                    Profit = deal.Profit,
                    CompanyIdGlobal = deal.CompanyIdGlobal,
                    NextActivityDate = deal.NextActivityDate,
                    ProfitUSDSpot = deal.ProfitUSDSpot,
                    RevenueUSDSpot = deal.RevenueUSDSpot,
                    CBMsSpot = deal.CBMsSpot,
                    KgsSpot = deal.KgsSpot,
                    LbsSpot = deal.LbsSpot,
                    TEUsSpot = deal.TEUsSpot,
                    TonnesSpot = deal.TonnesSpot,
                    OrignLocations = deal.OrignLocations,
                    OrignCountries = deal.OrignCountries,
                    DestinationLocations = deal.DestinationLocations,
                    DestinationCountries = deal.DestinationCountries,
                    ShipperNames = deal.ShipperNames,
                    ConsigneeNames = deal.ConsigneeNames
                });
            }

            response.Deals = finalList;

            if (response.Deals.Count > 0)
            {
                response.TotalPages = totalPages;
                response.Records = recordCount;
            }

            for (int i = 0; i < response.Deals.Count; i++)
            {
                DealExtended currentDeal = response.Deals[i];
                var company = context.Companies.Where(x => x.CompanyId == currentDeal.CompanyId).FirstOrDefault();

                if (company != null)
                {
                    currentDeal.CityName = company.City;
                    currentDeal.CountryName = company.CountryName;
                }

                var sharedConnection = LoginUser.GetSharedConnection();
                var sharedWriteableContext = new DbSharedDataContext(sharedConnection);

                var nextEvent = sharedWriteableContext.Activities.Where(x => x.SubscriberId == currentDeal.SubscriberId &&
                    (x.DealIds == currentDeal.DealId.ToString() || x.DealIds.Contains(","+currentDeal.DealId+",") || x.DealIds.StartsWith(currentDeal.DealId+",") || x.DealIds.EndsWith(","+currentDeal.DealId)) &&
                    x.StartDateTime > DateTime.UtcNow).OrderBy(x => x.ActivityDate).FirstOrDefault();

                if (nextEvent != null)
                {
                    currentDeal.ActivityId = nextEvent.ActivityId;
                    currentDeal.ActivityName = nextEvent.Subject;
                    currentDeal.NextActivityDate = nextEvent.ActivityDate;
                }

                currentDeal.SalesTeamMembersExtended = GetDealSalesTeamExtended(currentDeal.DealId);
            }

            // set the return deal list
            return response;
        }



        public string GetDealSalesTeam(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var salesTeamUsers = context.LinkUserToDeals.Where(t => t.DealId == dealId && !t.Deleted)
                                    .Select(t => t.UserName).ToList();
            return string.Join(", ", salesTeamUsers);
        }

        private List<SalesTeamMemberAndRole> GetDealSalesTeamExtended(int dealId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var salesTeamUsers = context.LinkUserToDeals.Where(t => t.DealId == dealId && !t.Deleted).ToList();

            return salesTeamUsers.Select(x => new SalesTeamMemberAndRole() { Role = context.Users.Where(y => y.UserId == x.UserId).FirstOrDefault()?.Title, SalesTeamMember = x.UserName }).ToList();
        }


        public int SaveDeal(SaveDealRequest request)
        {
            try
            {
                var deal = request.Deal;
                // save source deal
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                var sharedConnection = LoginUser.GetSharedConnection();
                var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);

                // check for existing deal
                var objDeal = context.Deals.FirstOrDefault(d => d.DealId == deal.DealId) ?? new Crm6.App_Code.Deal();
                // populate fields
                objDeal.DealName = deal.DealName;
                objDeal.DealDescription = deal.DealDescription;
                objDeal.DealNumber = deal.DealNumber;

                // sales stage
                objDeal.SalesStageName = deal.SalesStageName;
                var stage = context.SalesStages.FirstOrDefault(t => t.SalesStageName.ToLower() == deal.SalesStageName.ToLower()
                                                             && t.SubscriberId == deal.SubscriberId && !t.Deleted);
                if (stage != null)
                {
                    objDeal.SalesStageId = stage.SalesStageId;
                }

                objDeal.CompanyIdGlobal = deal.CompanyIdGlobal;
                // get company
                var gCompany = sharedContext.GlobalCompanies.FirstOrDefault(c => c.GlobalCompanyId == deal.CompanyIdGlobal);
                if (gCompany != null)
                {
                    objDeal.CompanyId = gCompany.CompanyId;
                    objDeal.CompanyName = gCompany.CompanyName;
                }
                objDeal.PrimaryContactId = deal.PrimaryContactId;
                objDeal.PrimaryContactName = context.Contacts.Where(t => t.ContactId == deal.PrimaryContactId).Select(t => t.ContactName).FirstOrDefault() ?? "";
                objDeal.Commodities = deal.Commodities;
                objDeal.Competitors = deal.Competitors;
                objDeal.Comments = deal.Comments;
                objDeal.DealOwnerId = deal.DealOwnerId;
                objDeal.DealType = deal.DealType;
                objDeal.DealRequestType = deal.DealRequestType;
                objDeal.Industry = deal.Industry;
                objDeal.Incoterms = deal.Incoterms;
                objDeal.Campaign = deal.Campaign;

                //  sales rep for deal
                var salesRep = deal.DealOwnerId > 0 ? context.Users.FirstOrDefault(u => u.UserId == deal.DealOwnerId) : null;
                if (salesRep != null)
                {
                    objDeal.SalesRepId = objDeal.DealOwnerId;
                    objDeal.SalesRepName = salesRep.FullName;
                    // location
                    var salesRepLocation = context.Locations.FirstOrDefault(a => a.LocationId == salesRep.LocationId);
                    if (salesRepLocation != null)
                    {
                        objDeal.CountryCode = salesRepLocation.CountryCode;
                        objDeal.CountryName = salesRepLocation.CountryName;
                        objDeal.DistrictCode = salesRepLocation.DistrictCode;
                        objDeal.DistrictName = salesRepLocation.DistrictName;
                        objDeal.LocationCode = salesRepLocation.LocationCode;
                        objDeal.LocationName = salesRepLocation.LocationName; 
                        objDeal.RegionName = salesRepLocation.RegionName;
                    }
                }

                // dates
                objDeal.ContractEndDate = deal.ContractEndDate;
                objDeal.DateProposalDue = deal.DateProposalDue;
                objDeal.DecisionDate = deal.DecisionDate;
                objDeal.EstimatedStartDate = deal.EstimatedStartDate;

                // last update
                objDeal.LastUpdate = DateTime.UtcNow;
                objDeal.UpdateUserId = deal.UpdateUserId;
                objDeal.UpdateUserName = context.Users.Where(u => u.UserId == deal.UpdateUserId)
                                             .Select(u => u.FullName).FirstOrDefault() ?? "";

                // won reason
                if (deal.SalesStageName == "Won")
                {
                    objDeal.SalesStageName = "Won";
                    objDeal.Won = true;
                    objDeal.Lost = false;
                    if (objDeal.DateWon == null)
                        objDeal.DateWon = DateTime.UtcNow;
                    objDeal.ReasonWonLost = deal.ReasonWonLost;

                }
                // lost reason
                else if (deal.SalesStageName == "Lost")
                {
                    objDeal.SalesStageName = "Lost";
                    objDeal.Lost = true;
                    objDeal.Won = false;
                    if (objDeal.DateLost == null)
                        objDeal.DateLost = DateTime.UtcNow;
                    objDeal.ReasonWonLost = deal.ReasonWonLost;
                }
                else
                {
                    objDeal.Lost = false;
                    objDeal.Won = false;
                    objDeal.DateLost = null;
                    objDeal.DateWon = null;
                    objDeal.ReasonWonLost = "";
                }

                // insert new deal
                if (objDeal.DealId < 1)
                {
                    objDeal.SubscriberId = deal.SubscriberId;
                    objDeal.CreatedUserId = deal.UpdateUserId;
                    objDeal.CreatedDate = DateTime.UtcNow;
                    objDeal.CreatedUserName = objDeal.UpdateUserName;
                    objDeal.DealOwnerId = deal.UpdateUserId;
                    objDeal.SalesTeam = deal.CompanyName;
                    context.Deals.InsertOnSubmit(objDeal);
                }


                context.SubmitChanges();

                // add deal owner | Only add for the new deals
                if (deal.DealId < 1 && objDeal.DealOwnerId > 0)
                {
                    AddDealUser(new LinkUserToDeal
                    {
                        DealId = objDeal.DealId,
                        DealName = objDeal.DealName,
                        SubscriberId = objDeal.SubscriberId,
                        UserId = objDeal.DealOwnerId,
                        UpdateUserId = objDeal.UpdateUserId
                    }, deal.SubscriberId);
                }

                // deal dates
                try
                {
                    if (deal.ContractEndDate != null)
                        UpdateDealDate(objDeal.DealId, deal.ContractEndDate.GetValueOrDefault(), "contract-end-date", deal.UpdateUserId, deal.SubscriberId);
                    if (deal.DateProposalDue != null)
                        UpdateDealDate(objDeal.DealId, deal.DateProposalDue.GetValueOrDefault(), "proposal-date", deal.UpdateUserId, deal.SubscriberId);
                    if (deal.DecisionDate != null)
                        UpdateDealDate(objDeal.DealId, deal.DecisionDate.GetValueOrDefault(), "decision-date", deal.UpdateUserId, deal.SubscriberId);
                    if (deal.EstimatedStartDate != null)
                        UpdateDealDate(objDeal.DealId, deal.EstimatedStartDate.GetValueOrDefault(), "first-shipment-date", deal.UpdateUserId, deal.SubscriberId);

                }
                catch (Exception) { }

                // add primary contact as deal contact 
                AddDealContact(new LinkContactToDeal
                {
                    ContactId = objDeal.PrimaryContactId,
                    ContactName = objDeal.PrimaryContactName,
                    DealId = objDeal.DealId,
                    DealName = objDeal.DealName,
                    SubscriberId = objDeal.SubscriberId,
                    UpdateUserId = objDeal.UpdateUserId,
                    UpdateUserName = objDeal.UpdateUserName
                });

                // log event
                new Logging().LogUserAction(new UserActivity
                {
                    UserId = deal.UpdateUserId,
                    DealId = objDeal.DealId,
                    DealName = deal.DealName,
                    SubscriberId = deal.SubscriberId,
                    UserActivityMessage = "Saved Deal: " + deal.DealName
                });

                //DONE: intercom Journey Step event
                var eventName = "Saved deal";
                var intercomHelper = new IntercomHelper();
                intercomHelper.IntercomTrackEvent(deal.UpdateUserId, deal.SubscriberId, eventName);

                // update company last activity date
                var company = context.Companies.FirstOrDefault(t => t.CompanyIdGlobal == deal.CompanyIdGlobal);

                // cyclomatic complexity sux.  we should invert all these ifs
                if (company != null)
                {
                    if (objDeal.Won)
                    {
                        company.IsCustomer = true;
                    }
                    company.LastActivityDate = DateTime.UtcNow;
                    context.SubmitChanges();

                    sharedConnection = LoginUser.GetWritableSharedConnectionForSubscriberId(deal.SubscriberId);
                    var sharedWriteableContext = new DbSharedDataContext(sharedConnection);
                    var globalCompany = sharedWriteableContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == company.CompanyId && t.SubscriberId == company.SubscriberId);
                    if (globalCompany != null)
                    {
                        company.LastActivityDate = DateTime.UtcNow;
                        if (objDeal.Won)
                        {
                            company.IsCustomer = true;
                            sharedWriteableContext.SubmitChanges();
                        }
                    }
                }

                // check if there is any external users has access to this deal, if yes update the deals for them
                var externalUsers = context.LinkUserToDeals.Where(t => t.DealId == deal.DealId &&
                                                !t.Deleted && t.SubscriberId == deal.SubscriberId && t.UserSubscriberId > 0 &&
                                                t.UserSubscriberId != deal.SubscriberId);
                foreach (var user in externalUsers)
                {
                    SaveTargetDeal(objDeal, user.UserSubscriberId);
                }

                // return deal Id
                return objDeal.DealId;
            }

            catch (Exception ex)
            {
                var error = new WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "SaveDeal",
                    PageCalledFrom = "Helper/Deals",
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = request.Deal.UpdateUserId
                };
                new Logging().LogWebAppError(error);
            }
            return 0;
        }


        public bool MarkAsWonLost(Crm6.App_Code.Deal wonLostRequest, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var deal = context.Deals.FirstOrDefault(d => d.DealId == wonLostRequest.DealId);
            if (deal != null)
            {
                if (wonLostRequest.Won)
                {
                    deal.SalesStageName = "Won";
                    deal.Won = true;
                    deal.DateWon = DateTime.UtcNow;
                    deal.ReasonWonLost = wonLostRequest.ReasonWonLost;
                }
                else if (wonLostRequest.Lost)
                {
                    deal.SalesStageName = "Lost";
                    deal.Lost = true;
                    deal.DateLost = DateTime.UtcNow;
                    deal.ReasonWonLost = wonLostRequest.ReasonWonLost;
                }
                deal.LastUpdate = DateTime.UtcNow;
                deal.UpdateUserId = wonLostRequest.UpdateUserId;
                context.SubmitChanges();

                if (wonLostRequest.Won)
                {
                    var company = context.Companies.Where(t => t.CompanyId == deal.CompanyId).FirstOrDefault();
                    if (company != null)
                    {
                        company.IsCustomer = true;
                        context.SubmitChanges();
                        var sharedConnection = LoginUser.GetSharedConnection();
                        var sharedContext = new DbSharedDataContext(sharedConnection);
                        var globalCompany = sharedContext.GlobalCompanies.FirstOrDefault(t => t.CompanyId == company.CompanyId && t.SubscriberId == company.SubscriberId);
                        if (globalCompany != null)
                        {
                            company.IsCustomer = true;
                            sharedContext.SubmitChanges();
                        }
                    }
                }

                return true;
            }
            return false;
        }


        public bool UpdateDealSalesStage(int dealId, string salesStage, int userId, int subscriberId)
        {
            try
            {
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                var deal = context.Deals.FirstOrDefault(d => d.DealId == dealId);
                if (deal != null)
                {
                    var username = new Users().GetUserFullNameById(userId, subscriberId);
                    var stage = context.SalesStages.FirstOrDefault(t => t.SalesStageName.ToLower() == salesStage.ToLower()
                                                               && t.SubscriberId == deal.SubscriberId && !t.Deleted);
                    if (stage != null)
                    {
                        // add new sales history
                        deal.SalesStageId = stage.SalesStageId;
                        deal.SalesStageName = stage.SalesStageName;
                        deal.LastUpdate = DateTime.UtcNow;
                        deal.UpdateUserName = username;
                        deal.UpdateUserId = userId;
                        context.SubmitChanges();

                        // if the stage is deceasing, remove the future stages from stage history table
                        var stageHistories = context.DealSalesStageHistories.Where(t => t.DealId == dealId && t.SalesStageId > stage.SalesStageId && !t.Removed).ToList();
                        foreach (var stageHistory in stageHistories)
                        {
                            stageHistory.Removed = true;
                            stageHistory.RemovedBy = userId;
                            stageHistory.RemovedByName = username;
                            stageHistory.RemovedDate = DateTime.UtcNow;
                            context.SubmitChanges();
                        }

                        // look for the sales stage in history table
                        var currentStageHistory = context.DealSalesStageHistories.FirstOrDefault(t => t.DealId == dealId && t.SalesStageId == stage.SalesStageId && !t.Removed);
                        if (currentStageHistory == null)
                        {
                            context.DealSalesStageHistories.InsertOnSubmit(new DealSalesStageHistory
                            {
                                SubscriberId = deal.SubscriberId,
                                SalesStageId = stage.SalesStageId,
                                DealId = deal.DealId,
                                AddedBy = userId,
                                AddedByName = username,
                                AddedDate = DateTime.UtcNow
                            });
                            context.SubmitChanges();
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                var error = new WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "UpdateDealSalesStage",
                    PageCalledFrom = "Helper/Deals",
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = userId
                };
                new Logging().LogWebAppError(error);
            }

            return false;
        }


        public bool SaveDealDateCalendarEvent(List<ActivititesMember> inviteUsers, int dealOwnerId, int subscriberId, DateTime oldDate, DateTime newDate, int dealId, string dealName, int companyId, string companyName, string eventSubject)
        {
            try
            {
                // check if calendar event exists for dealDate and type
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);


                var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                          .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                          .Select(t => t.DataCenter).FirstOrDefault();
                var sharedConnection = LoginUser.GetSharedConnectionForDataCenter(subscriberDataCenter);
                var sharedContext = new DbSharedDataContext(sharedConnection);

                var calendarEvent = sharedContext.Activities.FirstOrDefault(t =>
                                        t.DealIds.Contains(dealId.ToString()) || t.DealIds.Contains(dealId.ToString() + ",") || t.DealIds.Contains("," + dealId.ToString())
                                        && (t.CalendarEventId > 0 || t.ActivityType == "EVENT")
                                                            && t.Subject.Contains(eventSubject)) ?? new Crm6.App_Code.Shared.Activity();

                // set calendar event properties
                calendarEvent.CompanyId = companyId;
                calendarEvent.CompanyName = companyName;
                calendarEvent.DealIds = dealId.ToString();
                calendarEvent.DealNames = dealName;
                calendarEvent.IsAllDay = true;
                calendarEvent.LastUpdate = DateTime.UtcNow;
                calendarEvent.Subject = eventSubject;
                calendarEvent.StartDateTime = newDate;
                calendarEvent.OwnerUserId = dealOwnerId;
                calendarEvent.UpdateUserId = dealOwnerId;
                var username = new Users().GetUserFullNameById(dealOwnerId, subscriberId);
                calendarEvent.UpdateUserName = username;
                calendarEvent.SubscriberId = subscriberId;

                var linkedCompanies = new List<GlobalCompany>();
                if (companyId > 0)
                {
                    var company = new GlobalCompany();
                    company.CompanyId = companyId;
                    company.CompanyName = companyName;
                    company.SubscriberId = subscriberId;
                    linkedCompanies.Add(company);
                }
                // save calendar event
                new CalendarEvents().SaveCalendarEvent(new ActivityModel
                {
                    CalendarEvent = calendarEvent,
                    Company = linkedCompanies.Count > 0 ? linkedCompanies[0] : null,
                    Invites = inviteUsers
                });
            }
            catch (Exception ex)
            {
                var error = new Crm6.App_Code.Shared.WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    RoutineName = "SaveDealDateCalendarEvent",
                    PageCalledFrom = "SaveDeal",
                    SubscriberId = subscriberId,
                    SubscriberName = "",
                    ErrorMessage = ex.ToString(),
                    UserId = dealOwnerId
                };
                new Logging().LogWebAppError(error);
                return false;
            }
            return true;
        }


        public bool UpdateDealDate(int dealId, DateTime dateValue, string datetype, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var deal = context.Deals.FirstOrDefault(d => d.DealId == dealId);
            if (deal != null)
            {
                var updateUserId = userId;

                // get LinkUserToDeal userIds
                var dealUsers = GetDealUsers(dealId, deal.SubscriberId).Select(t => new ActivititesMember
                {
                    InviteType = "user",
                    UserId = t.User.UserId,
                    UserName = t.User.FullName
                }).Where(t => t.UserId != deal.DealOwnerId).ToList();

                string eventSubject;

                switch (datetype.ToLower())
                {
                    case "proposal-date":
                        if (deal.DateProposalDue != dateValue)
                        {
                            // if date changed - save calendar event
                            eventSubject = "Proposal due for " + deal.CompanyName + "-" + deal.DealName;
                            SaveDealDateCalendarEvent(dealUsers, deal.DealOwnerId, subscriberId, deal.DateProposalDue.GetValueOrDefault(), dateValue, deal.DealId, deal.DealName, deal.CompanyId, deal.CompanyName, eventSubject);
                        }
                        break;
                    case "decision-date":
                        if (deal.DecisionDate != dateValue)
                        {
                            // if date changed - save calendar event
                            eventSubject = "Decision due for " + deal.CompanyName + "-" + deal.DealName;
                            SaveDealDateCalendarEvent(dealUsers, deal.DealOwnerId, subscriberId, deal.DecisionDate.GetValueOrDefault(), dateValue, deal.DealId, deal.DealName, deal.CompanyId, deal.CompanyName, eventSubject);
                        }
                        break;
                    case "first-shipment-date":
                        if (deal.EstimatedStartDate != dateValue)
                        {
                            // if date changed - save calendar event
                            eventSubject = "First Shipment due for " + deal.CompanyName + "-" + deal.DealName;
                            SaveDealDateCalendarEvent(dealUsers, deal.DealOwnerId, subscriberId, deal.EstimatedStartDate.GetValueOrDefault(), dateValue, deal.DealId, deal.DealName, deal.CompanyId, deal.CompanyName, eventSubject);
                        }
                        break;
                    case "contract-end-date":
                        if (deal.ContractEndDate != dateValue)
                        {
                            // if date changed - save calendar event
                            eventSubject = "Contract ending for " + deal.CompanyName + "-" + deal.DealName;
                            SaveDealDateCalendarEvent(dealUsers, deal.DealOwnerId, subscriberId, deal.ContractEndDate.GetValueOrDefault(), dateValue, deal.DealId, deal.DealName, deal.CompanyId, deal.CompanyName, eventSubject);
                        }
                        break;
                }
                return true;
            }
            return false;
        }


        public string GetDealTotalRevenue(int dealId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            // get deal if not passed 
            var deal = context.Deals.FirstOrDefault(o => o.DealId == dealId);
            if (deal != null)
            {
                //  get total opportunity revenue
                var revenue = GetDealRevenue(deal.DealId, userId, subscriberId);
                //   render with user currency code
                return new Currencies().RenderCurrencyFromCurrencyCode(revenue.Revenue, revenue.CurrencySymbol, 0);
            }
            return "";
        }


        public string GetDealName(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Deals.Where(t => t.DealId == dealId)
                        .Select(t => t.DealName)
                        .FirstOrDefault() ?? "";
        }

        public string GetDealNameByDataCenter(int dealId, string dataCenter)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Deals.Where(t => t.DealId == dealId)
                        .Select(t => t.DealName)
                        .FirstOrDefault() ?? "";
        }

        public bool DeleteDeal(int dealId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var deal = context.Deals.FirstOrDefault(t => t.DealId == dealId);
            if (deal != null)
            {
                deal.Deleted = true;
                deal.DeletedUserId = userId;
                deal.DeletedDate = DateTime.UtcNow;
                deal.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();

                // delete all deal lanes
                var lanes = context.Lanes.Where(t => t.DealId == dealId).ToList();
                foreach (var lane in lanes)
                {
                    new Lanes().DeleteLane(lane.LaneId, userId, lane.SubscriberId);
                }
                return true;
            }
            return false;
        }


        public bool DeleteDeal(int dealId, int dealSubscriberId, int userId, int userSubscriberId)
        {
            var dealContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var deal = dealContext.Deals.FirstOrDefault(t => t.DealId == dealId);
            if (deal != null)
            {
                deal.Deleted = true;
                deal.DeletedUserId = userId;
                deal.DeletedDate = DateTime.UtcNow;
                deal.DeletedUserName = userContext.Users.Where(u => u.UserId == userId)
                                             .Select(u => u.FullName).FirstOrDefault() ?? "";
                dealContext.SubmitChanges();

                // delete all deal lanes
                var lanes = dealContext.Lanes.Where(t => t.DealId == dealId).ToList();
                foreach (var lane in lanes)
                {
                    new Lanes().DeleteLane(lane.LaneId, userId, lane.SubscriberId);
                }
                return true;
            }
            return false;
        }


        #region Deal Contacts


        /// <summary>
        /// this function first get the connection for the subscriber and then do the retrival - used for linked deals
        /// </summary>
        /// <param name="dealId"></param>
        /// <param name="subscriberId"></param>
        /// <returns></returns>
        public List<ContactModel> GetDealContacts(int dealId, int subscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                          .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                          .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);

            // var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var contacts = (from t in context.LinkContactToDeals
                            join j in context.Contacts on t.ContactId equals j.ContactId
                            where t.DealId == dealId && !t.Deleted && !j.Deleted
                            select new Contacts().GetContact(t.ContactId, subscriberId)).ToList();

            return contacts;
        }

        public bool AddDealContact(LinkContactToDeal dealContact)
        {
            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var found = context.LinkContactToDeals.FirstOrDefault(t => t.ContactId == dealContact.ContactId && t.DealId == dealContact.DealId && !t.Deleted);
            if (dealContact.UpdateUserId > 0)
            {
                var updatedUserName = context.Users
                                          .Where(u => u.UserId == dealContact.UpdateUserId)
                                          .Select(u => u.FullName)
                                          .FirstOrDefault() ?? "";
                if (found == null)
                {
                    dealContact.CreatedUserId = dealContact.UpdateUserId;
                    dealContact.CreatedDate = DateTime.UtcNow;
                    dealContact.CreatedUserName = updatedUserName;
                    dealContact.LastUpdate = DateTime.UtcNow;
                    dealContact.UpdateUserName = updatedUserName;

                    dealContact.ContactName = context.Contacts.Where(t => t.ContactId == dealContact.ContactId)
                        .Select(t => t.ContactName).FirstOrDefault() ?? "";
                    dealContact.LinkType = "";
                    dealContact.DealName = context.Deals.Where(t => t.DealId == dealContact.DealId)
                        .Select(t => t.DealName)
                        .FirstOrDefault() ?? "";
                    context.LinkContactToDeals.InsertOnSubmit(dealContact);
                }
                else
                {
                    found.LastUpdate = DateTime.UtcNow;
                    found.UpdateUserName = updatedUserName;
                    found.UpdateUserId = dealContact.UpdateUserId;
                }
            }
            context.SubmitChanges();
            return true;
        }


        public bool DeleteDealContact(int dealId, int userId, int contactId, int dealSubscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                .GlobalSubscribers.Where(t => t.SubscriberId == dealSubscriberId)
                .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var dealContact = context.LinkContactToDeals.FirstOrDefault(i => i.ContactId == contactId && i.DealId == dealId && !i.Deleted);
            if (dealContact != null)
            {
                dealContact.DeletedUserId = userId;
                dealContact.DeletedUserName = context.Users
                    .Where(u => u.UserId == userId)
                    .Select(u => u.FullName)
                    .FirstOrDefault() ?? "";
                dealContact.Deleted = true;
                dealContact.DeletedDate = DateTime.UtcNow;
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public List<Crm6.App_Code.Deal> GetContactDeals(int contactId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return (from t in context.Deals
                    join j in context.LinkContactToDeals on t.DealId equals j.DealId
                    where t.SubscriberId == subscriberId && j.ContactId == contactId && !t.Deleted && !j.Deleted
                    select t).ToList();
        }

        #endregion


        #region Deal Companies

        public int GetCompanyDealsCount(int companyId, int subscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            var context = new DbFirstFreightDataContext(connection);
            var dealCount = context.Deals.Count(t => t.CompanyId == companyId && !t.Deleted);
            return dealCount;
        }


        public List<Crm6.App_Code.Deal> GetCompanyDeals(int companyId, int subscriberId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            var context = new DbFirstFreightDataContext(connection);
            var deals = context.Deals.Where(t => t.CompanyId == companyId && !t.Deleted)
                               .Select(t => GetDeal(t.DealId, t.SubscriberId)).ToList();
            return deals;
        }

        #endregion


        #region Deal Stage Timeline

        public List<DealSalesStageTimeline> GetDealSalesStageTimeline(int subscriberId, int dealId)
        {
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                             .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                             .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            // var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var context = new DbFirstFreightDataContext(connection);

            var timeline = new List<DealSalesStageTimeline>();
            // load sales stages
            var salesStages = new SalesStages().GetSalesStages(subscriberId);

            foreach (var salesStage in salesStages)
            {
                var tLine = new DealSalesStageTimeline
                {
                    DealId = dealId,
                    SalesStageId = salesStage.SalesStageId,
                    SalesStage = salesStage.SalesStageName
                };

                // get historyItems
                var salesStageHistory = context.DealSalesStageHistories.FirstOrDefault(t => t.DealId == dealId && !t.Removed && t.SalesStageId == salesStage.SalesStageId);
                var salesStageNextHistory = (from t in context.DealSalesStageHistories
                                             join j in context.SalesStages on t.SalesStageId equals j.SalesStageId
                                             where t.DealId == dealId && !t.Removed && t.SalesStageId != salesStage.SalesStageId && j.SortOrder > salesStage.SortOrder
                                             orderby j.SortOrder
                                             select t).FirstOrDefault();

                // set added by
                if (salesStageHistory != null)
                {
                    tLine.AddedBy = salesStageHistory.AddedBy;
                    tLine.AddedName = salesStageHistory.AddedByName;
                }

                // set days in stage
                if (salesStageHistory != null && salesStageNextHistory != null)
                {
                    tLine.DaysInStage = (salesStageNextHistory.AddedDate - salesStageHistory.AddedDate).Days;
                }
                else if (salesStageHistory != null)
                {
                    tLine.DaysInStage = (DateTime.UtcNow - salesStageHistory.AddedDate).Days;
                }
                else if (salesStageNextHistory != null)
                {
                    tLine.DaysInStage = 0;
                }
                timeline.Add(tLine);
            }
            return timeline;
        }

        #endregion


        #region Deal Users

        public List<DealSalesTeamMember> GetDealUsers(int dealId, int subscriberId)
        {
            var users = new List<DealSalesTeamMember>();
            var subscriberDataCenter = new DbLoginDataContext(LoginUser.GetLoginConnection())
                                                            .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                            .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            //  var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var dealUsers = context.LinkUserToDeals.Where(t => t.DealId == dealId && !t.Deleted)
                .Select(t => t).ToList();
            foreach (var linkUser in dealUsers)
            {
                if (linkUser.UserSubscriberId > 0)
                {
                    var u = new Users().GetUser(linkUser.UserId, linkUser.UserSubscriberId);
                    if (u != null)
                    {
                        users.Add(new DealSalesTeamMember
                        {
                            User = u.User,
                            ProfilePicture = u.ProfilePicture,
                            LinkUseToDeal = linkUser,
                        });
                    }
                }
                else
                {
                    var u = new Users().GetUser(linkUser.UserId, subscriberId);
                    if (u != null)
                    {
                        users.Add(new DealSalesTeamMember
                        {
                            User = u.User,
                            ProfilePicture = u.ProfilePicture,
                            LinkUseToDeal = linkUser,
                        });
                    }

                }
            }

            return users;
        }


        public List<UserBasic> GetDealUsersBasicDetails(int dealId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var dealUsers = (from t in context.LinkUserToDeals
                             join j in context.Users on t.UserId equals j.UserId
                             where t.DealId == dealId && !t.Deleted
                             select new UserBasic
                             {
                                 UserId = j.UserId,
                                 FullName = j.FullName,
                                 ProfilePicture = ""
                             });

            var finalUserList = new List<UserBasic>();
            foreach (var user in dealUsers)
            {
                var doc = new Documents().GetDocumentsByDocType(1, user.UserId).FirstOrDefault();
                if (doc != null)
                {
                    user.ProfilePicture = doc.DocumentUrl;
                }
                finalUserList.Add(user);
            }
            return finalUserList;
        }


        public bool AddDealUser(LinkUserToDeal dealUser, int dealSubscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var updatedUserName = context.Users.Where(u => u.UserId == dealUser.UpdateUserId)
                                         .Select(u => u.FullName).FirstOrDefault() ?? "";

            var found = context.LinkUserToDeals.FirstOrDefault(u => u.DealId == dealUser.DealId && u.UserId == dealUser.UserId && !u.Deleted);
            if (found == null)
            {
                // add deal user
                dealUser.UserName = context.Users.Where(u => u.UserId == dealUser.UserId)
                                                     .Select(u => u.FullName).FirstOrDefault() ?? "";
                dealUser.CreatedUserId = dealUser.UpdateUserId;
                dealUser.CreatedUserName = updatedUserName;
                dealUser.CreatedDate = DateTime.UtcNow;
                dealUser.UpdateUserId = dealUser.UpdateUserId;
                dealUser.UpdateUserName = updatedUserName;
                dealUser.LastUpdate = DateTime.UtcNow;
                dealUser.LinkType = "";
                dealUser.DealName = context.Deals.Where(t => t.DealId == dealUser.DealId)
                        .Select(t => t.DealName)
                        .FirstOrDefault() ?? "";
                context.LinkUserToDeals.InsertOnSubmit(dealUser);
            }
            else
            {
                found.LastUpdate = DateTime.UtcNow;
                found.UpdateUserName = updatedUserName;
                found.UpdateUserId = dealUser.UpdateUserId;
                found.SalesTeamRole = dealUser.SalesTeamRole;
            }
            context.SubmitChanges();

            // sales team
            var deal = context.Deals.FirstOrDefault(t => t.DealId == dealUser.DealId);
            if (deal != null)
            {
                var salesTeamUsers = context.LinkUserToDeals.Where(t => t.DealId == dealUser.DealId && !t.Deleted)
                                 .Select(t => t.UserName).ToList();
                deal.SalesTeam = string.Join(", ", salesTeamUsers);
                context.SubmitChanges();
            }
            return true;
        }


        public bool AddDealSalesTeam(AddDealUserRequest request)
        {
            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new DbLoginDataContext(loginConnection);

            var context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var updatedUserName = new Users().GetUserFullNameById(request.UpdatedBy, request.DealSubscriberId);
            var isNewUser = false;

            // get sales team user
            var gUser = loginContext.GlobalUsers.FirstOrDefault(t => t.GlobalUserId == request.GlobalUserId);
            if (gUser != null)
            {
                // check if the user id is in the same subscriber
                if (gUser.SubscriberId == request.DealSubscriberId)
                {
                    // add as a normal request
                    var dealUser = context.LinkUserToDeals.FirstOrDefault(u => u.DealId == request.DealId && u.UserId == gUser.UserId && !u.Deleted);
                    if (dealUser == null)
                    {
                        isNewUser = true;
                        // add deal user
                        dealUser = new LinkUserToDeal
                        {
                            DealId = request.DealId,
                            UserId = gUser.UserId,
                            SubscriberId = request.DealSubscriberId,
                            UserName = gUser.FullName,
                            CreatedUserId = request.UpdatedBy,
                            CreatedUserName = updatedUserName,
                            CreatedDate = DateTime.UtcNow,
                            UpdateUserId = request.UpdatedBy,
                            UpdateUserName = updatedUserName,
                            LastUpdate = DateTime.UtcNow,
                            SalesTeamRole = request.SalesTeamRole,
                            LinkType = "",
                            UserSubscriberId = gUser.SubscriberId
                        };
                        dealUser.DealName = context.Deals.Where(t => t.DealId == dealUser.DealId)
                            .Select(t => t.DealName)
                            .FirstOrDefault() ?? "";
                        context.LinkUserToDeals.InsertOnSubmit(dealUser);
                    }
                    else
                    {
                        dealUser.SalesTeamRole = request.SalesTeamRole;
                        dealUser.LastUpdate = DateTime.UtcNow;
                        dealUser.UpdateUserName = updatedUserName;
                        dealUser.UpdateUserId = request.UpdatedBy;
                    }
                    context.SubmitChanges();

                    // sales team
                    var deal = context.Deals.FirstOrDefault(t => t.DealId == request.DealId);
                    if (deal != null)
                    {
                        deal.SalesTeam = GetDealSalesTeam(request.DealId, deal.SubscriberId);
                        context.SubmitChanges();
                    }

                    // send email notification
                    if (isNewUser)
                    {
                        SendDealInvitation(deal, dealUser);
                    }
                }
                else
                {
                    // adding a user from a different susbcriber 
                    // source deal context
                    var dealSubscriber = loginContext.GlobalSubscribers.FirstOrDefault(t => t.SubscriberId == request.DealSubscriberId);
                    var sourceDealConnection = LoginUser.GetConnectionForDataCenter(dealSubscriber.DataCenter);
                    var sourceDealContext = new DbFirstFreightDataContext(sourceDealConnection);

                    // target user context
                    var userSubscriber = loginContext.GlobalSubscribers.FirstOrDefault(t => t.SubscriberId == gUser.SubscriberId);
                    var userConnection = LoginUser.GetConnectionForDataCenter(userSubscriber.DataCenter);
                    var userContext = new DbFirstFreightDataContext(userConnection);

                    if (dealSubscriber != null && userSubscriber != null)
                    {
                        var sourceDeal = sourceDealContext.Deals.FirstOrDefault(t => t.DealId == request.DealId);
                        // check the deal in logged in users' DB
                        var targetDeal = userContext.Deals.FirstOrDefault(t => t.SourceDataCenterDealId == request.DealId);
                        if (targetDeal == null)
                        {
                            // add target deal
                            targetDeal = SaveTargetDeal(sourceDeal, gUser.SubscriberId);
                        }

                        // add external user to the target deal
                        if (targetDeal != null)
                        {
                            var dealUser = userContext.LinkUserToDeals.FirstOrDefault(u => u.DealId == targetDeal.DealId
                                        && u.UserId == gUser.UserId && !u.Deleted);

                            if (dealUser == null)
                            {
                                isNewUser = true;
                                // add deal user
                                dealUser = new LinkUserToDeal
                                {
                                    DealId = targetDeal.DealId,
                                    UserId = gUser.UserId,
                                    SubscriberId = targetDeal.SubscriberId,
                                    UserName = gUser.FullName,
                                    CreatedUserId = request.UpdatedBy,
                                    CreatedUserName = updatedUserName,
                                    CreatedDate = DateTime.UtcNow,
                                    UpdateUserId = request.UpdatedBy,
                                    UpdateUserName = updatedUserName,
                                    LastUpdate = DateTime.UtcNow,
                                    LinkType = "",
                                    DealName = targetDeal.DealName,
                                    UserSubscriberId = gUser.SubscriberId,
                                    SalesTeamRole = request.SalesTeamRole
                                };
                                userContext.LinkUserToDeals.InsertOnSubmit(dealUser);
                            }
                            else
                            {
                                dealUser.LastUpdate = DateTime.UtcNow;
                                dealUser.UpdateUserName = updatedUserName;
                                dealUser.UpdateUserId = request.UpdatedBy;
                            }
                            userContext.SubmitChanges();


                            // add exteral user to the source deal database with external subscriberId
                            var sourceDealUser = sourceDealContext.LinkUserToDeals.FirstOrDefault(u => u.DealId == sourceDeal.DealId
                                        && u.UserId == gUser.UserId && !u.Deleted && u.UserSubscriberId == gUser.SubscriberId);

                            if (sourceDealUser == null)
                            {
                                // add deal user
                                sourceDealUser = new LinkUserToDeal
                                {
                                    DealId = sourceDeal.DealId,
                                    SubscriberId = sourceDeal.SubscriberId,
                                    UserName = dealUser.UserName,
                                    CreatedUserId = request.UpdatedBy,
                                    CreatedUserName = updatedUserName,
                                    CreatedDate = DateTime.UtcNow,
                                    UpdateUserId = request.UpdatedBy,
                                    UpdateUserName = updatedUserName,
                                    LastUpdate = DateTime.UtcNow,
                                    LinkType = "",
                                    DealName = sourceDeal.DealName,
                                    UserId = gUser.UserId,
                                    UserSubscriberId = gUser.SubscriberId,
                                    SalesTeamRole = request.SalesTeamRole
                                };
                                sourceDealContext.LinkUserToDeals.InsertOnSubmit(sourceDealUser);
                            }
                            else
                            {
                                sourceDealUser.SalesTeamRole = request.SalesTeamRole;
                                sourceDealUser.LastUpdate = DateTime.UtcNow;
                                sourceDealUser.UpdateUserName = updatedUserName;
                                sourceDealUser.UpdateUserId = request.UpdatedBy;
                            }
                            sourceDealContext.SubmitChanges();

                            // update sales team
                            sourceDeal.SalesTeam = GetDealSalesTeam(sourceDeal.DealId, sourceDeal.SubscriberId);
                            sourceDealContext.SubmitChanges();

                            // update target deal sales team
                            targetDeal = userContext.Deals.FirstOrDefault(t => t.DealId == targetDeal.DealId);
                            if (targetDeal != null)
                            {
                                targetDeal.SalesTeam = sourceDeal.SalesTeam;
                                userContext.SubmitChanges();

                                // send email notification
                                if (isNewUser)
                                {
                                    SendDealInvitation(targetDeal, dealUser);
                                }
                            }
                        }

                    }
                }
            }

            return true;
        }


        /// <summary>
        /// save target deal on linked subscriber database if has invited
        /// </summary>
        /// <param name="sourceDeal"></param>
        /// <param name="userSubscriberId"></param>
        /// <returns></returns>
        private Crm6.App_Code.Deal SaveTargetDeal(Crm6.App_Code.Deal sourceDeal, int userSubscriberId)
        {
            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new DbLoginDataContext(loginConnection);

            var userSubscriber = loginContext.GlobalSubscribers.FirstOrDefault(t => t.SubscriberId == userSubscriberId);
            if (userSubscriber != null)
            {
                var connection = LoginUser.GetConnectionForDataCenter(userSubscriber.DataCenter);
                var context = new DbFirstFreightDataContext(connection);
                var targetDeal = context.Deals.FirstOrDefault(t => t.SourceDataCenterDealId == sourceDeal.DealId) ?? new Crm6.App_Code.Deal();

                // save target deal 
                targetDeal.DealName = sourceDeal.DealName;
                targetDeal.DealDescription = sourceDeal.DealDescription;
                targetDeal.DealNumber = sourceDeal.DealNumber;

                // source deal id and subscriber id
                targetDeal.SourceDataCenterDealId = sourceDeal.DealId;
                targetDeal.SourceSubscriberId = sourceDeal.SubscriberId;

                targetDeal.SalesStageName = sourceDeal.SalesStageName;
                targetDeal.SalesStageId = sourceDeal.SalesStageId;

                targetDeal.CompanyId = sourceDeal.CompanyId;
                targetDeal.CompanyName = sourceDeal.CompanyName;
                targetDeal.PrimaryContactId = sourceDeal.PrimaryContactId;
                targetDeal.PrimaryContactName = sourceDeal.PrimaryContactName;
                targetDeal.Commodities = sourceDeal.Commodities;
                targetDeal.Competitors = sourceDeal.Competitors;
                targetDeal.Comments = sourceDeal.Comments;
                targetDeal.SalesRepId = sourceDeal.SalesRepId;
                targetDeal.DealType = sourceDeal.DealType;
                targetDeal.DealRequestType = sourceDeal.DealRequestType;
                targetDeal.Industry = sourceDeal.Industry;
                targetDeal.SalesRepName = sourceDeal.SalesRepName;
                targetDeal.LocationCode = sourceDeal.LocationCode;
                targetDeal.LocationName = sourceDeal.LocationName;
                targetDeal.DistrictCode = sourceDeal.DistrictCode;
                targetDeal.DistrictName = sourceDeal.DistrictName; 
                targetDeal.RegionName = sourceDeal.RegionName;
                // dates
                targetDeal.DecisionDate = sourceDeal.DecisionDate;
                targetDeal.EstimatedStartDate = sourceDeal.EstimatedStartDate;
                targetDeal.ContractEndDate = sourceDeal.ContractEndDate;
                targetDeal.DateProposalDue = sourceDeal.DateProposalDue;

                // last update
                targetDeal.LastUpdate = DateTime.UtcNow;
                targetDeal.UpdateUserId = sourceDeal.UpdateUserId;
                targetDeal.UpdateUserName = sourceDeal.UpdateUserName;
                targetDeal.SalesTeam = sourceDeal.SalesTeam;

                // insert new deal
                if (targetDeal.DealId < 1)
                {
                    targetDeal.SubscriberId = userSubscriberId; // target user subscriber id
                    targetDeal.CreatedUserId = sourceDeal.CreatedUserId;
                    targetDeal.CreatedDate = sourceDeal.CreatedDate;
                    targetDeal.CreatedUserName = sourceDeal.CreatedUserName;
                    targetDeal.DealOwnerId = sourceDeal.DealOwnerId;
                    context.Deals.InsertOnSubmit(targetDeal);
                }
                context.SubmitChanges();

                return targetDeal;
            }

            return null;
        }


        public bool DeleteDealUser(int dealId, int userId, int deleteUserId, int dealSubscriberId, int userSubscriberId)
        {
            if (dealSubscriberId == userSubscriberId)
            {
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                var dealUser = context.LinkUserToDeals.FirstOrDefault(t => t.UserId == deleteUserId
                                && t.DealId == dealId && !t.Deleted);
                if (dealUser != null)
                {
                    dealUser.DeletedUserId = userId;
                    dealUser.Deleted = true;
                    dealUser.DeletedDate = DateTime.UtcNow;
                    context.SubmitChanges();

                    // sales team
                    var deal = context.Deals.FirstOrDefault(t => t.DealId == dealUser.DealId);
                    if (deal != null)
                    {
                        deal.SalesTeam = GetDealSalesTeam(dealUser.DealId, dealUser.SubscriberId);
                        context.SubmitChanges();
                    }
                    return true;
                }
            }
            else
            {
                // adding a user from a different susbcriber
                // check the data center
                var loginConnection = LoginUser.GetLoginConnection();
                var loginContext = new DbLoginDataContext(loginConnection);

                // source deal context
                var dealSubscriber = loginContext.GlobalSubscribers.FirstOrDefault(t => t.SubscriberId == dealSubscriberId);
                var sourceDealConnection = LoginUser.GetConnectionForDataCenter(dealSubscriber.DataCenter);
                var sourceDealContext = new DbFirstFreightDataContext(sourceDealConnection);

                // target user context
                var userSubscriber = loginContext.GlobalSubscribers.FirstOrDefault(t => t.SubscriberId == userSubscriberId);
                var userConnection = LoginUser.GetConnectionForDataCenter(userSubscriber.DataCenter);
                var userContext = new DbFirstFreightDataContext(userConnection);

                if (dealSubscriber != null && userSubscriber != null)
                {

                    var sourceDealUser = sourceDealContext.LinkUserToDeals.FirstOrDefault(u =>
                                    u.DealId == dealId
                                 && u.UserId == deleteUserId && !u.Deleted
                                 && u.UserSubscriberId == userSubscriberId);
                    if (sourceDealUser != null)
                    {
                        sourceDealUser.DeletedUserId = userId;
                        sourceDealUser.Deleted = true;
                        sourceDealUser.DeletedDate = DateTime.UtcNow;
                        sourceDealContext.SubmitChanges();
                    }

                    // update sales team
                    var sourceDeal = sourceDealContext.Deals.FirstOrDefault(t => t.DealId == dealId);
                    var salesTeamUsers = sourceDealContext.LinkUserToDeals.Where(t => t.DealId == dealId && !t.Deleted)
                                .Select(t => t.UserName).ToList();
                    sourceDeal.SalesTeam = string.Join(", ", salesTeamUsers);
                    sourceDealContext.SubmitChanges();

                    // check the deal in logged in users' DB
                    var targetDeal = userContext.Deals.FirstOrDefault(t => t.SourceDataCenterDealId == dealId);
                    if (targetDeal != null)
                    {
                        // add external user to the target deal
                        var dealUser = userContext.LinkUserToDeals.FirstOrDefault(u => u.DealId == targetDeal.DealId
                                    && u.UserId == deleteUserId && !u.Deleted);
                        if (dealUser != null)
                        {
                            dealUser.DeletedUserId = userId;
                            dealUser.Deleted = true;
                            dealUser.DeletedDate = DateTime.UtcNow;
                            userContext.SubmitChanges();
                        }

                        targetDeal.SalesTeam = sourceDeal.SalesTeam;
                        userContext.SubmitChanges();

                        // check if there is more deal users for target deal - Delete Deal if not 
                    }
                    return true;
                }
            }
            return false;
        }

        #endregion


        #region Global Location UNLOCO and IATA

      
        public string GetGlobalLocationDisplayValue(string locationCode, string countryName, string service)
        {
            var countryCode = new Countries().GetCountryCodeFromCountryName(countryName) ?? "";
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            GlobalLocation location = null;
            if (!string.IsNullOrEmpty(service))
            {
                switch (service.ToLower())
                {
                    case "air":
                        location = sharedContext.GlobalLocations.FirstOrDefault(t => t.Airport && t.LocationCode.Equals(locationCode) && t.CountryCode.ToLower().Equals(countryCode.ToLower()));
                        break;
                    case "ocean fcl":
                    case "ocean lcl":
                        location = sharedContext.GlobalLocations.FirstOrDefault(t => t.SeaPort && t.LocationCode.Equals(locationCode) && t.CountryCode.ToLower().Equals(countryCode.ToLower()));
                        break;
                    case "road fcl":
                        location = sharedContext.GlobalLocations.FirstOrDefault(t => t.RoadTerminal && t.LocationCode.Equals(locationCode) && t.CountryCode.ToLower().Equals(countryCode.ToLower()));
                        break;
                    default:
                        location = sharedContext.GlobalLocations.FirstOrDefault(t => t.LocationCode.Equals(locationCode) && t.CountryCode.ToLower().Equals(countryCode.ToLower()));
                        break;

                }
            } 

            if (location != null)
            {
                return location.LocationCode + (string.IsNullOrEmpty(location.LocationName) ? "" : " - " + location.LocationName);
            }
            return "";
        }

        #endregion


        public RevenueResponse GetDealRevenue(int dealId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context
                .Users
                .Where(u => u.UserId == userId)
                .FirstOrDefault();

            var response = new RevenueResponse
            {
                CurrencySymbol = !string.IsNullOrEmpty(user.CurrencyCode) ? new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode) : "",
                Revenue = 0.0f,
                Profit = 0.0f
            };

            var deal = context
                .Deals
                .Where(t => t.DealId == dealId)
                .FirstOrDefault();

            if (deal != null)
            {
                // yearly revenue total- for lanes where shipping frequency is 'Per Year'
                var yearlyRevenueTotal = 0.0d;
                var yearlyProfitTotal = 0.0d;

                // get lanes for the deal
                var lanes = context.Lanes.Where(t => t.ShippingFrequency.Equals("Per Year") && t.DealId == dealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;

                    var targetRevenueYearly = oLane.Revenue;
                    if (sourceCurrencyCode != user.CurrencyCode)
                    {
                        // revenue 
                        targetRevenueYearly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.Revenue);
                    }
                    yearlyRevenueTotal += targetRevenueYearly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != user.CurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    yearlyProfitTotal += profit;
                }

                // monthly revenue total- for lanes where shipping frequency is 'Per Month'
                var monthlyRevenueTotal = 0.0d;
                var monthlyProfitTotal = 0.0d;
                lanes = context.Lanes.Where(t => t.ShippingFrequency.Equals("Per Month") && t.DealId == dealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueMonthly = oLane.Revenue;
                    if (sourceCurrencyCode != user.CurrencyCode)
                    {
                        targetRevenueMonthly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.Revenue);
                    }
                    monthlyRevenueTotal += targetRevenueMonthly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != user.CurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    monthlyProfitTotal += profit;
                }

                // weekly revenue total- for lanes where shipping frequency is 'Per Week'
                var weeklyRevenueTotal = 0.0d;
                var weeklyProfitTotal = 0.0d;
                lanes = context.Lanes.Where(t => t.ShippingFrequency.Equals("Per Week") && t.DealId == dealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueWeekly = oLane.Revenue;
                    if (sourceCurrencyCode != user.CurrencyCode)
                    {
                        targetRevenueWeekly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.Revenue);
                    }
                    weeklyRevenueTotal += targetRevenueWeekly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != user.CurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    weeklyProfitTotal += profit;
                }



                // SPOT
                var otherRevenueTotal = 0.0d;
                var otherProfitTotal = 0.0d;
                lanes = context.Lanes.Where(t => !t.ShippingFrequency.Equals("Per Month") && !t.ShippingFrequency.Equals("Per Year") && !t.ShippingFrequency.Equals("Per Week")
                                                    && t.DealId == dealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueOther = oLane.Revenue;
                    if (sourceCurrencyCode != user.CurrencyCode)
                    {
                        targetRevenueOther = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.Revenue);
                    }

                    otherRevenueTotal += targetRevenueOther;

                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != user.CurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    otherProfitTotal += profit;
                }

                // return total
                var shippingFrquency = context.Subscribers.Where(t => t.SubscriberId == user.SubscriberId)
                                              .Select(t => t.DefaultShippingFrequency).FirstOrDefault();
                if (string.IsNullOrEmpty(shippingFrquency))
                {
                    shippingFrquency = "Per Year";
                }

                if (shippingFrquency == "Per Month")
                {
                    // return total
                    response.Revenue = yearlyRevenueTotal / 12 + (monthlyRevenueTotal) + otherRevenueTotal + (weeklyRevenueTotal * 4);
                    response.Profit = yearlyProfitTotal / 12 + (monthlyProfitTotal) + otherProfitTotal + (weeklyProfitTotal * 4);
                }
                else if (shippingFrquency == "Per Year")
                {
                    // return total
                    response.Revenue = yearlyRevenueTotal + (monthlyRevenueTotal * 12) + otherRevenueTotal + (weeklyRevenueTotal * 52);
                    response.Profit = yearlyProfitTotal + (monthlyProfitTotal * 12) + otherProfitTotal + (weeklyProfitTotal * 52);
                }
            }

            return response;
        }


        public RevenueResponse GetDealRevenue(int dealId, int userId, int userSubscriberId, int dealSubscriberId)
        {
            var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var dealContext = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var user = userContext.Users.Where(u => u.UserId == userId).FirstOrDefault();
            var response = new RevenueResponse
            {
                CurrencySymbol = !string.IsNullOrEmpty(user.CurrencyCode) ? new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode) : "",
                Revenue = 0.0f,
                Profit = 0.0f
            };

            var deal = dealContext.Deals.Where(t => t.DealId == dealId).FirstOrDefault();
            if (deal != null)
            {
                // yearly revenue total- for lanes where shipping frequency is 'Per Year'
                var yearlyRevenueTotal = 0.0d;
                var yearlyProfitTotal = 0.0d;

                // get lanes for the deal
                var lanes = dealContext.Lanes.Where(t => t.ShippingFrequency.Equals("Per Year") && t.DealId == dealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;

                    var targetRevenueYearly = oLane.Revenue;
                    if (sourceCurrencyCode != user.CurrencyCode)
                    {
                        // revenue 
                        targetRevenueYearly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.Revenue);
                    }
                    yearlyRevenueTotal += targetRevenueYearly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != user.CurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    yearlyProfitTotal += profit;
                }

                // monthly revenue total- for lanes where shipping frequency is 'Per Month'
                var monthlyRevenueTotal = 0.0d;
                var monthlyProfitTotal = 0.0d;
                lanes = dealContext.Lanes.Where(t => t.ShippingFrequency.Equals("Per Month") && t.DealId == dealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueMonthly = oLane.Revenue;
                    if (sourceCurrencyCode != user.CurrencyCode)
                    {
                        targetRevenueMonthly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.Revenue);
                    }
                    monthlyRevenueTotal += targetRevenueMonthly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != user.CurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    monthlyProfitTotal += profit;
                }


                // weekly revenue total- for lanes where shipping frequency is 'Per Week'
                var weeklyRevenueTotal = 0.0d;
                var weeklyProfitTotal = 0.0d;
                lanes = dealContext.Lanes.Where(t => t.ShippingFrequency.Equals("Per Week") && t.DealId == dealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueWeekly = oLane.Revenue;
                    if (sourceCurrencyCode != user.CurrencyCode)
                    {
                        targetRevenueWeekly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.Revenue);
                    }
                    weeklyRevenueTotal += targetRevenueWeekly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != user.CurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    weeklyProfitTotal += profit;
                }



                // SPOT
                var otherRevenueTotal = 0.0d;
                var otherProfitTotal = 0.0d;
                lanes = dealContext.Lanes.Where(t => !t.ShippingFrequency.Equals("Per Month") && !t.ShippingFrequency.Equals("Per Year") && !t.ShippingFrequency.Equals("Per Week")
                                                    && t.DealId == dealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueOther = oLane.Revenue;
                    if (sourceCurrencyCode != user.CurrencyCode)
                    {
                        targetRevenueOther = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.Revenue);
                    }

                    otherRevenueTotal += targetRevenueOther;

                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != user.CurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, user.CurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    otherProfitTotal += profit;
                }

                // return total
                var shippingFrquency = userContext.Subscribers.Where(t => t.SubscriberId == user.SubscriberId)
                                              .Select(t => t.DefaultShippingFrequency).FirstOrDefault();
                if (string.IsNullOrEmpty(shippingFrquency))
                {
                    shippingFrquency = "Per Year";
                }


                if (shippingFrquency == "Per Month")
                {
                    // return total
                    response.Revenue = yearlyRevenueTotal / 12 + (monthlyRevenueTotal) + otherRevenueTotal + (weeklyRevenueTotal * 4);
                    response.Profit = yearlyProfitTotal / 12 + (monthlyProfitTotal) + otherProfitTotal + (weeklyProfitTotal * 4);
                }
                else if (shippingFrquency == "Per Year")
                {
                    // return total
                    response.Revenue = yearlyRevenueTotal + (monthlyRevenueTotal * 12) + otherRevenueTotal + (weeklyRevenueTotal * 52);
                    response.Profit = yearlyProfitTotal + (monthlyProfitTotal * 12) + otherProfitTotal + (weeklyProfitTotal * 52);
                }
            }

            return response;
        }


        public RevenueResponse GetDealRevenueFromUserCurrency(int dealId, int userId)
        {
            var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var dealContext = new DbFirstFreightDataContext(LoginUser.GetConnection());

            var user = userContext.Users.Where(u => u.UserId == userId).FirstOrDefault();
            var response = new RevenueResponse
            {
                CurrencySymbol = !string.IsNullOrEmpty(user.CurrencyCode) ? new Currencies().GetCurrencySymbolFromCode(user.CurrencyCode) : "",
                Revenue = 0.0f,
                Profit = 0.0f
            };

            var deal = dealContext.Deals.Where(t => t.DealId == dealId).FirstOrDefault();
            if (deal != null)
            {
                response.Profit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, deal.ProfitUSD);
                response.Revenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, deal.RevenueUSD);
                response.SpotRevenue = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, deal.RevenueUSDSpot);
                response.SpotProfit = new Currencies().GetCalculatedCurrencyExchangeValue("USD", user.CurrencyCode, deal.ProfitUSDSpot);
            }
            return response;
        }

        public void UpdateDealLastUpdateDate(int dealId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            if (subscriberId > 0)
            {
                context = new DbFirstFreightDataContext(LoginUser.GetConnection());
            }
            var deal = context.Deals.Where(t => t.DealId == dealId).FirstOrDefault();
            if (deal != null)
            {
                deal.LastUpdate = DateTime.UtcNow;
                deal.UpdateUserId = userId;
                deal.UpdateUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
            }
        }


        public bool SendDealInvitation(Crm6.App_Code.Deal deal, LinkUserToDeal dealUser)
        {
            var subscriberId = deal.SubscriberId;
            if (dealUser.UserSubscriberId > 0)
            {
                subscriberId = dealUser.UserSubscriberId;
            }

            var connection = LoginUser.GetConnection();
            var userContext = new DbFirstFreightDataContext(connection);
            var user = userContext.Users.FirstOrDefault(t => t.UserId == dealUser.UserId);
            if (user != null)
            {
                // set the deal detail URL
                var dealDetailUrl = GetDealDetailUrl(subscriberId, deal.DealId);

                var emailTemplate = "";
                using (WebClient client = new WebClient())
                {
                    string path = HttpContext.Current.Server.MapPath("~/_email_templates/deal-invite.html");
                    emailTemplate = client.DownloadString(path);
                }

                // to first name - User in CRM LinkUserToDeal table
                var nameArray = (dealUser.UserName + "").Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                var firstName = "";
                if (nameArray.Length > 0)
                    firstName = nameArray[0];

                emailTemplate = emailTemplate.Replace("#inviteduser#", firstName);
                // set the body
                emailTemplate = emailTemplate.Replace("#DealDetailUrl#", dealDetailUrl);
                emailTemplate = emailTemplate.Replace("#dealname#", deal.DealName);
                emailTemplate = emailTemplate.Replace("#companyname#", deal.CompanyName);
                emailTemplate = emailTemplate.Replace("#invitedbyuser#", dealUser.UpdateUserName);

                var content = "";
                if (!string.IsNullOrEmpty(deal.CompanyName))
                    content += "<p style=\"margin: 0; font-size: 14px;\">Company: <strong>" + deal.CompanyName + "</strong></p>";

                if (!string.IsNullOrEmpty(deal.LocationName))
                    content += "<p style=\"margin: 0; font-size: 14px;\">Location: <strong>" + deal.LocationName + "</strong></p>";

                if (!string.IsNullOrEmpty(deal.PrimaryContactName))
                    content += "<p style=\"margin: 0; font-size: 14px;\">Contact: <strong>" + deal.PrimaryContactName + "</strong></p>";

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
                    Subject = "Deal Invite - " + deal.DealName,
                    HtmlBody = body,
                    OtherRecipients = new List<Recipient>
                                                        {
                                                            new Recipient{EmailAddress = "sendgrid@firstfreight.com" },
                                                            new Recipient{EmailAddress = "charles@firstfreight.com" },
                                                            new Recipient{EmailAddress =user.EmailAddress, Name = user.FullName,UserId = user.UserId }
                                                        }
                };

                // set the reply to email address
                var sendByEmail = new Users().GetUserEmailById(dealUser.UpdateUserId, subscriberId);
                if (!string.IsNullOrEmpty(sendByEmail))
                {
                    request.ReplyToEmail = sendByEmail;
                }

                new SendGridHelper().SendEmail(request);
                return true;
            }

            return false;
        }


        private string GetDealDetailUrl(int subscriberId, int dealId)
        {
            var url = "http://";
            if (HttpContext.Current.Request.IsSecureConnection)
                url = "https://";

            var uriAddress = HttpContext.Current.Request.Url.ToString();
            if (uriAddress.Contains("localhost"))
            {
                url += "localhost:64604/Deals/DealDetail/dealdetail.aspx?dealId=" + dealId;
            }
            else
            {
                var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
                var subscriberDomain = userContext.Subscribers.Where(t => t.SubscriberId == subscriberId).Select(t => t.SubDomain).FirstOrDefault();
                if (string.IsNullOrEmpty(subscriberDomain))
                    subscriberDomain = "crm6";

                url += subscriberDomain + ".firstfreight.com/Deals/DealDetail/dealdetail.aspx?dealId=" + dealId;
            }
            return url;
        }


        public void FixSpotDeals()
        {
            var dealContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
            var deals = dealContext.Deals.Where(t => t.SubscriberId == 229 && !t.Deleted).ToList();
            foreach (var deal in deals)
            {
                // get lanes that are not SPOT
                var targetCurrencyCode = "USD";
                var monthlyProfit = 0.0d;
                var monthlyRevenue = 0.0d;

                // get lanes for the deal
                var lanes = dealContext.Lanes.Where(t => t.ShippingFrequency.Equals("Per Year") && t.DealId == deal.DealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;

                    var targetRevenueYearly = oLane.Revenue;
                    if (sourceCurrencyCode != targetCurrencyCode)
                    {
                        // revenue 
                        targetRevenueYearly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, oLane.Revenue);
                    }

                    monthlyRevenue += targetRevenueYearly / 12;

                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != targetCurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    monthlyProfit += profit / 12;
                }

                // monthly revenue total- for lanes where shipping frequency is 'Per Month' 
                lanes = dealContext.Lanes.Where(t => t.ShippingFrequency.Equals("Per Month") && t.DealId == deal.DealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueMonthly = oLane.Revenue;
                    if (sourceCurrencyCode != targetCurrencyCode)
                    {
                        targetRevenueMonthly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, oLane.Revenue);
                    }
                    monthlyRevenue += targetRevenueMonthly;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != targetCurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    monthlyProfit += profit;
                }


                // weekly revenue total- for lanes where shipping frequency is 'Per Week' 
                lanes = dealContext.Lanes.Where(t => t.ShippingFrequency.Equals("Per Week") && t.DealId == deal.DealId && !t.Deleted).ToList();
                foreach (var oLane in lanes)
                {
                    var sourceCurrencyCode = oLane.CurrencyCode;
                    // revenue 
                    var targetRevenueWeekly = oLane.Revenue;
                    if (sourceCurrencyCode != targetCurrencyCode)
                    {
                        targetRevenueWeekly = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, oLane.Revenue);
                    }
                    monthlyRevenue += targetRevenueWeekly * 4;
                    // profit
                    var profit = 0.0;
                    if (oLane.TotalLaneProfit > 0)
                    {
                        profit = oLane.TotalLaneProfit;
                        if (sourceCurrencyCode != targetCurrencyCode)
                        {
                            profit = new Currencies().GetCalculatedCurrencyExchangeValue(sourceCurrencyCode, targetCurrencyCode, oLane.TotalLaneProfit);
                        }
                        profit = Math.Round(profit, 4);
                    }
                    monthlyProfit += profit * 4;
                }


                deal.RevenueUSD = monthlyRevenue;
                deal.ProfitUSD = monthlyProfit;

                dealContext.SubmitChanges();


            }
        }


    }


    public class AddDealUserRequest
    {
        public int DealId { get; set; }
        public int DealSubscriberId { get; set; }
        public int UpdatedBy { get; set; }
        //public int UserId { get; set; }
        public int UserSubscriberId { get; set; }
        public int GlobalUserId { get; set; }
        public string SalesTeamRole { get; set; }
    }


    //public class LinkContactToDealRequest
    //{
    //    public LinkContactToDeal LinkContactToDeal { get; set; }
    //    public int DealSourceSubscriberId { get; set; }
    //}


    public class SaveDealRequest
    {
        public int SavingUserId { get; set; }
        public int SavingUserSubscriberId { get; set; }
        public Crm6.App_Code.Deal Deal { get; set; }
    }
}
