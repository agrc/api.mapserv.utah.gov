using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace WebAPI.Common.Extensions
{
    public static class StringExtensions
    {
        public static string DefaultIfNullOrEmpty(this string x, string defaultValue)
        {
            return string.IsNullOrEmpty(x) ? defaultValue : x;
        }

        public static string DeCamel(this string text)
        {
            return Regex.Replace(text, @"([A-Z])", @" $&").Trim();
        }

        /// <summary>
        ///   Replaces string.format witha more fluent "".with(param) syntax
        /// </summary>
        /// <param name="source"> The string to format. </param>
        /// <param name="args"> The args. </param>
        /// <returns> A formatted string </returns>
        public static string With(this string source, params object[] args)
        {
            return string.Format(source, args);
        }

        public static string Join(this IEnumerable<string> source, string separater)
        {
            return string.Join(separater, source);
        }

        public static string ToCamelCase(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (!char.IsUpper(s[0]))
                return s;

            var sb = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var hasNext = (i + 1 < s.Length);
                if ((i == 0 || !hasNext) || char.IsUpper(s[i + 1]))
                {
                    var lowerCase = char.ToLower(s[i], CultureInfo.InvariantCulture);
                    sb.Append(lowerCase);
                }
                else
                {
                    sb.Append(s.Substring(i));
                    break;
                }
            }

            return sb.ToString();
        }

    }
}