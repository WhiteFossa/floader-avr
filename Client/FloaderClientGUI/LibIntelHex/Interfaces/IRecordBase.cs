namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// All records have to implement this interface
    /// </summary>
    public interface IRecordBase
    {
        /// <summary>
        /// Formats record to ready to write Intel HEX line (starting with ":" and ending with checksum)
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
