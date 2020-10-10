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
using System.IO.Ports;

namespace LibFloaderClient.Implementations.Port
{
    /// <summary>
    /// Possible and default settings for port
    /// </summary>
    public class PossiblePortSettings
    {
        /// <summary>
        /// Possible baudrates
        /// </summary>
        public static readonly List<int> StandardBaudrates = new List<int>()
            {110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };

        /// <summary>
        /// Default baudrate
        /// </summary>
        public const int DefaultBaudrate = 57600;

        /// <summary>
        /// Possible port parities
        /// </summary>
        public static readonly List<Parity> PossibleParities = new List<Parity>()
            { Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space };

        /// <summary>
        /// Default parity mode
        /// </summary>
        public const Parity DefaultParity = Parity.None;

        /// <summary>
        /// Possible port data bits
        /// </summary>
        public static readonly List<int> PossibleDataBits = new List<int>() { 5, 6, 7, 8, 9 };

        /// <summary>
        /// Default data bits
        /// </summary>
        public const int DefaultDataBits = 8;

        /// <summary>
        /// Possible stop bits
        /// </summary>
        public static readonly List<StopBits> PossbileStopBits = new List<StopBits>()
            { StopBits.One, StopBits.OnePointFive, StopBits.Two };

        /// <summary>
        /// Default stop bits
        /// </summary>
        public const StopBits DefaultStopBits = StopBits.One;
    }
}