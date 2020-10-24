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
using System.Linq;

namespace LibFloaderClient.Implementations.Device
{
    /// <summary>
    /// Use it to write FLASH
    /// </summary>
    public class ThreadedFlashWriter : BaseThreadedOperationsProvider
    {
        /// <summary>
        /// Operation name to be displayed near progressbar
        /// </summary>
        private const string ProgressOperationName = "Writing FLASH";

        /// <summary>
        /// Data to write
        /// </summary>
        private readonly List<byte> _toWrite;

        /// <summary>
        /// Callback on completed write
        /// </summary>
        private readonly FlashWriteCompletedCallbackDelegate _flashWriteCompletedCallbackDelegate;

        /// <summary>
        /// Delegate to show progress
        /// </summary>
        private readonly ProgressDelegate _progressDelegate;

        public ThreadedFlashWriter(DeviceIdentifierData identificationData,
            PortSettings portSettings,
            object versionSpecificDeviceData,
            ILogger logger,
            List<byte> toWrite,
            FlashWriteCompletedCallbackDelegate flashWriteCompletedCallbackDelegate,
            ProgressDelegate progressDelegate = null)
            : base(identificationData, portSettings, versionSpecificDeviceData, logger)
        {
            _toWrite = toWrite ?? throw new ArgumentNullException(nameof(toWrite));

            _flashWriteCompletedCallbackDelegate = flashWriteCompletedCallbackDelegate
                ?? throw new ArgumentNullException(nameof(flashWriteCompletedCallbackDelegate));
            _progressDelegate = progressDelegate;
        }

        public void Write()
        {
            _logger.LogInfo($"Writing FLASH (except bootloader)...");

            switch (_identificationData.Version)
            {
                case (int)ProtocolVersion.First:

                    var deviceData = GetDeviceDataV1();
                    using (var driver = GetDeviceDriverV1())
                    {
                        _progressDelegate?.Invoke(new ProgressData(0, deviceData.FlashPagesWriteable, ProgressOperationName));

                        for (var pageAddress = 0; pageAddress < deviceData.FlashPagesWriteable; pageAddress++)
                        {
                            // Preparing page data
                            var pageData = _toWrite.GetRange(pageAddress * deviceData.FlashPageSize, deviceData.FlashPageSize);

                            // Writing
                            driver.WriteFLASHPage(pageAddress, pageData);

                            // Verifying
                            var readback = driver.ReadFLASHPage(pageAddress);

                            if (!readback.SequenceEqual(pageData))
                            {
                                var message = $"Page { pageAddress + 1 } verification failed.";
                                _logger.LogError(message);
                                throw new InvalidOperationException(message);
                            }

                            _progressDelegate?.Invoke(new ProgressData(pageAddress + 1, deviceData.FlashPagesWriteable, ProgressOperationName));
                        }
                    }

                    break;

                default:
                    throw ReportUnsupportedVersion();
            }

            _logger.LogInfo($"Done");
            _flashWriteCompletedCallbackDelegate();
        }
    }
}
