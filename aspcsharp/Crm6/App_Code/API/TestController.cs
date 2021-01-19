using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class TestController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<DealType> GetDealTypes([FromUri]int subscriberId)
        {
            return new DealTypes().GetDealTypes(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveDealType([FromBody]DealType dealType)
        {
            return new DealTypes().SaveDealType(dealType);
        }


    }
}
