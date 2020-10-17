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
    /// Called when EEPROM write is completed
    /// </summary>
    public delegate void EepromWriteCompletedCallbackDelegate();

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
        /// Read all FLASH memory (including bootloader)
        /// </summary>
        List<byte> ReadAllFlash();

        /// <summary>
        /// Write all FLASH memory (NOT including bootloader)
        /// </summary>
        void WriteAllFlash(List<byte> toWrite);

        /// <summary>
        /// Read all EEPROM
        /// </summary>
        void ReadAllEEPROM(EepromReadCompletedCallbackDelegate readCompletedDelegate);

        /// <summary>
        /// Write all EEPROM
        /// </summary>
        void WriteAllEEPROM(List<byte> toWrite, EepromWriteCompletedCallbackDelegate writeCompletedDelegate);

        /// <summary>
        /// Reboot device to firmware
        /// </summary>
        void RebootToFirmware();

        /// <summary>
        /// Download data from device into given HEX files
        /// </summary>
        void DownloadFromDevice(string flashPath, string eepromPath);

        /// <summary>
        /// Uploads given files into device. If file path is null or empty - then don't try to upload it.
        /// Backups directory must be specified anyway
        /// </summary>
        void UploadToDevice(string flashPath, string eepromPath, string backupsDirectory);

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
