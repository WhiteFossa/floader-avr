using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHex.Implementations
{
    public class BytesReaderWriter : IBytesReaderWriter
    {
        /// <summary>
        /// Nibble (half-byte) to string representation conversion table
        /// </summary>
        private readonly Dictionary<int, string> NibbleToStringTable = new Dictionary<int, string>()
        {
            { 0, "0" },
            { 1, "1" },
            { 2, "2" },
            { 3, "3" },
            { 4, "4" },
            { 5, "5" },
            { 6, "6" },
            { 7, "7" },
            { 8, "8" },
            { 9, "9" },
            { 10, "A" },
            { 11, "B" },
            { 12, "C" },
            { 13, "D" },
            { 14, "E" },
            { 15, "F" },
        };

        /// <summary>
        /// Reverse of NibbleToStringTable
        /// </summary>
        private readonly Dictionary<string, int> StringToNibbleTable;

        public BytesReaderWriter()
        {
            StringToNibbleTable = new Dictionary<string, int>();
            foreach (var item in NibbleToStringTable)
            {
                StringToNibbleTable.Add(item.Value, item.Key);
            }
        }

        public byte FromHex(string hexString)
        {
            throw new NotImplementedException();
        }

        public string ToHex(byte b)
        {
            var hightNibble = ((b & 0xF0) >> 4);
            var lowNibble = b & 0x0F;

            return $"{ NibbleToStringTable[hightNibble] }{ NibbleToStringTable[lowNibble] }";
        }
    }
}
