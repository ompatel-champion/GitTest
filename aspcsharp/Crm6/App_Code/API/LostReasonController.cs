using Crm6.App_Code;
using Helpers;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class LostReasonController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<LostReason> GetLostReasons([FromUri]int subscriberId)
        {
            return new LostReasons().GetLostReasons(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveLostReason([FromBody]LostReason wonLostReason)
        {
            return new LostReasons().SaveLostReason(wonLostReason);
        }


        [AcceptVerbs("GET")]
        public bool DeleteLostReason([FromUri]int lostReasonId, int userId, int subscriberId)
        {
            return new LostReasons().DeleteLostReason(lostReasonId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new LostReasons().ChangeOrder(ids, subscriberId);
        }

    }
}
