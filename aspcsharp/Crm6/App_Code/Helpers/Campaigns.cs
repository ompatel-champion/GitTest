using Crm6.App_Code.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Campaigns
    {

        public Campaign GetCampaign(int campaignId, int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);
            return sharedContext.Campaigns.FirstOrDefault(t => t.CampaignId == campaignId && t.SubscriberId == subscriberId);
        }


        public List<Campaign> GetCampaigns(int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);
            return sharedContext.Campaigns.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.CampaignName).Select(t => t).ToList();
        }


        public int SaveCampaign(Campaign campaignDetails)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);

            // get the campaign by id or create new campaign object
            var campaign = sharedContext.Campaigns.FirstOrDefault(t => t.SubscriberId == campaignDetails.SubscriberId 
                                                                                    && t.CampaignId == campaignDetails.CampaignId) ?? new Campaign();
            // fill campaign details
            campaign.CampaignName = string.IsNullOrEmpty(campaignDetails.CampaignName) ? "" : campaignDetails.CampaignName;
            campaign.CampaignNumber = string.IsNullOrEmpty(campaignDetails.CampaignNumber) ? "" : campaignDetails.CampaignNumber;
            campaign.CampaignOwnerUserIdGlobal = campaignDetails.CampaignOwnerUserIdGlobal;
            campaign.CampaignOwnerName = new Users().GetUserFullNameById(campaignDetails.UpdateUserIdGlobal, campaignDetails.SubscriberId);
            campaign.CampaignStatus = string.IsNullOrEmpty(campaignDetails.CampaignStatus) ? "" : campaignDetails.CampaignStatus;
            campaign.CampaignType = string.IsNullOrEmpty(campaignDetails.CampaignType) ? "" : campaignDetails.CampaignType;
            campaign.Comments = campaignDetails.Comments;
            campaign.EndDate = campaignDetails.EndDate;
            campaign.LastUpdate = DateTime.UtcNow;
            campaign.StartDate = campaignDetails.StartDate;
            campaign.UpdateUserIdGlobal = campaignDetails.UpdateUserIdGlobal;
            campaign.UpdateUserName = new Users().GetUserFullNameById(campaignDetails.UpdateUserIdGlobal, campaignDetails.SubscriberId);
            campaign.CampaignOwnerName = new Users().GetUserFullNameByUserIdGlobal(campaignDetails.CampaignOwnerUserIdGlobal);
            campaign.CampaignOwnerUserIdGlobal = campaignDetails.CampaignOwnerUserIdGlobal;
            if (campaign.CampaignId < 1)
            {
                // add campaign 
                // set sort order
                var campaignSortOrders = sharedContext.Campaigns.Where(t => t.SubscriberId == campaignDetails.SubscriberId
                                                                    && !t.Deleted)
                                                                .Select(t => t.SortOrder)
                                                                .ToList(); 
                var maxSortOrderValue = 0;
                if (campaignSortOrders.Count > 0)
                {
                    maxSortOrderValue = campaignSortOrders.Max();
                }

                // new campaign - insert
                campaign.SubscriberId = campaignDetails.SubscriberId;
                campaign.CreatedUserIdGlobal = campaign.UpdateUserIdGlobal;
                campaign.CreatedUserName = campaign.UpdateUserName;
                campaign.CreatedDate = DateTime.UtcNow;
                campaign.SortOrder = maxSortOrderValue + 1;
                // add campaign
                sharedContext.Campaigns.InsertOnSubmit(campaign);
            }
            // update campaign
            sharedContext.SubmitChanges();
            // return the campaign id
            return campaign.CampaignId;
        }


        public bool DeleteCampaign(int campaignId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);
            var campaign = sharedContext.Campaigns.FirstOrDefault(t => t.CampaignId == campaignId && t.SubscriberId == subscriberId);
            if (campaign != null)
            {
                campaign.Deleted = true;
                campaign.DeletedUserIdGlobal = userId;
                campaign.DeletedDate = DateTime.UtcNow;
                campaign.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                sharedContext.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);
            var campaigns = sharedContext.Campaigns.Where(t => t.SubscriberId == subscriberId).ToList();
            var campaignId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in campaignId)
            {
                var campaign = campaigns.FirstOrDefault(t => t.CampaignId == int.Parse(id));
                if (campaign != null)
                {
                    campaign.SortOrder = order;
                    order += 1;
                }
            }
            sharedContext.SubmitChanges();
            return true;
        }
    }
}
