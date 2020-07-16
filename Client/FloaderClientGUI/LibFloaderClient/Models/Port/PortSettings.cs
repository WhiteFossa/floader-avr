using System;
using System.IO.Ports;

namespace LibFloaderClient.Models.Port
{
    /// <summary>
    /// Settings, enough to open port. All fields must be specified
    /// </summary>
    public class PortSettings
    {
        /// <summary>
        /// Port name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Baudrate
        /// </summary>
        public int Baudrate { get; }

        /// <summary>
        /// Parity
        /// </summary>
        public Parity Parity { get; }

        /// <summary>
        /// Data bits count
        /// </summary>
        public int DataBits { get; }

        /// <summary>
        /// Stop bits count
        /// </summary>
        public StopBits StopBits { get; }

        public PortSettings(string name, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException(nameof(name));
            }

            Name = name;
            Baudrate = baudrate;
            Parity = parity;
            DataBits = dataBits;
            StopBits = stopBits;
        }
    }
}