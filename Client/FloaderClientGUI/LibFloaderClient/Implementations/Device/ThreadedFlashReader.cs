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
    /// Class for reading all FLASH pages in a separate thread
    /// </summary>
    public class ThreadedFlashReader : BaseThreadedOperationsProvider
    {
        /// <summary>
        /// Call this when all data successfully read
        /// </summary>
        private readonly FlashReadCompletedCallbackDelegate _flashReadCompletedCallbackDelegate;

        /// <summary>
        /// Delegate to show progress
        /// </summary>
        private readonly ProgressDelegate _progressDelegate;

        public ThreadedFlashReader(DeviceIdentifierData identificationData,
           PortSettings portSettings,
           object versionSpecificDeviceData,
           ILogger logger,
           FlashReadCompletedCallbackDelegate flashReadCompletedCallbackDelegate,
           ProgressDelegate progressDelegate = null)
           : base(identificationData, portSettings, versionSpecificDeviceData, logger)
        {
            _flashReadCompletedCallbackDelegate = flashReadCompletedCallbackDelegate
                ?? throw new ArgumentNullException(nameof(flashReadCompletedCallbackDelegate));
            _progressDelegate = progressDelegate;
        }

        /// <summary>
        /// Attempt to read FLASH
        /// </summary>
        public void Read()
        {
            _logger.LogInfo($"Reading whole FLASH (bootloader included)");

            var result = new List<byte>();
            switch (_identificationData.Version)
            {
                case (int)ProtocolVersion.First:

                    var deviceData =GetDeviceDataV1();
                    using (var driver = GetDeviceDriverV1())
                    {

                        for (var pageAddress = 0; pageAddress < deviceData.FlashPagesAll; pageAddress++)
                        {
                            result.AddRange(driver.ReadFLASHPage(pageAddress));

                            _progressDelegate?.Invoke(new ProgressData(pageAddress + 1, deviceData.FlashPagesAll));
                        }
                    }

                    _logger.LogInfo($"{ result.Count } of expected { deviceData.FlashPagesAll * deviceData.FlashPageSize } bytes read.");
                    break;

                default:
                    throw ReportUnsupportedVersion();
            }

            _flashReadCompletedCallbackDelegate(new FlashReadResult(result));
        }
    }
}
