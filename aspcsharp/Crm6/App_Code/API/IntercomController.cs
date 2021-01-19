using Helpers;
using Models;
using System.Web.Http;
namespace API
{
    public class IntercomController : ApiController
    {

        [AcceptVerbs("GET")]
        public IntercomModel GetIntercomUserData([FromUri]int userId, int subscriberId)
        {
            return new IntercomHelper().GetIntercomData(userId, subscriberId);
        }

    }
}
