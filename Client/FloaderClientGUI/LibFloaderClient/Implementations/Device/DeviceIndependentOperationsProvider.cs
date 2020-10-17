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
using System.Threading;

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

        /// <summary>
        /// Save FLASH dump here
        /// </summary>
        private string _flashSavePath;

        /// <summary>
        /// Save EEPROM dump here
        /// </summary>
        private string _eepromSavePath;

        /// <summary>
        /// If not null, then will be called when download from device is completed
        /// </summary>
        DownloadFromDeviceCompletedCallbackDelegate _downloadCompletedDelegate;

        /// <summary>
        /// Do we need to upload FLASH?
        /// </summary>
        private bool _isUploadFlash;

        /// <summary>
        /// Do we need to upload EEPROM?
        /// </summary>
        private bool _isUploadEeprom;

        /// <summary>
        /// All FLASH data to upload
        /// </summary>
        List<byte> _flashDataToUpload = new List<byte>();

        /// <summary>
        /// All EEPROM data to upload
        /// </summary>
        List<byte> _eepromDataToUpload = new List<byte>();

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

        public void InitiateReadAllEEPROM(EepromReadCompletedCallbackDelegate readCompletedDelegate)
        {
            _ = readCompletedDelegate ?? throw new ArgumentNullException(nameof(readCompletedDelegate));

            IsSetUp();

            var threadedEepromReader = new ThreadedEepromReader(_deviceIdentificationData, _portSettings, _versionSpecificDeviceData, _logger,
                readCompletedDelegate);
            var eepromReaderThread = new Thread(new ThreadStart(threadedEepromReader.Read));
            eepromReaderThread.Start();
        }

        public void InitiateReadAllFlash(FlashReadCompletedCallbackDelegate readCompletedDelegate)
        {
            _ = readCompletedDelegate ?? throw new ArgumentNullException(nameof(readCompletedDelegate));

            IsSetUp();

            var threadedFlashReader = new ThreadedFlashReader(_deviceIdentificationData, _portSettings, _versionSpecificDeviceData, _logger,
                readCompletedDelegate);
            var flashReaderThread = new Thread(new ThreadStart(threadedFlashReader.Read));
            flashReaderThread.Start();
        }

        public void RebootToFirmware()
        {
            IsSetUp();

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    using (var driver = GetDeviceDriverV1(identificationData: _deviceIdentificationData, portSettings: _portSettings,
                        versionSpecificDeviceData: _versionSpecificDeviceData, logger: _logger))
                    {
                        driver.Reboot();
                    }
                    break;

                default:
                    throw ReportUnsupportedVersion(_deviceIdentificationData.Version);
            }
        }

        public void Setup(PortSettings portSettings, DeviceIdentifierData deviceIdentData, object versionSpecificDeviceData)
        {
            _portSettings = portSettings;
            _deviceIdentificationData = deviceIdentData;
            _versionSpecificDeviceData = versionSpecificDeviceData;
            _isSetUp = true;
        }

        public void DeSetup()
        {
            _portSettings = null;
            _deviceIdentificationData = null;
            _versionSpecificDeviceData = null;
            _isSetUp = false;
        }

        public void InitiateWriteAllEEPROM(List<byte> toWrite, EepromWriteCompletedCallbackDelegate writeCompletedDelegate)
        {
            _ = writeCompletedDelegate ?? throw new ArgumentNullException(nameof(writeCompletedDelegate));

            IsSetUp();

            var threadedEepromWriter = new ThreadedEepromWriter(_deviceIdentificationData, _portSettings, _versionSpecificDeviceData, _logger,
                toWrite, writeCompletedDelegate);
            var eepromWriterThread = new Thread(new ThreadStart(threadedEepromWriter.Write));
            eepromWriterThread.Start();
        }

        public void InitiateWriteAllFlash(List<byte> toWrite, FlashWriteCompletedCallbackDelegate writeCompletedDelegate)
        {
            _ = writeCompletedDelegate ?? throw new ArgumentNullException(nameof(writeCompletedDelegate));

            IsSetUp();

            var threadedFlashWriter = new ThreadedFlashWriter(_deviceIdentificationData, _portSettings, _versionSpecificDeviceData, _logger,
                toWrite, writeCompletedDelegate);
            var flashWriterThread = new Thread(new ThreadStart(threadedFlashWriter.Write));
            flashWriterThread.Start();
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

        public void InitiateDownloadFromDevice(string flashPath, string eepromPath, DownloadFromDeviceCompletedCallbackDelegate downloadCompletedDelegate)
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

            _eepromSavePath = eepromPath;
            _flashSavePath = flashPath;
            _downloadCompletedDelegate = downloadCompletedDelegate;

            _logger.LogInfo($"Downloading FLASH into { _flashSavePath }...");
           InitiateReadAllFlash(OnFlashReadCompletedDuringDownloadFromDevice);
        }

        /// <summary>
        /// Called when FLASH read completed during download from device
        /// </summary>
        private void OnFlashReadCompletedDuringDownloadFromDevice(FlashReadResult data)
        {
            _logger.LogInfo($"Downloaded. Writting to file...");
            _hexWriter.LoadFromList(FlashBaseAddress, data.Data);
            _hexWriter.WriteToFile(_flashSavePath);

            _flashSavePath = null; // To reduce risk of buggy reuse
            _logger.LogInfo("Done");

            _logger.LogInfo($"Downloading EEPROM into { _eepromSavePath }...");
            InitiateReadAllEEPROM(OnEepromReadCompletedDuringDownloadFromDevice);
        }

        /// <summary>
        /// Called when EEPROM read completed during download from device
        /// </summary>
        private void OnEepromReadCompletedDuringDownloadFromDevice(EepromReadResult data)
        {
            _logger.LogInfo($"Downloaded. Writting to file...");
            _hexWriter.LoadFromList(EepromBaseAddress, data.Data);
            _hexWriter.WriteToFile(_eepromSavePath);
            _logger.LogInfo("Done");

            _logger.LogInfo("Download completed.");

            _eepromSavePath = null; // To reduce risk of buggy reuse

            _downloadCompletedDelegate?.Invoke();
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

            _isUploadFlash = !string.IsNullOrEmpty(flashPath);
            _isUploadEeprom = !string.IsNullOrEmpty(eepromPath);

            if (!_isUploadFlash && !_isUploadEeprom)
            {
                throw new ArgumentException("At least either FLASH or EEPROM files must be specified");
            }

            // Parsing HEX files
            var flashHexData = new SortedDictionary<int, byte>();
            var eepromHexData = new SortedDictionary<int, byte>();

            if (_isUploadFlash)
            {
                flashHexData = _hexReader.ReadFromFile(flashPath);
            }

            if (_isUploadEeprom)
            {
                eepromHexData = _hexReader.ReadFromFile(eepromPath);
            }

            // Are addresses valid?
            int maxWriteableFlashAddress = 0;
            int maxWriteableEepromAddress = 0;

            switch (_deviceIdentificationData.Version)
            {
                case (int)ProtocolVersion.First:
                    var deviceData = GetDeviceDataV1(_deviceIdentificationData, _versionSpecificDeviceData);

                    maxWriteableFlashAddress = deviceData.FlashPagesWriteable * deviceData.FlashPageSize - 1;
                    maxWriteableEepromAddress = deviceData.EepromSize - 1;

                    break;

                default:
                    throw ReportUnsupportedVersion(_deviceIdentificationData.Version);
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
            if (_isUploadFlash)
            {
                for (int address = 0; address <= maxWriteableFlashAddress; address++)
                {
                    _flashDataToUpload.Add(EmptyDataFiller);
                }

                foreach (var address in flashHexData.Keys)
                {
                    _flashDataToUpload[address] = flashHexData[address];
                }
            }

            if (_isUploadEeprom)
            {
                for (int address = 0; address <= maxWriteableEepromAddress; address++)
                {
                    _eepromDataToUpload.Add(EmptyDataFiller);
                }

                foreach (var address in eepromHexData.Keys)
                {
                    _eepromDataToUpload[address] = eepromHexData[address];
                }
            }

            // Making backups
            var flashBackupFilename = Path.Combine(backupsDirectory, GenerateFlashFileName(true));
            var eepromBackupFilename = Path.Combine(backupsDirectory, GenerateEepromFileName(true));

            _logger.LogInfo("Backing up...");
            _logger.LogInfo($"FLASH backup file: { flashBackupFilename }");
            _logger.LogInfo($"EEPROM backup file: { eepromBackupFilename }");

            InitiateDownloadFromDevice(flashBackupFilename, eepromBackupFilename, MakeActualUpload);
        }

        /// <summary>
        /// Make actual upload (called when backups are done)
        /// </summary>
        private void MakeActualUpload()
        {
            _logger.LogInfo("Done");

            // Uploading
            _logger.LogInfo("Uploading...");

            if (_isUploadFlash)
            {
                InitiateWriteAllFlash(_flashDataToUpload, OnFlashWriteCompletedDuringUploadToDevice);
            }
            else
            {
                UploadEeprom();
            }
        }

        private void OnFlashWriteCompletedDuringUploadToDevice()
        {
            UploadEeprom();
        }
        private void UploadEeprom()
        {
            if (_isUploadEeprom)
            {
                InitiateWriteAllEEPROM(_eepromDataToUpload, OnEepromWriteCompletedDuringUploadToDevice);
            }
            else
            {
                CompleteUpload();
            }
        }

        private void OnEepromWriteCompletedDuringUploadToDevice()
        {
            CompleteUpload();
        }

        private void CompleteUpload()
        {
            _logger.LogInfo("Done");
        }

        #region Helper stuff

        public static InvalidOperationException ReportUnsupportedVersion(int version)
        {
            return new InvalidOperationException($"Unsupported version { version }.");
        }

        public static InvalidOperationException ReportNonV1Version()
        {
            return new InvalidOperationException("Version must be 1.");
        }

        public static DeviceDataV1 GetDeviceDataV1(DeviceIdentifierData identificationData, object versionSpecificDeviceData)
        {
            if (identificationData.Version != (int)ProtocolVersion.First)
            {
                throw ReportNonV1Version();
            }

            return (DeviceDataV1)versionSpecificDeviceData;
        }

        public static IDeviceDriverV1 GetDeviceDriverV1(DeviceIdentifierData identificationData,
            object versionSpecificDeviceData,
            PortSettings portSettings,
            ILogger logger)
        {
            if (identificationData.Version != (int)ProtocolVersion.First)
            {
                throw ReportNonV1Version();
            }

            return (IDeviceDriverV1)new DeviceDriverV1(portSettings, GetDeviceDataV1(identificationData, versionSpecificDeviceData), logger);
        }

        #endregion
    }
}
