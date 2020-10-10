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

using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using System;

namespace FloaderClientGUI.Models
{
    /// <summary>
    /// Main model for GUI client
    /// </summary>
    public class MainModel
    {
        /// <summary>
        /// Port settings. Might be null if port not selected yet.
        /// </summary>
        /// <value></value>
        public PortSettings PortSettings { get; set; }

        /// <summary>
        /// Serial port driver. Use via using() {}
        /// </summary>
        /// <value></value>
        public ISerialPortDriver PortDriver { get; set; }

        /// <summary>
        /// Device identification data
        /// </summary>
        public DeviceIdentifierData DeviceIdentData { get; set; }

        /// <summary>
        /// Human-readable device description
        /// </summary>
        public DeviceHumanReadableDescription DeviceHumanReadableDescription { get; set; }

        /// <summary>
        /// Information about device (protocol version specific, so Object)
        /// </summary>
        public Object VersionSpecificDeviceData { get; set; }
    }
}