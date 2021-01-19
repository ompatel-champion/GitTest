using Crm6.App_Code;
using System;
using Crm6;

namespace Helpers {
    public static class GenericDataHelper
    {

        public static void AddUpdateStamp(this IUpdatable entity, int userId, int subscriberId)
        {
            entity.UpdateUserName = new Users().GetUserFullNameById(userId, subscriberId);
            entity.InterimLastUpdate = DateTime.UtcNow;
            entity.UpdateUserId = userId;
        }


        public static void PrepareForInsert(this IInsertable entity, int userId, int subscriberId)
        {
            AddUpdateStamp(entity, userId, subscriberId);
            entity.SubscriberId = subscriberId;
            entity.CreatedUserId = userId;  
            entity.SubscriberId = subscriberId;
            entity.CreatedUserId = userId; 
            entity.CreatedUserName = entity.UpdateUserName;
            entity.InterimCreatedDate = DateTime.UtcNow;
        }


        public static void PrepareForSoftDelete(this IDeletable entity, int userId, int subscriberId)
        {
            entity.Deleted = true;
            entity.DeletedUserId = userId;
            entity.DeletedDate = DateTime.UtcNow;
            entity.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
        }
    }
}

