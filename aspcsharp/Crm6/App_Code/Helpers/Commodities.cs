using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Commodities
    {

        public Commodity GetCommodity(int commodityId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Commodities.FirstOrDefault(t => t.CommodityId == commodityId && t.SubscriberId == subscriberId);
        }

        public string GetCommodityName(int commodityId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Commodities.Where(t => t.CommodityId == commodityId && t.SubscriberId == subscriberId).Select(t => t.CommodityName).FirstOrDefault();
        }

        public List<Commodity> GetCommodities(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Commodities.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }

        public int SaveCommodity(Commodity commodityDetails)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get the commodity by id or create new commodity object
            var commodity = context.Commodities.FirstOrDefault(t => t.SubscriberId == commodityDetails.SubscriberId &&
                                                    t.CommodityId == commodityDetails.CommodityId) ?? new Commodity();
            // fill details
            commodity.CommodityName = commodityDetails.CommodityName;
            commodity.UpdateUserId = commodityDetails.UpdateUserId;
            commodity.UpdateUserName = new Users().GetUserFullNameById(commodityDetails.UpdateUserId, commodityDetails.SubscriberId);
            commodity.LastUpdate = DateTime.UtcNow;

            // new commodity- insert
            if (commodity.CommodityId < 1)
            {
                // set sort order
                var maxSortOrderValue = context.Commodities.Where(t => t.SubscriberId == commodityDetails.SubscriberId && !t.Deleted)
                                                .OrderByDescending(t => t.SortOrder).Select(t => t.SortOrder).FirstOrDefault();

                commodity.SubscriberId = commodityDetails.SubscriberId;
                commodity.SortOrder = maxSortOrderValue + 1;
                commodity.CreatedUserId = commodity.CreatedUserId;
                commodity.CreatedUserName = commodity.UpdateUserName;
                commodity.CreatedDate = DateTime.UtcNow;
                context.Commodities.InsertOnSubmit(commodity);
            }
            context.SubmitChanges();
            // return commodity id
            return commodity.CommodityId;
        }

        public bool DeleteCommodity(int commodityId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var commodity = context.Commodities.FirstOrDefault(t => t.CommodityId == commodityId && t.SubscriberId == subscriberId);
            if (commodity != null)
            {
                commodity.Deleted = true;
                commodity.DeletedUserId = userId;
                commodity.DeletedDate = DateTime.UtcNow;
                commodity.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }

        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var commodities = context.Commodities.Where(t => t.SubscriberId == subscriberId).ToList();
            var commodityId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in commodityId)
            {
                var commodity = commodities.FirstOrDefault(t => t.CommodityId == int.Parse(id));
                if (commodity != null)
                {
                    commodity.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }

    }
}
