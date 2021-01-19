using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class Sources
    {


        public Source GetSource(int sourceId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Sources.FirstOrDefault(t => t.SourceId == sourceId);
        }


        public string GetSourceName(int sourceId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Sources.Where(t => t.SourceId == sourceId).Select(t => t.SourceName).FirstOrDefault();
        }


        public List<Source> GetSources(int subscriberId)
        {
            var securityContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
            var dataCenter = securityContext.GlobalSubscribers.Where(t => t.SubscriberId == subscriberId).Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(dataCenter);

            var context = new DbFirstFreightDataContext(connection);

            return context.Sources.Where(t => !t.Deleted 
                                    && t.SubscriberId == subscriberId)
                                    .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public int SaveSource(Source sourceDetails)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);

            // get the source by id or create new source object
            var source = context.Sources.FirstOrDefault(t => t.SubscriberId == sourceDetails.SubscriberId 
                                                             && t.SourceId == sourceDetails.SourceId) ?? new Source();
            // fill details
            source.SourceName = sourceDetails.SourceName;
            source.UpdateUserId = sourceDetails.UpdateUserId;
            source.UpdateUserName = new Users().GetUserFullNameById(sourceDetails.UpdateUserId, sourceDetails.SubscriberId);
            source.LastUpdate = DateTime.UtcNow;

            if (source.SourceId < 1)
            {
                // set sort order
                var maxSortOrderValue = context.Sources.Where(t => t.SubscriberId == sourceDetails.SubscriberId && !t.Deleted)
                                                .Select(t => t.SortOrder).Max();
                // new source - insert
                source.SubscriberId = sourceDetails.SubscriberId;
                source.SortOrder = maxSortOrderValue + 1;
                source.CreatedUserId = source.UpdateUserId;
                source.CreatedUserName = source.UpdateUserName;
                source.CreatedDate = DateTime.UtcNow;
                context.Sources.InsertOnSubmit(source);
            }
            context.SubmitChanges();
            // return the source id
            return source.SourceId;
        }


        public bool DeleteSource(int sourceId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var source = context.Sources.FirstOrDefault(t => t.SourceId == sourceId);
            if (source != null)
            {
                source.Deleted = true;
                source.DeletedUserId = userId;
                source.DeletedDate = DateTime.UtcNow;
                source.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var sources = context.Sources.Where(t => t.SubscriberId == subscriberId).ToList();
            var sourceId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in sourceId)
            {
                var source = sources.FirstOrDefault(t => t.SourceId == int.Parse(id));
                if (source != null)
                {
                    source.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }
    }
}
