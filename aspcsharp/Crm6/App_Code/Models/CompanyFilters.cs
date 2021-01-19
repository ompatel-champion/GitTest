using Crm6.App_Code;
using Crm6.App_Code.Shared;
using System.Collections.Generic;

namespace Models
{
    public class CompanyFilters
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public string FilterType { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string Keyword { get; set; }
        public string CompanyType { get; set; }
        public string SortBy { get; set; }
        public int LocationId { get; set; }
        public string CountryCode { get; set; }
        public int SalesRep { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string CountryName { get; set; }
        public bool IsCustomer { get; set; }
    }


    public class CompanyListResponse
    {
        public List<Company> Companies { get; set; }
        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
    }

     

    public class GlobalCompanyListResponse
    {
        public List<GlobalCompany> Companies { get; set; }
        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
        public List<int> AccessibleCompanyIds { get; set; }
    }
}