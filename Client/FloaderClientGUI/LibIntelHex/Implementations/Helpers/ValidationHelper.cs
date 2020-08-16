using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHex.Implementations.Helpers
{
    /// <summary>
    /// Class to help with various validations
    /// </summary>
    public static class ValidationHelper
    {
        /// <summary>
        /// Max address for 8 bit Intel HEX
        /// </summary>
        public const int MaxAddress = 65535;

        /// <summary>
        /// Validates address, throws exception if address invalid for 8bit Intel HEX
        /// </summary>
        public static void ValidateAddress(int address)
        {
            if (address < 0 || address > MaxAddress)
            {
                throw new ArgumentOutOfRangeException(nameof(address), address, $"Address must be in [0, { MaxAddress }] range.");
            }
        }
    }
}
