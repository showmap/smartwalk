using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWalk.Server.Extensions
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
    }
}