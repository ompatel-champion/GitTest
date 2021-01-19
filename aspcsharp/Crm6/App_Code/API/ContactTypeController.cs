using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class ContactTypeController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<ContactType> GetContactTypes([FromUri]int subscriberId)
        {
            return new ContactTypes().GetContactTypes(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetContactTypesForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetContactTypes(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveContactType([FromBody]ContactType contactType)
        {
            return new ContactTypes().SaveContactType(contactType);
        }


        [AcceptVerbs("GET")]
        public ContactType GetContactType([FromUri]int contactTypeId, int subscriberId)
        {
            return new ContactTypes().GetContactType(contactTypeId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteContactType([FromUri]int contactTypeId, int userId, int subscriberId)
        {
            return new ContactTypes().DeleteContactType(contactTypeId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new ContactTypes().ChangeOrder(ids, subscriberId);
        }

    }
}
