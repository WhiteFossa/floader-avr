using System.Collections.Generic;

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

        /// TODO: Cover it with tests
        /// <summary>
        /// Load data to write from list.
        /// </summary>
        /// <param name="baseAddress">Address of first byte in list</param>
        void LoadFromList(int baseAddress, List<byte> data);

        /// <summary>
        /// Generate Intel HEX file contents and put it into string
        /// </summary>
        string ToString();

        /// <summary>
        /// Generates and writes generated Intel HEX into file. If file exists, it would be overwritten.
        /// </summary>
        void WriteToFile(string path);
    }
}
