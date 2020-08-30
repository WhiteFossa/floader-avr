using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHex.Interfaces
{
    /// <summary>
    /// Intel HEX reader (only 8 bit)
    /// </summary>
    public interface IHexReader
    {
        /// <summary>
        /// Parses Intel HEX file content, returning it as a sorted dictionary with data.
        /// </summary>
        /// <param name="hexFileContent"></param>
        /// <returns></returns>
        SortedDictionary<int, byte> ReadFromString(string hexFileContent);
    }
}
