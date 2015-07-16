using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QPP
{
    public static class StringExpension
    {
        private const int LOCALE_SYSTEM_DEFAULT = 0x0800;
        private const int LCMAP_SIMPLIFIED_CHINESE = 0x02000000;
        private const int LCMAP_TRADITIONAL_CHINESE = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int Locale, int dwMapFlags, string lpSrcStr, int cchSrc, [Out] string lpDestStr, int cchDest);

        public static bool IsMatch(this string str, string value)
        {
            if (str.IsNullOrEmpty() || value.IsNullOrEmpty())
                return false;
            var a = str.ToLower();
            var b = value.ToLower();
            return a.Contains(b) || b.Contains(a);
        }

        /// <summary>
        /// 不区分大小写
        /// </summary>
        public static bool CIEquals(this string str, string value)
        {
            if (str == null)
                return value == null;
            return str.Equals(value, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        public static bool IsNotEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static string FormatArgs(this string str, params object[] args)
        {
            return string.Format(str, args);
        }
        /// <summary>
        /// Concat strings by specified separator.
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="separator"></param>
        /// <returns>string</returns>
        public static string Concat(this IEnumerable<string> arr, string separator)
        {
            if (object.Equals(arr, null) || !arr.Any())
                return string.Empty;
            var str = "";
            foreach (var s in arr)
                str += s + separator;
            return str.Remove(str.Length - separator.Length);
        }

        /// <summary>
        /// 簡體轉繁體
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToTraditional(this string source)
        {
            String target = new String(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_TRADITIONAL_CHINESE, source, source.Length, target, source.Length);
            return target;
        }

        /// <summary>
        /// 繁體轉簡體
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToSimplified(this string source)
        {
            String target = new String(' ', source.Length);
            int ret = LCMapString(LOCALE_SYSTEM_DEFAULT, LCMAP_SIMPLIFIED_CHINESE, source, source.Length, target, source.Length);
            return target;
        }
    }
}
