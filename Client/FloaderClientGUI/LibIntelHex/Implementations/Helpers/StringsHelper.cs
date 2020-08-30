using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
