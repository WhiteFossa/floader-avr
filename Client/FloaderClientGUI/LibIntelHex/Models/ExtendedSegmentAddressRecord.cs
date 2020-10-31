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
using LibIntelHex.Resources;
using System;
using System.Collections.Generic;

namespace LibIntelHex.Models
{
    public class ExtendedSegmentAddressRecord : RecordBase
    {
        /// <summary>
        /// ESA record must have this data length
        /// </summary>
        private const int ExpectedDataLength = 2;

        public ExtendedSegmentAddressRecord(IRecordFormatter recordFormatter) : base(0, RecordType.ExtendedSegmentAddress, new List<byte>(), recordFormatter)
        {

        }

        /// <summary>
        /// Constructs ExtendedSegmentAddressRecord from base record and trying to validate it
        /// </summary>
        public ExtendedSegmentAddressRecord(RecordBase baseRecord, IRecordFormatter formatter = null) : base(baseRecord.Address, baseRecord.Type, baseRecord.Data, formatter)
        {
            if (!Validate(baseRecord))
            {
                throw new ArgumentException(Language.NotValidESARecord, nameof(baseRecord));
            }
        }

        /// <summary>
        /// Returns true if this base record is a valid End of File record.
        /// </summary>
        public static bool Validate(RecordBase baseRecord)
        {
            if (baseRecord.Type != RecordType.ExtendedSegmentAddress)
            {
                return false;
            }

            if (baseRecord.DataLength != ExpectedDataLength)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns segment base address from record
        /// </summary>
        /// <returns></returns>
        public int GetSegmentBaseAddress()
        {
            return Address << 4; // As in Intel HEX specification
        }

    }
}
