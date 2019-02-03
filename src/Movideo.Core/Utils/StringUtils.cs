using System;
using System.Text.RegularExpressions;

namespace Grappachu.Movideo.Core.Utils
{
    public static class StringUtils
    {
        /// <summary>
        /// Gets the first instance of the subtring inside the two portions of string specified.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="beforeStr"></param>
        /// <param name="afterStr"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static string TakeBetween(this string src, string beforeStr, string afterStr,
            StringComparison comparison = StringComparison.Ordinal)
        {
            var idxStr = src.IndexOf(beforeStr, comparison);
            var idxEnd = src.IndexOf(afterStr, idxStr + 1, comparison);
            if (idxStr >= 0 && idxEnd > 0)
            {
                if (idxStr > idxEnd)
                    Swap(ref idxStr, ref idxEnd);
                return src.Substring(idxStr + beforeStr.Length, idxEnd - idxStr - beforeStr.Length);
            }

            return src;
        }

        private static void Swap<T>(ref T idxStr, ref T idxEnd)
        {
            var temp = idxEnd;
            idxEnd = idxStr;
            idxStr = temp;
        }

        public static string ReplaceBetween(this string s, char begin, char end, string replacement = null)
        {
            var regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(s, replacement ?? string.Empty);
        }


    }
}