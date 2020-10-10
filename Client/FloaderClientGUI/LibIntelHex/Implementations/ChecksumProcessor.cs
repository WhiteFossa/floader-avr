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
using System;
using System.Collections.Generic;

namespace LibIntelHex.Implementations
{
    public class ChecksumProcessor : IChecksumProcessor
    {
        public byte CalculateChecksum(List<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte checksum = 0;
            foreach (var dataByte in data)
            {
                checksum += dataByte;
            }

            return (byte)((0 - checksum) & 0xFF);
        }

        public bool VerifyChecksum(List<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte checksum = 0;
            foreach (var dataByte in data)
            {
                checksum += dataByte;
            }

            return checksum == 0;
        }
    }
}
