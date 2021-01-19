using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class UserActivityReportFilters
    {
        public int SubscriberId { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<int> UserIds { get; set; }
        public List<int> DealIds { get; set; }
        public List<int> CompanyIds { get; set; }
        public List<int> ContactIds { get; set; }
        public int UserId { get; set; }
    }
    
    public class UserActivityReportResponse
    {
        public List<UserActivityReportItem> Activities { get; set; }
        public int RecordCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string ExcelUri { get; set; }
    }

    public class UserActivityReportItem
    {
        public string UserName { get; set; }
        public DateTime? UserActivityTimestamp { get; set; }
        public string UserActivityMessage { get; set; }
        public string CalendarEventSubject { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string DealName { get; set; }
        public string NoteContent { get; set; }
        public string TaskName { get; set; }
    }
}