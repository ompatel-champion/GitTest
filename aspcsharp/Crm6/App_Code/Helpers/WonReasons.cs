using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class WonReasons
    {

        public WonReason GetWonReason(int wonReasonId, int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.WonReasons.FirstOrDefault(t => t.WonReasonId == wonReasonId && t.SubscriberId == subscriberId);
        }


        public string GetWonReasonName(int wonReasonId, int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.WonReasons.Where(t => t.WonReasonId == wonReasonId && t.SubscriberId == subscriberId).Select(t => t.WonReasonName).FirstOrDefault();
        }


        public List<WonReason> GetWonReasons(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.WonReasons.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public int SaveWonReason(WonReason wonReasonDetails)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get the won lost reason by id or create new won lost reason object
            var wonReason = context.WonReasons.FirstOrDefault(t => t.SubscriberId == wonReasonDetails.SubscriberId &&
                                                    t.WonReasonId == wonReasonDetails.WonReasonId) ?? new WonReason();
            // fill details
            wonReason.WonReasonName = wonReasonDetails.WonReasonName;
            wonReason.LastUpdate = DateTime.UtcNow;
            wonReason.UpdateUserId = wonReasonDetails.UpdateUserId;
            wonReason.UpdateUserName = new Users().GetUserFullNameById(wonReasonDetails.UpdateUserId, wonReasonDetails.SubscriberId);

            if (wonReason.WonReasonId < 1)
            {
                // set sort order
                var maxSortOrderValue = 1;
                try
                {
                    maxSortOrderValue = context.WonReasons.Where(t => t.SubscriberId == wonReasonDetails.SubscriberId && !t.Deleted)
                                                .Select(t => t.SortOrder).Max();
                }
                catch (Exception) { }


                // new won reason - insert
                wonReason.SubscriberId = wonReasonDetails.SubscriberId;
                wonReason.CreatedUserId = wonReason.UpdateUserId;
                wonReason.CreatedUserName = wonReason.UpdateUserName;
                wonReason.CreatedDate = DateTime.UtcNow;
                wonReason.SortOrder = maxSortOrderValue + 1;
                context.WonReasons.InsertOnSubmit(wonReason);
            }
            context.SubmitChanges();
            // return the won reason id
            return wonReason.WonReasonId;
        }


        public bool DeleteWonReason(int wonReasonId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var wonLostReason = context.WonReasons.FirstOrDefault(t => t.WonReasonId == wonReasonId);
            if (wonLostReason != null)
            {
                wonLostReason.Deleted = true;
                wonLostReason.DeletedUserId = userId;
                wonLostReason.DeletedDate = DateTime.UtcNow;
                wonLostReason.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var wonReasons = context.WonReasons.Where(t => t.SubscriberId == subscriberId).ToList();
            var wonReasonId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in wonReasonId)
            {
                var wonReason = wonReasons.FirstOrDefault(t => t.WonReasonId == int.Parse(id));
                if (wonReason != null)
                {
                    wonReason.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }
    }

}