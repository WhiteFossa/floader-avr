/*
                    Fossa's AVR bootloader client
Copyright (C) 2020 White Fossa aka Artyom Vetrov <whitefossa@protonmail.com>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using LibIntelHex.Implementations.Writer;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void TestIsAppendable(int address1, int address2, bool isAppendable)
        {
            var block = new DataBlock(address1, 0x00);
            Assert.AreEqual(isAppendable, block.IsByteAppendable(address2));
        }

        /// <summary>
        /// Test for bytes append
        /// </summary>
        [TestCaseSource(nameof(TestByteAppendSource))]
        public void TestByteAppned(Tuple<DataBlock, int, byte, int, List<byte>> testCase)
        {
            testCase.Item1.AppendByte(testCase.Item2, testCase.Item3);
            Assert.AreEqual(testCase.Item1.BaseAddress, testCase.Item4);
            Assert.AreEqual(true, testCase.Item1.Data.SequenceEqual(testCase.Item5));
        }

        public static IEnumerable<Tuple<DataBlock, int, byte, int, List<byte>>> TestByteAppendSource()
        {
            yield return new Tuple<DataBlock, int, byte, int, List<byte>>(
                new DataBlock(0, 0xA5),
                1,
                0x5A,
                0,
                new List<byte>() { 0xA5, 0x5A });

            yield return new Tuple<DataBlock, int, byte, int, List<byte>>(
                new DataBlock(65534, 0xA5),
                65535,
                0x5A,
                65534,
                new List<byte>() { 0xA5, 0x5A });
        }

        /// <summary>
        /// Test for incorrect byte address detection during append
        /// </summary>
        [Test]
        public void TestByteAppendError()
        {
            Assert.Throws<ArgumentException>(() => (new DataBlock(0, 0x5A)).AppendByte(2, 0xA5));
        }
    }
}
