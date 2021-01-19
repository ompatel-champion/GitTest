using Crm6.App_Code;
using Crm6.App_Code.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Regions
    {

        public Region GetRegion(int regionId, int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(connection);
            return context.Regions.FirstOrDefault(t => t.RegionId == regionId && t.SubscriberId == subscriberId);
        }


        public string GetRegionName(int regionId, int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(connection);
            return context.Regions.Where(t => t.RegionId == regionId && t.SubscriberId == subscriberId).Select(t => t.RegionName).FirstOrDefault();
        }

         
        public List<Region> GetRegions(int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(connection);
            return context.Regions.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                                                             .OrderBy(t => t.RegionName).Select(t => t).ToList();
        }


        public int SaveRegion(Region regionDetails)
        {
            var connection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(connection);

            // get the region by id or create new region object
            var region = context.Regions.FirstOrDefault(t => t.SubscriberId == regionDetails.SubscriberId && t.RegionId == regionDetails.RegionId) ?? new Region();
            // fill details  
            region.RegionManagerUserIdGlobal = regionDetails.RegionManagerUserIdGlobal;
            region.RegionName = regionDetails.RegionName;
            region.UpdateUserIdGlobal = regionDetails.UpdateUserIdGlobal;
            region.UpdateUserName = new Users().GetUserFullNameByUserIdGlobal(regionDetails.UpdateUserIdGlobal);
            region.LastUpdate = DateTime.UtcNow;

            if (region.RegionId < 1)
            {
                // new region - insert
                region.SubscriberId = regionDetails.SubscriberId;
                region.CreatedUserIdGlobal = region.UpdateUserIdGlobal;
                region.CreatedUserName = region.UpdateUserName;
                region.CreatedDate = DateTime.UtcNow;
                context.Regions.InsertOnSubmit(region);
            }
            context.SubmitChanges();
            // return the region id
            return region.RegionId;
        }


        public bool DeleteRegion(int regionId, int userIdGlobal, int subscriberId)
        {
            var connection = LoginUser.GetSharedConnection();
            var context = new DbSharedDataContext(connection);
            var region = context.Regions.FirstOrDefault(t => t.RegionId == regionId);
            if (region != null)
            {
                region.Deleted = true;
                region.DeletedUserIdGlobal = userIdGlobal;
                region.DeletedDate = DateTime.UtcNow;
                region.DeletedUserName = new Users().GetUserFullNameByUserIdGlobal(userIdGlobal);
                context.SubmitChanges();
                return true;
            }
            return false;
        }

    }
}
