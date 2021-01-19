using System.Collections.Generic;
using Crm6.App_Code;
using System.Linq;
using System;

namespace Helpers
{
    public class DistrictManagers
    {

        ///// <summary>
        ///// get district managers for subscriber
        ///// </summary>
        ///// <param name="subscriberId"></param>
        ///// <returns></returns>
        //public List<DistrictManager> GetDistrictManagers(int subscriberId)
        //{
        //    var context = new DbFirstFreightDataContext();
        //    return context.DistrictManagers.Where(t => t.SubscriberId == subscriberId && !t.Deleted).Select(t => t).ToList();
        //}




        ///// <summary>
        ///// save district manager
        ///// </summary>
        ///// <param name="districtManagerDetails"></param>
        ///// <returns></returns>
        //public int SaveDistrictManager(DistrictManager districtManagerDetails)
        //{
        //    var context = new DbFirstFreightDataContext();

        //    // get the district manager by id or create new district manager object
        //    var districtManager = context.DistrictManagers.FirstOrDefault(l => l.DistrictManagerId == districtManagerDetails.DistrictManagerId) ?? new DistrictManager();

        //    // fill update details
        //    districtManager.DistrictId = districtManagerDetails.DistrictId;
        //    districtManager.DistrictName = new Districts().GetDistrictName(districtManagerDetails.DistrictId);
        //    var user = new Users().GetUser(districtManagerDetails.DistrictManagerUserId);
        //    if (user != null)
        //    {
        //        districtManager.DistrictManagerUserId = districtManagerDetails.DistrictManagerUserId;
        //        districtManager.DistrictManagerName = user.User.FullName;
        //        districtManager.DistrictManagerTitle = user.User.Title;
        //    }
        //    else
        //        return 0;

        //    districtManager.LastUpdate = DateTime.UtcNow;
        //    districtManager.UpdateUserId = districtManagerDetails.UpdateUserId;
        //    districtManager.UpdateUserName = new Users().GetUserFullNameById(districtManagerDetails.UpdateUserId);

        //    if (districtManager.DistrictManagerId < 1)
        //    {
        //        // new district manager - insert
        //        districtManager.SubscriberId = districtManagerDetails.SubscriberId;
        //        districtManager.CreatedUserId = districtManager.UpdateUserId;
        //        districtManager.CreatedUserName = districtManager.UpdateUserName;
        //        districtManager.CreatedDate = DateTime.UtcNow;
        //        context.DistrictManagers.InsertOnSubmit(districtManager);
        //    }
        //    context.SubmitChanges();

        //    // return the district manager id
        //    return districtManager.DistrictManagerId;
        //}


        ///// <summary>
        ///// delete district manger
        ///// </summary>
        ///// <param name="districtMangerId"></param>
        ///// <param name="userId"></param>
        ///// <returns></returns>
        //public bool DeleteDistrictManager(int districtMangerId, int userId)
        //{
        //    var context = new DbFirstFreightDataContext();
        //    var districtManger = context.DistrictManagers.FirstOrDefault(t => t.DistrictManagerId == districtMangerId);
        //    if (districtManger != null)
        //    {
        //        districtManger.Deleted = true;
        //        districtManger.DeletedById = userId;
        //        districtManger.DateDeleted = DateTime.UtcNow; 
        //        context.SubmitChanges();
        //        return true;
        //    }
        //    return false;
        //}

    }
}