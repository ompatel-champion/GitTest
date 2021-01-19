using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class DealTypeController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<DealType> GetDealTypes([FromUri]int subscriberId)
        {
            return new DealTypes().GetDealTypes(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetDealTypesForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetDealTypes(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveDealType([FromBody]DealType dealType)
        {
            return new DealTypes().SaveDealType(dealType);
        }


        [AcceptVerbs("GET")]
        public DealType GetDealType([FromUri]int dealTypeId, int subscriberId)
        {
            return new DealTypes().GetDealType(dealTypeId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteDealType([FromUri]int dealTypeId, int userId, int subscriberId)
        {
            return new DealTypes().DeleteDealType(dealTypeId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new DealTypes().ChangeOrder(ids, subscriberId);
        }

    }
}
