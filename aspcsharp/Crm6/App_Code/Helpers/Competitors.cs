using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Competitors
    {

        public Competitor GetCompetitor(int competitorId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Competitors.FirstOrDefault(t => t.CompetitorId == competitorId && t.SubscriberId == subscriberId);
        }


        public string GetCompetitorName(int competitorId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Competitors.Where(t => t.CompetitorId == competitorId && t.SubscriberId == subscriberId).Select(t => t.CompetitorName).FirstOrDefault();
        }


        public List<Competitor> GetCompetitors(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Competitors.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public int SaveCompetitor(Competitor competitorDetails)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get the commodity by id or create new commodity object
            var competitor = context.Competitors.FirstOrDefault(t => t.SubscriberId == competitorDetails.SubscriberId &&
                                                    t.CompetitorId == competitorDetails.CompetitorId) ?? new Competitor();
            // fill details
            competitor.CompetitorName = competitorDetails.CompetitorName;
            competitor.UpdateUserId = competitorDetails.UpdateUserId;
            competitor.UpdateUserName = new Users().GetUserFullNameById(competitorDetails.UpdateUserId, competitorDetails.SubscriberId);
            competitor.LastUpdate = DateTime.UtcNow;

            // new commodity- insert
            if (competitor.CompetitorId < 1)
            {
                // set sort order
                var maxSortOrderValue = context.Competitors.Where(t => t.SubscriberId == competitor.SubscriberId && !t.Deleted)
                                                .OrderByDescending(t => t.SortOrder).Select(t => t.SortOrder).FirstOrDefault();

                competitor.SubscriberId = competitorDetails.SubscriberId;
                competitor.SortOrder = maxSortOrderValue + 1;
                competitor.CreatedUserId = competitor.CreatedUserId;
                competitor.CreatedUserName = competitor.UpdateUserName;
                competitor.CreatedDate = DateTime.UtcNow;
                context.Competitors.InsertOnSubmit(competitor);
            }
            context.SubmitChanges();
            // return competitor id
            return competitor.CompetitorId;
        }


        public bool DeleteCompetitor(int competitorId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var competitor = context.Competitors.FirstOrDefault(t => t.CompetitorId == competitorId && t.SubscriberId == subscriberId );
            if (competitor != null)
            {
                competitor.Deleted = true;
                competitor.DeletedUserId = userId;
                competitor.DeletedDate = DateTime.UtcNow;
                competitor.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var competitors = context.Competitors.Where(t => t.SubscriberId == subscriberId).ToList();
            var competitorId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in competitorId)
            {
                 var competitor = competitors.FirstOrDefault(t => t.CompetitorId == int.Parse(id) );
                if (competitor != null)
                {
                    competitor.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }
    }
}
