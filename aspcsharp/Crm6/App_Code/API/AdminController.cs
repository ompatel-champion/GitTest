using Helpers;
using System.Web.Http;

namespace API
{
    public class AdminController : ApiController
    {

        [AcceptVerbs("GET")]
        public bool UpdateLastActivityDates([FromUri]int subscriberId)
        {
            return new Admin().UpdateLastActivityDates(subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool HkgCompanyImport([FromUri]int subscriberId, string blobReference, string containerReference)
        {
            return new Admin().HkgCompanyImport(subscriberId, blobReference, containerReference);
        }


        [AcceptVerbs("GET")]
        public bool VisaGlobalCompanyImport([FromUri]int subscriberId, string blobReference, string containerReference)
        {
            return new Admin().VisaGlobalCompanyImport(subscriberId, blobReference, containerReference);
        }


        [AcceptVerbs("GET")]
        public bool HashPasswords()
        {
            return new Admin().HashPasswords();
        }

    }
}
