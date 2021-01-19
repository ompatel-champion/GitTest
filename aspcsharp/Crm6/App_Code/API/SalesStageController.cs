using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class SalesStageController : ApiController
    {


        [AcceptVerbs("GET")]
        public List<SalesStage> GetSalesStages([FromUri]int subscriberId)
        {
            return new SalesStages().GetSalesStages(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetSalesStagesForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetSalesStages(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveSalesStage([FromBody]SalesStage salesStage)
        {
            return new SalesStages().SaveSalesStage(salesStage);
        }


        [AcceptVerbs("GET")]
        public SalesStage GetSalesStage([FromUri]int salesStageId, int subscriberId)
        {
            return new SalesStages().GetSalesStage(salesStageId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteSalesStage([FromUri]int salesStageId, int userId, int subscriberId)
        {
            return new SalesStages().DeleteSalesStage(salesStageId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new SalesStages().ChangeOrder(ids, subscriberId);
        }

    }
}
