using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SmartWalk.Shared.Utils
{
    public static class StringExtensions
    {
        private static readonly char[] Separators = { ' ', '-', '/', ',', '.', ':', '&', '@' };

        private const string HTMLRegExp =
            @"</{0,1}(!DOCTYPE|a|abbr|acronym|address|applet|area|article|" +
            "aside|audio|b|base|basefont|bdi|bdo|big|blockquote|body|br|button|canvas|caption|center" +
            "|cite|code|col|colgroup|datalist|dd|del|details|dfn|dialog|dir|div|dl|dt|em|embed|fieldset" +
            "|figcaption|figure|font|footer|form|frame|frameset|h1|h2|h3|h4|h5|h6|head|header|hr|html|i|" +
            "iframe|img|input|ins|kbd|keygen|label|legend|li|link|main|map|mark|menu|menuitem|meta|meter|nav|" +
            "noframes|noscript|object|ol|optgroup|option|output|p|param|pre|progress|q|rp|rt|ruby|s|samp|script|" +
            "section|select|small|source|span|strike|strong|style|sub|summary|sup|table|tbody|td|textarea|tfoot|th|" +
            @"thead|time|title|tr|track|tt|u|ul|var|video|wbr){1}(\s*/{0,1}>|\s+.*?/{0,1}>)";

        private const string URLRegExp =
            @"((www\.|(http|https|ftp|news|file)+\:\/\/)[&#95;.a-z0-9-]+\." +
            @"[a-z0-9\/&#95;:@=.+?,##%&~-]*[^.|\'|\# |!|\(|?|,| |>|<|;|\)])";

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
                    .Select(
                        str =>
                        str.ToCharArray()
                           .FirstOrDefault(char.IsLetterOrDigit))
                    .Where(ch => ch != default(char))
                    .Take(length)
                    .Aggregate(
                        string.Empty,
                        (str, ch) => str + ch.ToString().ToUpperInvariant());
        }

        public static string StripTags(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var regex = new Regex(HTMLRegExp, RegexOptions.Singleline);
            var result = regex.Replace(text, string.Empty);
            return result;
        }

        public static string ActivateLinks(this string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            var regex = new Regex(URLRegExp, RegexOptions.IgnoreCase);
            var result = regex
                .Replace(text, "<a href=\"$1\">$1</a>")
                .Replace("href=\"www", "href=\"http://www");

            regex = new Regex(@">[a-z]*\:\/\/", RegexOptions.IgnoreCase);
            result = regex.Replace(result, ">");

            return result;
        }

        public static bool IsWebUrl(this string text)
        {
            Uri result;
            return Uri.TryCreate(text, UriKind.Absolute, out result) && 
                result.Scheme.ToLowerInvariant().StartsWith("http");
        }
    }
}