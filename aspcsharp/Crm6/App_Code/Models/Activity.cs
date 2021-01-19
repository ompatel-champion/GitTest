using Crm6.App_Code;
using Crm6.App_Code.Shared;
using System;
using System.Collections.Generic;

namespace Models
{
    public class ActivityModel
    {
        public Activity CalendarEvent { get; set; }
        public Activity Task { get; set; }
        public Note Note { get; set; }
        public GlobalCompany Company { get; set; }
        public List<Deal> Deals { get; set; }
        public List<ActivititesMember> Invites { get; set; }
        public List<DocumentModel> Documents { get; set; }
        public bool NotifyInternalAttendees { get; set; }
        public bool NotifyExternalAttendees { get; set; }
        public DocumentModel Document { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ActivityDate { get; set; }

    }

    public class ActivityFilter
    {
        public ActivityFilter()
        {
            SortOrder = "createddate desc";
        }
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public int DealId { get; set; }
        public int ContactId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyIdGlobal { get; set; }
        public List<string> ActivityTypes { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string SortOrder { get; set; }
        public int OwnerUserIdGlobal { get; set; }
        public string SortBy { get; set; }
    }

    public class ActivitiesFilter
    {
        public ActivitiesFilter()
        {
            SortOrder = "createddate desc";
        }
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public int DealId { get; set; }
        public int ContactId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyIdGlobal { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string SortOrder { get; set; }
        public int OwnerUserIdGlobal { get; set; }
        public string SortBy { get; set; }
        public bool getNextLast { get; set; }
        public bool MatchActive { get; set; }
    }

    public enum ActivityType
    {
        Note = 1,
        CalendarEvent = 2,
        Document = 3,
        Task = 4
    }


    //---- Following models are used to load the data in user activies page

    public class ActivityChartDataFilter
    {
        public int UserId { get; set; }
        public int SubscriberId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }

    public class ActivityChartDataResponse
    {
        public List<ActivityCountByType> ActivityCountByTypes { get; set; }
        public List<ActivityChartData> ChartData { get; set; }

    }

    public class ActivityCountByType
    {
        public string ActivityType { get; set; }
        public int ActivityCount { get; set; }
    }

    /// <summary>
    /// These labels and values are used to load the activity chart - chart js
    /// </summary>
    public class ActivityChartData
    {
        public DateTime ActivityDate { get; set; }
        public int ActivityCount { get; set; }
    }

    public class ActivitesResponse
    {
        public List<Activity> Activites { get; set; }
        public Activity NextActivity { get; set; }
        public Activity LastActivity { get; set; }
    }

}