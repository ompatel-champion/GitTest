using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crm6.App_Code.Sync
{
    public class RecurringEventProperties
    {
        public const int DailyNumberOfCalendarEvents = 30;
        public const int WeeklyNumberOfCalendarEvents = 8;
        public const int MonthlyNumberOfCalendarEvents = 3;
    }

    public enum RecurringType
    {
        NONE,
        DAILY,
        WEEKLY,
        MONTHLY
    }
}