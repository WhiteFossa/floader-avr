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

using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using System;
using System.Collections.Generic;

namespace LibFloaderClient.Interfaces.Device
{
    /// <summary>
    /// Called when EEPROM read is completed
    /// </summary>
    public delegate void EepromReadCompletedCallbackDelegate(EepromReadResult data);

    /// <summary>
    /// Called when all FLASH read is completed
    /// </summary>
    public delegate void FlashReadCompletedCallbackDelegate(FlashReadResult data);

    /// <summary>
    /// Called when EEPROM write is completed
    /// </summary>
    public delegate void EepromWriteCompletedCallbackDelegate();

    /// <summary>
    /// Called when FLASH write is completed
    /// </summary>
    public delegate void FlashWriteCompletedCallbackDelegate();

    /// <summary>
    /// Called when all data is downloaded from device
    /// </summary>
    public delegate void DownloadFromDeviceCompletedCallbackDelegate();

    /// <summary>
    /// Called when all data is uploaded to device
    /// </summary>
    public delegate void UploadToDeviceCompletedCallbackDelegate();

    /// <summary>
    /// Delegate, used to indicate current progress.
    /// </summary>
    public delegate void ProgressDelegate(ProgressData data);

    /// <summary>
    /// Called when device rebooted into firmware
    /// </summary>
    public delegate void RebootToFirmwareCompletedCallbackDelegate(DeviceRebootResult rebootResult);

    /// <summary>
    /// Called when unhandled exception happens
    /// </summary>
    public delegate void UnhandledExceptionCallbackDelegate(Exception exception);

    /// <summary>
    /// Wrapper over device driver, hiding versioned details and allowing to write/read all pages at once
    /// </summary>
    public interface IDeviceIndependentOperationsProvider
    {
        /// <summary>
        /// Setup provider to work. Any calls before this one will fail
        /// </summary>
        void Setup(PortSettings portSettings, DeviceIdentifierData deviceIdentData, Object versionSpecificDeviceData);

        /// <summary>
        /// Reverts Setup() action
        /// </summary>
        void DeSetup();

        /// <summary>
        /// Initiates all FLASH (including bootloader) read. Exits immediately, without waiting for read completion
        /// </summary>
        void InitiateReadAllFlash(FlashReadCompletedCallbackDelegate readCompletedDelegate, UnhandledExceptionCallbackDelegate unhandledExceptionDelegate,
            ProgressDelegate progressDelegate = null);

        /// <summary>
        /// Initiates all FLASH memory (NOT including bootloader) write. Exits immediately, without waiting for write completion
        /// </summary>
        void InitiateWriteAllFlash(List<byte> toWrite, FlashWriteCompletedCallbackDelegate writeCompletedDelegate,
            UnhandledExceptionCallbackDelegate unhandledExceptionDelegate, ProgressDelegate progressDelegate = null);

        /// <summary>
        /// Initiates all EEPROM read. Exits immediately, without waiting for read completion
        /// </summary>
        void InitiateReadAllEEPROM(EepromReadCompletedCallbackDelegate readCompletedDelegate, UnhandledExceptionCallbackDelegate unhandledExceptionDelegate,
            ProgressDelegate progressDelegate = null);

        /// <summary>
        /// Initiates all EEPROM write. Exits immediately, without waiting for write completion
        /// </summary>
        void InitiateWriteAllEEPROM(List<byte> toWrite, EepromWriteCompletedCallbackDelegate writeCompletedDelegate,
            UnhandledExceptionCallbackDelegate unhandledExceptionDelegate, ProgressDelegate progressDelegate = null);

        /// <summary>
        /// Reboot device to firmware
        /// </summary>
        void InitiateRebootToFirmware(RebootToFirmwareCompletedCallbackDelegate rebootCompletedDelegate,
            UnhandledExceptionCallbackDelegate unhandledExceptionDelegate);

        /// <summary>
        /// Starts download from device into given HEX files. Exits immediately. If downloadCompleteDelegate isn't null, then
        /// that delegate will be called on completion
        /// </summary>
        void InitiateDownloadFromDevice(string flashPath, string eepromPath, UnhandledExceptionCallbackDelegate unhandledExceptionCallbackDelegate,
            DownloadFromDeviceCompletedCallbackDelegate downloadCompletedDelegate = null, ProgressDelegate progressDelegate = null);

        /// <summary>
        /// Checks can given data (from HEX file) fit into EEPROM
        /// </summary>
        bool CheckEepromAddressesForCorrectness(SortedDictionary<int, byte> data);

        /// <summary>
        /// Checks can given data (from HEX file) fit into FLASH. This method returns true if data
        /// contains bytes in bootloader area
        /// </summary>
        bool CheckFlashAddressesForCorrectness(SortedDictionary<int, byte> data);
        
        /// <summary>
        /// Checks if given data (from HEX file) contain no bootloader bytes.
        /// </summary>
        /// <returns>True if data contains no bootloader bytes</returns>
        bool CheckIfFlashAddressesOutsideBootloader(SortedDictionary<int, byte> data);
        
        /// <summary>
        /// Initializes given files upload into device. If file path is null or empty - then don't try to upload it.
        /// Backups directory must be specified anyway. Exist immediately.
        /// If uploadCompletedDelegate isn't null, then call it on completion
        /// </summary>
        void InitiateUploadToDevice(string flashPath, string eepromPath, string backupsDirectory,
            UnhandledExceptionCallbackDelegate unhandledExceptionCallbackDelegate, 
            UploadToDeviceCompletedCallbackDelegate uploadCompletedDelegate = null, ProgressDelegate progressDelegate = null);

        /// <summary>
        /// Generate filename for FLASH file download/backup
        /// </summary>
        string GenerateFlashFileName(bool isBackup);

        /// <summary>
        /// Generate filename for EEPROM file download/backup
        /// </summary>
        string GenerateEepromFileName(bool isBackup);
    }
}
