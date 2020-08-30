using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LibIntelHex.Implementations.Helpers
{
    /// <summary>
    /// Usefull stuff to work with strings
    /// </summary>
    public static class StringsHelper
    {
        /// <summary>
        /// Splitting source string by all possible newlines (\r\n, \r, \n). Empty entries are being removed
        /// </summary>
        public static List<string> SplitByNewlines(string source)
        {
            return source
                .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
        }

        /// <summary>
        /// Splits string into pairs of characters (like 0F, 05, 5A), ready to convert into bytes
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<string> SplitStringIntoPairs(string source)
        {
            var result = Regex.Split(source, @"(?<=\G.{2})")
                .ToList();
            result.RemoveAt(result.Count - 1); // Removing empty pair at the end

            return result;
        }
    }
}
