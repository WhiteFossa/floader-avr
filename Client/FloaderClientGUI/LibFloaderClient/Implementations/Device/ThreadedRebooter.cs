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
using LibFloaderClient.Implementations.Resources;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using System;

namespace LibFloaderClient.Implementations.Device
{
    public class ThreadedRebooter : BaseThreadedOperationsProvider
    {
        private readonly RebootToFirmwareCompletedCallbackDelegate _rebootCompletedDelegate;
        private readonly UnhandledExceptionCallbackDelegate _unhandledExceptionCallbackDelegate;

        public ThreadedRebooter(DeviceIdentifierData identificationData,
            PortSettings portSettings,
            object versionSpecificDeviceData,
            ILogger logger,
            RebootToFirmwareCompletedCallbackDelegate rebootCompletedDelegate,
            UnhandledExceptionCallbackDelegate unhandledExceptionDelegate)
            : base(identificationData, portSettings, versionSpecificDeviceData, logger)
        {
            _rebootCompletedDelegate = rebootCompletedDelegate ?? throw new ArgumentNullException(nameof(rebootCompletedDelegate));
            _unhandledExceptionCallbackDelegate = unhandledExceptionDelegate ??
                                                  throw new ArgumentNullException(nameof(unhandledExceptionDelegate));
        }

        /// <summary>
        /// Attempt to reboot device
        /// </summary>
        public void Reboot()
        {
            try
            {
                _logger.LogInfo(Language.RequestingReboot);

                var isSuccessfull = false;
                switch (_identificationData.Version)
                {
                    case (int) ProtocolVersion.First:
                        using (var driver = GetDeviceDriverV1())
                        {
                            isSuccessfull = driver.Reboot();
                        }
                        
                        break;

                    default:
                        throw ReportUnsupportedVersion();
                }

                _logger.LogInfo(Language.Done);

                _rebootCompletedDelegate(new DeviceRebootResult(isSuccessfull));
            }
            catch (Exception ex)
            {
                _unhandledExceptionCallbackDelegate(ex);
            }
        }

    }
}
