using System.Collections.Generic;
using System.Linq;
using Crm6.App_Code;
using Crm6.App_Code.Shared;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Models;
using Country = Crm6.App_Code.Shared.Country;

namespace Helpers
{
    public class Countries
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
                sharedContext.LinkCountryRegions.Where(lcr => lcr.SubscriberId == subscriberId && (lcr.RegionName != null && lcr.RegionName != "") && lcr.Deleted == false),
                x => x.CountryCode,
                y => y.CountryCode,
                (country, lcr) => new { country, lcr });
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
            countries.Add("available", ungroupedCountries.Select(o => new SelectList() { SelectText = o.CountryName, SelectValue = o.CountryCode.ToString(), Selected = false }).ToList());
            return countries;
        }

        public Country GetCountry(int countryId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var country = sharedContext.Countries.Where(t => t.CountryId == countryId).FirstOrDefault();
            return country;
        }


        public string GetCountryNameFromCountryCode(string countryCode)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var countryName = sharedContext.Countries.Where(t => t.CountryCode.ToLower() == countryCode.ToLower()).Select(t => t.CountryName).FirstOrDefault();
            return countryName;
        }


        public Crm6.App_Code.Shared.Country GetCountryFromCountryName(string countryName)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var country = sharedContext.Countries.Where(c => c.CountryName.ToLower() == countryName.ToLower()).Select(c => c).FirstOrDefault();
            return country;
        }


        public string GetCountryCodeFromCountryName(string countryName)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.Countries.Where(c => c.CountryName.ToLower() == countryName.ToLower()).Select(c => c.CountryCode).FirstOrDefault();
        }

    }
}