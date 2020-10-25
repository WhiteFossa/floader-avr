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

using LibFloaderClient.Implementations.Resources;
using System;
using System.Collections.Generic;

namespace LibFloaderClient.Implementations.Helpers
{
    /// <summary>
    /// Various useful stuff to work with port data
    /// </summary>
    public static class PortDataHelper
    {
        /// <summary>
        /// 3 bytes lenght in response
        /// </summary>
        private const int ThreeBytesLength = 3;

        /// <summary>
        /// 4 bytes lenght in response
        /// </summary>
        private const int FourBytesLength = 4;

        /// <summary>
        /// 0x03 -> 3, 0x7F -> 127, 0x80 -> 128, 0xFF -> 255 etc
        /// </summary>
        public static int UnsignedByteToInt(byte b)
        {
            return (int)(b & 0xFF);
        }

        /// <summary>
        /// Takes byte at given position in response and returns it as int (byte is unsigned).
        /// I.e. 0x03 -> 3, 0xFF -> 255 etc
        /// </summary>
        public static int ExtractUnsignedByteAsInt(List<byte> response, int position)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if ((position < 0) || (position >= response.Count))
            {
                throw new ArgumentOutOfRangeException(Language.PositionIsOutsideResponse, nameof(position));
            }

            return UnsignedByteToInt(response[position]);
        }

        /// <summary>
        /// Takes 3 bytes from response starting at given position and converts it into int (always positive)
        /// </summary>
        public static int ExtractThreeBytesAsInt(List<byte> response, int position)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if ((position < 0) || (position > response.Count - ThreeBytesLength))
            {
                throw new ArgumentOutOfRangeException(Language.IncorrectPosition, nameof(position));
            }

            var bt2 = ExtractUnsignedByteAsInt(response, position);
            var bt1 = ExtractUnsignedByteAsInt(response, position + 1);
            var bt0 = ExtractUnsignedByteAsInt(response, position + 2);

            return 65536 * bt2 + 256 * bt1 + bt0;
        }

        /// <summary>
        /// Takes 4 bytes from response starting at given position and converts it into long (always positive)
        /// </summary>
        public static long ExtractFourBytesAsLong(List<byte> response, int position)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if ((position < 0) || (position > response.Count - FourBytesLength))
            {
                throw new ArgumentOutOfRangeException(Language.IncorrectPosition, nameof(position));
            }

            var bt3 = ExtractUnsignedByteAsInt(response, position);
            var bt2 = ExtractUnsignedByteAsInt(response, position + 1);
            var bt1 = ExtractUnsignedByteAsInt(response, position + 2);
            var bt0 = ExtractUnsignedByteAsInt(response, position + 3);

            return 16777216 * bt3 + 65536 * bt2 + 256 * bt1 + bt0;
        }
    }
}