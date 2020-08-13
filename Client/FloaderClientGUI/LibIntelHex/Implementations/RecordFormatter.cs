using LibIntelHex.Interfaces;
using System;
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
