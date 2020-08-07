using LibFloaderClient.Models.Device.Versioned;
using LibFloaderClient.Models.Port;
using System.Collections.Generic;

namespace LibFloaderClient.Interfaces.Versioned.Driver
{
    /// <summary>
    /// Device driver, protocol version 1
    /// </summary>
    public interface IDeviceDriverV1
    {
        /// <summary>
        /// Setup driver to use given connection
        /// </summary>
        void Setup(PortSettings portSettings, DeviceDataV1 deviceData);

        /// <summary>
        /// Creates serial port driver and attaches it to port. Only after this call operations with device are allowed
        /// </summary>
        void OccupyPort();

        /// <summary>
        /// Frees port and destroys driver
        /// </summary>
        void ReleasePort();

        /// <summary>
        /// Reboots target device
        /// </summary>
        /// <returns>True if device successfully reported reboot</returns>
        bool Reboot();

        /// <summary>
        /// Read all EEPROM bytes. Result never is null.
        /// </summary>
        /// <returns></returns>
        List<byte> ReadEEPROM();

        /// <summary>
        /// Writes data to EEPROM. toWrite list must have EEPROM size elements.
        /// </summary>
        void WriteEEPROM(List<byte> toWrite);

        /// <summary>
        /// Reads FLASH page with given address or throws an exception in case of error. Result never is null.
        /// </summary>
        List<byte> ReadFLASHPage(int pageAddress);

        /// <summary>
        /// Writes FLASH page with given address or throws an exception in case of error. Please note that bootloader pages is write-protected.
        /// </summary>
        void WriteFLASHPage(int pageAddress, List<byte> toWrite);
    }
}
