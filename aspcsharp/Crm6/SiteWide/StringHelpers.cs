using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crm6.SiteWide
{
    public static class StringHelpers
    {
        private static string FormatString(string stringToFormat, string stringFormat)
        {
            return string.IsNullOrWhiteSpace(stringToFormat) ? string.Empty : string.Format(stringFormat, stringToFormat);
        }

        public static string FormatUsername(string stringToFormat)
        {
            return FormatString(stringToFormat, Settings.UsernameFormat);
        }

        public static string FormatSource(string stringToFormat)
        {
            return string.IsNullOrWhiteSpace(stringToFormat) || stringToFormat == 0.ToString() ? "-" : stringToFormat;
        }

        public static string FormatDivision(string stringToFormat)
        {
            return FormatString(stringToFormat, Settings.DivisionFormat);
        }
    }
}