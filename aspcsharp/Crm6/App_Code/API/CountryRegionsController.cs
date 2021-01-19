using System.Collections.Generic;
using Helpers;
using Models;
using System.Web.Http;

namespace API
{
    public class CountryRegionsController : ApiController
    {
        private CountriesToRegions _countriesToRegions;
        private readonly Countries _countries;

        public CountryRegionsController()
        {
            _countriesToRegions = new CountriesToRegions();
            _countries = new Countries();
        }

        [AcceptVerbs("GET")]
        public Dictionary<string, List<SelectList>> GetCountries([FromUri]int subscriberId)
        {
            var countries = _countries.GetRegionGroupedCountries(subscriberId);
            return countries;
        }


        [AcceptVerbs("POST")]
        public int UpdateCountries([FromBody]CountriesToRegions.UpdateCountriesResponseObject requestObject)
        {
            _countriesToRegions.UpdateCountries(requestObject);
            return 0;
        }

    }
}
