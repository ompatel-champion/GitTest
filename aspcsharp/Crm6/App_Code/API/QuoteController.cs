using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Crm6.App_Code.API
{
    public class QuoteController : ApiController
    {

        [System.Web.Http.AcceptVerbs("POST")]
        public int SaveQuote([FromBody]Quote quote)
        {
            var subscriberId = HttpContext.Current.Session["subscriberId"] as int? ?? 0;
            return new Helpers.QuotesHelper().SaveQuote(quote, subscriberId);
        }
    }
}