using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class CompaniesReportFilters
    {
        public int SubscriberId { get; set; }
        public string Status { get; set; }
        public string Country { get; set; }
        public string Industry { get; set; }
        public string Source { get; set; }
        public string Competitor { get; set; }
        public string Campaign { get; set; }
        public int UserId { get; set; }
    }


    public class CompaniesReportResponse
    {
        public List<CompanyReportItem> Companies { get; set; }
        public int RecordCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string ExcelUri { get; set; }
    }

    public class CompanyReportItem
    {
        public string Company { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Industry { get; set; }
        public string Source { get; set; }
        public string Competitor { get; set; }
        public string Campaign { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }
}