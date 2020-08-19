using LibIntelHex.Implementations.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibIntelHex.Implementations.Writer
{
    /// <summary>
    /// One data block to write to file
    /// </summary>
    public class DataBlock
    {
        /// <summary>
        /// Block base address
        /// </summary>
        public int BaseAddress { get; private set; }

        /// <summary>
        /// Sequential data in block
        /// </summary>
        public List<byte> Data { get; private set; }

        /// <summary>
        /// Creates data block with a first byte in it
        /// </summary>
        public DataBlock(int address, byte data)
        {
            ValidationHelper.ValidateAddress(address);

            BaseAddress = address;
            Data = new List<byte>() { data };
        }

        /// <summary>
        /// Checks, can byte with this address be appended to the end of the block
        /// </summary>
        public bool IsByteAppendable(int address)
        {
            ValidationHelper.ValidateAddress(address);

            return address == BaseAddress + Data.Count;
        }

        /// <summary>
        /// As IsByteAppendable(), but checks can byte be prepended to the beginning of the block
        /// </summary>
        public bool IsBytePrependable(int address)
        {
            ValidationHelper.ValidateAddress(address);

            return BaseAddress != 0 && address == BaseAddress - 1;
        }

        /// <summary>
        /// Attempts to append a byte to the end ot data block
        /// </summary>
        public void AppendByte(int address, byte data)
        {
            if (!IsByteAppendable(address))
            {
                throw new ArgumentException($"Byte with address { address } can't be appended.", nameof(address));
            }

            Data.Add(data);
        }

        /// <summary>
        /// As AppendByte(), but to prepend byte
        /// </summary>
        public void PrependByte(int address, byte data)
        {
            if (!IsBytePrependable(address))
            {
                throw new ArgumentException($"Byte with address { address } can't be prepended.", nameof(address));
            }

            Data = Data.Prepend<byte>(data).ToList();
            BaseAddress --;
        }

        /// <summary>
        /// Appends new block (if possible) to the end of current one
        /// </summary>
        public void AppendBlock(DataBlock block)
        {
            if (!IsByteAppendable(block.BaseAddress))
            {
                throw new ArgumentException($"Block with base address { block.BaseAddress } can't be appended.", nameof(block.BaseAddress));
            }

            Data.AddRange(block.Data);
        }
    }
}
