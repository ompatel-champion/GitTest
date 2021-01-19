using Crm6.App_Code;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class ActivityController : ApiController
    {
  

        [AcceptVerbs("POST")]
        public ActivityChartDataResponse GetActivityChartData([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartData(filters);
        }


        [AcceptVerbs("GET")]
        public void UpdateActivities()
        {
            new DataUpdater().UpdateActivities();
        }

        [AcceptVerbs("GET")]
        public bool IsActivityRecurring([FromUri]int activityId)
        {
            return new Activities().IsActivityRecurring(activityId);
        }


        [AcceptVerbs("POST")]
        public List<DealModel> GetActivityChartNewDeals([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartNewDeals(filters);
        }


        [AcceptVerbs("POST")]
        public List<DealModel> GetActivityChartLostDeals([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartLostDeals(filters);
        }


        [AcceptVerbs("POST")]
        public List<DealModel> GetActivityChartWonDeals([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartWonDeals(filters);
        }


        [AcceptVerbs("POST")]
        public List<UserActivity> GetActivityChartLogins([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartLogins(filters);
        }


        [AcceptVerbs("POST")]
        public List<CalendarEventModel> GetActivityChartEvents([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartEvents(filters);
        }


        [AcceptVerbs("POST")]
        public List<Crm6.App_Code.Shared.Activity> GetActivityChartTasks([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartTasks(filters);
        }


        [AcceptVerbs("POST")]
        public List<NoteModel> GetActivityChartNotes([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartNotes(filters);
        }


        [AcceptVerbs("POST")]
        public List<Company> GetActivityChartCompanies([FromBody]ActivityChartDataFilter filters)
        {
            return new Activities().GetActivityChartCompanies(filters);
        }


        [AcceptVerbs("POST")]
        public List<Crm6.App_Code.Shared.Activity> GetActivitiesForCalendar([FromBody] ActivityFilter filters)
        {
            return new Activities().GetActivitiesForCalendar(filters);
        }


        [AcceptVerbs("POST")]
        public bool ChangeActivityTaskDate([FromBody]Crm6.App_Code.Shared.Activity task)
        {
            return new Activities().ChangeActivityTaskDate(task);
        }

    }


}