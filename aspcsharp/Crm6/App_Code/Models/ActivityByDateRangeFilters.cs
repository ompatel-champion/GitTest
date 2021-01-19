using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class ActivityByDateRangReportFilters
    {
        public int SubscriberId { get; set; }
        public List<string> ActivityTypes { get; set; }
        public List<string> Categories { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public List<string> Location { get; set; }
        public List<string> Country { get; set; }
        public List<int> GlobalComapnyIds { get; set; }
        public List<string> Campaigns { get; set; }
        public string DealType { get; set; }
        public List<int> SalesReps { get; set; }
        public List<int> UserIds { get; set; }
        public List<string> Competitors { get; set; }
        public int LoggedInUserId { get; set; }
        public List<int> GlobalUserIds { get; set; }
        public int GlobalUserId { get; set; }
        public bool NoUsersSelectedInForGlobalCampaigns { get; set; }
    }


    public class ActivityByDateRangeReportResponse
    {
        public List<ActivityReportItem> Activities { get; set; }
        public int RecordCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string ExcelUri { get; set; }
    }

    public class ActivityReportItem
    {
        public int ActivtyId { get; set; }
        public string Subject { get; set; }
        public string TaskName { get; set; }
        public string ActivityType { get; set; }
        public string User { get; set; }
        public string ActivityDateStr { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Description { get; set; }
        public string Deals { get; set; } 
        public string DealIds { get; set; }
        public string Location { get; set; } 
        public string DealType { get; set; }
        public string Competitors { get; set; }
        public string Campaigns { get; set; } 
        public string Percentage { get; set; }
        public bool Completed { get; set; }
        public int CompanyId { get; set; }
        public int CompanySubscriberId { get; set; }
        public int CompanyIdGlobal { get; set; }
        public string CompanyName { get; set; }
        public List<AutoComplete> Contacts { get; set; }
        public string ContactsStr { get; set; } 
        public int SubscriberId { get; set; }
        public string Category { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}