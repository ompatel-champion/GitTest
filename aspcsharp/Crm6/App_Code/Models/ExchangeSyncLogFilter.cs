using Crm6.App_Code;
using System.Collections.Generic;

namespace Models
{

    public class ExchangeSyncLogFilter
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; } 
        public string SortBy { get; set; }

        // paging
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }


    public class ExchangeSyncLogResponse
    {
        public List<ExchangeSyncLog> SyncLogEntries { get; set; }
        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
    }

}