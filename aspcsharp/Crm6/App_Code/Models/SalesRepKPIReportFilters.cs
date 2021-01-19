using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Models
{
    public class SalesRepKPIReportFilters
    {
        public int SubscriberId { get; set; }
        public string CountryName { get; set; }
        public string LocationCode { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public int UserId { get; set; }
        public int SalesRepId { get; set; }
        public List<string> Categories { get; set; }
    }


    public class SalesRepKPIReportResponse
    {
        public List<SalesRepKPIReportItem> SalesReps { get; set; }
        public string ExcelUri { get; set; }
    }

    public class SalesRepKPIReportItem
    {
        public int SalesRepId { get; set; }
        public string SalesRepName { get; set; }
        public int NewDeals { get; set; }
        public int WonDeals { get; set; }
        public int LostDeals { get; set; }
        public int Events { get; set; }
        public int Tasks { get; set; }
        public int Notes { get; set; }
        public int Logins { get; set; }
        public string Country { get; set; }
        public string Location { get; set; }

    }
}
