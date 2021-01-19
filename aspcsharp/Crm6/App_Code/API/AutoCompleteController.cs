 using System.Collections.Generic;
using Helpers;
using Models; 
using System.Web.Http;

namespace API
{
    public class AutoCompleteController : ApiController
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter">Types: Company/Contact/Deal </param>
        /// <returns></returns> 
        public IEnumerable<AutoComplete> Get([FromUri]  AutoCompleteFilter filter)
        {
            return new AutoCompletes().GetAutoComplete(filter);
        }

        [AcceptVerbs("GET")]
        public AutoComplete GetDealCompany([FromUri]int dealId, int subscriberId)
        {
            return new AutoCompletes().GetDealCompany(dealId, subscriberId);
        }
        
    }
}