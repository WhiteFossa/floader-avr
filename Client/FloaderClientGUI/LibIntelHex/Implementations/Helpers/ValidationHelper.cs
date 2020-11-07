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

using LibIntelHex.Resources;
using System;

namespace LibIntelHex.Implementations.Helpers
{
    /// <summary>
    /// Class to help with various validations
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Max address for 8 bit Intel HEX
        /// </summary>
        public const int MaxAddress = 65535;

        /// <summary>
        /// Validates address, throws exception if address invalid for 8bit Intel HEX
        /// </summary>
        public static void ValidateAddress(int address)
        {
            if (address < 0 || address > MaxAddress)
            {
                throw new ArgumentOutOfRangeException(nameof(address), address, string.Format(Language.InvalidAddress, MaxAddress));
            }
        }
    }
}
