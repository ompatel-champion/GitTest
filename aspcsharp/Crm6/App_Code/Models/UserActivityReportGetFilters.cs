using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Crm6.App_Code;

namespace CRM.Dev.Api.Report
{
    public class GetUserActivityFilters
    {
        [Required]
        public List<int> CompaniesId { get; set; }

        [Required]
        public List<int> ContactsId { get; set; }

        [Required]
        public List<int> DealsId { get; set; }

        public DateTime From { get; set; }

        [Required]
        public List<int> SuscribersId { get; set; }

        public DateTime To { get; set; }

        [Required]
        public List<int> UsersId { get; set; }
    }
}