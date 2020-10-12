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

using LibFloaderClient.Implementations.Enums.Device;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using System;

namespace LibFloaderClient.Implementations.Device
{
    /// <summary>
    /// Use it to read EEPROM
    /// </summary>
    public class ThreadedEepromReader
    {
        /// <summary>
        /// Device identification data
        /// </summary>
        private readonly DeviceIdentifierData _identificationData;

        /// <summary>
        /// Port settings
        /// </summary>
        private readonly PortSettings _portSettings;

        /// <summary>
        /// Version-specific device data
        /// </summary>
        private readonly object _versionSpecificDeviceData;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Callback on completed read
        /// </summary>
        private readonly EepromReadCompletedCallbackDelegate _eepromReadCompletedCallbackDelegate;

        public ThreadedEepromReader(DeviceIdentifierData identificationData,
            PortSettings portSettings,
            object versionSpecificDeviceData,
            ILogger logger,
            EepromReadCompletedCallbackDelegate eepromReadCompletedCallbackDelegate)
        {
            _identificationData = identificationData ?? throw new ArgumentNullException(nameof(identificationData));
            _portSettings = portSettings ?? throw new ArgumentNullException(nameof(portSettings));
            _versionSpecificDeviceData = versionSpecificDeviceData ?? throw new ArgumentNullException(nameof(versionSpecificDeviceData));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _eepromReadCompletedCallbackDelegate = eepromReadCompletedCallbackDelegate
                ?? throw new ArgumentNullException(nameof(eepromReadCompletedCallbackDelegate));
        }

        /// <summary>
        /// Attempt to read device EEPROM
        /// </summary>
        public void Read()
        {
            EepromReadResult result = null;

            switch (_identificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    using (var driver = DeviceIndependentOperationsProvider.GetDeviceDriverV1(version: _identificationData.Version, portSettings: _portSettings,
                        versionSpecificDeviceData: _versionSpecificDeviceData, _logger))
                    {
                        result = new EepromReadResult(true, driver.ReadEEPROM());
                    }
                    break;
                default:
                    throw DeviceIndependentOperationsProvider.ReportUnsupportedVersion(_identificationData.Version);
            }

            _eepromReadCompletedCallbackDelegate(result);
        }
    }
}
