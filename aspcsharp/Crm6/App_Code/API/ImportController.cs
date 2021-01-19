using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Crm6.App_Code.Helpers;

namespace API
{
    public class ImportController : ApiController
    {
        [AcceptVerbs("GET")]
        public bool CompanyContactImport([FromUri]int subscriberId, int userId, string blobReference, string containerReference)
        {
            return new ImportCompanies().ImportCompaniesContacts(subscriberId, userId, blobReference, containerReference);
        }
         
        [AcceptVerbs("GET")]
        public bool DealImport([FromUri]int subscriberId, int userId, string blobReference, string containerReference)
        {
            return new Helpers.ImportDeals().PerformDealsImport(subscriberId, userId, blobReference, containerReference);
        }
        
    }
}
