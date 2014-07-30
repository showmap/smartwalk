using System;

namespace SmartWalk.Shared.Utils
{
    public static class DateTimeExtensions
    {
        public static bool IsTimeThisDay(
            this DateTime time,
            DateTime? day,
            Tuple<DateTime, DateTime?> range = null)
        {
            return IsTimeThisDay((DateTime?)time, day, range);
        }

        public static bool IsTimeThisDay(
            this DateTime? time, 
            DateTime? day, 
            Tuple<DateTime, DateTime?> range = null)
        {
            if (!time.HasValue || !day.HasValue) return true; // if time is not set we asume it goes to all days

            var t = time.Value;
            var nextDay = day.Value.AddDays(1);
            var firstDay = range != null ? (DateTime?)range.Item1 : null;
            var lastDay = range != null ? range.Item2 : null;

            var result =
                (firstDay != null && t.Date <= firstDay && day == firstDay) || // times ahead of first day
                (t.Date == day && t.Hour >= 6) ||
                (t.Date == nextDay && t.Hour < 6) || // late night times go to next day
                (lastDay != null && t.Date >= lastDay && day == lastDay); // times behind of last day

            return result;
        }
    }
}