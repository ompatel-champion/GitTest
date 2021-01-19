using System.Linq;
using Crm6.App_Code;
using Crm6.App_Code.Shared;

namespace Models
{
    public class UserActivityReportModel
    {
        public IQueryable<Company> Companies { get; set; }
        public IQueryable<Contact> Contacts { get; set; }
        public IQueryable<Country> Countries { get; set; }
        public string DateTimePickerFormat { get; set; }
        public IQueryable<Deal> Deals { get; set; }
        public IQueryable<Location> Locations { get; set; }
        public IQueryable<Region> Regions { get; set; }
        public IQueryable<Subscriber> Subscribers { get; set; }
        public IQueryable<User> Users { get; set; }
    }
}