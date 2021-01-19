using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class WeeklyActivityFilters
    {
        public int SubscriberId { get; set; }
        public List<string> ActivityTypes { get; set; }
        public List<string> Categories { get; set; }
        public string DateMonday { get; set; } 
        public int GlobalUserId { get; set; } 
        public int LoggedInUserId { get; set; }
        public List<string> Location { get; set; }
        public List<string> Country { get; set; }
        public List<string> Campaigns { get; set; }
        public List<int> GlobalUserIds { get; set; }
    }

    public class WeeklyActivityReportResponse
    {
        public List<ActivitiesByDay> ActivitiesByDay { get; set; }
        public string ExcelUri { get; set; }
    }


    public class ActivitiesByDay
    {
        public DateTime DayOfWeek { get; set; }
        public string DayOfWeekStr { get; set; }
        public List<WeeklyActivityReportItem> Activities { get; set; }
    }


    public class WeeklyActivityReportItem
    {
        public string Subject { get; set; }
        public string ActivityType { get; set; }
        public string User { get; set; }
        public string ActivityDateStr { get; set; }
        public DateTime ActivityDate { get; set; }
        public string Description { get; set; }
        public string Deals { get; set; }
        public string DealIds { get; set; }
        public string Location { get; set; }
        public string Percentage { get; set; }
        public bool Completed { get; set; }
        public string CompanyName { get; set; }
        public int CompanyId { get; set; }
        public int CompanyIdGlobal { get; set; }
        public int CompanySubscriberId { get; set; }
        public List<AutoComplete> Contacts { get; set; }
        public string  ContactStr { get; set; }
        public int ActivtyId { get; set; }
        public int SubscriberId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public string TaskName { get; internal set; }
        public string Category { get; internal set; }
    }



}