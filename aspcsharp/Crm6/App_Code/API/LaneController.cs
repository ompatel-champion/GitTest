using System.Web.Http;
using Helpers;
using Models;
using Crm6.App_Code;

namespace API
{
    public class LaneController : ApiController
    {

        [AcceptVerbs("POST")]
        public LaneListResponse GetLanes([FromBody]LaneFilter filters)
        {
            return new Lanes().GetLanes(filters);
        }


        // lane save
        [AcceptVerbs("POST")]
        public int SaveLane([FromBody]Lane lane)
        {
            return new Lanes().SaveLane(lane);
        }


        [AcceptVerbs("GET")]
        public LaneModel GetLane([FromUri]int laneId, int subscriberId)
        {
            return new Lanes().GetLane(laneId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteLane(int laneId, int userId, int laneSubscriberId)
        {
            return new Lanes().DeleteLane(laneId, userId, laneSubscriberId);
        }

    }
}
