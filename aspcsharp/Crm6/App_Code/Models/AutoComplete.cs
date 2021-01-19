using System.Collections.Generic;

namespace Models
{
    public class AutoComplete
    {
        public int id { get; set; }
        public string name { get; set; }
        public string susbcriberId { get; set; }
        public object dataObj { get; set; } 
        public string type { get; set; }
        public string companyName { get; set; }
        public int globalCompanyId { get; set; }
    }

    public class AutoCompleteFilter
    {
        public AutoCompleteFilter()
        {
            Count = 25;
            SelectedIds = new List<int>();
        } 

        public string Type { get; set; }
        public List<int> SelectedIds { get; set; }
        public int SusbcriberId { get; set; }
        public int CompanyId { get; set; }
        public int GlobalCompanyId { get; set; }
        public int PrimaryContactId { get; set; }
        public string Prefix { get; set; }
        public int Count { get; set; }
        public int UserId { get; set; }
    }

}