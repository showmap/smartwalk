using System;

namespace SmartWalk.Server.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime? date, string format)
        {
            return date.HasValue ? date.Value.ToString(format) : string.Empty;
        }
    }
}