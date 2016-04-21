using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Soe.Common.Extensions
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
    }
}