using LibIntelHex.Enums;
using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHex.Models
{
    /// <summary>
    /// Base class for all types of Intel HEX records
    /// </summary>
    public class RecordBase
    {
        /// <summary>
        /// Maximal possible data length
        /// </summary>
        private const int MaxDataLength = 255;

        /// <summary>
        /// Maximal possible address
        /// </summary>
        private const int MaxAddress = 0xFFFF;

        /// <summary>
        /// Data field length
        /// </summary>
        public byte DataLength { get; }

        /// <summary>
        /// Address / offset
        /// </summary>
        public int Address { get; }

        /// <summary>
        /// Record type
        /// </summary>
        public RecordType Type { get; }

        /// <summary>
        /// Record payload
        /// </summary>
        public List<byte> Data { get; }

        private readonly IRecordFormatter _recordFormatter;

        public RecordBase(int address, RecordType type, List<byte> data, IRecordFormatter recordFormatter)
        {
            _recordFormatter = recordFormatter;

            if (address < 0 || address > MaxAddress)
            {
                throw new ArgumentOutOfRangeException(nameof(address), address, $"Address must be within [0, { MaxAddress }] interval.");
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Count > MaxDataLength)
            {
                throw new ArgumentOutOfRangeException(nameof(data.Count), data.Count, $"Data must be no longer than { MaxDataLength }.");
            }

            Address = address;
            Type = type;
            Data = data;
            DataLength = (byte)(data.Count & 0xFF);
        }

        /// <summary>
        /// Formats record to data list, i.e. bytes between ":" and checksum.
        /// </summary>
        /// <returns></returns>
        public List<byte> ToDataList()
        {
            var result = new List<byte>();

            result.Add(DataLength); // Length
            result.Add((byte)((Address & 0xFF00) >> 8)); // High address
            result.Add((byte)(Address & 0xFF)); // Low address
            result.Add((byte)(((int)Type) & 0xFF)); // Type
            result.AddRange(Data);

            return result;
        }

        /// <summary>
        /// Formats record to ready to write Intel HEX line (starting with ":" and ending with checksum)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var data = ToDataList();
            return _recordFormatter.FormatRawRecord(data);
        }
    }
}
