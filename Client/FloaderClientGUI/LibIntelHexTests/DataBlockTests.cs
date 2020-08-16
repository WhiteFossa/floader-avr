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
    }
}
