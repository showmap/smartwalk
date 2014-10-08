using System;
using System.Globalization;

namespace SmartWalk.Shared.Utils
{
    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime? date, string format, CultureInfo culture)
        {
            return date.HasValue ? date.Value.ToString(format, culture) : string.Empty;
        }

        public static bool IsMultiDay(DateTime? startDate, DateTime? endDate)
        {
            var result = DaysCount(startDate, endDate) > 1;
            return result;
        }

        public static int DaysCount(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue && endDate.HasValue)
            {
                var span = endDate.Value - startDate.Value;
                var result = span.Days + 1;
                return result;
            }

            return 0;
        }

        public static bool IsTimeThisDay(
            this DateTime time,
            DateTime? day,
            Tuple<DateTime?, DateTime?> range = null)
        {
            return IsTimeThisDay((DateTime?)time, day, range);
        }

        public static bool IsTimeThisDay(
            this DateTime? time, 
            DateTime? day, 
            Tuple<DateTime?, DateTime?> range = null)
        {
            if (!time.HasValue || !day.HasValue) return true; // if time is not set we asume it goes to all days

            var t = time.Value;
            var nextDay = day.Value.AddDays(1);
            var firstDay = range != null ? range.Item1 : null;
            var lastDay = range != null ? range.Item2 ?? range.Item1 : null; // using first day as last one to do not lose some out-of-range shows

            var result =
                (firstDay != null && t.Date <= firstDay && day == firstDay) || // times ahead of first day
                (t.Date == day && t.Hour >= 6) ||
                (t.Date == nextDay && t.Hour < 6) || // late night times go to next day
                (lastDay != null && t.Date >= lastDay && day == lastDay); // times behind of last day

            return result;
        }
    }
}