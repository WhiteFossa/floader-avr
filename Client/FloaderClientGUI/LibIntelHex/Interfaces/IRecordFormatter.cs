using System.Collections.Generic;

namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// Interface to format raw record (i.e. list of bytes) to Intel HEX format string
    /// </summary>
    public interface IRecordFormatter
    {
        /// <summary>
        /// Formats raw record as iHEX string (starting with ":", ending with checksum and so on).
        /// Raw record MUST NOT contain trailing checksum
        /// </summary>
        string FormatRawRecord(List<byte> data);
    }
}
