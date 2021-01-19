using System.Collections.Generic;
using System.Linq;
using Models;
using System;
using Crm6.App_Code;
using Crm6.App_Code.Shared;

namespace Helpers
{
    public class DropdownHelper
    {

        public List<SelectList> GetCampaigns(int subscriberId, string keyword)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);
            var campaigns = sharedContext.Campaigns.Where(t => t.SubscriberId == subscriberId
                  && !t.Deleted).OrderBy(t => t.CampaignName)
                .Select(t => t).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                campaigns = campaigns.Where(t => (t.CampaignName).ToLower().StartsWith(keyword.ToLower())).ToList();
            }

            return campaigns.Select(t => new SelectList
            {
                SelectText = t.CampaignName,
                SelectValue = t.CampaignId.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetCampaigns(int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);

            var campaigns = (from c in sharedContext.Campaigns
                             where c.SubscriberId == subscriberId && !c.Deleted
                             select new SelectList
                             {
                                 SelectText = c.CampaignName,
                                 SelectValue = c.CampaignName
                             }
                         ).OrderBy(cm => cm.SelectText).ToList();

            return campaigns;
        }


        public List<SelectList> GetCommodities(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            //   var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var commodities = context.Commodities.Where(t => t.SubscriberId == subscriberId
                                                            && !t.Deleted).OrderBy(t => t.SortOrder)
                                                         .Select(t => t).ToList();
            return commodities.Select(t => new SelectList
            {
                SelectText = t.CommodityName,
                SelectValue = t.CommodityName.ToString()
            }).ToList();
        }


        public List<SelectList> GetUserCompanies(int subscriberId, int userId, string keyword)
        {
            var companyFilter = new CompanyFilters()
            {
                SubscriberId = subscriberId,
                UserId = userId,
                FilterType = "ALL"
            };
            var companies = new Companies().GetCompaniesGlobal(companyFilter).Companies.ToList();
            if (!string.IsNullOrEmpty(keyword)) companies = companies.Where(t => (t.CompanyName).ToLower().StartsWith(keyword.ToLower())).ToList();
            return companies.Select(t => new SelectList
            {
                SelectText = t.CompanyName + (t.City != null ? " - " + t.City : ""),
                SelectValue = t.CompanyId.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetCompanies(int subscriberId, string keyword)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var companies = sharedContext.GlobalCompanies.Where(i => !i.Deleted).ToList();
            if (!string.IsNullOrEmpty(keyword)) companies = companies.Where(t => (t.CompanyName).ToLower().StartsWith(keyword.ToLower())).ToList();
            return companies.Select(t => new SelectList
            {
                SelectText = t.CompanyName + (t.City != null ? " - " + t.City : ""),
                SelectValue = t.CompanyId.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetGlobalCompanies(int subscriberId, string keyword)
        {
            var companies = new List<SelectList>();
            var sharedConnection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(sharedConnection);
            var suscriberCompanies = context.LinkGlobalSuscriberToSubscribers
                                            .Where(s => s.GlobalSubscriberId == subscriberId)
                                            .Select(s => s.LinkedSubscriberId)
                                            .ToList();

            var gCompanies = context.GlobalCompanies.Where(t => t.SubscriberId == subscriberId
                        && !t.Deleted && t.CompanyName != "" && (keyword == null || keyword == "" || (t.CompanyName).ToLower().StartsWith(keyword.ToLower()))).OrderBy(t => t.CompanyName)
                        .Select(t => t).Take(25).ToList();

            return gCompanies.Select(t => new SelectList
            {
                SelectText = t.CompanyName + (t.City != null ? " - " + t.City : ""),
                SelectValue = t.GlobalCompanyId.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetCompanyCountries(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companyCountries = (from t in context.Companies
                                    where t.SubscriberId == subscriberId && !t.Deleted
                                    select new SelectList
                                    {
                                        SelectText = t.CompanyName,
                                        SelectValue = t.CompanyName.ToString()
                                    }).OrderBy(t => t.SelectText).Distinct().ToList();
            return companyCountries;
        }


        public List<SelectList> GetCompanyLinkTypes(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var linkTypes = (from j in context.CompanyLinkTypes
                             where j.SubscriberId == subscriberId
                             select new SelectList
                             {
                                 SelectText = j.CompanyLinkTypeName,
                                 SelectValue = j.CompanyLinkTypeId.ToString()
                             }).OrderBy(t => t.SelectText).Distinct().ToList();
            return linkTypes;
        }


        public List<SelectList> GetCompanySegments(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companySegments = context.CompanySegments.Where(t => t.SubscriberId == subscriberId
                                                            && !t.Deleted).OrderBy(t => t.SortOrder)
                                                         .Select(t => t).ToList();
            return companySegments.Select(t => new SelectList
            {
                SelectText = t.SegmentName,
                SelectValue = t.SegmentCode.ToString()
            }).ToList();
        }


        public List<SelectList> GetCompanyTypes(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companyTypes = context.CompanyTypes.Where(t => t.SubscriberId == subscriberId
                                                    && !t.Deleted).OrderBy(t => t.CompanyTypeName)
                .Select(t => t).ToList();
            return companyTypes.Select(t => new SelectList
            {
                SelectText = t.CompanyTypeName,
                SelectValue = t.CompanyTypeName.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetCompetitors(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var competitors = context.Competitors.Where(t => t.SubscriberId == subscriberId
                                                            && !t.Deleted).OrderBy(t => t.SortOrder)
                                                         .Select(t => t).ToList();
            return competitors.Select(t => new SelectList
            {
                SelectText = t.CompetitorName,
                SelectValue = t.CompetitorName.ToString()
            }).ToList();
        }


        public List<SelectList> GetContacts(int subscriberId, int companyId, string keyword)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            //   var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var contacts = context.Contacts.Where(t => t.SubscriberId == subscriberId
                && (companyId == 0 || t.CompanyId == companyId)
                && !t.Deleted).Select(t => t).ToList();

            if (!string.IsNullOrEmpty(keyword))
            {
                contacts = contacts.Where(t => (t.ContactName).ToLower().StartsWith(keyword.ToLower())).ToList();
            }

            return contacts.Select(t => new SelectList
            {
                SelectText = t.FirstName + " " + t.LastName,
                SelectValue = t.ContactId.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetContactTypes(int subscriberId)
        {
            var securityContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
            var dataCenter = securityContext.GlobalSubscribers.Where(t => t.SubscriberId == subscriberId).Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(dataCenter);
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var contactTypes = context.ContactTypes
                .Where(c => c.SubscriberId == subscriberId && !c.Deleted)
                .OrderBy(c => c.SortOrder)
                .Select(c => new SelectList
                {
                    SelectText = c.ContactTypeName,
                    SelectValue = c.ContactTypeId.ToString()
                }).ToList();
            return contactTypes;
        }


        public List<SelectList> GetCountries()
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.Countries.OrderBy(t => t.SortOrder).Select(l => new SelectList
            {
                SelectText = l.CountryName,
                SelectValue = l.CountryName
            }).ToList();
        }


        public List<SelectList> GetCountriesForSubscriberUsers(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var subscriberCountries = (from c in context.Users
                                       where (c.SubscriberId == subscriberId && !c.Deleted)
                                       orderby c.CountryName
                                       select new SelectList
                                       {
                                           SelectText = c.CountryName,
                                           SelectValue = c.CountryName
                                       }).Distinct().ToList();
            return subscriberCountries;
        }


        public List<SelectList> GetCountriesWithCode(int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.Countries.OrderBy(t => t.SortOrder).Select(l => new SelectList
            {
                SelectText = l.CountryName,
                SelectValue = l.CountryCode
            }).ToList();
        }


        public List<SelectList> GetCountriesWithCodeForSubscriberUsers(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var subscriberCountries = (from c in context.Users
                                       where (c.SubscriberId == subscriberId && !c.Deleted)
                                       orderby c.CountryName
                                       select new SelectList
                                       {
                                           SelectText = c.CountryName,
                                           SelectValue = c.CountryCode
                                       }).Distinct().ToList();
            return subscriberCountries;
        }


        public List<SelectList> GetCurrencies(string currencyCode)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.Currencies.Select(l => new SelectList
            {
                SelectText = l.CurrencyName,
                SelectValue = l.CurrencyCode //+ "|" + l.CurrencySymbol
            }).OrderBy(l => l.SelectText).ToList();
        }


        public List<SelectList> GetDateFormats()
        {
            return new List<SelectList> {
                        new SelectList{ SelectText = "MM/dd/yyyy", SelectValue ="MM/dd/yyyy" },
                        new SelectList{ SelectText = "dd/MM/yyyy", SelectValue ="dd/MM/yyyy" },
            };
        }


        public List<SelectList> GetDealTypes(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var dealTypes = context.DealTypes.Where(t => t.SubscriberId == subscriberId && !t.Deleted).OrderBy(t => t.SortOrder)
                .Select(t => t).ToList();
            return dealTypes.Select(l => new SelectList
            {
                SelectText = l.DealTypeName,
                SelectValue = l.DealTypeName
            }).ToList();
        }


        public List<SelectList> GetDeals(int subscriberId, int companyId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var deals = context.Deals.Where(t => t.SubscriberId == subscriberId && !t.Deleted && (companyId == 0 || t.CompanyId == companyId))
                .Select(t => t).ToList();
            return deals.Select(l => new SelectList
            {
                SelectText = l.DealName,
                SelectValue = l.DealId.ToString()
            }).ToList();
        }


        public List<SelectList> GetDistricts(int subscriberId, int userId = 0, string countryNames = "")
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var districts = (from j in context.Districts
                             where j.SubscriberId == subscriberId && !j.Deleted
                             select j).ToList();

            // filter by country
            if (!string.IsNullOrEmpty(countryNames))
            {
                var cNames = countryNames.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                districts = districts.Where(t => cNames.Contains(t.CountryName)).ToList();
            }

            if (userId > 0)
            {
                var userRole = context.Users.Where(t => t.UserId == userId).Select(t => t.UserRoles).FirstOrDefault() ?? "";
                var countryCode = context.Users.Where(t => t.UserId == userId).Select(t => t.CountryCode).FirstOrDefault() ?? "";
                if (userRole.Contains("Country Manager") || userRole.Contains("Country Admin"))
                {
                    if (!string.IsNullOrEmpty(countryCode))
                        districts = new Districts().GetDistricts(subscriberId).Where(t => t.CountryCode.Equals(countryCode)).ToList();
                }
            }

            return districts.Select(j => new SelectList
            {
                SelectText = j.DistrictName,
                SelectValue = j.DistrictCode
            }).OrderBy(t => t.SelectText).Distinct().ToList();
        }



        public List<SelectList> GetIndustries(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Industries.Where(t => t.SubscriberId == subscriberId && !t.Deleted).OrderBy(t => t.SortOrder)
                .Select(t => new SelectList
                {
                    SelectText = t.IndustryName,
                    SelectValue = t.IndustryName.ToString()
                }).Distinct().ToList();
        }


        public List<SelectList> GetLanguages(int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(connection);
            // var connection = LoginUser.GetConnectionForSubscriberId(subscriberId); 
            var languages = context.Languages.OrderBy(t => t.LanguageName)
                 .Select(t => new SelectList
                 {
                     SelectText = t.LanguageName,
                     SelectValue = t.LanguageCode.ToString()
                 }).OrderBy(t => t.SelectText).Distinct().ToList();
            return languages;
        }


        public string GetLatestMonday()
        {
            var latestMonday = DateTime.Today.AddDays(1 - Convert.ToInt32(DateTime.Today.DayOfWeek));
            if (latestMonday > DateTime.Today)
                latestMonday = latestMonday.AddDays(-7);
            return latestMonday.ToString("yyyy-MM-dd");
        }


        public List<SelectList> GetLocationCountries(int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var countryNames = context.Locations.Where(t => !t.Deleted && t.SubscriberId == subscriberId
                                             && t.CountryName != "").Select(t => t.CountryName).ToList();

            var locationCountries = (from c in sharedContext.Countries
                                     where countryNames.Contains(c.CountryName)
                                     select new SelectList
                                     {
                                         SelectText = c.CountryName,
                                         SelectValue = c.CountryCode
                                     }).Distinct().ToList();
            return locationCountries;
        }


        public List<SelectList> GetLocations(int subscriberId, string keyword, string countryNames, string districtCodes)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var locations = (from l in context.Locations
                             where l.SubscriberId == subscriberId && !l.Deleted
                             orderby l.LocationName
                             select l);

            if (!string.IsNullOrEmpty(keyword))
            {
                locations = (from l in locations
                             where l.LocationName.ToLower().Contains(keyword.ToLower())
                             || l.LocationCode.ToLower().Contains(keyword.ToLower())
                             orderby l.LocationName
                             select l);
            }


            if (!string.IsNullOrEmpty(countryNames))
            {
                var cNames = countryNames.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                locations = (from l in locations
                             where cNames.Contains(l.CountryName)
                             orderby l.LocationName
                             select l);
            }

            if (!string.IsNullOrEmpty(districtCodes))
            {
                var districts = districtCodes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                locations = (from l in locations
                             where districts.Contains(l.DistrictCode)
                             orderby l.LocationName
                             select l);
            }

            // select locations
            var finalList = locations.Select(l => new SelectList
            {
                SelectText = l.LocationName,
                SelectValue = l.LocationCode
            }).Distinct().ToList();

            return finalList;
        }


        public List<SelectList> GetLocations(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var locations = (from l in context.Locations
                             where l.SubscriberId == subscriberId && !l.Deleted
                             orderby l.LocationName
                             select new SelectList
                             {
                                 SelectText = l.LocationName,
                                 SelectValue = l.LocationId.ToString()
                             }).Distinct().ToList();
            return locations;
        }


        public List<SelectList> GetLocationsForSubscriberUsers(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var subscriberLocations = (from l in context.Users
                                       where (l.SubscriberId == subscriberId && !l.Deleted && l.LocationName != null)
                                       orderby l.LocationName
                                       select new SelectList
                                       {
                                           SelectText = l.LocationName,
                                           SelectValue = l.LocationName
                                       }).Distinct().ToList();
            return subscriberLocations;
        }


        public List<SelectList> GetLocationTypes()
        {
            return new List<SelectList> {
                new SelectList{SelectText = "Agent", SelectValue ="Agent" },
                new SelectList{ SelectText = "Branch", SelectValue ="Branch" },
                new SelectList{SelectText = "Office",SelectValue = "Office"},
                new SelectList{SelectText = "Station",SelectValue = "Station"},
                new SelectList{SelectText = "Terminal", SelectValue ="Terminal"} ,
                new SelectList{SelectText = "Warehouse",SelectValue = "Warehouse"}
            };
        }


        public List<SelectList> GetLostReasons(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            //   var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var lostReasons = (from j in context.LostReasons
                               where j.SubscriberId == subscriberId && !j.Deleted
                               select new SelectList
                               {
                                   SelectText = j.LostReasonName,
                                   SelectValue = j.LostReasonName.ToString()
                               }).OrderBy(t => t.SelectText).Distinct().ToList();
            return lostReasons;
        }


        // load all the users managed by the logged in user
        public List<SelectList> GetManagingUsersByUser(int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var user = context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                var userRole = user.UserRoles;
                var userIds = new List<int>();
                // get sales manager user's location codes
                if (user.UserRoles.Contains("Sales Manager"))
                {
                    userIds = (from t in context.LinkUserToManagers
                               where t.ManagerUserId == userId && !t.Deleted
                               select t.UserId).Distinct().ToList();
                }

                if (user.UserRoles.Contains("CRM Admin"))
                {
                    userIds = (from t in context.Users
                               where t.SubscriberId == user.SubscriberId && !t.Deleted
                               select t.UserId).Distinct().ToList();
                }
                else if (user.UserRoles.Contains("Region Manager"))
                {
                    if (!string.IsNullOrEmpty(user.RegionName))
                    {
                        userIds.AddRange(context.Users.Where(u => u.RegionName.Equals(user.RegionName))
                           .Select(u => u.UserId).ToList());
                    }
                }
                else if (user.UserRoles.Contains("Country Manager"))
                {
                    if (!string.IsNullOrEmpty(user.CountryCode))
                    {
                        userIds.AddRange(context.Users.Where(c => c.CountryCode.Equals(user.CountryCode))
                           .Select(c => c.UserId).ToList());
                    }
                }
                else if (user.UserRoles.Contains("District Manager"))
                {
                    if (!string.IsNullOrEmpty(user.DistrictCode))
                    {
                        var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                        if (district != null)
                        {
                            userIds.AddRange(context.Users.Where(u => u.DistrictCode.Equals(district.DistrictCode))
                                   .Select(u => u.UserId).ToList());
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
                            userIds.AddRange(context.Users.Where(u => u.LocationCode.Equals(location.LocationCode)).Select(u => u.UserId)
                               .ToList());
                        }
                    }
                }
                else
                {
                    userIds.Add(user.UserId);
                }

                userIds = userIds.Distinct().ToList();
                return context.Users.Where(t => userIds.Contains(t.UserId) && t.SubscriberId == user.SubscriberId && !t.Deleted)
                       .Select(t => new SelectList
                       {
                           SelectText = t.FirstName + " " + t.LastName,
                           SelectValue = t.UserId.ToString()
                       }).OrderBy(t => t.SelectText).ToList();
            }
            return new List<SelectList>();
        }


        public MondaysResponse GetMondays(int year)
        {
            var response = new MondaysResponse();
            var mondays = new List<string>();
            string mondayDate;
            var allMondays = new List<string>();
            var startOfYear = new DateTime(year, 1, 1);
            var endOfYear = new DateTime(year, 12, 31);
            int daysInYear = endOfYear.DayOfYear;
            for (var i = 1; i <= daysInYear; i++)
            {
                if (startOfYear.DayOfWeek == DayOfWeek.Monday)
                {
                    mondayDate = startOfYear.ToString("yyyy-MM-dd");
                    allMondays.Add(mondayDate);
                }
                startOfYear = startOfYear.AddDays(1);
            }

            response.Mondays = allMondays;
            if (DateTime.Now.Year == year)
            {
                response.DefaultDate = GetLatestMonday();
            }
            else
            {
                response.DefaultDate = allMondays[0];
            }
            return response;
        }


        public List<SelectList> GetProfitTypes()
        {
            return new List<SelectList> {
                        new SelectList{ SelectText = "Percentage", SelectValue ="Percentage" },
                        new SelectList{ SelectText = "Flat Rate", SelectValue ="Flat Rate" },
                        new SelectList{ SelectText = "Per KG", SelectValue ="Per KG" },
                        new SelectList{ SelectText = "Per Container", SelectValue ="Per Container" }
            };
        }


        public List<SelectList> GetRegions(int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);
            var users = (from j in sharedContext.Regions
                         where j.SubscriberId == subscriberId && !j.Deleted
                         select new SelectList
                         {
                             SelectText = j.RegionName,
                             SelectValue = j.RegionName
                         }).OrderBy(t => t.SelectText).Distinct().ToList();
            return users;
        }

        public List<SelectList> GetRegionsShared(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());

            var regions = (from j in sharedContext.Regions
                           where j.SubscriberId == subscriberId && !j.Deleted
                           select new SelectList
                           {
                               SelectText = j.RegionName,
                               SelectValue = j.RegionName
                           }).OrderBy(t => t.SelectText).Distinct().ToList();
            return regions;
        }

        public List<SelectList> GetRegionsByCountry(int subscriberId, string countryCode)
        {
            var connection = LoginUser.GetConnection();
            var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());

            var regions = (from j in sharedContext.LinkCountryRegions
                           where j.SubscriberId == subscriberId && !j.Deleted && j.CountryCode.ToUpper().Equals(countryCode.ToUpper())
                           select new SelectList
                           {
                               SelectText = j.RegionName,
                               SelectValue = j.RegionName
                           }).OrderBy(t => t.SelectText).Distinct().ToList();
            return regions;
        }

        public List<SelectList> GetCountriesByRegion(int subscriberId, string regionName)
        {
            if (regionName == null)
            {
                regionName = "";
            }

            var connection = LoginUser.GetConnection();
            var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());

            var regions = (from j in sharedContext.LinkCountryRegions
                           where j.SubscriberId == subscriberId && !j.Deleted && j.RegionName.ToUpper().Equals(regionName.ToUpper())
                           select new SelectList
                           {
                               SelectText = j.CountryName,
                               SelectValue = j.CountryCode
                           }).OrderBy(t => t.SelectText).Distinct().ToList();
            return regions;
        }


        public List<SelectList> GetReportDateFormats()
        {
            return new List<SelectList> {
                        new SelectList{ SelectText = "MM/dd/yyyy", SelectValue ="MM/dd/yyyy" },
                        new SelectList{ SelectText = "dd/MM/yyyy", SelectValue ="dd/MM/yyyy" },
            };
        }


        public List<SelectList> GetSalesReps(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var salesReps = context.Users.Where(t => t.SubscriberId == subscriberId && !t.Deleted)
            .Select(t => new SelectList
            {
                SelectText = t.FirstName + " " + t.LastName,
                SelectValue = t.UserId.ToString()
            }).OrderBy(t => t.SelectText).ToList();

            return salesReps;
        }

        public List<SelectList> GetSalesRepGlobalUserIds(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);
            var salesReps = context.Users.Where(t => t.SubscriberId == subscriberId && !t.Deleted)
            .Select(t => new SelectList
            {
                SelectText = t.FirstName + " " + t.LastName,
                SelectValue = t.UserIdGlobal.ToString()
            }).OrderBy(t => t.SelectText).ToList();

            return salesReps;
        }

        public List<SelectList> GetLinkedSubsciberSalesRepGlobalUserIds(int subscriberId, string prefix = "")
        {
            var sharedContext = new DbSharedDataContext(LoginUser.GetSharedConnection());
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
            var subscriberIds = new List<int> { subscriberId };
            subscriberIds.AddRange(sharedContext.LinkGlobalSuscriberToSubscribers
                                                  .Where(s => s.GlobalSubscriberId == subscriberId && s.DataCenter != "")
                                                  .Select(s => s.LinkedSubscriberId)
                                                  .ToList());
            var salesReps = loginContext.GlobalUsers.Where(t => subscriberIds.Contains(t.SubscriberId)
            && (prefix == null || prefix == "" || t.FullName.StartsWith(prefix)))
            .Select(t => new SelectList
            {
                SelectText = t.FullName,
                SelectValue = t.GlobalUserId.ToString()
            }).OrderBy(t => t.SelectText).ToList();

            return salesReps;
        }

        public List<SelectList> GetSalesRepsByLocation(int subscriberId, string locationCode)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Users?.Where(t => t.SubscriberId == subscriberId && !t.Deleted && t.LocationCode.ToLower() == locationCode.ToLower())
            .Select(t => new SelectList
            {
                SelectText = t.FirstName + " " + t.LastName,
                SelectValue = t.UserId.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }

        public List<SelectList> GetSalesRepsGlobalByLocation(int subscriberId, string locationCode)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Users?.Where(t => t.SubscriberId == subscriberId && !t.Deleted && t.LocationCode.ToLower() == locationCode.ToLower())
            .Select(t => new SelectList
            {
                SelectText = t.FirstName + " " + t.LastName,
                SelectValue = t.UserIdGlobal.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetAllSalesReps(GlobalSalesRepSearchRequest request)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            var users = context.Users.Where(t => t.SubscriberId == request.SubscriberId && !t.Deleted);

            if (request.CountryCodes != null && request.CountryCodes.Count > 0)
            {
                users = context.Users.Where(t => request.CountryCodes.Contains(t.CountryCode));
            }
            if (request.LocationCodes != null && request.LocationCodes.Count > 0)
            {
                users = context.Users.Where(t => request.LocationCodes.Contains(t.LocationCode));
            }

            return users.Select(t => new SelectList
            {
                SelectText = t.FirstName + " " + t.LastName,
                SelectValue = t.UserIdGlobal.ToString()
            }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetUserLocations(UserLocationRequest request)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var user = context.Users.FirstOrDefault(t => t.UserId == request.UserId);
            if (user != null)
            {
                var locations = new List<Location>();
                var salesManagerLocations = new List<Location>();

                // get sales manager user's location codes
                if (user.UserRoles.Contains("Sales Manager"))
                {
                    var locationCodes = (from t in context.LinkUserToManagers
                                         join j in context.Users on t.UserId equals j.UserId
                                         where t.ManagerUserId == request.UserId && !t.Deleted && !j.Deleted && j.CountryName != ""
                                         select j.LocationCode).Distinct().ToList();

                    salesManagerLocations = (from j in context.Locations
                                             where locationCodes.Contains(j.LocationCode)
                                             && j.SubscriberId == request.SubscriberId && !j.Deleted
                                             && (request.CountryCodes == null || request.CountryCodes.Count == 0 || request.CountryCodes.Contains(j.CountryCode))
                                             select j).Distinct().ToList();
                }

                if (!string.IsNullOrEmpty(user.UserRoles))
                {
                    if (user.UserRoles.Contains("CRM Admin"))
                    {
                        // Admin - don't filter for anything except subscriberId
                        locations = (from j in context.Locations
                                     where j.SubscriberId == request.SubscriberId && !j.Deleted
                                            && (request.CountryCodes == null || request.CountryCodes.Count == 0 || request.CountryCodes.Contains(j.CountryCode))
                                     select j).Distinct().ToList();
                    }
                    else if (user.UserRoles.Contains("Region Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.RegionName))
                        {
                            // region manager - get all locations of users for the region
                            locations = context.Locations
                                 .Where(t => t.RegionName == user.RegionName && t.SubscriberId == user.SubscriberId && !t.Deleted
                                      && (request.CountryCodes == null || request.CountryCodes.Count == 0 || request.CountryCodes.Contains(t.CountryCode)))
                                 .Select(t => t).ToList();


                        }
                    }
                    else if (user.UserRoles.Contains("Country Manager") || user.UserRoles.Contains("Country Admin"))
                    {
                        if (!string.IsNullOrEmpty(user.CountryName))
                            locations = context.Locations
                                .Where(t => t.CountryName == user.CountryName
                                      && t.SubscriberId == user.SubscriberId && !t.Deleted
                                      && (request.CountryCodes == null || request.CountryCodes.Count == 0 || request.CountryCodes.Contains(t.CountryCode)))
                                .Select(t => t).ToList();
                    }
                    else if (user.UserRoles.Contains("District Manager"))
                    {
                        if (!string.IsNullOrEmpty(user.DistrictCode))
                        {
                            var district = new Districts().GetDistrictFromCode(user.DistrictCode, user.SubscriberId);
                            if (district != null)
                            {
                                // country manager - get all locations of users for the country
                                locations = context.Locations
                                                    .Where(t => t.DistrictCode == district.DistrictCode
                                                     && t.SubscriberId == user.SubscriberId && !t.Deleted
                                                              && (request.CountryCodes == null || request.CountryCodes.Count == 0 || request.CountryCodes.Contains(t.CountryCode)))
                                                 .Select(t => t).ToList();
                            }
                        }
                    }
                    else if (user.LocationId > 0)
                    {
                        locations = context.Locations
                                                       .Where(t => t.LocationId == user.LocationId
                                                        && t.SubscriberId == user.SubscriberId && !t.Deleted
                                                                && (request.CountryCodes == null || request.CountryCodes.Count == 0 || request.CountryCodes.Contains(t.CountryCode)))
                                                    .Select(t => t).ToList();
                    }
                }

                // add locations from sales reps
                foreach (var sml in salesManagerLocations)
                {
                    if (locations.FirstOrDefault(t=>t.LocationCode == sml.LocationCode) == null)
                    {
                        locations.Add(sml);
                    } 
                }
               
                locations.Distinct();
            
                // locations
                return locations.Select(t => new SelectList
                {
                    SelectText = t.LocationName,
                    SelectValue = t.LocationCode
                }).OrderBy(t => t.SelectText).ToList();

            }

            return new List<SelectList>();
        }



        public List<SelectList> GetAllSalesReps(int subsbcriberId)
        {

            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var loginConnection = LoginUser.GetLoginConnection();
            var loginContext = new Crm6.App_Code.Login.DbLoginDataContext(loginConnection);

            // get all subscriber ids
            var linkedSubscriberIds = sharedContext.LinkGlobalSuscriberToSubscribers
                                        .Where(t => t.GlobalSubscriberId == subsbcriberId)
                                                .Select(t => t.LinkedSubscriberId).ToList();
            linkedSubscriberIds.Add(subsbcriberId);
            linkedSubscriberIds = linkedSubscriberIds.Distinct().ToList();
            return loginContext.GlobalUsers.Where(t => linkedSubscriberIds.Contains(t.SubscriberId))
                                .Select(t => new SelectList
                                {
                                    SelectText = t.FullName + " - " + t.LocationName,
                                    SelectValue = t.GlobalUserId.ToString()
                                }).OrderBy(t => t.SelectText).ToList();
        }


        public List<SelectList> GetSalesStages(int subscriberId, bool includeWonLostStalled = false)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            //  var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var salesStages = context.SalesStages.Where(s => !s.Deleted && s.SubscriberId == subscriberId)
                .OrderBy(s => s.SortOrder).Select(s => new SelectList
                {
                    SelectText = s.SalesStageName,
                    SelectValue = s.SalesStageId.ToString()
                }).ToList();
            if (includeWonLostStalled)
            {
                salesStages.Add(new SelectList { SelectText = "Won", SelectValue = "-1" });
                salesStages.Add(new SelectList { SelectText = "Lost", SelectValue = "-2" });
                salesStages.Add(new SelectList { SelectText = "Stalled", SelectValue = "-3" });
            }
            return salesStages;
        }


        public List<SelectList> GetSalesTeamRoles(int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var salesTeamUserRoles = (from s in sharedContext.SalesTeamRoles
                                      where !s.Deleted && s.SubscriberId == subscriberId
                                      select s).OrderBy(t => t.SortOrder).Select(t => new SelectList
                                      {
                                          SelectText = t.SalesTeamRole1,
                                          SelectValue = t.SalesTeamRole1
                                      }).ToList();
            return salesTeamUserRoles;
        }


        public List<OriginDestinationLoction> GetServiceLocations(string countryCodes, string service, string keyword)
        {
            keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;
            var cCodes = new List<string>();
            if (!string.IsNullOrEmpty(countryCodes))
            {
                cCodes = countryCodes.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }

            var countryCode = "";
            if (cCodes.Count > 0)
                countryCode = cCodes[0];

            var locationCodes = GleGlobalLocation(cCodes, service, keyword);
            var finalList = locationCodes.OrderBy(t => t.SelectText).ToList();

            if (("various").Contains(keyword))
            {
                finalList.Add(new OriginDestinationLoction
                {
                    SelectText = "Various",
                    SelectValue = "Various",
                    CountryCode = "Various",
                    CountryName = "Various"
                });
            }
            return finalList;

        }


        public List<SelectList> GetServices()
        {
            return new List<SelectList> {
                new SelectList{SelectText = "Air", SelectValue ="Air" },
                new SelectList{SelectText = "Brokerage",SelectValue = "Brokerage"},
                new SelectList{SelectText = "Ocean FCL",SelectValue = "Ocean FCL"},
                new SelectList{SelectText = "Ocean LCL",SelectValue = "Ocean LCL"},
                new SelectList{SelectText = "Road FTL", SelectValue ="Road FTL"} ,
                new SelectList{SelectText = "Road LTL", SelectValue ="Road LTL"},
                new SelectList{SelectText = "Logistics",SelectValue = "Logistics"},
                new SelectList{SelectText = "Warehouse",SelectValue = "Warehouse"},
                new SelectList{SelectText = "RoRo - Breakbulk",SelectValue = "RoRo - Breakbulk"}
            };
        }


        public List<SelectList> GetShippingFrequency()
        {
            return new List<SelectList> {
                        new SelectList{ SelectText = "Per Month", SelectValue ="Per Month" },
                        new SelectList{ SelectText = "Per Year", SelectValue ="Per Year" },
                        new SelectList{ SelectText = "Per Week", SelectValue ="Per Week" },
                        new SelectList{ SelectText = "Spot", SelectValue ="Spot" }
            };
        }


        public List<SelectList> GetSources(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Sources.Where(s => !s.Deleted && s.SubscriberId == subscriberId)
              .OrderBy(s => s.SortOrder).Select(s => new SelectList
              {
                  SelectText = s.SourceName,
                  SelectValue = s.SourceName.ToString()
              }).ToList();
        }


        public List<SelectList> GetTags(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var tags = context.Tags.Where(t => t.SubscriberId == subscriberId
                                            && !t.Deleted).OrderBy(t => t.SortOrder)
                .Select(t => t).ToList();
            return tags.Select(t => new SelectList
            {
                SelectText = t.TagName,
                SelectValue = t.TagName.ToString()
            }).ToList();
        }


        public List<SelectList> GetTimeZones()
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var timezones = sharedContext.TimeZones.OrderBy(t => t.SortOrder)
                .Select(t => new SelectList
                {
                    SelectValue = t.TimeZoneId.ToString(),
                    SelectText = "(" + t.UtcOffset + ") " + t.CityNames
                }).ToList();
            return timezones;
        }


        public List<SelectList> GetTimeZones(int userId = 0, int subscriberId = 0)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var userTimeZone = "";
            if (userId > 0 && subscriberId > 0)
            {
                var userContext = new DbFirstFreightDataContext(LoginUser.GetConnection());
                userTimeZone = userContext.Users.Where(t => t.UserId == userId).Select(t => t.TimeZone).FirstOrDefault();
            }
            var timezones = sharedContext.TimeZones.OrderBy(t => t.SortOrder)
                .Select(t => new SelectList
                {
                    SelectValue = t.TimeZoneId.ToString(),
                    SelectText = "(" + t.UtcOffset + ") " + t.CityNames,
                    Selected = (userTimeZone == t.TimeZoneName)
                }).ToList();
            return timezones;
        }



        public List<OriginDestinationLoction> GleGlobalLocation(List<string> countryCodes, string service, string keyword)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            // get locations
            var locations = sharedContext.GlobalLocations.Where(t => (keyword == "" || t.LocationName.StartsWith(keyword) || t.LocationCode.StartsWith(keyword)));
            if (countryCodes.Count > 0)
                locations = locations.Where(t => (countryCodes == null || countryCodes.Contains(t.CountryCode)));

            // filter by service
            if (!string.IsNullOrEmpty(service))
            {
                switch (service.ToLower())
                {
                    case "air":
                        locations = locations.Where(t => t.Airport);
                        break;
                    case "ocean fcl":
                        locations = locations.Where(t => t.SeaPort);
                        break;
                    case "ocean lcl":
                        locations = locations.Where(t => t.SeaPort);
                        break;
                    case "ocean":
                        locations = locations.Where(t => t.SeaPort);
                        break;
                    case "road fcl":
                        locations = locations.Where(t => !t.Airport);
                        break;
                    case "road ltl":
                        locations = locations.Where(t => !t.Airport);
                        break;
                    case "road":
                        locations = locations.Where(t => !t.Airport);
                        break;
                }
            }

            // pass them as list items 
            return locations.Select(t => new OriginDestinationLoction
            {
                SelectText = t.LocationCode + ((t.LocationName != null && t.LocationName != "") ? " - " + t.LocationName : ""),
                SelectValue = t.LocationCode,
                CountryName = t.CountryName,
                CountryCode = t.CountryCode
            }).OrderBy(t => t.SelectText).ToList();
        }



        public List<SelectList> GetUserRoles()
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var userRoles = sharedContext.UserRoles.OrderBy(t => !t.Deleted)
                .Select(t => new SelectList
                {
                    SelectValue = t.UserRole1.ToString(),
                    SelectText = t.UserRole1
                }).OrderBy(t => t.SelectText).Distinct().ToList();

            var ordereduserRoles = new List<SelectList>();

            if (userRoles.FirstOrDefault(t => t.SelectValue == "Sales Rep") != null)
                ordereduserRoles.Add(userRoles.FirstOrDefault(t => t.SelectValue == "Sales Rep"));
            if (userRoles.FirstOrDefault(t => t.SelectValue == "Sales Manager") != null)
                ordereduserRoles.Add(userRoles.FirstOrDefault(t => t.SelectValue == "Sales Manager"));
            if (userRoles.FirstOrDefault(t => t.SelectValue == "Location Manager") != null)
                ordereduserRoles.Add(userRoles.FirstOrDefault(t => t.SelectValue == "Location Manager"));
            if (userRoles.FirstOrDefault(t => t.SelectValue == "District Manager") != null)
                ordereduserRoles.Add(userRoles.FirstOrDefault(t => t.SelectValue == "District Manager"));
            if (userRoles.FirstOrDefault(t => t.SelectValue == "Country Manager") != null)
                ordereduserRoles.Add(userRoles.FirstOrDefault(t => t.SelectValue == "Country Manager"));
            if (userRoles.FirstOrDefault(t => t.SelectValue == "Country Admin") != null)
                ordereduserRoles.Add(userRoles.FirstOrDefault(t => t.SelectValue == "Country Admin"));
            if (userRoles.FirstOrDefault(t => t.SelectValue == "Region Manager") != null)
                ordereduserRoles.Add(userRoles.FirstOrDefault(t => t.SelectValue == "Region Manager"));
            if (userRoles.FirstOrDefault(t => t.SelectValue == "CRM Admin") != null)
                ordereduserRoles.Add(userRoles.FirstOrDefault(t => t.SelectValue == "CRM Admin"));

            return ordereduserRoles;
        }


        public List<SelectList> GetUsers(int subscriberId, string keyword = "")
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                             .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                             .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            // var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var users = context.Users
                               .Where(t => (string.IsNullOrEmpty(keyword) || t.FullName.StartsWith(keyword)) && t.SubscriberId == subscriberId && !t.Deleted && t.LoginEnabled)
                               .OrderBy(t => t.FullName).ToList();
            return users.Select(t => (new SelectList
            {
                SelectText = t.FullName,
                SelectValue = t.UserId.ToString()
            })).ToList();
        }


        public List<SelectList> GetUsersByCountry(int subscriberId, string countryName)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var users = context.Users
                               .Where(t => t.CountryName == countryName && t.SubscriberId == subscriberId && !t.Deleted && t.LoginEnabled)
                               .OrderBy(t => t.FullName).ToList();
            return users.Select(t => (new SelectList
            {
                SelectText = t.FullName,
                SelectValue = t.UserId.ToString()
            })).ToList();
        }


        public List<SelectList> GetVolumeUnits()
        {
            return new List<SelectList> {
                        new SelectList{ SelectText = "CBMs", SelectValue ="CBMs" },
                        new SelectList{ SelectText = "KGs", SelectValue ="KGs" },
                        new SelectList{ SelectText = "LBs", SelectValue ="LBs" },
                        new SelectList{ SelectText = "TEUs", SelectValue ="TEUs" },
                        new SelectList{ SelectText = "Tonnes", SelectValue ="Tonnes" }
            };
        }


        public List<SelectList> GetWonReasons(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                                 .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                                 .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);

            //   var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var wonReasons = (from j in context.WonReasons
                              where j.SubscriberId == subscriberId && !j.Deleted
                              select new SelectList
                              {
                                  SelectText = j.WonReasonName,
                                  SelectValue = j.WonReasonName.ToString()
                              }).OrderBy(t => t.SelectText).Distinct().ToList();
            return wonReasons;
        }


        public List<SelectList> GetIncoterms(int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(sharedConnection);

            var comms = (from i in context.Incoterms
                         select new SelectList
                         {
                             SelectText = i.Incoterm1,
                             SelectValue = i.IncotermId.ToString()
                         }
                         ).OrderBy(cm => cm.SelectText).ToList();

            return comms;
        }

    }


    public class MondaysResponse
    {
        public List<string> Mondays { get; set; }
        public string DefaultDate { get; set; }
    }

    public class GlobalSalesRepSearchRequest
    {
        public int SubscriberId { get; set; }
        public List<string> LocationCodes { get; set; }
        public List<string> CountryCodes { get; set; }
    }

    public class UserLocationRequest
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public List<string> CountryCodes { get; set; }
    }
}
