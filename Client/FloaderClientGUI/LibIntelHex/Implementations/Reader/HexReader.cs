using LibIntelHex.Implementations.Helpers;
using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LibIntelHex.Implementations.Reader
{
    public class HexReader : IHexReader
    {
        /// <summary>
        /// Intel HEX must have at least two lines
        /// </summary>
        private const int MinLinesCount = 2;

        /// <summary>
        /// Correct line regexp:
        /// 1) Starts with ":"
        /// 2) ":" is followed by at least 5 pairs of 0-9A-F
        /// </summary>
        private readonly Regex CorrectLineRegexp = new Regex(@"^:([0-9A-F]{2}){5,}$");

        public SortedDictionary<int, byte> ReadFromString(string hexFileContent)
        {
            if (string.IsNullOrEmpty(hexFileContent))
            {
                throw new ArgumentException("HEX file string must not be null or empty.", nameof(hexFileContent));
            }

            var lines = StringsHelper.SplitByNewlines(hexFileContent);
            if (lines.Count < MinLinesCount)
            {
                throw new ArgumentException("HEX file must contain at least two lines (DATA + EoF records)", nameof(hexFileContent));
            }

            // To upper
            lines = lines
                .Select(l => l.ToUpper())
                .ToList();

            // Check lines for correctness
            var linesCorrectness = lines
                .ToDictionary(l => l, l => CorrectLineRegexp.IsMatch(l));

            var firstIncorrectLine = linesCorrectness
                .FirstOrDefault(lc => !lc.Value)
                .Key;

            if (!string.IsNullOrEmpty(firstIncorrectLine))
            {
                throw new ArgumentException($"HEX file contains incorrect line. Line: {firstIncorrectLine}", nameof(hexFileContent));
            }

            // Removing leading ":"
            lines = lines
                .Select(l => l.Substring(1))
                .ToList();

            // Converting lines to byte arrays
            List<List<string>> pairsLines = lines
                .Select(l => StringsHelper.SplitStringIntoPairs(l))
                .ToList();

            throw new NotImplementedException();
        }
    }
}
