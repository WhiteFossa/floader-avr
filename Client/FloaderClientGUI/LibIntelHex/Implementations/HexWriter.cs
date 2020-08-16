using LibIntelHex.Implementations.Helpers;
using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace LibIntelHex.Implementations
{
    public class HexWriter : IHexWriter
    {
        

        /// <summary>
        /// Here we stored sorted data bytes
        /// </summary>
        private SortedDictionary<int, byte> _sortedData = new SortedDictionary<int, byte>();

        public void AddByte(int address, byte data)
        {
            ValidationHelper.ValidateAddress(address);

            try
            {
                _sortedData.Add(address, data);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Byte with address { address } was already added to writer.");
            }

            throw new NotImplementedException();
        }

        public void Reset()
        {
            _sortedData.Clear();
        }
    }
}
