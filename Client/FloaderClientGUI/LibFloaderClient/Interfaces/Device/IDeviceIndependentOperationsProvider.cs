using LibFloaderClient.Models.Device;
using LibFloaderClient.Models.Port;
using System;
using System.Collections.Generic;

namespace LibFloaderClient.Interfaces.Device
{
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
        List<byte> ReadAllEEPROM();

        /// <summary>
        /// Write all EEPROM
        /// </summary>
        void WriteAllEEPROM(List<byte> toWrite);

        /// <summary>
        /// Reboot device to firmware
        /// </summary>
        void RebootToFirmware();

        /// <summary>
        /// Download data from device into given HEX files
        /// </summary>
        void DownloadFromDevice(string flashPath, string eepromPath);

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
