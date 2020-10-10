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

using System;
using System.IO.Ports;

namespace LibFloaderClient.Models.Port
{
    /// <summary>
    /// Settings, enough to open port. All fields must be specified
    /// </summary>
    public class PortSettings
    {
        /// <summary>
        /// Port name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Baudrate
        /// </summary>
        public int Baudrate { get; }

        /// <summary>
        /// Parity
        /// </summary>
        public Parity Parity { get; }

        /// <summary>
        /// Data bits count
        /// </summary>
        public int DataBits { get; }

        /// <summary>
        /// Stop bits count
        /// </summary>
        public StopBits StopBits { get; }

        public PortSettings(string name, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            Name = name;
            Baudrate = baudrate;
            Parity = parity;
            DataBits = dataBits;
            StopBits = stopBits;
        }
    }
}