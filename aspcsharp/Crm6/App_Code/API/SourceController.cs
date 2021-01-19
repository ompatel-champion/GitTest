using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class SourceController : ApiController
    {


        [AcceptVerbs("GET")]
        public List<Source> GetSources([FromUri]int subscriberId)
        {
            return new Sources().GetSources(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetSourcesForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetSources(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveSource([FromBody]Source source)
        {
            return new Sources().SaveSource(source);
        }


        [AcceptVerbs("GET")]
        public Source GetSource([FromUri]int sourceId, int subscriberId)
        {
            return new Sources().GetSource(sourceId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteSource([FromUri]int sourceId, int userId, int subscriberId)
        {
            return new Sources().DeleteSource(sourceId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new Sources().ChangeOrder(ids, subscriberId);
        }

    }
}
