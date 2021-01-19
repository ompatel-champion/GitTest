using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class TagController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<Tag> GetTags([FromUri]int subscriberId)
        {
            return new Tags().GetTags(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetTagsForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetTags(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveTag([FromBody]Tag tag)
        {
            return new Tags().SaveTag(tag);
        }


        [AcceptVerbs("GET")]
        public Tag GetTag([FromUri]int tagId, int subscriberId)
        {
            return new Tags().GetTag(tagId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteTag([FromUri]int tagId, int userId, int subscriberId)
        {
            return new Tags().DeleteTag(tagId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new Tags().ChangeOrder(ids, subscriberId);
        }

    }
}
