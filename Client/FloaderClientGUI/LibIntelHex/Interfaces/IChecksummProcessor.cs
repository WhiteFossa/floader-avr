using System.Collections.Generic;

namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// Interface to calculate / check Intel HEX checksums
    /// </summary>
    public interface IChecksummProcessor
    {
        /// <summary>
        /// Calculates checksumm for given data.
        /// </summary>
        byte CalculateChecksumm(List<byte> data);

        /// <summary>
        /// Checks checksumm for given data, checksumm must be in the last byte
        /// </summary>
        bool CheckChecksumm(List<byte> data);
    }
}
