using Crm6.App_Code;
using Crm6.App_Code.Shared;
using Helpers;
using Models;
using System.Collections.Generic;
using System.Web.Http;

namespace API
{
    public class CalendarEventController : ApiController
    {

        // calendar event save
        [AcceptVerbs("POST")]
        public int SaveCalendarEvent([FromBody]ActivityModel calendarEventItem)
        {
            return new CalendarEvents().SaveCalendarEvent(calendarEventItem);
        }


        // get calendar events
        [AcceptVerbs("POST")]
        public List<Crm6.App_Code.Shared.Activity> GetCalendarEvents(CalendarEventFilter filters)
        {
            return new CalendarEvents().GetCalendarEvents(filters);
        }

        // get company calendar event
        [AcceptVerbs("GET")]
        public ActivityModel GetCalendarEvent([FromUri]int calendarEventId, int userId, int subscriberId)
        {
            return new CalendarEvents().GetCalendarEvent(calendarEventId, userId, subscriberId);
        }

        // get company calendar events
        [AcceptVerbs("POST")]
        public List<ActivityExtended> GetCompanyCalendarEvents(CalendarEventFilter filters)
        {
            return new CalendarEvents().GetCompanyCalendarEvents(filters);
        }

        // get contact calendar events
        [AcceptVerbs("POST")]
        public List<ActivityExtended> GetContactCalendarEvents(CalendarEventFilter filters)
        {
            return new CalendarEvents().GetContactCalendarEvents(filters);
        }


        [AcceptVerbs("GET")]
        public List<ActivityExtended> GetDealEvents(int dealId, int userId, int subscriberId)
        {
            return new CalendarEvents().GetDealEvents(dealId, userId, subscriberId);
        }

        [AcceptVerbs("GET")]
        public List<ActivityExtended> GetUserEventsForActivities(int userId, int subscriberId)
        {
            return new CalendarEvents().GetUserEventsForActivities(userId, subscriberId);
        }

        [AcceptVerbs("GET")]
        public bool DeleteCalendarEvent([FromUri]int calendarEventId, int userId)
        {
            return new CalendarEvents().DeleteCalendarEvent(calendarEventId, false, userId);
        }


        [AcceptVerbs("GET")]
        public bool DeleteCalendarEvent([FromUri]int calendarEventId, [FromUri]bool deleteRecurring, int userId, int subscriberId)
        {
            return new CalendarEvents().DeleteCalendarEvent(calendarEventId, deleteRecurring, userId, subscriberId);
        }


        [AcceptVerbs("POST")]
        public bool UpdateCalendarEventDates([FromBody]Crm6.App_Code.Shared.Activity calendarEvent)
        {
            return new CalendarEvents().UpdateCalendarEventDates(calendarEvent);
        }


        [AcceptVerbs("GET")]
        public List<ActivititesMember> GetCalendarEventInvites([FromUri]int calendarEventId, int subscriberId)
        {
            return new CalendarEvents().GetCalendarEventInvites(calendarEventId, subscriberId);
        }

        [AcceptVerbs("POST")]
        public int SaveEventCategory([FromBody]EventCategory category)
        {
            return new CalendarEvents().SaveEventCategory(category);
        }

        [AcceptVerbs("GET")]
        public bool DeleteEventCategory([FromUri]int id, int userId, int subscriberId)
        {
            return new CalendarEvents().DeleteEventCategory(id, userId, subscriberId);
        }

        [AcceptVerbs("GET")]
        public List<EventCategory> GetEventCategories(int subscriberId)
        {
            return new CalendarEvents().GetEventCategories(subscriberId);
        }

    }
}
