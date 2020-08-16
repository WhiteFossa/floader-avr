namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// Intel HEX writer (only 8bit)
    /// </summary>
    public interface IHexWriter
    {
        /// <summary>
        /// Resets writer, cleaning all data
        /// </summary>
        void Reset();

        /// <summary>
        /// Add next data byte to writer
        /// </summary>
        void AddByte(int address, byte data);

        /// <summary>
        /// Generate Intel HEX file contents and put it into string
        /// </summary>
        string ToString();
    }
}
