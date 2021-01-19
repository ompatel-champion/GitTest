using Crm6.App_Code.Shared;
using Helpers;
using System.Web.Http;

namespace API
{
    public class TimezoneController : ApiController
    {

        [AcceptVerbs("GET")]
        public TimeZone GetTimezone([FromUri] int id)
        {
            return new Timezones().GetTimezone(id);
        }

    }
}
