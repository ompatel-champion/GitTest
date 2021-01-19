using System;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Shared;
using Microsoft.Exchange.WebServices.Data;
using O365Contact = Microsoft.Exchange.WebServices.Data.Contact;

namespace Helpers.Sync
{
    public class TestO365SyncInitializer
    {

        public bool TestO365Sync()
        {
            try
            {
                const string url = "https://outlook.office365.com/EWS/Exchange.asmx";
                var lService = new ExchangeService(ExchangeVersion.Exchange2013_SP1) //.Exchange2010_SP1
                {
                    Url = new Uri(url),
                    //Credentials = new WebCredentials("Charles.Emerson@am.kwe.com", "1stFreight")
                    Credentials = new WebCredentials("dean.martin@firstfreight0.onmicrosoft.com", "Dino#1350")
                };

                var o365Contact = new O365Contact(lService);
                o365Contact.EmailAddresses[EmailAddressKey.EmailAddress1] = new Microsoft.Exchange.WebServices.Data.EmailAddress("test@123.com");
                o365Contact.CompanyName = "Test Company";
                o365Contact.GivenName = "TestFirstName";
                o365Contact.Surname = "TestLastName";
                try
                {
                    o365Contact.Save();
                }
                catch (ServiceResponseException ex)
                {
                    var error = new WebAppError
                    {
                        ErrorCallStack = ex.StackTrace,
                        ErrorDateTime = DateTime.UtcNow,
                        ErrorMessage = ex.ToString(),
                        PageCalledFrom = "TestO365SyncInitializer",
                        RoutineName = "TestO365Sync - inner catch",
                        SubscriberName = "",
                    };
                    new Logging().LogWebAppError(error);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                    var error = new WebAppError
                    {
                        ErrorCallStack = ex.StackTrace,
                        ErrorDateTime = DateTime.UtcNow,
                        ErrorMessage = ex.ToString(),
                        PageCalledFrom = "TestO365SyncInitializer",
                        RoutineName = "TestO365Sync - outer catch",
                        SubscriberName = "",
                    };
                    new Logging().LogWebAppError(error);
                return false;
            }
        }

    }
}
