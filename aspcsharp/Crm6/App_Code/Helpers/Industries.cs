using System.Collections.Generic;
using System.Linq;
using Models;
using System;
using Crm6.App_Code;

namespace Helpers
{
    public class Industries
    { 

        public Industry GetIndustry(int industryId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            return context.Industries.FirstOrDefault(t => t.IndustryId == industryId);
        }


        public int SaveIndustry(Industry industryDetails)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
             
            // get the industry by id or create new industry object
            var industry = context.Industries.FirstOrDefault(l => l.IndustryId == industryDetails.IndustryId) ?? new Industry();

            // fill update details
            industry.IndustryName = industryDetails.IndustryName; 
            industry.LastUpdate = DateTime.UtcNow;
            industry.SortOrder = industryDetails.SortOrder;
            industry.UpdateUserId = industryDetails.UpdateUserId;
            industry.UpdateUserName = new Users().GetUserFullNameById(industryDetails.UpdateUserId, industryDetails.SubscriberId);
             
            if (industry.IndustryId < 1)
            {
                // new industry - insert
                industry.SubscriberId = industryDetails.SubscriberId;
                industry.CreatedUserId = industry.UpdateUserId;
                industry.CreatedUserName = industry.UpdateUserName;
                industry.CreatedDate = DateTime.UtcNow;
                context.Industries.InsertOnSubmit(industry); 
            }
            context.SubmitChanges();
  
            // return the industry id
            return industry.IndustryId;
        }


        public List<Industry> GetIndustries(int subscriberId)
        {
            var securityContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
            var dataCenter = securityContext.GlobalSubscribers.Where(t => t.SubscriberId == subscriberId).Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(dataCenter);

            var context = new DbFirstFreightDataContext(connection);
            return context.Industries.Where(t => !t.Deleted && t.SubscriberId == subscriberId)
                .OrderBy(t => t.SortOrder).Select(t => t).ToList();
        }


        public List<SelectList> GetIndustriesForDropdown(int subscriberId)
        {
            var securityContext = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection());
            var dataCenter = securityContext.GlobalSubscribers.Where(t => t.SubscriberId == subscriberId).Select(t => t.DataCenter).FirstOrDefault();
            var connection = LoginUser.GetConnectionForDataCenter(dataCenter);
             
            var context = new DbFirstFreightDataContext(connection); 
            return context.Industries.Where(i => !i.Deleted && i.SubscriberId == subscriberId)
                .OrderBy(i => i.IndustryName).Select(i => new SelectList
                {
                    SelectText = i.IndustryName,
                    SelectValue = i.IndustryName
                }).ToList();
        }


        public bool DeleteIndustry(int industryId, int userId, int subscriberId)
        {
            var connection = LoginUser.GetConnection();
            var context = new DbFirstFreightDataContext(connection);
            var industry = context.Industries.FirstOrDefault(t => t.IndustryId == industryId);
            if (industry != null)
            {
                industry.Deleted = true;
                industry.DeletedByUserId = userId;
                industry.DeletedDate = DateTime.UtcNow;
                industry.DeletedByUserName = new Users().GetUserFullNameById(userId, subscriberId);
                context.SubmitChanges();
                return true;
            }
            return false;
        }


        public bool ChangeOrder(string ids, int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            var industries = context.Industries.Where(t => t.SubscriberId == subscriberId).ToList();
            var industryIds = ids.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var order = 1;
            foreach (var id in industryIds)
            {
                var industry = industries.FirstOrDefault(t => t.IndustryId == int.Parse(id));
                if (industry != null)
                {
                    industry.SortOrder = order;
                    order += 1;
                }
            }
            context.SubmitChanges();
            return true;
        }

    }
}
