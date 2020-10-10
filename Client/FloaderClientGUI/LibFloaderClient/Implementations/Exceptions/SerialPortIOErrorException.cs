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

namespace LibFloaderClient.Implementations.Exceptions
{
    /// <summary>
    /// Exception, indicating serial port IO error
    /// </summary>
    public class SerialPortIOErrorException : Exception
    {
        /// <summary>
        /// Error type
        /// </summary>
        public SerialError? Type { get; private set; }

        public SerialPortIOErrorException() : base()
        {
            Type = null;
        }

        public SerialPortIOErrorException(string message) : base(message)
        {
            Type = null;
        }

        public SerialPortIOErrorException(string message, Exception inner) : base(message, inner)
        {
            Type = null;
        }

        public SerialPortIOErrorException(string message, SerialError type) : base(message)
        {
            Type = type;
        }

        public SerialPortIOErrorException(string message, Exception inner, SerialError type) : base(message, inner)
        {
            Type = type;
        }
    }
}