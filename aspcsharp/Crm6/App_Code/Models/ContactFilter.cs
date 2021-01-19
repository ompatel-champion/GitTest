using Crm6.App_Code;
using System.Collections.Generic;

namespace Models
{ 

    public class ContactFilter
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; } 
        public string ContactsIdsIn { get; set; }
        public int CompanyId { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }

        // paging
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }

    public class ContactListResponse
    {
        public List<Contact> Contacts { get; set; }
        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
    }

    public class ContactSaveRequest
    {
        public Contact Contact { get; set; } 
        public DocumentModel ProfilePic { get; set; }
        public bool CreateSession { get; set; }
    }
}