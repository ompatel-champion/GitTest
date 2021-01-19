using System;
using System.Linq;
using System.Globalization;
using Crm6.App_Code.Shared;
using Crm6.App_Code;
using System.Collections.Generic;

namespace Helpers
{
    public class Timezones
    {

        public Crm6.App_Code.Shared.TimeZone GetTimezone(int id)
        {
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            return sharedContext.TimeZones.FirstOrDefault(t => t.TimeZoneId == id);
        }

        /// <summary>
        /// Use the cached version if calling it on an iteration.
        /// </summary>
        /// <param name="userDateTime"></param>
        /// <param name="userId"></param>
        /// <param name="subscriberId"></param>
        /// <returns></returns>
        public DateTime ConvertUserDateTimeToUtc(DateTime userDateTime, int userId, int subscriberId)
        {
            var user = new Users().GetUser(userId, subscriberId);
            // Get Offset Hours from User TimeZone to UTC
            var utcOffset = GetUtcOffsetFromUserTimeZone(user.User.TimeZone, userDateTime);
            // Get Offset Seconds from User TimeZone to UTC
            var utcOffsetSeconds = Convert.ToInt32(ConvertUtcOffsetToSeconds(utcOffset));
            var utcDateTime = userDateTime.AddSeconds(utcOffsetSeconds);
            return utcDateTime;
        }

        public DateTime ConvertUserDateTimeToUtcCached(DateTime userDateTime, User user, List<TimeZonesDaylightSaving> listTimezonesDstCached, List<Crm6.App_Code.Shared.TimeZone> listTimezonesCached)
        {
            // Get Offset Hours from User TimeZone to UTC
            var utcOffset = GetUtcOffsetFromUserTimeZoneCached(user.TimeZone, userDateTime, listTimezonesDstCached, listTimezonesCached);
            // Get Offset Seconds from User TimeZone to UTC
            var utcOffsetSeconds = Convert.ToInt32(ConvertUtcOffsetToSeconds(utcOffset));
            var utcDateTime = userDateTime.AddSeconds(utcOffsetSeconds);
            return utcDateTime;
        }

        private string GetUtcOffsetFromUserTimeZoneCached(string userTimeZone, DateTime userDateTime, List<TimeZonesDaylightSaving> listTimezonesDstCached, List<Crm6.App_Code.Shared.TimeZone> listTimezonesCached)
        {
            if (string.IsNullOrWhiteSpace(userTimeZone))
            {
                return "";
            }

            var utcOffset = "";
            // Check for Daylight Savings Time 
            utcOffset = listTimezonesDstCached.Where(t => t.TimeZoneName.Trim() == userTimeZone.Trim()
                                                               && t.DstStartDate <= userDateTime
                                                               && t.DstEndDate >= userDateTime
                                                             ).OrderByDescending(t => t.TimeZonesDaylightSavingsId)
                                                              .Select(t => t.UtcOffset).FirstOrDefault();
            if (utcOffset == null)
            {
                // Not Daylight Savings Time
                utcOffset = listTimezonesCached.Where(t => t.TimeZoneName == userTimeZone).Select(t => t.UtcOffset).FirstOrDefault();
            }
            return utcOffset;
        }


        public DateTime ConvertUtcToUserDateTime(DateTime utcDateTime, int userId, string userTimeZone = "")
        {
            if (string.IsNullOrEmpty(userTimeZone))
            { 
                var connection = LoginUser.GetConnection();
                var context = new DbFirstFreightDataContext(connection);
                userTimeZone = context.Users.Where(t => t.UserId == userId).Select(t => t.TimeZone).FirstOrDefault();
            }

            // Get Offset Hours from User TimeZone to UTC
            var utcOffset = GetUtcOffsetFromUserTimeZone(userTimeZone, utcDateTime);
            var offSetWithoutPlus = utcOffset;
            if (utcOffset.Contains("+"))
            {
                offSetWithoutPlus = offSetWithoutPlus.Replace("+", "");
            }

            if (string.IsNullOrWhiteSpace(offSetWithoutPlus) == false)
            {
                TimeSpan offSet = TimeSpan.Parse(offSetWithoutPlus);
                DateTime userDateTime = utcDateTime + offSet;
                return userDateTime;
            }

            // Get Offset Seconds from User TimeZone to UTC
            //  var utcOffsetSeconds = Convert.ToInt32(ConvertUtcOffsetToSeconds(utcOffset)) * -1;
            //  var userDateTime = utcDateTime.AddSeconds(utcOffsetSeconds);
            return utcDateTime;
        }


        public int ConvertUtcOffsetToSeconds(string utcOffset)
        {
            // ex: +02:00
            var offsetPlusMinus = utcOffset.Substring(0, 1);
            var offSetWithoutPlusMinus = utcOffset.Replace(offsetPlusMinus, "");
            var utcOffsetSeconds = TimeSpan.ParseExact(offSetWithoutPlusMinus, new[] { @"hh\:mm" }, CultureInfo.InvariantCulture).TotalSeconds;
            var offsetPlusMinusInt = offsetPlusMinus == "-" ? 1 : -1;
            return (int)(offsetPlusMinusInt * utcOffsetSeconds);
        }


        public string GetUtcOffsetFromUserTimeZone(string userTimeZone, DateTime userDateTime)
        {
            if (string.IsNullOrWhiteSpace(userTimeZone))
            {
                return "";
            }

            var utcOffset = "";
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            // Check for Daylight Savings Time 
            utcOffset = sharedContext.TimeZonesDaylightSavings.Where(t => t.TimeZoneName.Trim() == userTimeZone.Trim()
                                                               && t.DstStartDate <= userDateTime
                                                               && t.DstEndDate >= userDateTime
                                                             ).OrderByDescending(t => t.TimeZonesDaylightSavingsId)
                                                              .Select(t => t.UtcOffset).FirstOrDefault();
            if (utcOffset == null)
            {
                // Not Daylight Savings Time
                utcOffset = sharedContext.TimeZones.Where(t => t.TimeZoneName == userTimeZone).Select(t => t.UtcOffset).FirstOrDefault();
            }
            return utcOffset;
        }


        public Crm6.App_Code.Shared.TimeZone GetTimeZoneIdByTimeZoneNameOffcetAndName(string timezoneName, string cityNames, string utcOffset)
        {
            timezoneName += ""; cityNames += ""; utcOffset += "";
            var sharedConnection = LoginUser.GetSharedConnection();
            var sharedContext = new DbSharedDataContext(sharedConnection);
            var timezone = sharedContext.TimeZones.FirstOrDefault(t =>
                                                      t.TimeZoneName.ToLower().Equals(timezoneName.ToLower()) &&
                                                      t.CityNames.ToLower().Equals(cityNames.ToLower()) &&
                                                      t.UtcOffset.ToLower().Equals(utcOffset.ToLower()));
            return timezone;
        }

    }
}
