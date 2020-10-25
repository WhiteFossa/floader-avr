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
using System.IO.Ports;
using System.Linq;

namespace LibFloaderClient.Implementations.Port
{
    /// <summary>
    /// Various usefull stuff for port selection
    /// </summary>
    public static class PortSelectionHelper
    {

#region Constants
        /// <summary>
        /// Dictionary for parity to name and name to parity mapping
        /// </summary>
        private static readonly Dictionary<Parity, string> ParityNames = new Dictionary<Parity, string>()
        {
            { Parity.None, Language.ParityNone },
            { Parity.Odd, Language.ParityOdd },
            { Parity.Even, Language.ParityEven },
            { Parity.Mark, Language.ParityMark },
            { Parity.Space, Language.ParitySpace }
        };

        /// <summary>
        /// Dictionary for stop bits to name and name to stop bits mapping
        /// </summary>
        private static readonly Dictionary<StopBits, string> StopBitsNames = new Dictionary<StopBits, string>()
        {
            { StopBits.One, Language.StopBitsOne },
            { StopBits.OnePointFive, Language.StopBitsOnePointFive },
            { StopBits.Two, Language.StopBitsTwo }
        };

#endregion

#region Mappers

        /// <summary>
        /// Parity to combobox value
        /// </summary>
        public static string MapParityToString(Parity parity)
        {
            var result = ParityNames
                .Where(pn => pn.Key == parity);

            if (!result.Any())
            {
                throw new ArgumentException(nameof(parity));
            }

            return result.FirstOrDefault().Value;
        }

        /// <summary>
        /// Combobox value to parity
        /// </summary>
        public static Parity MapStringToParity(string parityStr)
        {
            var result = ParityNames
                .Where(pn => string.Equals(pn.Value, parityStr));

            if (!result.Any())
            {
                throw new ArgumentException(nameof(parityStr));
            }

            return result.FirstOrDefault().Key;
        }

        /// <summary>
        /// Stop bits count to combobox value
        /// </summary>
        public static string MapStopBitsToString(StopBits stopBits)
        {
            var result = StopBitsNames
                .Where(sb => sb.Key == stopBits);

            if (!result.Any())
            {
                throw new ArgumentException(nameof(stopBits));
            }

            return result.FirstOrDefault().Value;
        }

        /// <summary>
        /// Combobox value to stop bits count
        /// </summary>
        public static StopBits MapStringToStopBits(string str)
        {
            var result = StopBitsNames
                .Where(sb => string.Equals(sb.Value, str));

            if (!result.Any())
            {
                throw new ArgumentException(nameof(str));
            }

            return result.FirstOrDefault().Key;
        }

#endregion
    }
}