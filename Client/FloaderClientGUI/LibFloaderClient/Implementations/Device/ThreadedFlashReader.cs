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
        /// Operation name to be displayed near progressbar
        /// </summary>
        private const string ProgressOperationName = "Reading FLASH";

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
            _logger.LogInfo($"Reading FLASH (bootloader included)...");

            var result = new List<byte>();
            switch (_identificationData.Version)
            {
                case (int)ProtocolVersion.First:

                    var deviceData =GetDeviceDataV1();
                    using (var driver = GetDeviceDriverV1())
                    {
                        _progressDelegate?.Invoke(new ProgressData(0, deviceData.FlashPagesAll, ProgressOperationName));

                        for (var pageAddress = 0; pageAddress < deviceData.FlashPagesAll; pageAddress++)
                        {
                            result.AddRange(driver.ReadFLASHPage(pageAddress));

                            _progressDelegate?.Invoke(new ProgressData(pageAddress + 1, deviceData.FlashPagesAll, ProgressOperationName));
                        }
                    }

                    _logger.LogInfo($"Done");
                    break;

                default:
                    throw ReportUnsupportedVersion();
            }

            _flashReadCompletedCallbackDelegate(new FlashReadResult(result));
        }
    }
}
