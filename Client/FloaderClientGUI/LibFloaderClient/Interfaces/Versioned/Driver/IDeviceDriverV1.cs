using LibFloaderClient.Models.Device.Versioned;
using LibFloaderClient.Models.Port;

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
        /// Reboots target device
        /// </summary>
        /// <returns>True if device successfully reported reboot</returns>
        bool Reboot();
    }
}
