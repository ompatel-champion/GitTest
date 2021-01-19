using Crm6.App_Code.Shared;
using System.Collections.Generic;

namespace Models
{

    public class GlobalLocationFilter
    {

        public int UserId { get; set; } 
        public string GlobalLocationsIdsIn { get; set; }
        public int GlobalLocationId { get; set; }
        public string Keyword { get; set; }
        public string SortBy { get; set; }

        // paging
        public int RecordsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }


    public class GlobalLocationListResponse
    {
        public List<GlobalLocation> GlobalLocations { get; set; }
        // Paging
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int Records { get; set; }
    }

}
