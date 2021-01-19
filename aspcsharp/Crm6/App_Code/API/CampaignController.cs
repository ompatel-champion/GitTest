using Crm6.App_Code.Shared;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class CampaignController : ApiController
    {

        [AcceptVerbs("GET")]
        public List<Campaign> GetCampaigns([FromUri]int subscriberId)
        {
            return new Campaigns().GetCampaigns(subscriberId);
        }


        [AcceptVerbs("POST")]
        public int SaveCampaign([FromBody]Campaign campaign)
        {
            return new Campaigns().SaveCampaign(campaign);
        }


        [AcceptVerbs("GET")]
        public Campaign GetCampaign([FromUri]int campaignId, int subscriberId)
        {
            return new Campaigns().GetCampaign(campaignId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteCampaign([FromUri]int campaignId, int userId, int subscriberId)
        {
            return new Campaigns().DeleteCampaign(campaignId, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool ChangeOrder([FromUri]string ids, int subscriberId)
        {
            return new Campaigns().ChangeOrder(ids, subscriberId);
        }

    }
}
