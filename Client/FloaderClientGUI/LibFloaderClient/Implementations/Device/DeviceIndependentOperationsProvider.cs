using LibFloaderClient.Implementations.Enums.Device;
using LibFloaderClient.Implementations.Versioned.Driver;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Interfaces.Versioned.Driver;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Device.Versioned;
using LibFloaderClient.Models.Port;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibFloaderClient.Implementations.Device
{
    public class DeviceIndependentOperationsProvider : IDeviceIndependentOperationsProvider
    {
        private ILogger _logger;

        /// <summary>
        /// Is provider ready to work?
        /// </summary>
        private bool _isSetUp = false;

        /// <summary>
        /// Port settings
        /// </summary>
        private PortSettings _portSettings;

        /// <summary>
        /// Version-specific device data
        /// </summary>
        private object _versionSpecificDeviceData;

        /// <summary>
        /// Driver to device (version-specific)
        /// </summary>
        private object _deviceDriver;

        /// <summary>
        /// Device identification data
        /// </summary>
        private DeviceIdentifierData _deviceIdentificationData;

        public DeviceIndependentOperationsProvider(ILogger logger)
        {
            _logger = logger;
        }

        public List<byte> ReadAllEEPROM()
        {
            IsSetUp();

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    var driver = GetDeviceDriverV1();
                    driver.OccupyPort();
                    var result = driver.ReadEEPROM();
                    driver.ReleasePort();

                    return result;

                default:
                    throw ReportUnsupportedVersion();
            }
        }

        public List<byte> ReadAllFlash()
        {
            IsSetUp();

            _logger.LogInfo($"Reading whole FLASH (bootloader included)");

            var result = new List<byte>();
            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    var deviceData = GetDeviceDataV1();
                    var driver = GetDeviceDriverV1();
                    driver.OccupyPort();

                    for (var pageAddress = 0; pageAddress < GetDeviceDataV1().FlashPagesAll; pageAddress ++)
                    {
                        result.AddRange(driver.ReadFLASHPage(pageAddress));
                    }

                    driver.ReleasePort();
                    _logger.LogInfo($"{ result.Count } of expected { deviceData.FlashPagesAll * deviceData.FlashPageSize } bytes read.");
                    break;

                default:
                    throw ReportUnsupportedVersion();
            }

            return result;
        }

        public void RebootToFirmware()
        {
            IsSetUp();

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    var driver = GetDeviceDriverV1();
                    driver.OccupyPort();
                    driver.Reboot();
                    driver.ReleasePort();

                    break;

                default:
                    throw ReportUnsupportedVersion();
            }
        }

        public void Setup(PortSettings portSettings, DeviceIdentifierData deviceIdentData, object versionSpecificDeviceData)
        {
            _portSettings = portSettings;
            _deviceIdentificationData = deviceIdentData;
            _versionSpecificDeviceData = versionSpecificDeviceData;

            // Spawning driver
            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:

                    _deviceDriver = new DeviceDriverV1(_logger);
                    GetDeviceDriverV1().Setup(_portSettings, GetDeviceDataV1());
                    break;

                default:
                    throw ReportUnsupportedVersion();
            }

            _isSetUp = true;
        }

        public void WriteAllEEPROM(List<byte> toWrite)
        {
            IsSetUp();

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    var driver = GetDeviceDriverV1();
                    driver.OccupyPort();
                    driver.WriteEEPROM(toWrite);
                    driver.ReleasePort();

                    break;

                default:
                    throw ReportUnsupportedVersion();
            }
        }

        public void WriteAllFlash(List<byte> toWrite)
        {
            IsSetUp();

            throw new NotImplementedException();
        }

        private void IsSetUp()
        {
            if (!_isSetUp)
            {
                var msg = "Attempt to use not set up mass read/write provider.";
                _logger.LogError(msg);
                throw new InvalidOperationException(msg);
            }
        }

        private InvalidOperationException ReportUnsupportedVersion()
        {
            return new InvalidOperationException($"Unsupported version { _deviceIdentificationData.Version }.");
        }

        private IDeviceDriverV1 GetDeviceDriverV1()
        {
            if (_deviceIdentificationData.Version != (int)ProtocolVersion.First)
            {
                throw ReportNonV1Version();
            }

            return (IDeviceDriverV1)_deviceDriver;
        }

        private DeviceDataV1 GetDeviceDataV1()
        {
            if (_deviceIdentificationData.Version != (int)ProtocolVersion.First)
            {
                throw ReportNonV1Version();
            }

            return (DeviceDataV1)_versionSpecificDeviceData;
        }

        private InvalidOperationException ReportNonV1Version()
        {
            return new InvalidOperationException("Version must be 1.");
        }
    }
}
