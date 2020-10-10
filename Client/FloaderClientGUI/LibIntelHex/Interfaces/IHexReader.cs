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
    /// Intel HEX reader (only 8 bit)
    /// </summary>
    public interface IHexReader
    {
        /// <summary>
        /// Parses Intel HEX file content, returning it as a sorted dictionary with data.
        /// </summary>
        SortedDictionary<int, byte> ReadFromString(string hexFileContent);

        /// <summary>
        /// As ReadFromString(), but reads Intel HEX from file
        /// </summary>
        SortedDictionary<int, byte> ReadFromFile(string hexFilePath);
    }
}
