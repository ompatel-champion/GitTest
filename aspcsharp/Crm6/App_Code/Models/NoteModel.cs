using Crm6.App_Code;
using System;
using System.Collections.Generic; 
using System.Web;

namespace Models
{
    public class NoteModel
    {
        public Note Note { get; set; }   
        public int DealId { get; set; }
        public string DealName { get; set; }
        public string DueDate { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string CreatedDateStr { get; set; }

    }
}