using Crm6.App_Code;
using System;
using System.Collections.Generic;

namespace Models
{

    //public class TaskModel
    //{
    //    public Task Task { get; set; }
    //    public Deal Deal { get; set; }
    //    public List<Contact> Contacts { get; set; }
    //    public List<Company> Companies { get; set; }
    //    public string ContactsStr { get; set; }
    //    public string CompaniesStr { get; set; }
    //    public int DealId { get; set; }
    //    public string DealName { get; set; }
    //    public string DueDate { get; set; }
    //}


    public class TaskFilter
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public int UserIdGlobal { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
        public string TaskIdsIn { get; set; }
        public string SortBy { get; set; }
        public int DealId { get; set; }
        public bool? Completed { get; set; }
        public int ContactId { get; set; }
        public int CompanyId { get; set; }
        public int CompanyIdGlobal { get; set; }
        public int LoggedinUserId { get; set; }
        // paging
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }

}
