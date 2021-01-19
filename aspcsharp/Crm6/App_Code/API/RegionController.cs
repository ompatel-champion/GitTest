using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class RegionController : ApiController
    {


        [AcceptVerbs("GET")]
        public List<Region> GetRegions([FromUri]int subscriberId)
        {
            return new Regions().GetRegions(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetRegionsForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetRegions(subscriberId);
        }

        [AcceptVerbs("GET")]
        public List<SelectList> GetRegionsForDropdown([FromUri]int subscriberId, [FromUri]string countryCode)
        {
            return new DropdownHelper().GetRegionsByCountry(subscriberId, countryCode);
        }

        [AcceptVerbs("GET")]
        public List<SelectList> GetCountriesByRegionForDropdown([FromUri]int subscriberId, [FromUri]string regionName)
        {
            return new DropdownHelper().GetCountriesByRegion(subscriberId, regionName);
        }


        [AcceptVerbs("POST")]
        public int SaveRegion([FromBody]Region region)
        {
            return new Regions().SaveRegion(region);
        }


        [AcceptVerbs("GET")]
        public Region GetRegion([FromUri]int regionId, int subscriberId)
        {
            return new Regions().GetRegion(regionId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteRegion([FromUri]int regionId, int userIdGlobal, int subscriberId)
        {
            return new Regions().DeleteRegion(regionId, userIdGlobal, subscriberId);
        }

    }
}
