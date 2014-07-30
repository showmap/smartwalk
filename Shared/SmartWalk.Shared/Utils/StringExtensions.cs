using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartWalk.Shared.Utils
{
    public static class StringExtensions
    {
        private static readonly char[] Separators = {' ', '-', '/', ',', '.', ':', '&' , '@'};

        public static bool EqualsIgnoreCase(this string str1, string str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this IEnumerable<string> strings, string str)
        {
            return strings.Contains(str, StringComparer.OrdinalIgnoreCase);
        }

        public static string TrimIt(this string str)
        {
            return str != null ? str.Trim() : null;
        }

        public static string GetAbbreviation(this string name, byte length = byte.MaxValue)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;

            return 
                name
                    .Split(Separators)
                    .Select(str => str
                        .ToCharArray()
                        .FirstOrDefault(char.IsLetterOrDigit))
                    .Where(ch => ch != default(char))
                    .Take(length)
                    .Aggregate(
                        string.Empty, 
                        (str, ch) => str + ch.ToString().ToUpperInvariant());
        }
    }
}