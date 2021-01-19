using Crm6.App_Code;
using System.Collections.Generic;

namespace Models
{
    public class LaneFilter
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; }
        public int DealId { get; set; }
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public string Keyword { get; set; }
    }

    public class LaneListResponse
    {
        public List<Lane> Lanes { get; set; } 
        public string UserCurrencySymbol { get; set; }
        public string UserCurrencyCode { get; set; }
    }
}