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
    /// Record with data.
    /// </summary>
    public class DataRecord : RecordBase
    {
        /// <summary>
        /// Constructs data record from base record and trying to validate it
        /// </summary>
        public DataRecord(RecordBase baseRecord, IRecordFormatter formatter = null) : base(baseRecord.Address, baseRecord.Type, baseRecord.Data, formatter)
        {
            if (!Validate(baseRecord))
            {
                throw new ArgumentException(Language.NotValidDataRecord, nameof(baseRecord));
            }
        }

        public DataRecord(int address, List<byte> data, IRecordFormatter recordFormatter) : base(address, RecordType.Data, data, recordFormatter)
        {

        }

        /// <summary>
        /// Returns true if giben base record is a valid Data Record.
        /// </summary>
        public static bool Validate(RecordBase recordBase)
        {
            if (recordBase.Type != RecordType.Data)
            {
                return false;
            }

            return true;
        }
    }
}
