using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class ContactTypes
    {


        public ContactType GetContactType(int contactTypeId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.ContactTypes.FirstOrDefault(t => t.ContactTypeId == contactTypeId);
        }


        public string GetContactTypeName(int contactTypeId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.ContactTypes.Where(t => t.ContactTypeId == contactTypeId).Select(t => t.ContactTypeName).FirstOrDefault();
        }


        public List<ContactType> GetContactTypes(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.ContactTypes.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public int SaveContactType(ContactType contactTypeDetails)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get the contact type by id or create new contact type object
            var contactType = context.ContactTypes.FirstOrDefault(t => t.SubscriberId == contactTypeDetails.SubscriberId &&
                                                                 t.ContactTypeId == contactTypeDetails.ContactTypeId) ?? new ContactType();
            // fill details
            contactType.ContactTypeName = contactTypeDetails.ContactTypeName;
            contactType.UpdateUserId = contactTypeDetails.UpdateUserId;
            contactType.UpdateUserName = new Users().GetUserFullNameById(contactTypeDetails.UpdateUserId, contactTypeDetails.SubscriberId);
            contactType.LastUpdate = DateTime.UtcNow;

            if (contactType.ContactTypeId < 1)
            {
                // set sort order
                var maxSortOrderValue = 0;
                var cts = context.ContactTypes.Where(t => t.SubscriberId == contactTypeDetails.SubscriberId && !t.Deleted)
                    .Select(t => t.SortOrder).ToList();
                if (cts.Count > 0)
                {
                    maxSortOrderValue = cts.Max();
                }

                // new contact type - insert
                contactType.SubscriberId = contactTypeDetails.SubscriberId;
                contactType.SortOrder = maxSortOrderValue + 1;
                contactType.CreatedUserId = contactType.UpdateUserId;
                contactType.CreatedUserName = contactType.UpdateUserName;
                contactType.CreatedDate = DateTime.UtcNow;
                context.ContactTypes.InsertOnSubmit(contactType);
            }
            context.SubmitChanges();
            // return the contact type id
            return contactType.ContactTypeId;
        }


        public bool DeleteContactType(int contactTypeId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var contactType = context.ContactTypes.FirstOrDefault(t => t.ContactTypeId == contactTypeId);
            if (contactType != null)
            {
                contactType.Deleted = true;
                contactType.DeletedUserId = userId;
                contactType.DeletedDate = DateTime.UtcNow;
                contactType.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var contactTypes = context.ContactTypes.Where(t => t.SubscriberId == subscriberId).ToList();
            var contactTypeIds = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in contactTypeIds)
            {
                var contactType = contactTypes.FirstOrDefault(t => t.ContactTypeId == int.Parse(id));
                if (contactType != null)
                {
                    contactType.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }
    }

}
