//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

namespace Crm6
{
    internal class GoogleOAuth2AuthenticationOptions
    {
        public GoogleOAuth2AuthenticationOptions()
        {
        }

        public string AccessType { get; set; }
        public object ClientId { get; set; }
        public object ClientSecret { get; set; }
        public object Provider { get; set; }
    }
}