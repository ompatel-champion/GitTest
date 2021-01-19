using Crm6.App_Code;
using Crm6.App_Code.Shared;
using System;
using System.Collections.Generic;

namespace Models
{
    public class CalendarEventModel
    {
        public Crm6.App_Code.Shared.Activity CalendarEvent { get; set; }
        public Crm6.App_Code.Deal Deal { get; set; }
        public List<Contact> Contacts { get; set; }
        public GlobalCompany Company { get; set; }
        public string ContactsStr { get; set; }
        public string CompaniesStr { get; set; }
        public string DealIds { get; set; }
        public string DealName { get; set; } 
        public List<ActivititesMember> Invites { get; set; }
        public List<Crm6.App_Code.Deal> Deals { get; set; }
        public List<DocumentModel> Documents { get; set; }
        public string StartDateTimeStr { get; set; }
        public bool NotifyInternalAttendees { get; set; }
        public bool NotifyExternalAttendees { get; set; }
    }

    public class CalendarEventFilter
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; } 
        public string CalendarEventIdsIn { get; set; }
        public string SortBy { get; set; }
        public int DealId { get; set; }
        public int ContactId { get; set; } 
        public int CompanyIdGlobal  { get; set; }
        public int CompanyId { get; set; }
        public int OwnerUserId { get; set; }
        public int OwnerUserIdGlobal { get; set; }
        // paging
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }


    public class ActivityExtended : Crm6.App_Code.Shared.Activity
    {
        public string Invites { get; set; }

        public string EventStartEndTime { get; set; }
    }
}
 