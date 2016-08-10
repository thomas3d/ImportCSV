using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace dk.theng.Helpers
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {

        /// <summary>
        /// Determines whether the string [is null or empty]
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified s]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Determines whether [is not null or empty] [the specified s].
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        ///   <c>true</c> if [is not null or empty] [the specified s]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrEmpty(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// Determines whether the specified s is empty.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        ///   <c>true</c> if the specified s is empty; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEmpty(this string s)
        {
            return (s.Length == 0);
        }

        /// <summary>
        /// Determines whether the specified s is null.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        ///   <c>true</c> if the specified s is null; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNull(this string s)
        {
            return (s == null);
        }

        /// <summary>
        /// Determines whether [is not null] [the specified s].
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns>
        ///   <c>true</c> if [is not null] [the specified s]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNull(this string s)
        {
            return (s != null);
        }

        /// <summary>
        /// Ases the null if empty.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        public static string AsNullIfEmpty(this string s)
        {
            if (String.IsNullOrEmpty(s))
            {
                return null;
            }

            return s;
        }

        public static string StringBefore(this string s, string match, bool returnEmptyIfNotFound = true)
        {
            int pos = s.IndexOf(match);
            if (pos == -1)
                return returnEmptyIfNotFound? String.Empty:s;
            else
                return s.Substring(0, pos);
        }

        public static string StringAfter(this string s, string match, bool returnEmptyIfNotFound = true)
        {
            int pos = s.IndexOf(match);
            if (pos == -1)
                return returnEmptyIfNotFound ? String.Empty : s;
            else
                return s.Substring(pos + match.Length);
        }

        public static string StringBetween(this string s, string match1, string match2, bool returnEmptyIfNotFound = true)
        {
            string tmp = StringAfter(s, match1, returnEmptyIfNotFound);
            int pos = tmp.IndexOf(match2);
            if (pos == -1)
                return returnEmptyIfNotFound ? String.Empty : s;
            else
                return tmp.Substring(0, pos);
        }

        public static IEnumerable<string> StringsBetween(this string s, string match1, string match2)
        {
            List<string> result = new List<string>();
            string tmp = StringAfter(s, match1);
            while (tmp.StartsWith("\r\n"))
                tmp = tmp.Substring(2);
            int pos = tmp.IndexOf(match2);
            while (pos != -1)
            {
                if(pos > 0)
                    result.Add(tmp.Substring(0, pos));

                tmp = StringAfter(tmp.Substring(pos), match1);
                while (tmp.StartsWith("\r\n"))
                    tmp = tmp.Substring(2);
                pos = tmp.IndexOf(match2);
            }

            return result;
        }

        public static string StringDelete(this string s, string beginAt, string endAfter)
        {
            string tmp = String.Empty;
            int pos = s.IndexOf(beginAt);
            if (pos != -1)
                tmp = s.Substring(0, pos);
            return tmp + StringAfter(s, endAfter);

        }

        public static string ReplaceBetween(this string s, string match1, string match2, string newStr)
        {
            if (!s.Contains(match1))
                return s;

            int pos = s.IndexOf(match1) + match1.Length;
            string s2 = s.Substring(pos);

            if (!s2.Contains(match2))
                return s;

            return StringBefore(s, match1) + match1 + newStr + match2 + StringAfter(s2, match2);
        }

        public static int ToInt(this string s)
        {
            return int.Parse(s);
        }

        public static DateTime ToDateTime(this string s)
        {
            return DateTime.Parse(s, CultureInfo.InvariantCulture);
        }

        public static IEnumerable<int> ToIntList(this string s, char splitAt)
        {
            List<int> list = new List<int>();
            if (string.IsNullOrWhiteSpace(s))
                return list;
            foreach (string i in s.Split(splitAt))
                list.Add(int.Parse(i));
            return list;
        }

        public static Dictionary<string,string> ToDictionary(this string s)
        {
            return s.Split('&').Select(x => x.Split('=')).ToDictionary(y => y[0], y => y[1]);
        }

        public static string RemoveHTML(this string s)
        {
            string cleanBodyText = Regex.Replace(s, @"<[^>]*>", String.Empty);
            return cleanBodyText.Replace("&nbsp;", " ");
        }

        public static string RemoveNewLine(this string s)
        {
            return s.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        }

        // the string.Split() method from .NET tend to run out of memory on 80 Mb strings. 
        // this has been reported several places online. 
        // This version is fast and memory efficient and return no empty lines. 
        public static List<string> LowMemSplit(this string s, string seperator)
        {
            List<string> list = new List<string>();
            int lastPos = 0;
            int pos = s.IndexOf(seperator);
            while (pos > -1)
            {
                while(pos == lastPos)
                {
                    lastPos += seperator.Length;
                    pos = s.IndexOf(seperator, lastPos);
                    if (pos == -1)
                        return list;
                }

                string tmp = s.Substring(lastPos, pos - lastPos);
                if(tmp.Trim().Length > 0)
                    list.Add(tmp);
                lastPos = pos + seperator.Length;
                pos = s.IndexOf(seperator, lastPos);
            }

            if (lastPos < s.Length)
            {
                string tmp = s.Substring(lastPos, s.Length - lastPos);
                if (tmp.Trim().Length > 0)
                    list.Add(tmp);
            }

            return list;
        }
    }
}
