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
using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;

namespace LibIntelHex.Models
{
    /// <summary>
    /// Base class for all types of Intel HEX records
    /// </summary>
    public class RecordBase : IRecordBase
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
        /// Record length field position in record bytes list
        /// </summary>
        private const int ReclenPosition = 0;

        /// <summary>
        /// High and low address bytes positions in record bytes list
        /// </summary>
        private const int HighAddressPosition = 1;
        private const int LowAddressPosition = 2;

        /// <summary>
        /// Record type field position in record bytes list
        /// </summary>
        private const int RecordTypePosition = 3;

        /// <summary>
        /// Data starts from this position in record bytes list
        /// </summary>
        private const int DataStartPosition = 4;

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
            _recordFormatter = recordFormatter; // Can be null if not provided

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
            if (_recordFormatter == null)
            {
                throw new InvalidOperationException("This record was created without formatter, thus being non-convertable to string.");
            }

            var data = ToDataList();
            return _recordFormatter.FormatRawRecord(data);
        }

        /// <summary>
        /// Try to parse data bytes as base record. Data bytes must be stripped of ":" and checksum.
        /// </summary>
        /// <param name="bytes"></param>
        public static RecordBase ParseBytes(List<byte> bytes, IRecordFormatter formatter)
        {
            var dataLength = (int)bytes[ReclenPosition];

            var addressHigh = (int)bytes[HighAddressPosition];
            var addressLow = (int)bytes[LowAddressPosition];

            var address = (addressHigh << 8) + addressLow;

            var recordTypeRaw = (int)bytes[RecordTypePosition];

            var data = bytes.GetRange(DataStartPosition, bytes.Count - DataStartPosition);

            if (dataLength != data.Count)
            {
                // TODO: Add formatter to display bytes content
                throw new ArgumentException($"Wrong data length for sequence { bytes }. Expected { dataLength }, but got { data.Count }.");
            }

            if (!Enum.IsDefined(typeof(RecordType), recordTypeRaw))
            {
                throw new ArgumentException($"Undefined record type for sequence { bytes }: { recordTypeRaw }");
            }

            return new RecordBase(address, (RecordType)recordTypeRaw, data, formatter);
        }
    }
}
