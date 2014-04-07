using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SmartWalk.Shared.Extensions
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> strings, string str)
        {
            return strings.Contains(str, StringComparer.OrdinalIgnoreCase);
        }

        public static DateTime? ParseDateTime(this string dtValue, CultureInfo culture)
        {
            DateTime dtParse;

            if (DateTime.TryParse(dtValue, culture, DateTimeStyles.None, out dtParse))
                return dtParse;

            return null;
        }

        public static string TrimIt(this string str)
        {
            return str != null ? str.Trim() : null;
        }
    }
}