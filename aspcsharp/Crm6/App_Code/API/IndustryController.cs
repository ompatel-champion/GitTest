using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class IndustryController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<Industry> GetIndustries([FromUri]int subscriberId)
        {
            return new Industries().GetIndustries(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetIndustriesForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetIndustries(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveIndustry([FromBody]Industry industry)
        {
            return new Industries().SaveIndustry(industry);
        }

        /// <returns></returns>
        [AcceptVerbs("GET")]
        public Industry GetIndustry([FromUri]int industryId, int subscriberId)
        {
            return new Industries().GetIndustry(industryId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteIndustry([FromUri]int industryId, int userId, int subscriberId)
        {
            return new Industries().DeleteIndustry(industryId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new Industries().ChangeOrder(ids, subscriberId);
        }

    }
}
