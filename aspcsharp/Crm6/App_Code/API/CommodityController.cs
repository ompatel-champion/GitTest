using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class CommodityController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<Commodity> GetCommodities([FromUri]int subscriberId)
        {
            return new Commodities().GetCommodities(subscriberId);
        }


        // TODO: is this USED?????
        [AcceptVerbs("GET")]
        public List<SelectList> GetCommoditiesForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetCommodities(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveCommodity([FromBody]Commodity commodity)
        {
            return new Commodities().SaveCommodity(commodity);
        }


        [AcceptVerbs("GET")]
        public bool DeleteCommodity([FromUri]int commodityId, int userId, int subscriberId)
        {
            return new Commodities().DeleteCommodity(commodityId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new Commodities().ChangeOrder(ids, subscriberId);
        }

    }
}
