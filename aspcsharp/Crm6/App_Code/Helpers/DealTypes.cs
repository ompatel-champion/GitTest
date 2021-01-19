using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class DealTypes
    {

        public DealType GetDealType(int dealTypeId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.DealTypes.FirstOrDefault(t => t.DealTypeId == dealTypeId);
        }


        public string GetDealTypeName(int dealTypeId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.DealTypes.Where(t => t.DealTypeId == dealTypeId).Select(t => t.DealTypeName).FirstOrDefault();
        }


        public List<DealType> GetDealTypes(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.DealTypes.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public int SaveDealType(DealType dealTypeDetails)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get the deal type by id or create new deal type object
            var dealType = context.DealTypes.FirstOrDefault(t => t.SubscriberId == dealTypeDetails.SubscriberId &&
                                                    t.DealTypeId == dealTypeDetails.DealTypeId) ?? new DealType();
            // fill details
            dealType.DealTypeName = dealTypeDetails.DealTypeName;
            dealType.UpdateUserId = dealTypeDetails.UpdateUserId;
            dealType.UpdateUserName = new Users().GetUserFullNameById(dealTypeDetails.UpdateUserId, dealTypeDetails.SubscriberId);
            dealType.LastUpdate = DateTime.UtcNow;

            if (dealType.DealTypeId < 1)
            {
                // set sort order
                var maxSortOrderValue = 0;
                var dts = context.DealTypes.Where(t => t.SubscriberId == dealTypeDetails.SubscriberId && !t.Deleted)
                                              .Select(t => t.SortOrder).ToList();
                if (dts.Count > 0)
                {
                    maxSortOrderValue = dts.Max();
                }
              
                 

                // new deal type - insert
                dealType.SubscriberId = dealTypeDetails.SubscriberId;
                dealType.CreatedUserId = dealType.UpdateUserId;
                dealType.CreatedUserName = dealType.UpdateUserName;
                dealType.CreatedDate = DateTime.UtcNow;
                dealType.SortOrder = maxSortOrderValue + 1;
                context.DealTypes.InsertOnSubmit(dealType);
            }
            context.SubmitChanges();
            // return the deal type id
            return dealType.DealTypeId;
        }


        public bool DeleteDealType(int dealTypeId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var dealType = context.DealTypes.FirstOrDefault(t => t.DealTypeId == dealTypeId);
            if (dealType != null)
            {
                dealType.Deleted = true;
                dealType.DeletedUserId = userId;
                dealType.DeletedDate = DateTime.UtcNow;
                dealType.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var dealTypes = context.DealTypes.Where(t => t.SubscriberId == subscriberId).ToList();
            var dealTypeIds = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in dealTypeIds)
            {
                var dealType = dealTypes.FirstOrDefault(t => t.DealTypeId == int.Parse(id));
                if (dealType != null)
                { 
                    dealType.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }
    }

}