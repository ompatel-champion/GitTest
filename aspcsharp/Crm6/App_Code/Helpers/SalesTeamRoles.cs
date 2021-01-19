using System;
using System.Linq;
using Crm6.App_Code.Shared;
using System.Collections.Generic;

namespace Helpers
{
    public class SalesTeamRoles
    {
        public List<SalesTeamRole> GetSalesTeamRoles(int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(connection);

            return sharedContext.SalesTeamRoles.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SalesTeamRole1).Select(t => t).ToList();
        }


        public int SaveSalesTeamRole(SalesTeamRole salesTeamRoleDetails)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);

            var salesTeamRole = sharedContext.SalesTeamRoles.FirstOrDefault(t => t.SubscriberId == salesTeamRoleDetails.SubscriberId &&
                                                    t.SalesTeamRoleId == salesTeamRoleDetails.SalesTeamRoleId) ?? new SalesTeamRole();
            // fill details
            salesTeamRole.SalesTeamRole1 = salesTeamRoleDetails.SalesTeamRole1 ?? ""; // TODO: SalesTeamRoleName
            salesTeamRole.LastUpdate = DateTime.UtcNow;
            salesTeamRole.UpdateUserId = salesTeamRoleDetails.UpdateUserId;
            salesTeamRole.UpdateUserName = new Users().GetUserFullNameById(salesTeamRoleDetails.UpdateUserId, salesTeamRoleDetails.SubscriberId);

            if (salesTeamRole.SalesTeamRoleId < 1)
            {
                // set sort order
                var maxSortOrderValue = 1;
                try
                {
                    maxSortOrderValue = sharedContext.SalesTeamRoles.Where(t => t.SubscriberId == salesTeamRoleDetails.SubscriberId && !t.Deleted)
                                                .Select(t => t.SortOrder).Max();
                }
                catch (Exception) { }

                salesTeamRole.SubscriberId = salesTeamRoleDetails.SubscriberId;
                salesTeamRole.CreatedUserId = salesTeamRole.UpdateUserId;
                salesTeamRole.CreatedUserName = salesTeamRole.UpdateUserName;
                salesTeamRole.CreatedDate = DateTime.UtcNow;
                salesTeamRole.SortOrder = maxSortOrderValue + 1;
                sharedContext.SalesTeamRoles.InsertOnSubmit(salesTeamRole);
            }
            sharedContext.SubmitChanges();
            return salesTeamRole.SalesTeamRoleId;
        }


        public bool DeleteSalesTeamRole(int id, int userId, int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var salesTeamRole = sharedContext.SalesTeamRoles.FirstOrDefault(t => t.SalesTeamRoleId == id);
            if (salesTeamRole != null)
            {
                salesTeamRole.Deleted = true;
                salesTeamRole.DeletedUserId = userId;
                salesTeamRole.DeletedDate = DateTime.UtcNow;
                salesTeamRole.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                sharedContext.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var salesTeamRoles = sharedContext.SalesTeamRoles.Where(t => t.SubscriberId == subscriberId).ToList();
            var salesTeamRoleId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in salesTeamRoleId)
            {
                var salesTeamRole = salesTeamRoles.FirstOrDefault(t => t.SalesTeamRoleId == int.Parse(id));
                if (salesTeamRole != null)
                {
                    salesTeamRole.SortOrder = order;
                    order += 1;
                }
            }
            sharedContext.SubmitChanges();
            return true;
        }

    }
}
