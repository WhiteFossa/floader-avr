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

using LibIntelHex.Enums;
using LibIntelHex.Implementations.Helpers;
using LibIntelHex.Interfaces;
using LibIntelHex.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        /// <summary>
        /// Mask address from the data record with this value before adding it to segment address.
        /// </summary>
        private const int RecordAddressMask = 0xFFFF;

        /// <summary>
        /// Allowed record types.
        /// StartSegmentAddress is meaningless, but IAR adds it, so we need to allow it for compatibility.
        /// </summary>
        private readonly List<RecordType> AllowedRecords = new List<RecordType>() { RecordType.Data, RecordType.EndOfFile, RecordType.StartSegmentAddress, RecordType.ExtendedSegmentAddress };

        private readonly IChecksumProcessor _checksumProcessor;
        private readonly IBytesReaderWriter _bytesReaderWriter;

        public HexReader(IChecksumProcessor checksumProcessor,
            IBytesReaderWriter bytesReaderWriter)
        {
            _checksumProcessor = checksumProcessor;
            _bytesReaderWriter = bytesReaderWriter;
        }

        public SortedDictionary<int, byte> ReadFromFile(string hexFilePath)
        {
            if (string.IsNullOrEmpty(hexFilePath))
            {
                throw new ArgumentException("Path must not be empty.", nameof(hexFilePath));
            }

            var content = File.ReadAllText(hexFilePath);

            return ReadFromString(content);
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

            // Check lines for correctness)
            var linesCorrectness = lines
                .Select(l => new { line = l, isCorrect = CorrectLineRegexp.IsMatch(l) });

            var firstIncorrectLine = linesCorrectness
                .FirstOrDefault(lc => !lc.isCorrect);

            if (firstIncorrectLine != null)
            {
                throw new ArgumentException($"HEX file contains incorrect line. Line: { firstIncorrectLine.line }", nameof(hexFileContent));
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

            // Raw, unprocessed records
            var rawRecords = bytesLines
                .Select(bl => RecordBase.ParseBytes(bl, null)) // We don't need to format this records
                .ToList();

            // Do we have non-allowed record types?
            var notAllowedRecords = rawRecords
                .Where(rr => !AllowedRecords.Contains(rr.Type));

            if (notAllowedRecords.Any())
            {
                throw new ArgumentException($"HEX file contains not allowed record with type { notAllowedRecords.First().Type }.");
            }

            // Only one EoF record
            var eofCount = rawRecords
                .Where(rr => rr.Type == RecordType.EndOfFile)
                .Count();

            if (eofCount != 1)
            {
                throw new ArgumentException($"HEX file must contain one and only one End of File record.");
            }

            // EoF must be the last record
            if (rawRecords.Last().Type != RecordType.EndOfFile)
            {
                throw new ArgumentException($"HEX file must end with End of File record.");
            }

            // Processing records one by one and populating data dictionary
            var result = new SortedDictionary<int, byte>();

            var segmentBaseAddress = 0;

            foreach (var record in rawRecords)
            {
                switch(record.Type)
                {
                    case RecordType.Data:
                        var dataRecord = new DataRecord(record);

                        var recordBaseAddress = dataRecord.Address;

                        for (var recordOffset = 0; recordOffset < dataRecord.Data.Count; recordOffset ++)
                        {
                            var actualAddress = segmentBaseAddress + ((recordBaseAddress + recordOffset) & RecordAddressMask);
                            result.Add(actualAddress, dataRecord.Data[recordOffset]);
                        }

                        break;
                    case RecordType.StartSegmentAddress:
                        // Just do nothing
                        break;

                    case RecordType.ExtendedSegmentAddress:
                        // Updating segment base address
                        var esaRecord = new ExtendedSegmentAddressRecord(record);
                        segmentBaseAddress = esaRecord.GetSegmentBaseAddress();

                        break;

                    case RecordType.EndOfFile:
                        // Constructing EoF record. We don't use it, but construction causes record validation
                        _ = new EndOfFileRecord(record);
                        return result;
                }
            }

            throw new InvalidOperationException("Must never reach this place.");
        }
    }
}
