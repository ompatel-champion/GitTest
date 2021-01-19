using Crm6.App_Code;
using System.Collections.Generic;

namespace Models
{

    public class LocationFilter
    {
        public int SubscriberId { get; set; }
        public int UserId { get; set; } 
        public string LocationsIdsIn { get; set; }
        public int LocationId { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }

        // paging
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }


    public class LocationListResponse
    {
        public List<Location> Locations { get; set; }
        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
    }


    public class LocationSaveRequest
    {
        public Location Location { get; set; } 
        public DocumentModel LocationPic { get; set; } 
    }

}