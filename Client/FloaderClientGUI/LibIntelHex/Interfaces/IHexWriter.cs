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

using System.Collections.Generic;

namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// Intel HEX writer (only 8bit)
    /// </summary>
    public interface IHexWriter
    {
        /// <summary>
        /// Resets writer, cleaning all data
        /// </summary>
        void Reset();

        /// <summary>
        /// Add next data byte to writer
        /// </summary>
        void AddByte(int address, byte data);

        /// <summary>
        /// Load data to write from list.
        /// </summary>
        /// <param name="baseAddress">Address of first byte in list</param>
        /// <param name="data">Data to write</param>
        void LoadFromList(int baseAddress, List<byte> data);

        /// <summary>
        /// Generate Intel HEX file contents and put it into string
        /// </summary>
        string ToString();

        /// <summary>
        /// Generates and writes generated Intel HEX into file. If file exists, it would be overwritten.
        /// </summary>
        void WriteToFile(string path);
    }
}
