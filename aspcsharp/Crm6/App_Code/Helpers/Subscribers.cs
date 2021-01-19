using System.Linq;
using Crm6.App_Code;
using System.Web;
using System;
using System.Collections.Generic;

namespace Helpers
{
    public class Subscribers
    {

        public Subscriber GetSubscriber(int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.Subscribers.Where(s => s.SubscriberId == subscriberId)
                .Select(s => s).FirstOrDefault();

            //  SubscriberLogo = new Documents().GetDocumentsByDocType(3, subscriberId).FirstOrDefault(),
        }


        public string GetDataCenter(int subscriberId)
        {
            var subscriberDataCenter = new Crm6.App_Code.Login.DbLoginDataContext(LoginUser.GetLoginConnection())
                                                .GlobalSubscribers.Where(t => t.SubscriberId == subscriberId)
                                                .Select(t => t.DataCenter).FirstOrDefault();
            return subscriberDataCenter;
        }


        public string GetShippingFrequency(int subscriberId)
        {
            var connection = LoginUser.GetConnection( );
            var context = new DbFirstFreightDataContext(connection);
            return context.Subscribers.Where(s => s.SubscriberId == subscriberId)
                .Select(s => s.DefaultShippingFrequency).FirstOrDefault();
        }


        public string GetLogo(int subscriberId)
        {
            var url = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            switch (subscriberId)
            {
                case 229:
                case 1001:
                case 30002:
                case 30003:
                case 30005:
                    url = "https://kweus.firstfreight.com/_content/_img/subscribers/kwe.png";
                    break;
                default:
                    break;
            }
            // url = HttpContext.Current.Server.MapPath(url);
            return url;
        }


        public List<int> GetLinkedSubscriberIds(int subscriberId)
        {
            var sharedConnection = LoginUser.GetSharedConnection( );
            var sharedContext = new Crm6.App_Code.Shared.DbSharedDataContext(sharedConnection);

            var subscriberIds = sharedContext.LinkGlobalSuscriberToSubscribers.Where(t => t.GlobalSubscriberId == subscriberId)
                                             .Select(t => t.LinkedSubscriberId).ToList();
            return subscriberIds;
        }

    }
}
