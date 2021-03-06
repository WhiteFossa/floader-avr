﻿/*
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

namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// Interface to convert bytes from/to Intel HEX format
    /// </summary>
    public interface IBytesReaderWriter
    {
        /// <summary>
        /// Convert given byte to 2-bytes HEX representation
        /// </summary>
        string ToHex(byte b);

        /// <summary>
        /// Trying to parse string and get byte from it, throws an exception in case of incorrect string
        /// </summary>
        byte FromHex(string hexString);
    }
}
