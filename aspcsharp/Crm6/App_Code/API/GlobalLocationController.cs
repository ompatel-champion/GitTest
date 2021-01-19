using Helpers;
using Models;
using System.Web.Http;
using Crm6.App_Code.Shared;

namespace API
{
    public class GlobalLocationController : ApiController
    {

        [AcceptVerbs("POST")]
        public GlobalLocationListResponse GetGlobalLocations([FromBody]GlobalLocationFilter filters)
        {
            return new GlobalLocations().GetGlobalLocations(filters);
        }


        // global location save
        [AcceptVerbs("POST")]
        public int SaveGlobalLocation([FromBody]GlobalLocationSaveRequest request)
        {
            return new GlobalLocations().SaveGlobalLocation(request);
        }


        [AcceptVerbs("GET")]
        public GlobalLocation GetGlobalLocation([FromUri]int globalLocationId, int subscriberId)
        {
            return new GlobalLocations().GetGlobalLocation(globalLocationId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteGlobalLocation([FromUri]int globalLocationId, int userId, int subscriberId)
        {
            return new GlobalLocations().DeleteGlobalLocation(globalLocationId, userId, subscriberId);
        }

    }

   
}
