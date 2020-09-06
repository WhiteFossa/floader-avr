using LibIntelHex.Implementations.Helpers;
using LibIntelHex.Interfaces;
using LibIntelHex.Models;
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

        private readonly IChecksumProcessor _checksumProcessor;
        private readonly IBytesReaderWriter _bytesReaderWriter;

        public HexReader(IChecksumProcessor checksumProcessor,
            IBytesReaderWriter bytesReaderWriter)
        {
            _checksumProcessor = checksumProcessor;
            _bytesReaderWriter = bytesReaderWriter;
        }

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
            var bytesLinesWithChecksums = lines
                .Select(l => StringsHelper.SplitStringIntoPairs(l))
                .Select(pl => pl.Select(p => _bytesReaderWriter.FromHex(p)).ToList())
                .ToList();

            // Checking checksums
            var linesWithTestedChecksums = bytesLinesWithChecksums
                .ToDictionary(pl => pl, pl => _checksumProcessor.VerifyChecksum(pl));

            var firstLineWithWrongChecksum = linesWithTestedChecksums
                .FirstOrDefault(lc => !lc.Value)
                .Key;

            if (firstLineWithWrongChecksum != null)
            {
                throw new ArgumentException($"HEX file contains line with a wrong checksum. Byte sequence with it: { firstLineWithWrongChecksum }", nameof(firstLineWithWrongChecksum));
            }

            // Stripping checksums
            var bytesLines = bytesLinesWithChecksums
                .Select(bl => bl.GetRange(0, bl.Count - 1))
                .ToList();

            // Debug
            var records = bytesLines
                .Select(bl => RecordBase.ParseBytes(bl))
                .ToList();

            throw new NotImplementedException();
        }
    }
}
