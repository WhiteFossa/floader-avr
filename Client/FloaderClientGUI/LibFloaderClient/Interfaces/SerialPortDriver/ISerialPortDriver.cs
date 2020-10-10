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
using System.Collections.Generic;

namespace LibFloaderClient.Interfaces.SerialPortDriver
{
    /// <summary>
    /// Pseudo-synchronous serial port IO driver
    /// </summary>
    public interface ISerialPortDriver : IDisposable
    {
        /// <summary>
        /// Blocks IO and trying to write given bytes into port. If there is no succes during the timeout,
        /// then throws SerialPortTimeoutException.
        /// </summary>
        void Write(List<byte> contentToWrite);

        /// <summary>
        /// Blocks IO and trying to read requiredSize of bytes from it. If there is no succes during the timeout,
        /// then throws SerialPortTimeoutException.
        /// </summary>
        List<byte> Read(int requiredSize);
    }
}