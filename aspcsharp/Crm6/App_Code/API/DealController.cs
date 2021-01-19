using Crm6.App_Code;
using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Web.Http;


namespace API
{
    public class DealController : ApiController
    {

        [AcceptVerbs("POST")]
        public DealListResponse GetDeals([FromBody]DealFilters filters)
        {
            return new Deals().GetDeals(filters);
        }


        [AcceptVerbs("POST")]
        public int SaveDeal([FromBody]SaveDealRequest deal)
        {
            return new Deals().SaveDeal(deal);
        }


        [AcceptVerbs("GET")]
        public Deal GetDeal([FromUri]int dealId)
        {
            throw new ArgumentException("Obsolete:  Must pass subscriberId.");
            //return new Deals().GetDeal(dealId);
        }


        [AcceptVerbs("GET")]
        public Deal GetDeal([FromUri]int dealId, int dealSubscriberId)
        {
            return new Deals().GetDeal(dealId, dealSubscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteDeal([FromUri]int dealId, int userId, int subscriberId)
        {
            return new Deals().DeleteDeal(dealId, userId, subscriberId);
        }

        [AcceptVerbs("GET")]
        public bool DeleteDeal([FromUri]int dealId, int dealSubscriberId, int userId, int userSubscriberId)
        {
            return new Deals().DeleteDeal(dealId, dealSubscriberId, userId, userSubscriberId);
        }


        [AcceptVerbs("GET")]
        public bool UpdateDealSalesStage([FromUri]int dealId, string salesStage, int userId, int subscriberId)
        {
            return new Deals().UpdateDealSalesStage(dealId, salesStage, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<ContactModel> GetDealContacts([FromUri]int dealId)
        {
            throw new ArgumentException("Obsolete:  Must pass subscriberId.");
            //return new Deals().GetDealContacts(dealId);
        }


        [AcceptVerbs("GET")]
        public List<ContactModel> GetDealContacts([FromUri]int dealId, int subscriberId)
        {
            return new Deals().GetDealContacts(dealId, subscriberId);
        }



        [AcceptVerbs("POST")]
        public bool AddDealContact([FromBody]LinkContactToDeal dealContact)
        {
            return new Deals().AddDealContact(dealContact);
        }


        [AcceptVerbs("GET")]
        public List<Deal> GetContactDeals(int contactId, int subscriberId)
        {
            return new Deals().GetContactDeals(contactId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteDealContact([FromUri]int dealId, int userId, int contactId, int dealSubscriberId)
        {
            return new Deals().DeleteDealContact(dealId, userId, contactId, dealSubscriberId);
        }


        [AcceptVerbs("GET")]
        public List<DealSalesStageTimeline> GetDealSalesStageTimeline(int subscriberId, int dealId)
        {
            return new Deals().GetDealSalesStageTimeline(subscriberId, dealId);
        }


        [AcceptVerbs("GET")]
        public int GetCompanyDealsCount(int companyId, int subscriberId)
        {
            return new Deals().GetCompanyDealsCount(companyId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<Deal> GetCompanyDeals(int companyId, int subscriberId)
        {
            return new Deals().GetCompanyDeals(companyId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public List<DealSalesTeamMember> GetDealUsers([FromUri]int dealId, int subscriberId)
        {
            return new Deals().GetDealUsers(dealId, subscriberId);
        }

 

        [AcceptVerbs("GET")]
        public List<UserBasic> GetDealUsersBasicDetails([FromUri]int dealId)
        {
            var subscriberId = HttpContext.Current.Session["subscriberId"] as int? ?? 0;
            return new Deals().GetDealUsersBasicDetails(dealId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteDealUser([FromUri]int dealId, int userId, int deleteUserId, int dealSubscriberId, int userSubscriberId)
        {
            return new Deals().DeleteDealUser(dealId, userId, deleteUserId, dealSubscriberId, userSubscriberId);
        }


        [AcceptVerbs("GET")]
        public bool UpdateDealDate([FromUri]int dealId, DateTime dateValue, string datetype, int userId, int subscriberId)
        {
            return new Deals().UpdateDealDate(dealId, dateValue, datetype, userId, subscriberId);
        }


        [AcceptVerbs("GET")]
        public RevenueResponse GetDealRevenue([FromUri] int dealId, int userId)
        {
            var subscriberId = HttpContext.Current.Session["subscriberId"] as int? ?? 0;
            return new Deals().GetDealRevenue(dealId, userId, subscriberId);
        }

        [AcceptVerbs("GET")]
        public RevenueResponse GetDealRevenue([FromUri] int dealId, int userId, int subscriberId)
        {
            return new Deals().GetDealRevenue(dealId, userId, subscriberId);
        }

        [AcceptVerbs("GET")]
        public RevenueResponse GetDealRevenue(int dealId, int userId, int userSubscriberId, int dealSubscriberId)
        {
            return new Deals().GetDealRevenue(dealId, userId, userSubscriberId, dealSubscriberId);
        }

        [AcceptVerbs("GET")]
        public RevenueResponse GetDealRevenueFromUserCurrency(int dealId, int userId)
        {
            return new Deals().GetDealRevenueFromUserCurrency(dealId, userId);
        }


        [AcceptVerbs("POST")]
        public bool AddDealSalesTeam([FromBody]AddDealUserRequest request)
        {
            return new Deals().AddDealSalesTeam(request);
        }

    }
}
