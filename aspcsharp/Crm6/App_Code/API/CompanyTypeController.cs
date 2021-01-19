using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class CompanyTypeController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<CompanyType> GetCompanyTypes([FromUri]int subscriberId)
        {
            return new CompanyTypes().GetCompanyTypes(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetCompanyTypesForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetCompanyTypes(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveCompanyType([FromBody]CompanyType companyType)
        {
            return new CompanyTypes().SaveCompanyType(companyType);
        }


        [AcceptVerbs("GET")]
        public bool DeleteCompanyType([FromUri]int companyTypeId, int userId, int subscriberId)
        {
            return new CompanyTypes().DeleteCompanyType(companyTypeId, userId, subscriberId);
        }

        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new CompanyTypes().ChangeOrder(ids, subscriberId);
        }
    }
}
