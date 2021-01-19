using Crm6.App_Code;
using System;
using System.Collections.Generic;

namespace Models
{
    public class DealModel
    {
        public int DealId { get; set; }
        public string DealName { get; set; }
        public int SubscriberId { get; set; }
        public string ContactsStr { get; set; }
        public string CreatedDateStr { get; set; }
        public string SalesStageName { get; set; }
        public string CompanyName { get; set; }
        public float Revenue { get; set; }
        public string LastActivityDateStr { get; set; }
        public string NextActivityDateStr { get; set; }
        public string WonLostDateStr { get; set; }
        public string WonLostReason { get; set; }

    }

    public class LaneModel
    {
        public Lane Lane { get; set; }
        public SelectList OriginLocation { get; set; }
        public SelectList DestinationLocation { get; set; }
    }

    public class DealFilters
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public int ContactId { get; set; }
        public List<string> SalesStages { get; set; }
        public int SalesRepId { get; set; }
        public string CountryName { get; set; }
        public string Location { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }
    }


    public class DealWonLost
    {
        public int DealId { get; set; }
        public bool Won { get; set; }
        public bool Lost { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; }
    }

    public class DealExtended : Deal
    {
        public string CityName;
        public string CountryName;
        public int ActivityId;
        public string ActivityName;
        public List<SalesTeamMemberAndRole> SalesTeamMembersExtended; 
    }

    public class SalesTeamMemberAndRole
    {
        public string SalesTeamMember;
        public string Role;
    }

    public class DealListResponse
    {
        public List<DealExtended> Deals { get; set; }

        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
    }
    

    public class RevenueResponse
    {
        public double Revenue { get; set; }
        public double Profit { get; set; }
        public double SpotProfit { get; set; }
        public double SpotRevenue { get; set; }
        public string CurrencySymbol { get; set; }
    }

    public class DealSalesTeamMember
    {
        public User User { get; set; }
        public DocumentModel ProfilePicture { get; set; }

        public LinkUserToDeal LinkUseToDeal { get; set; }
    }
}