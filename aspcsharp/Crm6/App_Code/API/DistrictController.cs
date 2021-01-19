using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;

namespace API
{
    public class DistrictController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<District> GetDistricts([FromUri]int subscriberId)
        {
            return new Districts().GetDistricts(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetDistrictsForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetDistricts(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveDistrict([FromBody]District district)
        {
            return new Districts().SaveDistrict(district);
        }


        [AcceptVerbs("GET")]
        public District GetDistrict([FromUri]int districtId , int subscriberId)
        {
            return new Districts().GetDistrict(districtId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteDistrict([FromUri]int districtId, int userId, int subscriberId)
        {
            return new Districts().DeleteDistrict(districtId, userId, subscriberId);
        }

    }
}
