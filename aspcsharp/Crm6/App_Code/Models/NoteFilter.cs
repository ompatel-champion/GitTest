using System.Collections.Generic;

namespace Models
{
    public class NoteFilter
    {
        public int CompanyId { get; set; }
        public int ContactId { get; set; }
        public int DealId { get; set; }
        public int GlobalCompanyId{ get; set; }
        public string SortBy { get; set; }
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public int UserIdGlobal { get; set; }
        // paging
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }


    public class NoteListResponse
    {
        public List<Crm6.App_Code.Shared.Activity> Notes { get; set; }
        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
    }
}
