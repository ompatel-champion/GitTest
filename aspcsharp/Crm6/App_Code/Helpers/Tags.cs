using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Tags
    {


        public Tag GetTag(int tagId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Tags.FirstOrDefault(t => t.TagId == tagId);
        }


        public string GetTagName(int tagId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Tags.Where(t => t.TagId == tagId).Select(t => t.TagName).FirstOrDefault();
        }


        public List<Tag> GetTags(int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.Tags.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public int SaveTag(Tag tagDetails)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            // get the tag by id or create new tag object
            var tag = context.Tags.FirstOrDefault(t => t.SubscriberId == tagDetails.SubscriberId &&
                                                    t.TagId == tagDetails.TagId) ?? new Tag();
            // fill details
            tag.LastUpdate = DateTime.UtcNow;
            tag.SortOrder = tagDetails.SortOrder;
            tag.TagName = tagDetails.TagName;
            tag.UpdateUserId = tagDetails.UpdateUserId;
            tag.UpdateUserName = new Users().GetUserFullNameById(tagDetails.UpdateUserId, tagDetails.SubscriberId);

            if (tag.TagId < 1)
            {
                // set sort order
                var maxSortOrderValue = 1;
                try
                {
                    maxSortOrderValue = context.Tags.Where(t => t.SubscriberId == tagDetails.SubscriberId 
                                                        && !t.Deleted)
                                                    .Select(t => t.SortOrder).Max();
                }
                catch (Exception) { }


                // new tag - insert
                tag.SubscriberId = tagDetails.SubscriberId;
                tag.CreatedUserId = tag.UpdateUserId;
                tag.CreatedUserName = tag.UpdateUserName;
                tag.CreatedDate = DateTime.UtcNow;
                tag.SortOrder = maxSortOrderValue + 1;
                context.Tags.InsertOnSubmit(tag);
            }
            context.SubmitChanges();
            // return the tag id
            return tag.TagId;
        }


        public bool DeleteTag(int tagId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var tag = context.Tags.FirstOrDefault(t => t.TagId == tagId);
            if (tag != null)
            {
                tag.Deleted = true;
                tag.DeletedUserId = userId;
                tag.DeletedDate = DateTime.UtcNow;
                tag.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var tags = context.Tags.Where(t => t.SubscriberId == subscriberId).ToList();
            var tagIds = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in tagIds)
            {
                var tag = tags.FirstOrDefault(t => t.TagId == int.Parse(id));
                if (tag != null)
                {
                    tag.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }
    }

}