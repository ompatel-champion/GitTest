using Helpers;
using Models;
using System.Web.Http;
using Crm6.App_Code;

namespace API
{
    public class LocationController : ApiController
    {

        [AcceptVerbs("POST")]
        public LocationListResponse GetLocations([FromBody]LocationFilter filters)
        {
            return new Locations().GetLocations(filters);
        }


        // location save
        [AcceptVerbs("POST")]
        public int SaveLocation([FromBody]LocationSaveRequest request)
        {
            return new Locations().SaveLocation(request);
        }


        [AcceptVerbs("GET")]
        public Location GetLocation([FromUri]int locationId, int subscriberId)
        {
            return new Locations().GetLocation(locationId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteLocation([FromUri]int locationId, int userId, int subscriberId)
        {
            return new Locations().DeleteLocation(locationId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public string GetLocationPic([FromUri]int locationId)
        {
            return new Locations().GetLocationPicUrl(locationId);
        }

    }
}
