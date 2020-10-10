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

using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using System;

namespace LibFloaderClient.Implementations.Device
{
    /// <summary>
    /// Called by ThreadedDeviceIdentifier when device identified.
    /// </summary>
    public delegate void DeviceIdentifiedCallbackDelegate(DeviceIdentifierData data);

    public class ThreadedDeviceIdentifier
    {
        /// <summary>
        /// Try to identify device on this port
        /// </summary>
        private PortSettings _portSettings;

        /// <summary>
        /// Called when device identified
        /// </summary>
        private DeviceIdentifiedCallbackDelegate _deviceIdentifiedCallbackDelegate;

        public ThreadedDeviceIdentifier(PortSettings portSettings, DeviceIdentifiedCallbackDelegate deviceIdentifiedCallbackDelegate)
        {
            _portSettings = portSettings ?? throw new ArgumentNullException(nameof(portSettings));
            _deviceIdentifiedCallbackDelegate = deviceIdentifiedCallbackDelegate ?? throw new ArgumentNullException(nameof(deviceIdentifiedCallbackDelegate));
        }

        /// <summary>
        /// Attempt to identify device
        /// </summary>
        public void Identify()
        {
            // TODO: Implement me
            var result = new DeviceIdentifierData(Enums.Device.DeviceIdentificationStatus.Identified, 1, 0, 1, 1);

            _deviceIdentifiedCallbackDelegate(result);
        }
    }
}
