using Models;
using System.Web.Http;

namespace API
{
    public class EmailController : ApiController
    {

        [AcceptVerbs("POST")]
        public SendEmailResponse SendEmail([FromBody]SendEmailRequest email)
        {
            //return new Emails().SendEmail(email);
            return null;
        }


        [AcceptVerbs("POST")]
        public EmailListResponse GetEmails([FromBody]EmailFilter filter)
        {
            // return new Emails().GetEmails(filter);
            return null;
        }

    }
}
