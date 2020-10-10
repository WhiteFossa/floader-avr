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

using LibIntelHex.Implementations.Helpers;
using System;
using System.Collections.Generic;

namespace LibIntelHex.Implementations.Writer
{
    /// <summary>
    /// One data block to write to file
    /// </summary>
    public class DataBlock
    {
        /// <summary>
        /// Block base address
        /// </summary>
        public int BaseAddress { get; private set; }

        /// <summary>
        /// Sequential data in block
        /// </summary>
        public List<byte> Data { get; private set; }

        /// <summary>
        /// Creates data block with a first byte in it
        /// </summary>
        public DataBlock(int address, byte data)
        {
            ValidationHelper.ValidateAddress(address);

            BaseAddress = address;
            Data = new List<byte>() { data };
        }

        /// <summary>
        /// Checks, can byte with this address be appended to the end of the block
        /// </summary>
        public bool IsByteAppendable(int address)
        {
            ValidationHelper.ValidateAddress(address);

            return address == BaseAddress + Data.Count;
        }

        /// <summary>
        /// Attempts to append a byte to the end ot data block
        /// </summary>
        public void AppendByte(int address, byte data)
        {
            if (!IsByteAppendable(address))
            {
                throw new ArgumentException($"Byte with address { address } can't be appended.", nameof(address));
            }

            Data.Add(data);
        }
    }
}
