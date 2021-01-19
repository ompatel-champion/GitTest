using Crm6.App_Code;
using Helpers;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class WonReasonController : ApiController
    {


        [AcceptVerbs("GET")]
        public List<WonReason> GetWonReasons([FromUri]int subscriberId)
        {
            return new WonReasons().GetWonReasons(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveWonReason([FromBody]WonReason wonLostReason)
        {
            return new WonReasons().SaveWonReason(wonLostReason);
        }


        [AcceptVerbs("GET")]
        public bool DeleteWonReason([FromUri]int wonReasonId, int userId, int subscriberId)
        {
            return new WonReasons().DeleteWonReason(wonReasonId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new WonReasons().ChangeOrder(ids, subscriberId);
        }

    }
}
