using System;
using System.Collections.Generic;
using Crm6.App_Code.Helpers;
using Crm6.App_Code.Shared;
using Crm6.Components.Common;
using Intercom.Clients;
using Intercom.Core;
using Intercom.Data;
using Intercom.Exceptions;
using Models;

namespace Helpers
{
    public interface IIntercomClientFactory
    {
        EventsClient GetEventsClient(string token);
        UsersClient GetUsersClient(string token);
        CompanyClient GetCompanyClient(string token);
        User InitIntercomUser(UserModel crmUserModel, Dictionary<string, object> customAttributes);
    }

    public class IntercomClientFactory : IIntercomClientFactory
    {
        private static Authentication IntercomAuth(string token) => new Authentication(token);

        public EventsClient GetEventsClient(string token)
        {
            return new EventsClient(IntercomAuth(token));
        }

        public UsersClient GetUsersClient(string token)
        {
            return new UsersClient(IntercomAuth(token));
        }

        public CompanyClient GetCompanyClient(string token)
        {
            return new CompanyClient(IntercomAuth(token));
        }


        public User InitIntercomUser(UserModel crmUserModel, Dictionary<string, object> customAttributes)
        {
            return new User
            {
                email = crmUserModel.User.EmailAddress,
                name = crmUserModel.User.FullName,
                user_id = crmUserModel.User.UserId.ToString(),
                custom_attributes = customAttributes
            };
        }
    }

    public class IntercomHelper
    {
        //*********************************************************************************************************
        // to stop onborading campaign - set the Journey Stage to None or empty string
        // intercom campaign = Journey Stage in API

        // c# .NET API github
        //https://github.com/intercom/intercom-dotnet

        // Delete intercom

        //developer.intercom.com

        //y3oan1ik - production app id
        //jgrb3sow - test app id

        //https://developers.intercom.com/

        // GoogleTagManager - GTM setup test and production environments

        //*********************************************************************************************************

        // Login
        // save calendar event
        // save company
        // save contact
        // save deal
        // save document
        // save note
        // save task
        // ran kpi report
        // ran deals report
        // ran activity range report
        // ran weekly sales activity report

        // TODO: other user tasks to track and encourage engagement

        // More Onboarding steps
        // add lane
        // upload document
        // win deal

        // deleted various objects

        // used advanced deal report filter
        // update profile
        // change language
        // change currency
        // change location
        // change user roles
        // change timezone
        // change password
        // setup sync

        private readonly string _accessToken;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IIntercomClientFactory _intercomClientFactory;
        private static Authentication GetAuth(string token) => new Authentication(token);

        private static Dictionary<string, object> OnboardingAttributes(UserOnboardingStep step) =>
            new Dictionary<string, object>
            {
                {"Journey Stage", "Onboarding"},
                {"Journey Step", ((int)step).ToString()},
            };

        // until we have IOC in place, this will do...
        public IntercomHelper() : this(new DateTimeProvider(), new IntercomClientFactory())
        {
        }

        // for testability, inject the factory
        public IntercomHelper(IDateTimeProvider dateTimeProvider, IIntercomClientFactory intercomClientFactory)
        {
            //_accessToken = "dG9rOjBhYzUzNzYwXzVhY2NfNGY1N184MzRjXzdmOWE1MDVhZTQ1ZToxOjA=";
            _accessToken = System.Configuration.ConfigurationManager.AppSettings["IntercomAccessToken"];
            _intercomClientFactory = intercomClientFactory;
            _dateTimeProvider = dateTimeProvider;
        }


        public void IntercomAddUpdateUser(int userId, int subscriberId)
        {
            // add or update user in intercom from crm
            try
            {
                var userModel = new Helpers.Users().GetUser(userId, subscriberId);
                // note: the subscriber is returned on the userModel
                // defined above and is apparnetly redundant
                var subscriber = new Helpers.Subscribers().GetSubscriber(subscriberId);

                var username = userModel.User.FullName;
                var email = userModel.User.EmailAddress;
                var subscriberName = subscriber.CompanyName;

                var unixTimestamp = _dateTimeProvider.UnixTimeStamp;

                var usersClient = _intercomClientFactory.GetUsersClient(_accessToken);

                // Update user's custom attributes; these must be defined with same names in intercom, only then will these show up there
                var customAttributes = new Dictionary<string, object>
                {
                    { "Browser", userModel.User.BrowserName },
                    { "Browser Version", userModel.User.BrowserVersion },
                    { "City", userModel.User.City },
                    { "Country", userModel.User.CountryName },
                    { "Currency" , userModel.User.CurrencyCode },
                    { "Data Center", userModel.User.DataCenter },
                    { "Job Title", userModel.User.Title },
                    { "Last Login", DateTime.UtcNow.ToString("dd-MMM-yyyy HH:mm:ss") },
                    { "Language", userModel.User.DisplayLanguage },
                    { "Location", userModel.User.LocationName },
                    { "Region", userModel.User.RegionName },
                    { "Screen Size", userModel.User.ScreenResolution },
                    { "State / Province", userModel.User.StateProvince },
                    { "Subscriber Id", subscriberId },
                    { "Subscriber Name", subscriber.CompanyName },
                    { "TimeZone", userModel.User.TimeZone }
                };

                // check if user exists in intercom
                try
                {
                    var intercomUser = usersClient.View(new User() { email = email });
                    if (intercomUser != null)
                    {
                        // intercom user exists
                        intercomUser.custom_attributes = customAttributes;
                        // update intercom user custom attributes
                        var retval = usersClient.Update(intercomUser);

                    }
                }
                catch  
                {
                    // populate intercom company from CRM company (subscriber) for CRM user
                    var intercomCompanies = new List<Company>
                    {
                        new Company()
                        {
                            company_id = userModel.User.SubscriberId.ToString(),
                            name = userModel.Subscriber.CompanyName
                        }
                    };

                    // intercom user does not exist - creat intercom user from CRM user
                    var intercomUser = new User
                    {
                        email = email,
                        user_id = userId.ToString(),
                        name = username,
                        companies = intercomCompanies,
                        last_request_at = unixTimestamp,
                        type = "User"
                    };

                    // create intercom user
                    usersClient.Create(intercomUser);
                    intercomUser.custom_attributes = customAttributes;

                    // update intercom user custom attributes
                    usersClient.Update(intercomUser);

                    // check if intercom company exists
                    var companyClient = _intercomClientFactory.GetCompanyClient(_accessToken);

                    var intercomCompany = companyClient.View(new Company { name = subscriberName });

                    if (intercomCompany == null)
                    {
                        // create intercom company if it doesn't exist
                        intercomCompany = new Intercom.Data.Company
                        {
                            name = subscriberName,
                            company_id = subscriberId.ToString(),
                        };
                        companyClient.Create(intercomCompany);
                    }
                }
            }

            catch (ApiException ex)
            {/* {"type":"error.list","request_id":"b33b6nohspu8okvlf3lg","errors":[{"code":"not_found","message":"User Not Found"}]}*/
                Console.Write(ex.ApiResponseBody);
            }

            catch (Exception ex)
            {
                Console.Write(ex.StackTrace);
            }
        }

