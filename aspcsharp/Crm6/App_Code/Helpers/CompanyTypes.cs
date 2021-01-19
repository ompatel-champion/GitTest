using Crm6.App_Code;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public class CompanyTypes
    {

        public CompanyType GetCompanyType(int companyTypeId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.CompanyTypes.FirstOrDefault(t => t.CompanyTypeId == companyTypeId);
        }

        public List<SelectList> GetCompanyTypesForDropdown(int subscriberId)
        {
            var securityContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
            var dataCenter = securityContext.GlobalSubscribers.Where(t => t.SubscriberId == subscriberId).Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(dataCenter);

            var context = new DbFirstFreightDataContext(connection);
            return context.CompanyTypes.Where(i => !i.Deleted && i.SubscriberId == subscriberId)
                .OrderBy(i => i.CompanyTypeName).Select(i => new SelectList
                {
                    SelectText = i.CompanyTypeName,
                    SelectValue = i.CompanyTypeName
                }).ToList();
        }


        public string GetCompanyTypeName(int companyTypeId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.CompanyTypes.Where(t => t.CompanyTypeId == companyTypeId).Select(t => t.CompanyTypeName).FirstOrDefault();
        }


        public List<CompanyType> GetCompanyTypes(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                             .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                             .Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(subscriberDataCenter);
            var context = new DbFirstFreightDataContext(connection);

            return context.CompanyTypes.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.CompanyTypeName).Select(t => t).ToList();
        }


        public int SaveCompanyType(CompanyType companyTypeDetails)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);

            // get the company type by id or create new company type object
            var companyType = context.CompanyTypes.FirstOrDefault(t => t.SubscriberId == companyTypeDetails.SubscriberId &&
                                                    t.CompanyTypeId == companyTypeDetails.CompanyTypeId) ?? new CompanyType();            // fill details
            companyType.CompanyTypeName = companyTypeDetails.CompanyTypeName;
            companyType.UpdateUserId = companyTypeDetails.UpdateUserId;
            companyType.UpdateUserName = new Users().GetUserFullNameById(companyTypeDetails.UpdateUserId, companyTypeDetails.SubscriberId);
            companyType.LastUpdate = DateTime.UtcNow;

        // new company type - insert
        if (companyType.CompanyTypeId < 1)
            {
                companyType.SubscriberId = companyTypeDetails.SubscriberId;
                companyType.CreatedUserId = companyType.UpdateUserId;
                companyType.CreatedUserName = companyType.UpdateUserName;
                companyType.CreatedDate = DateTime.UtcNow;
                context.CompanyTypes.InsertOnSubmit(companyType);
            }
            context.SubmitChanges();
            // return the company type id
            return companyType.CompanyTypeId;
        }


        public bool DeleteCompanyType(int companyTypeId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companyType = context.CompanyTypes.FirstOrDefault(t => t.CompanyTypeId == companyTypeId);
            if (companyType != null)
            {
                companyType.Deleted = true;
                companyType.DeletedUserId = userId;
                companyType.DeletedDate = DateTime.UtcNow;
                companyType.DeletedUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }

        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var companyTypes = context.CompanyTypes.Where(t => t.SubscriberId == subscriberId).ToList();
            var companyTypeId = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in companyTypeId)
            {
                var source = companyTypes.FirstOrDefault(t => t.CompanyTypeId == int.Parse(id));
                if (source != null)
                {
                    //TODO - Fix sort order
                    //source.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }

    }
}
