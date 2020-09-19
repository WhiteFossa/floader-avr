using LibFloaderClient.Implementations.Enums.Device;
using LibFloaderClient.Implementations.Versioned.Driver;
using LibFloaderClient.Interfaces.Auxiliary;
using LibFloaderClient.Interfaces.Device;
using LibFloaderClient.Interfaces.Logger;
using LibFloaderClient.Interfaces.Versioned.Driver;
using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Device.Versioned;
using LibFloaderClient.Models.Port;
using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibFloaderClient.Implementations.Device
{
    public class DeviceIndependentOperationsProvider : IDeviceIndependentOperationsProvider
    {
        /// <summary>
        /// When ReadAllFlash() called, first read byte will have this address.
        /// </summary>
        private const int FlashBaseAddress = 0;

        /// <summary>
        /// As FlashBaseAddress, but for EEPROM
        /// </summary>
        private const int EepromBaseAddress = 0;

        /// <summary>
        /// If data for address isn't specified in HEX file, use this filler when uploading data to MCU
        /// </summary>
        private const byte EmptyDataFiller = 255;

        private readonly ILogger _logger;
        private readonly IHexWriter _hexWriter;
        private readonly IHexReader _hexReader;
        private readonly IFilenamesGenerator _filenamesGenerator;

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
        /// Device identification data
        /// </summary>
        private DeviceIdentifierData _deviceIdentificationData;

        public DeviceIndependentOperationsProvider(ILogger logger,
            IHexWriter hexWriter,
            IHexReader hexReader,
            IFilenamesGenerator filenamesGenerator)
        {
            _logger = logger;
            _hexWriter = hexWriter;
            _filenamesGenerator = filenamesGenerator;
            _hexReader = hexReader;
        }

        public List<byte> ReadAllEEPROM()
        {
            IsSetUp();

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    using (var driver = GetDeviceDriverV1())
                    {
                        return driver.ReadEEPROM();
                    }

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
                    using (var driver = GetDeviceDriverV1())
                    {

                        for (var pageAddress = 0; pageAddress < deviceData.FlashPagesAll; pageAddress++)
                        {
                            result.AddRange(driver.ReadFLASHPage(pageAddress));
                        }
                    }

                    
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
                    using (var driver = GetDeviceDriverV1())
                    {
                        driver.Reboot();
                    }
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
            _isSetUp = true;
        }

        public void WriteAllEEPROM(List<byte> toWrite)
        {
            IsSetUp();

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    using (var driver = GetDeviceDriverV1())
                    {
                        driver.WriteEEPROM(toWrite);
                    }
                    break;

                default:
                    throw ReportUnsupportedVersion();
            }
        }

        public void WriteAllFlash(List<byte> toWrite)
        {
            IsSetUp();

            _logger.LogInfo($"Writing whole FLASH (except bootloader)...");

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:

                    var deviceData = GetDeviceDataV1();
                    using (var driver = GetDeviceDriverV1())
                    {
                        for (var pageAddress = 0; pageAddress < deviceData.FlashPagesWriteable; pageAddress++)
                        {
                            // Preparing page data
                            var pageData = toWrite.GetRange(pageAddress * deviceData.FlashPageSize, deviceData.FlashPageSize);

                            // Writing
                            _logger.LogInfo("Writing...");
                            driver.WriteFLASHPage(pageAddress, pageData);

                            // Verifying
                            _logger.LogInfo("Verifying...");
                            var readback = driver.ReadFLASHPage(pageAddress);

                            if (!readback.SequenceEqual(pageData))
                            {
                                var message = $"Page { pageAddress + 1 } verification failed.";
                                _logger.LogError(message);
                                throw new InvalidOperationException(message);
                            }

                            _logger.LogInfo("Verification is OK");
                        }
                    }

                    _logger.LogInfo("FLASH written successfully.");

                    break;

                default:
                    throw ReportUnsupportedVersion();
            }
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

            return (IDeviceDriverV1)new DeviceDriverV1(_portSettings, GetDeviceDataV1(), _logger);
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

        public void DownloadFromDevice(string flashPath, string eepromPath)
        {
            if (string.IsNullOrEmpty(flashPath))
            {
                throw new ArgumentException("FLASH file to download into must be specified.", nameof(flashPath));
            }

            if (string.IsNullOrEmpty(eepromPath))
            {
                throw new ArgumentException("EEPROM file to download into must be specified.", nameof(eepromPath));
            }

            if (flashPath.Equals(eepromPath))
            {
                throw new ArgumentException("FLASH and EEPROM files must differ.");
            }

            _logger.LogInfo($"Downloading FLASH into { flashPath }...");
            var flashData = ReadAllFlash();

            _logger.LogInfo($"Downloaded. Writting to file...");
            _hexWriter.LoadFromList(FlashBaseAddress, flashData);
            _hexWriter.WriteToFile(flashPath);
            _logger.LogInfo("Done");

            _logger.LogInfo($"Downloading EEPROM into { eepromPath }...");
            var eepromData = ReadAllEEPROM();

            _logger.LogInfo($"Downloaded. Writting to file...");
            _hexWriter.LoadFromList(EepromBaseAddress, eepromData);
            _hexWriter.WriteToFile(eepromPath);
            _logger.LogInfo("Done");

            _logger.LogInfo("Download completed.");
        }

        public string GenerateFlashFileName(bool isBackup)
        {
            IsSetUp();

            return _filenamesGenerator.GenerateFLASHFilename(_deviceIdentificationData, isBackup);
        }

        public string GenerateEepromFileName(bool isBackup)
        {
            IsSetUp();

            return _filenamesGenerator.GenerateEEPROMFilename(_deviceIdentificationData, isBackup);
        }

        public void UploadToDevice(string flashPath, string eepromPath, string backupsDirectory)
        {
            if (string.IsNullOrEmpty(backupsDirectory))
            {
                throw new ArgumentException("Backups directory must be specified.", nameof(backupsDirectory));
            }

            bool isUploadFlash = !string.IsNullOrEmpty(flashPath);
            bool isUploadEeprom = !string.IsNullOrEmpty(eepromPath);

            if (!isUploadFlash && !isUploadEeprom)
            {
                throw new ArgumentException("At least either FLASH or EEPROM files must be specified");
            }

            // Parsing HEX files
            var flashHexData = new SortedDictionary<int, byte>();
            var eepromHexData = new SortedDictionary<int, byte>();

            if (isUploadFlash)
            {
                flashHexData = _hexReader.ReadFromFile(flashPath);
            }

            if (isUploadEeprom)
            {
                eepromHexData = _hexReader.ReadFromFile(eepromPath);
            }

            // Are addresses valid?
            int maxWriteableFlashAddress = 0;
            int maxWriteableEepromAddress = 0;

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    var deviceData = GetDeviceDataV1();

                    maxWriteableFlashAddress = deviceData.FlashPagesWriteable * deviceData.FlashPageSize - 1;
                    maxWriteableEepromAddress = deviceData.EepromSize - 1;

                    break;

                default:
                    ReportUnsupportedVersion();
                    break;
            }

            if (flashHexData.Any(fhd => fhd.Key > maxWriteableFlashAddress))
            {
                throw new ArgumentException("Given FLASH file contains data for addresses in non-writeable FLASH area. Check file correctness.");
            }

            if (eepromHexData.Any(ehd => ehd.Key > maxWriteableEepromAddress))
            {
                throw new ArgumentException("Given EEPROM file contains data for addressess outside EEPROM. Check file correctness.");
            }

            // Constructing data arrays for upload
            var flashDataToUpload = new List<byte>();
            var eepromDataToUpload = new List<byte>();

            if (isUploadFlash)
            { 
                for (int address = 0; address <= maxWriteableFlashAddress; address++)
                {
                    flashDataToUpload[address] = flashHexData.ContainsKey(address) ? flashHexData[address] : EmptyDataFiller;
                }
            }

            if (isUploadEeprom)
            {
                for (int address = 0; address <= maxWriteableEepromAddress; address++)
                {
                    eepromDataToUpload[address] = eepromHexData.ContainsKey(address) ? eepromHexData[address] : EmptyDataFiller;
                }
            }

            // Making backups
            var flashBackupFilename = Path.Combine(backupsDirectory, GenerateFlashFileName(true));
            var eepromBackupFilename = Path.Combine(backupsDirectory, GenerateEepromFileName(true));

            _logger.LogInfo("Backing up...");
            _logger.LogInfo($"FLASH backup file: { flashBackupFilename }");
            _logger.LogInfo($"EEPROM backup file: { eepromBackupFilename }");

            DownloadFromDevice(flashBackupFilename, eepromBackupFilename);

            _logger.LogInfo("Done");

            // Uploading
            _logger.LogInfo("Uploading...");

            if (isUploadFlash)
            {
                WriteAllFlash(flashDataToUpload);
            }

            if (isUploadEeprom)
            {
                WriteAllEEPROM(eepromDataToUpload);
            }

            _logger.LogInfo("Done");
        }
    }
}
