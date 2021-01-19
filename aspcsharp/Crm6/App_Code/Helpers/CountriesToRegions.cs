using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Models;
using Country = Crm6.App_Code.Shared.Country;

namespace Helpers
{
    public class CountriesToRegions
    {
        public IQueryable<Country> GetCountries()
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.Countries;
        }

        public Dictionary<string, List<SelectList>> GetRegionGroupedCountries(int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var result = sharedContext.Countries.Join(
                sharedContext.LinkCountryRegions.Where(lcr => lcr.SubscriberId == subscriberId && lcr.RegionName != null && lcr.Deleted == false), 
                x => x.CountryCode, 
                y => y.CountryCode,
                (country, lcr) => new {country, lcr});
            var groups = result.GroupBy(cs => cs.lcr.RegionName);
            var countries = groups.ToDictionary(
                group => group.Key, 
                group => group.Select(
                    o => new SelectList() { SelectText = o.country.CountryName, SelectValue = o.country.CountryCode.ToString(), Selected = true }).ToList()
                );
            var a = countries.Sum(country => country.Value.Count);
            var ungroupedCountries = sharedContext.Countries.Where(country =>
                !sharedContext.LinkCountryRegions.Any(lcr => lcr.SubscriberId == 100 && lcr.CountryCode == country.CountryCode && lcr.Deleted == false));
            var b = ungroupedCountries.Count();
            countries.Add("available", ungroupedCountries.Select(o => new SelectList() { SelectText = o.CountryName, SelectValue = o.CountryCode.ToString(), Selected = false}).ToList());
            return countries;
        }

        public int UpdateCountries(UpdateCountriesResponseObject requestObject)
        {
            var GroupedCountries = requestObject.GroupedCountries;
            var UserId = requestObject.UserId;
            var SubscriberId = requestObject.SubscriberId;
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var links = sharedContext.LinkCountryRegions;

            foreach (var countryGroup in GroupedCountries.Where(gc => gc.Key != "available"))
            {
                foreach (var groupedCountry in countryGroup.Value)
                {
                    var link = links.FirstOrDefault(c => c.CountryName == groupedCountry.SelectText && c.SubscriberId == SubscriberId && c.Deleted == false) ?? new LinkCountryRegion();
                    if (link.RegionName != countryGroup.Key)
                    {
                        link.RegionName = countryGroup.Key;
                        link.AddUpdateStamp(UserId, SubscriberId);
                        if (link.LinkCountryRegionId == 0)
                        {
                            link.CountryCode = groupedCountry.SelectValue;
                            link.CountryName = groupedCountry.SelectText;
                            link.PrepareForInsert(UserId, SubscriberId);
                            sharedContext.LinkCountryRegions.InsertOnSubmit(link);
                        }
                    }
                }
            }

            foreach (var countryGroup in GroupedCountries.Where(gc => gc.Key == "available"))
            {
                foreach (var groupedCountry in countryGroup.Value)
                {
                    var link = links.FirstOrDefault(c => c.CountryName == groupedCountry.SelectText && c.SubscriberId == SubscriberId && c.Deleted == false);
                    link?.PrepareForSoftDelete(UserId, SubscriberId);
                }
            }
            sharedContext.SubmitChanges();
            return 0;
        }

        public class UpdateCountriesResponseObject
        {
            public Dictionary<string, List<SelectList>> GroupedCountries { get; set; }
            public int UserId { get; set; }
            public int SubscriberId { get; set; }
        }

    }
}