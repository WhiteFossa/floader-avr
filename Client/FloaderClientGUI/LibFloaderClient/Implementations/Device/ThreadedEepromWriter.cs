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
using System.Collections.Generic;

namespace LibFloaderClient.Implementations.Device
{
    /// <summary>
    /// Use it to write EEPROM
    /// </summary>
    public class ThreadedEepromWriter : BaseThreadedOperationsProvider
    {
        /// <summary>
        /// Data to write
        /// </summary>
        private readonly List<byte> _toWrite;

        /// <summary>
        /// Callback on completed write
        /// </summary>
        private readonly EepromWriteCompletedCallbackDelegate _eepromWriteCompletedCallbackDelegate;

        /// <summary>
        /// Delegate to show progress
        /// </summary>
        private readonly ProgressDelegate _progressDelegate;

        public ThreadedEepromWriter(DeviceIdentifierData identificationData,
            PortSettings portSettings,
            object versionSpecificDeviceData,
            ILogger logger,
            List<byte> toWrite,
            EepromWriteCompletedCallbackDelegate eepromWriteCompletedCallbackDelegate,
            ProgressDelegate progressDelegate = null)
            : base(identificationData, portSettings, versionSpecificDeviceData, logger)
        {
            _toWrite = toWrite ?? throw new ArgumentNullException(nameof(toWrite));

            _eepromWriteCompletedCallbackDelegate = eepromWriteCompletedCallbackDelegate
                ?? throw new ArgumentNullException(nameof(eepromWriteCompletedCallbackDelegate));
            _progressDelegate = progressDelegate;
        }

        public void Write()
        {
            _logger.LogInfo("Writing EEPROM...");

            switch (_identificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    using (var driver = GetDeviceDriverV1())
                    {
                        driver.WriteEEPROM(_toWrite, _progressDelegate);
                    }
                    break;

                default:
                    throw ReportUnsupportedVersion();
            }

            _logger.LogInfo("Done");
            _eepromWriteCompletedCallbackDelegate();
        }
    }
}
