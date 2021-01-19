using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class CompanySegmentController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<CompanySegment> GetCompanySegments([FromUri]int subscriberId)
        {
            return new CompanySegments().GetCompanySegments(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetCompanySegmentsForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetCompanySegments(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveCompanySegment([FromBody]CompanySegment companySegment)
        {
            return new CompanySegments().SaveCompanySegment(companySegment);
        }


        [AcceptVerbs("GET")]
        public CompanySegment GetCompanySegment([FromUri]int companySegmentId, int subscriberId)
        {
            return new CompanySegments().GetCompanySegment(companySegmentId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteCompanySegment([FromUri]int companySegmentId, int userId, int subscriberId)
        {
            return new CompanySegments().DeleteCompanySegment(companySegmentId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new CompanySegments().ChangeOrder(ids, subscriberId);
        }

    }
}
