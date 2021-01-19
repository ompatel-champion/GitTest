using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class LostReasons
    {
        public LostReason GetLostReason(int LostReasonId, int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.LostReasons.FirstOrDefault(t => t.LostReasonId == LostReasonId);
        }


        public string GetLostReasonName(int LostReasonId, int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.LostReasons.Where(t => t.LostReasonId == LostReasonId).Select(t => t.LostReasonName).FirstOrDefault();
        }


        public List<LostReason> GetLostReasons(int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.LostReasons.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public int SaveLostReason(LostReason LostReasonDetails)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get the Lost reason by id or create new Lost reason object
            var LostReason = context.LostReasons.FirstOrDefault(t => t.SubscriberId == LostReasonDetails.SubscriberId &&
                                                    t.LostReasonId == LostReasonDetails.LostReasonId) ?? new LostReason();
            // fill details
            LostReason.LostReasonName = LostReasonDetails.LostReasonName;
            LostReason.LastUpdate = DateTime.UtcNow;
            LostReason.UpdateUserId = LostReasonDetails.UpdateUserId;
            LostReason.UpdateUserName = new Users().GetUserFullNameById(LostReasonDetails.UpdateUserId, LostReasonDetails.SubscriberId);

            if (LostReason.LostReasonId < 1)
            {
                // set sort order
                var maxSortOrderValue = 1;
                try
                {
                    maxSortOrderValue = context.LostReasons.Where(t => t.SubscriberId == LostReasonDetails.SubscriberId && !t.Deleted)
                                                .Select(t => t.SortOrder).Max();
                }
                catch (Exception) { }


                // new Lost reason - insert
                LostReason.SubscriberId = LostReasonDetails.SubscriberId;
                LostReason.CreatedUserId = LostReason.UpdateUserId;
                LostReason.CreatedUserName = LostReason.UpdateUserName;
                LostReason.CreatedDate = DateTime.UtcNow;
                LostReason.SortOrder = maxSortOrderValue + 1;
                context.LostReasons.InsertOnSubmit(LostReason);
            }
            context.SubmitChanges();
            // return the Lost reason id
            return (int)LostReason.LostReasonId;
        }


        public bool DeleteLostReason(int LostReasonId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var LostReason = context.LostReasons.FirstOrDefault(t => t.LostReasonId == LostReasonId);
            if (LostReason != null)
            {
                LostReason.Deleted = true;
                LostReason.DeletedUserId = userId;
                LostReason.DeletedDate = DateTime.UtcNow;
                LostReason.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var LostReasons = context.LostReasons.Where(t => t.SubscriberId == subscriberId).ToList();
            var LostReasonId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in LostReasonId)
            {
                var LostReason = LostReasons.FirstOrDefault(t => t.LostReasonId == int.Parse(id));
                if (LostReason != null)
                {
                    LostReason.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }
    }

}
