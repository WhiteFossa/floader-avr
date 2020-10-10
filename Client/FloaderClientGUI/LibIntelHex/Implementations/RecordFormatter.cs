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

using LibIntelHex.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHex.Implementations
{
    public class RecordFormatter : IRecordFormatter
    {
        /// <summary>
        /// All records starts with this string
        /// </summary>
        private const string RecordStartMark = ":";

        /// <summary>
        /// Expected maximal record length for string builder. (16 bytes of data + 5 bytes of service fields + 2 bytes of newline) * 2
        /// </summary>
        private const int ExpectedMaximalRecordLength = 46;

        private readonly IChecksumProcessor _checksumProcessor;
        private readonly IBytesReaderWriter _bytesReaderWriter;

        public RecordFormatter(IChecksumProcessor checksumProcessor,
            IBytesReaderWriter bytesReaderWriter)
        {
            _checksumProcessor = checksumProcessor;
            _bytesReaderWriter = bytesReaderWriter;
        }

        public string FormatRawRecord(List<byte> data)
        {
            var sb = new StringBuilder(ExpectedMaximalRecordLength);

            sb.Append(RecordStartMark);

            foreach (var dataByte in data)
            {
                sb.Append(_bytesReaderWriter.ToHex(dataByte));
            }

            var checksum = _checksumProcessor.CalculateChecksum(data);
            sb.Append(_bytesReaderWriter.ToHex(checksum));
            sb.AppendLine();

            return sb.ToString();
        }
    }
}
