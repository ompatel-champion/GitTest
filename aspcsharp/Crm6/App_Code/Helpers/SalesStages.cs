using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class SalesStages
    { 
        public SalesStage GetSalesStage(int salesStageId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.SalesStages.FirstOrDefault(t => t.SalesStageId == salesStageId);
        }


        public string GetSalesStageName(int salesStageId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.SalesStages.Where(t => t.SalesStageId == salesStageId).Select(t => t.SalesStageName).FirstOrDefault();
        }


        public List<SalesStage> GetSalesStages(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                              .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                              .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
         //   var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection); 
            return context.SalesStages.Where(s => !s.Deleted && s.SubscriberId == subscriberId)
                .OrderBy(s => s.SortOrder).Select(t => t).ToList();
        }


        public int SaveSalesStage(SalesStage salesStageDetails)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            // get the sales stage by id or create new sales stage object
            var salesStage = context.SalesStages.FirstOrDefault(l => l.SubscriberId == salesStageDetails.SubscriberId 
                                                                     && l.SalesStageId == salesStageDetails.SalesStageId) ?? new SalesStage();
            // fill details
            salesStage.SalesStageName = salesStageDetails.SalesStageName;
            salesStage.StagePercentage = salesStageDetails.StagePercentage;
            salesStage.UpdateUserId = salesStageDetails.UpdateUserId;
            salesStage.UpdateUserName = new Users().GetUserFullNameById(salesStageDetails.UpdateUserId, salesStageDetails.SubscriberId);
            salesStage.LastUpdate = DateTime.UtcNow;

            if (salesStage.SalesStageId < 1)
            {
                //set sort order
                var maxSortOrderValue = context.SalesStages.Where(l => l.SubscriberId == salesStageDetails.SubscriberId && !l.Deleted)
                                                .Select(l => l.SortOrder).Max() ?? 0;
                // new sales stage - insert
                salesStage.SubscriberId = salesStageDetails.SubscriberId;
                salesStage.CreatedUserId = salesStage.UpdateUserId;
                salesStage.CreatedUserName = salesStage.UpdateUserName;
                salesStage.CreatedDate = DateTime.UtcNow;
                salesStage.SortOrder = maxSortOrderValue + 1;
                context.SalesStages.InsertOnSubmit(salesStage);
            }
            context.SubmitChanges();

            // return the sales stage id
            return salesStage.SalesStageId;
        }


        public bool DeleteSalesStage(int salesStageId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var salesStage = context.SalesStages.FirstOrDefault(t => t.SalesStageId == salesStageId);
            if (salesStage != null)
            {
                salesStage.Deleted = true;
                salesStage.DeletedUserId = userId;
                salesStage.DeletedDate = DateTime.UtcNow;
                salesStage.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var salesStages = context.SalesStages.Where(t => t.SubscriberId == subscriberId).ToList();
            var salesStageIds = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in salesStageIds)
            {
                var stage = salesStages.FirstOrDefault(t => t.SalesStageId == int.Parse(id));
                if (stage != null)
                {
                    stage.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }

    }
}
