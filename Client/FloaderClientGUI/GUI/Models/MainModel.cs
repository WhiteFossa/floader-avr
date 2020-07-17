using System;
using LibFloaderClient.Models.Port;
using System.IO.Ports;

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
    }
}