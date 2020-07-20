using System;
using LibFloaderClient.Models.Port;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Models.Device;

namespace FloaderClientGUI.Models
{
    /// <summary>
    /// Main model for GUI client
    /// </summary>
    public class MainModel
    {
        /// <summary>
        /// Port settings. Might be null if port not selected yet.
        /// </summary>
        /// <value></value>
        public PortSettings PortSettings { get; set; }

        /// <summary>
        /// Serial port driver. Use via using() {}
        /// </summary>
        /// <value></value>
        public ISerialPortDriver PortDriver { get; set; }

        /// <summary>
        /// Device identification data (from bootloader)
        /// </summary>
        public DeviceIdentificator DeviceIdentDataBL { get; set; }
    }
}