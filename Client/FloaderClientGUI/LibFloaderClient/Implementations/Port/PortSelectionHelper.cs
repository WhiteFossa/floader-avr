using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

namespace LibFloaderClient.Implementations.Port
{
    /// <summary>
    /// Various usefull stuff for port selection
    /// </summary>
    public static class PortSelectionHelper
    {

#region Constants
        /// <summary>
        /// Text representations of various parities
        /// </summary>
        private const string ParityNone = "No parity control";
        private const string ParityOdd = "Odd";
        private const string ParityEven = "Even";
        private const string ParityMark = "Mark";
        private const string ParitySpace = "Space";

        /// <summary>
        /// Dictionary for parity to name and name to parity mapping
        /// </summary>
        private static readonly Dictionary<Parity, string> ParityNames = new Dictionary<Parity, string>()
        {
            { Parity.None, ParityNone },
            { Parity.Odd, ParityOdd },
            { Parity.Even, ParityEven },
            { Parity.Mark, ParityMark },
            { Parity.Space, ParitySpace }
        };

        /// <summary>
        /// Text representation of various stop bits
        /// </summary>
        private const string StopBitsOne = "One";
        private const string StopBitsOnePointFive = "One and half";
        private const string StopBitsTwo = "Two";

        /// <summary>
        /// Dictionary for stop bits to name and name to stop bits mapping
        /// </summary>
        private static readonly Dictionary<StopBits, string> StopBitsNames = new Dictionary<StopBits, string>()
        {
            { StopBits.One, StopBitsOne },
            { StopBits.OnePointFive, StopBitsOnePointFive },
            { StopBits.Two, StopBitsTwo }
        };

#endregion

#region Mappers

        /// <summary>
        /// Parity to combobox value
        /// </summary>
        public static string MapParityToString(Parity parity)
        {
            var result = ParityNames
                .Where(pn => pn.Key == parity);

            if (!result.Any())
            {
                throw new ArgumentException(nameof(parity));
            }

            return result.FirstOrDefault().Value;
        }

        /// <summary>
        /// Combobox value to parity
        /// </summary>
        public static Parity MapStringToParity(string parityStr)
        {
            var result = ParityNames
                .Where(pn => string.Equals(pn.Value, parityStr));

            if (!result.Any())
            {
                throw new ArgumentException(nameof(parityStr));
            }

            return result.FirstOrDefault().Key;
        }

        /// <summary>
        /// Stop bits count to combobox value
        /// </summary>
        public static string MapStopBitsToString(StopBits stopBits)
        {
            var result = StopBitsNames
                .Where(sb => sb.Key == stopBits);

            if (!result.Any())
            {
                throw new ArgumentException(nameof(stopBits));
            }

            return result.FirstOrDefault().Value;
        }

        /// <summary>
        /// Combobox value to stop bits count
        /// </summary>
        public static StopBits MapStringToStopBits(string str)
        {
            var result = StopBitsNames
                .Where(sb => string.Equals(sb.Value, str));

            if (!result.Any())
            {
                throw new ArgumentException(nameof(str));
            }

            return result.FirstOrDefault().Key;
        }

#endregion
    }
}