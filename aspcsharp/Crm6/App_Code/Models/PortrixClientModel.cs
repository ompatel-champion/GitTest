using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crm6.App_Code.Models
{
    public class PortrixClientResponseModel
    {
        public int id { get; set; }
    }

    public class PortrixClientModel
    {
        public string name { get; set; }
        public string code { get; set; }
        public string extref { get; set; }
        public int parentId { get; set; }
        public List<int> children { get; set; }
        public List<Contact> contacts { get; set; }
        public List<Contactdetail> contactdetails { get; set; }
        public string remarks { get; set; }

        public class Email
        {
            public string emailaddress { get; set; }
        }

        public class Phone
        {
            public string countrycode { get; set; }
            public string areacode { get; set; }
            public string number { get; set; }
            public string extension { get; set; }
        }

        public class Contactdetail
        {
            public string preferred { get; set; }
            public string description { get; set; }
            public Email email { get; set; }
            public Phone phone { get; set; }
        }

        public class Contact
        {
            public string salutation { get; set; }
            public string title { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public List<Contactdetail> contactdetails { get; set; }
        }
    }
}