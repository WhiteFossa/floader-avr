using LibFloaderClient.Implementations.Enums.Device;
using LibFloaderClient.Implementations.Resources;
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
            _logger.LogInfo(Language.ReadingFlash);

            var result = new List<byte>();
            switch (_identificationData.Version)
            {
                case (int)ProtocolVersion.First:

                    var deviceData =GetDeviceDataV1();
                    using (var driver = GetDeviceDriverV1())
                    {
                        _progressDelegate?.Invoke(new ProgressData(0, deviceData.FlashPagesAll, Language.ProgressOperationReadingFlash));

                        for (var pageAddress = 0; pageAddress < deviceData.FlashPagesAll; pageAddress++)
                        {
                            result.AddRange(driver.ReadFLASHPage(pageAddress));

                            _progressDelegate?.Invoke(new ProgressData(pageAddress + 1, deviceData.FlashPagesAll, Language.ProgressOperationReadingFlash));
                        }
                    }

                    _logger.LogInfo(Language.Done);
                    break;

                default:
                    throw ReportUnsupportedVersion();
            }

            _flashReadCompletedCallbackDelegate(new FlashReadResult(result));
        }
    }
}
