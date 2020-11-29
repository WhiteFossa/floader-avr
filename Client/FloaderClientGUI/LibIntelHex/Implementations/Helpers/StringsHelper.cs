/*
                    Fossa's AVR bootloader client
Copyright (C) 2020 White Fossa aka Artyom Vetrov <whitefossa@protonmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
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
