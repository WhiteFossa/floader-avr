namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// Interface to convert bytes from/to Intel HEX format
    /// </summary>
    public interface IBytesReaderWriter
    {
        /// <summary>
        /// Convert given byte to 2-bytes HEX representation
        /// </summary>
        string ToHex(byte b);

        /// <summary>
        /// Trying to parse string and get byte from it, throws an exception in case of incorrect string
        /// </summary>
        byte FromHex(string hexString);
    }
}
