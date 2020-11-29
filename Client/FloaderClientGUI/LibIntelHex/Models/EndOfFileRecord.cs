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
    /// <summary>
    /// Record for End of File
    /// </summary>
    public class EndOfFileRecord : RecordBase
    {
        /// <summary>
        /// EoF record must have this data length
        /// </summary>
        private const int ExpectedDataLength = 0;

        /// <summary>
        /// EoF record must have this address inside
        /// </summary>
        private const int ExpectedAddress = 0;

        /// <summary>
        /// Constructs EndOfFileRecord from base record and trying to validate it
        /// </summary>
        public EndOfFileRecord(RecordBase baseRecord, IRecordFormatter formatter = null) : base(baseRecord.Address, baseRecord.Type, baseRecord.Data, formatter)
        {
            if (!Validate(baseRecord))
            {
                throw new ArgumentException(Language.NotValidEoFRecord, nameof(baseRecord));
            }
        }

        public EndOfFileRecord(IRecordFormatter recordFormatter) : base(0, RecordType.EndOfFile, new List<byte>(), recordFormatter)
        {

        }

        /// <summary>
        /// Returns true if this base record is a valid End of File record.
        /// </summary>
        public static bool Validate(RecordBase baseRecord)
        {
            if (baseRecord.Type != RecordType.EndOfFile)
            {
                return false;
            }

            if (baseRecord.DataLength != ExpectedDataLength)
            {
                return false;
            }

            if (baseRecord.Address != ExpectedAddress)
            {
                return false;
            }

            return true;
        }
    }
}
