using Helpers;
using Models;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using Crm6.App_Code;
using Crm6.App_Code.Helpers;

namespace API
{
    public class ReportController : ApiController
    {
        private readonly IntercomHelper _intercomHelper;
        private readonly Logging _logging;

        public ReportController() : this(new IntercomHelper(), new Logging())
        {
        }

        public ReportController(IntercomHelper intercomHelper, Logging logging)
        {
            _intercomHelper = intercomHelper;
            _logging = logging;
        }

        [AcceptVerbs("POST")]
        public DealsReportResponse GetDealsReport([FromBody]DealsReportFilters filters)
        {
            DealsReportResponse response;
            try
            {
                response = new DealsReport().GetDealsForReport(filters);
                // log user activity
                _logging.LogUserAction(new UserActivity
                {
                    UserId = filters.UserId,
                    UserActivityMessage = "Deals Report",
                    SubscriberId = filters.SubscriberId
                });
                // Log action to intercom

                // DONE intercom Journey Step event
                var eventName = "Ran Deals report";
                _intercomHelper.IntercomTrackEvent(filters.UserId, filters.SubscriberId, eventName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return response;
        }


        [AcceptVerbs("POST")]
        public ActivityByDateRangeReportResponse GetActivitiesByDateRangeReport([FromBody] ActivityByDateRangReportFilters filters)
        {
            ActivityByDateRangeReportResponse response;
            try
            {
                response = new ActitivtyByDateRangeReport().GetReport(filters);
                // log user activity
                _logging.LogUserAction(new UserActivity
                {
                    UserId = filters.LoggedInUserId,
                    UserActivityMessage = "Activities By DateRange Report"
                });

                // Log action to intercom
                var eventName = "Activities By DateRange Report";
                //Done: intercom Journey Step event
                _intercomHelper.IntercomTrackEvent(filters.LoggedInUserId, filters.SubscriberId, eventName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return response;
        }


        [AcceptVerbs("POST")]
        public WeeklyActivityReportResponse GetActivitiesByWeekReport([FromBody] WeeklyActivityFilters filters)
        {
            WeeklyActivityReportResponse response;
            try
            {
                response = new WeeklyActivityReport().GetReport(filters);
                // log user activity
                _logging.LogUserAction(new UserActivity
                {
                    UserId = filters.LoggedInUserId,
                    UserActivityMessage = "Weekly Activity Report"
                });

                // Log action to intercom
                //DONE: intercom Journey Step event
                var eventName = "Weekly Activity Report";
                _intercomHelper.IntercomTrackEvent(filters.LoggedInUserId, filters.SubscriberId, eventName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return response;
        }


        [AcceptVerbs("POST")]
        public SalesRepKPIReportResponse GetSalesrepKpiReport([FromBody] SalesRepKPIReportFilters filters)
        {
            SalesRepKPIReportResponse response;
            try
            {
                response = new SalesRepKPIReport().GetReport(filters);

                // log user activity
                _logging.LogUserAction(new UserActivity
                {
                    UserId = filters.UserId,
                    UserActivityMessage = "Sales Rep KPI Report"
                });

                // Log action to intercom
                //TODO: intercom Journey Step event
                var eventName = "Sales Rep KPI Report";
                _intercomHelper.IntercomTrackEvent(filters.UserId, filters.SubscriberId, eventName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return response;
        }

        [AcceptVerbs("POST")]
        public UserActivityReportResponse GetUserActivityReport([FromBody] UserActivityReportFilters filters)
        {
            UserActivityReportResponse response;
            try
            {
                response = new UserActivityReport().GetReport(filters);
                // log user activity
                _logging.LogUserAction(new UserActivity
                {
                    UserId = filters.UserId,
                    UserActivityMessage = "User Activity Report"
                });

                // Log action to intercom
                var eventName = "User Activity Report";
                //Done: intercom Journey Step event
                _intercomHelper.IntercomTrackEvent(filters.UserId, filters.SubscriberId, eventName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return response;
        }

        [AcceptVerbs("POST")]
        public CompaniesReportResponse GetCompaniesReport([FromBody] CompaniesReportFilters filters)
        {
            CompaniesReportResponse response;
            try
            {
                response = new CompaniesReport().GetReport(filters);
                // log user activity
                _logging.LogUserAction(new UserActivity
                {
                    UserId = filters.UserId,
                    UserActivityMessage = "Companies Report"
                });

                // Log action to intercom
                var eventName = "Companies Report";
                //Done: intercom Journey Step event
                _intercomHelper.IntercomTrackEvent(filters.UserId, filters.SubscriberId, eventName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return response;
        }


        [AcceptVerbs("GET")]
        public List<SelectList> GetLocations([FromUri] string countryCodes, string services, string keyword, int subscriberId)
        {
            List<SelectList> list;
            try
            {
                list = new DealsReport().GetLocationsByCountryCodesServices(countryCodes, services, keyword, subscriberId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return list;
        }

    }
}
