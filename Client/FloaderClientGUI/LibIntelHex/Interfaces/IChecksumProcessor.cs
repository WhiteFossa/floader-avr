using System.Collections.Generic;

namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// Interface to calculate / check Intel HEX checksums
    /// </summary>
    public interface IChecksumProcessor
    {
        /// <summary>
        /// Calculates checksum for given data.
        /// </summary>
        byte CalculateChecksum(List<byte> data);

        /// <summary>
        /// Checks checksum for given data, checksum must be in the last byte
        /// </summary>
        bool VerifyChecksum(List<byte> data);
    }
}
