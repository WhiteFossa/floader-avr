using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHex.Implementations
{
    public class BytesReaderWriter : IBytesReaderWriter
    {
        /// <summary>
        /// One byte is encoded as two characters
        /// </summary>
        private const int ByteLengthInChars = 2;

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
            if (string.IsNullOrEmpty(hexString) || hexString.Length != ByteLengthInChars)
            {
                throw new ArgumentException(nameof(hexString));
            }

            var upcasedStr = hexString.ToUpper();
            var highChar = upcasedStr.Substring(0, 1);
            var lowChar = upcasedStr.Substring(1, 1);

            try
            {
                return (byte)(((StringToNibbleTable[highChar] << 4) + StringToNibbleTable[lowChar]) & 0xFF);
            }
            catch (KeyNotFoundException)
            {
                throw new ArgumentException(hexString);
            }
        }

        public string ToHex(byte b)
        {
            var highNibble = ((b & 0xF0) >> 4);
            var lowNibble = b & 0x0F;

            return $"{ NibbleToStringTable[highNibble] }{ NibbleToStringTable[lowNibble] }";
        }
    }
}
