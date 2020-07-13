using System.Collections.Generic;
using System.IO.Ports;

namespace LibFloaderClient.Implementations.Port
{
    /// <summary>
    /// Possible and default settings for port
    /// </summary>
    public class PossiblePortSettings
    {
        /// <summary>
        /// Possible baudrates
        /// </summary>
        public static readonly List<int> StandardBaudrates = new List<int>()
            {110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };

        /// <summary>
        /// Default baudrate
        /// </summary>
        public const int DefaultBaudrate = 57600;

        /// <summary>
        /// Possible port parities
        /// </summary>
        public static readonly List<Parity> PossibleParities = new List<Parity>()
            { Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space };

        /// <summary>
        /// Default parity mode
        /// </summary>
        public const Parity DefaultParity = Parity.None;

        /// <summary>
        /// Possible port data bits
        /// </summary>
        public static readonly List<int> PossibleDataBits = new List<int>() { 5, 6, 7, 8, 9 };

        /// <summary>
        /// Default data bits
        /// </summary>
        public const int DefaultDataBits = 8;
    }
}