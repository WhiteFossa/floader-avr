using LibIntelHex.Implementations.Writer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibIntelHexTests
{
    /// <summary>
    /// Tests for data block (which is internal library stuff)
    /// </summary>
    [TestFixture]
    public class DataBlockTests
    {
        /// <summary>
        /// Incorrect addresses
        /// </summary>
        [TestCase(-1)]
        [TestCase(65536)]
        public void TestAddressIncorrectAddresses(int address)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new DataBlock(address, 0));
        }

        /// <summary>
        /// Correct addresses and data check
        /// </summary>
        /// <param name="address"></param>
        [TestCase(0, 0xA5)]
        [TestCase(123, 0xFF)]
        [TestCase(65535, 0x00)]
        public void TestAddressCorrectAddresses(int address, byte data)
        {
            var block = new DataBlock(address, data);
            Assert.AreEqual(address, block.BaseAddress);
            Assert.AreEqual(data, block.Data.First());
        }

        /// <summary>
        /// Checks if byte with address 2 can be appended to byte with address 1
        /// </summary>
        [TestCase(0, 1, true)]
        [TestCase(1, 1, false)]
        [TestCase(1, 0, false)]
        [TestCase(65534, 65535, true)]
        public void CheckForIsAppendable(int address1, int address2, bool isAppendable)
        {
            var block = new DataBlock(address1, 0x00);
            Assert.AreEqual(isAppendable, block.IsByteAppendable(address2));
        }

        /// <summary>
        /// As CheckForIsAppendable(), but for byte prepending
        /// </summary>
        [TestCase(0, 1, false)]
        [TestCase(1, 1, false)]
        [TestCase(1, 0, true)]
        [TestCase(65535, 65534, true)]
        public void CheckForIsPrependable(int address1, int address2, bool isPrependable)
        {
            var block = new DataBlock(address1, 0x00);
            Assert.AreEqual(isPrependable, block.IsBytePrependable(address2));
        }
    }
}
