using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class CompetitorController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<Competitor> GetCompetitors([FromUri]int subscriberId)
        {
            return new Competitors().GetCompetitors(subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetCompetitorsForDropdown([FromUri]int subscriberId)
        {
            return new DropdownHelper().GetCompetitors(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveCompetitor([FromBody]Competitor competitor)
        {
            return new Competitors().SaveCompetitor(competitor);
        }


        [AcceptVerbs("GET")]
        public Competitor GetCompetitor([FromUri]int competitorId, int subscriberId)
        {
            return new Competitors().GetCompetitor(competitorId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteCompetitor([FromUri]int competitorId, int userId, int subscriberId)
        {
            return new Competitors().DeleteCompetitor(competitorId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new Competitors().ChangeOrder(ids, subscriberId);
        }

    }
}
