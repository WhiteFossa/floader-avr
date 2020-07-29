using System;
using System.Collections.Generic;
using LibFloaderClient.Models.Port;
using LibFloaderClient.Interfaces.SerialPortDriver;
using LibFloaderClient.Models.Device;
using SharpDX.Direct2D1;

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
        /// Device identification data
        /// </summary>
        public DeviceIdentifierData DeviceIdentData { get; set; }

        /// <summary>
        /// Human-readable device description
        /// </summary>
        public DeviceHumanReadableDescription DeviceHumanReadableDescription { get; set; }
    }
}