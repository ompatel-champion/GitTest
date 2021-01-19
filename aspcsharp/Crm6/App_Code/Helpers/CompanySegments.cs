using Crm6.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class CompanySegments
    {


        public CompanySegment GetCompanySegment(int companySegmentId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.CompanySegments.FirstOrDefault(t => t.CompanySegmentId == companySegmentId);
        }


        public string GetCompanySegmentName(int companySegmentId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.CompanySegments.Where(t => t.CompanySegmentId == companySegmentId).Select(t => t.SegmentName).FirstOrDefault();
        }


        public List<CompanySegment> GetCompanySegments(int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.CompanySegments.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public int SaveCompanySegment(CompanySegment companySegmentDetails)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get the company segment by id or create new company segment object
            var companySegment = context.CompanySegments.FirstOrDefault(t => t.SubscriberId == companySegmentDetails.SubscriberId &&
                                                    t.CompanySegmentId == companySegmentDetails.CompanySegmentId) ?? new CompanySegment();
            // fill details
            companySegment.SegmentName = companySegmentDetails.SegmentName; 
            companySegment.SegmentCode = string.IsNullOrEmpty(companySegmentDetails.SegmentCode) ? "" : companySegmentDetails.SegmentCode;
            companySegment.UpdateUserId = companySegmentDetails.UpdateUserId;
            companySegment.UpdateUserName = new Users().GetUserFullNameById(companySegmentDetails.UpdateUserId, companySegmentDetails.SubscriberId);
            companySegment.LastUpdate = DateTime.UtcNow;

            if (companySegment.CompanySegmentId < 1)
            {
                // set sort order
                var maxSortOrderValue = context.CompanySegments.Where(t => t.SubscriberId == companySegmentDetails.SubscriberId && !t.Deleted)
                                                .Select(t => t.SortOrder).Max();

                // new company segment - insert
                companySegment.SubscriberId = companySegmentDetails.SubscriberId;
                companySegment.SortOrder = maxSortOrderValue + 1;
                companySegment.CreatedUserId = companySegment.UpdateUserId;
                companySegment.CreatedUserName = companySegment.UpdateUserName;
                companySegment.CreatedDate = DateTime.UtcNow;
                context.CompanySegments.InsertOnSubmit(companySegment);
            }
            context.SubmitChanges();
            // return the company segment id
            return companySegment.CompanySegmentId;
        }


        public bool DeleteCompanySegment(int companySegmentId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companySegment = context.CompanySegments.FirstOrDefault(t => t.CompanySegmentId == companySegmentId);
            if (companySegment != null)
            {
                companySegment.Deleted = true;
                companySegment.DeletedUserId = userId;
                companySegment.DeletedDate = DateTime.UtcNow;
                companySegment.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companySegments = context.CompanySegments.Where(t => t.SubscriberId == subscriberId).ToList();
            var companySegmentId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in companySegmentId)
            {
                var companySegment = companySegments.FirstOrDefault(t => t.CompanySegmentId == int.Parse(id));
                if (companySegment != null)
                {
                    companySegment.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }
    }
}
