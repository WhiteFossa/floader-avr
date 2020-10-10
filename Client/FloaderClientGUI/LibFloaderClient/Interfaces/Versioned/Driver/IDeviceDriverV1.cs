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

namespace LibFloaderClient.Interfaces.Versioned.Driver
{
    /// <summary>
    /// Device driver, protocol version 1
    /// </summary>
    public interface IDeviceDriverV1 : IDisposable
    {
        /// <summary>
        /// Reboots target device
        /// </summary>
        /// <returns>True if device successfully reported reboot</returns>
        bool Reboot();

        /// <summary>
        /// Read all EEPROM bytes. Result never is null.
        /// </summary>
        /// <returns></returns>
        List<byte> ReadEEPROM();

        /// <summary>
        /// Writes data to EEPROM. toWrite list must have EEPROM size elements.
        /// </summary>
        void WriteEEPROM(List<byte> toWrite);

        /// <summary>
        /// Reads FLASH page with given address or throws an exception in case of error. Result never is null.
        /// </summary>
        List<byte> ReadFLASHPage(int pageAddress);

        /// <summary>
        /// Writes FLASH page with given address or throws an exception in case of error. Please note that bootloader pages is write-protected.
        /// </summary>
        void WriteFLASHPage(int pageAddress, List<byte> toWrite);
    }
}
