using System;
using System.Globalization;

namespace SmartWalk.Server.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime? date, string format, CultureInfo culture)
        {
            return date.HasValue ? date.Value.ToString(format, culture) : string.Empty;
        }
    }
}