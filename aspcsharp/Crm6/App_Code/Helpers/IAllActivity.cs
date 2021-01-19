using System.Collections.Generic;
using Crm6.App_Code.Shared;
using Models;

namespace Helpers
{
    public interface IActivity
    {
        List<Crm6.App_Code.Shared.Activity> GetCalendarEvents(CalendarEventFilter filters, bool onlyOne = false);
    }
}