        public IntercomModel GetIntercomData(int userId, int subscriberId)
        {
            try
            {
                // get user / subscriber data from intercom and return json to CRM
                var userModel = new Helpers.Users().GetUser(userId, subscriberId);
                if (userModel != null)
                {
                    var intercomData = new IntercomModel
                    {
                        AccessToken = System.Configuration.ConfigurationManager.AppSettings["IntercomAccessToken"],
                        Username = userModel.User.FullName,
                        Email = userModel.User.EmailAddress,
                        City = userModel.User.City,
                        Country = userModel.User.CountryName,
                        SubscriberId = userModel.User.SubscriberId
                    };
                    var subscriber = new Helpers.Subscribers().GetSubscriber(intercomData.SubscriberId);
                    intercomData.SubscriberName = subscriber.CompanyName;
                    intercomData.Timezone = userModel.User.TimeZone;
                    intercomData.JobTitle = userModel.User.Title;

                    //var webUrl = "";        // CRM subscriber's Account URL

                    // date timestamp
                    var date = new DateTime(1970, 1, 1, 0, 0, 0);
                    long unixTimestamp = System.Convert.ToInt64((System.DateTime.UtcNow - date).TotalSeconds);
                    intercomData.CreatedDateTime = unixTimestamp.ToString();

                    return intercomData;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public Event IntercomTrackEvent(int userId, int subscriberId, string eventName)
        {
            var intercomModel = GetIntercomData(userId, subscriberId);
            return IntercomTrackEvent(userId, intercomModel, eventName);
        }

        public Event IntercomTrackEvent(int userId, IntercomModel intercomData, string eventName)
        {
            try
            {
                if (intercomData == null)
                    return null;

                var eventsClient = _intercomClientFactory.GetEventsClient(_accessToken);
                var eventToLog = new Event()
                {
                    user_id = userId.ToString(),
                    email = intercomData.Email,
                    event_name = eventName,
                    created_at = _dateTimeProvider.UnixTimeStamp,
                };
                var createdIntercomEvent = eventsClient.Create(eventToLog);
                return createdIntercomEvent;
            }
            catch (Exception ex)
            {
                var error = new WebAppError
                {
                    ErrorCallStack = ex.StackTrace,
                    ErrorDateTime = DateTime.UtcNow,
                    ErrorMessage = ex.ToString(),
                    RoutineName = "IntercomTrackEvent",
                    SubscriberName = "",
                    UserId = userId,
                    SubscriberId = intercomData.SubscriberId,
                };
                new Logging().LogWebAppError(error);                 return null;
            }
        }

        public Event TrackUserOnboardingStep(int userId, int subscriberId, UserOnboardingStep step)
        {
            var crmUserModel = new Users().GetUser(userId, subscriberId);
            var intercomUsersClient = _intercomClientFactory.GetUsersClient(_accessToken);
            var customAttributes = OnboardingAttributes(step);
            var intercomUser = _intercomClientFactory.InitIntercomUser(crmUserModel, customAttributes);

            // update intercom user
            intercomUsersClient.Update(intercomUser);

            // track intercom event
            var intercomModel = GetIntercomData(userId, subscriberId);
            var trackedEvent = IntercomTrackEvent(crmUserModel.User.UserId, intercomModel, IntercomEvents.OnboardingEvents[step]);
            return trackedEvent;
        }

        private void TrackPaidForSubscription(int userId)
        {
            // to set the conversion goal at the last message, we need to know when a user has paid for a subscription

            //TODO: intercom event for subscription payment

            var intercomAccessToken = System.Configuration.ConfigurationManager.AppSettings["IntercomAccessToken"];
            EventsClient eventsClient = new EventsClient(new Authentication(intercomAccessToken));
            //Event ev = eventsClient.Create(new Event()
            //{
            //    user_id = "1000_user_id",
            //    email = "user_email@example.com",
            //    event_name = "paid for a subscription"
            //});
        }
    }
}
