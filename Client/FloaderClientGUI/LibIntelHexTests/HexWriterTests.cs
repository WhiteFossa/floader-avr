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

using LibIntelHex.Implementations;
using LibIntelHex.Interfaces;
using NUnit.Framework;
using System.Collections.Generic;

namespace LibIntelHexTests
{
    /// <summary>
    /// Tests for Intel HEX writer
    /// </summary>
    [TestFixture]
    public class HexWriterTests
    {
        private IHexWriter _hexWriter;


        [SetUp]
        public void SetUp()
        {
            _hexWriter = new HexWriter(
                new RecordFormatter(
                    new ChecksumProcessor(),
                    new BytesReaderWriter()));
        }

        /// <summary>
        /// Data blocks to file content
        /// </summary>
        [Test]
        public void TestDataBlocksWriteToString()
        {
            _hexWriter.Reset();

            // Two fuseable data sequences
            _hexWriter.AddByte(0, 0x09);
            _hexWriter.AddByte(1, 0xC0);
            _hexWriter.AddByte(2, 0x0E);
            _hexWriter.AddByte(3, 0xC0);
            _hexWriter.AddByte(4, 0x0D);
            _hexWriter.AddByte(5, 0xC0);
            _hexWriter.AddByte(6, 0x0C);
            _hexWriter.AddByte(7, 0xC0);
            _hexWriter.AddByte(8, 0x0B);
            _hexWriter.AddByte(9, 0xC0);
            _hexWriter.AddByte(10, 0x0A);
            _hexWriter.AddByte(11, 0xC0);
            _hexWriter.AddByte(12, 0x09);
            _hexWriter.AddByte(13, 0xC0);
            _hexWriter.AddByte(14, 0x08);
            _hexWriter.AddByte(15, 0xC0);

            // Reverse direction
            _hexWriter.AddByte(31, 0xC0);
            _hexWriter.AddByte(30, 0x17);
            _hexWriter.AddByte(29, 0xD0);
            _hexWriter.AddByte(28, 0x02);
            _hexWriter.AddByte(27, 0xBF);
            _hexWriter.AddByte(26, 0xCD);
            _hexWriter.AddByte(25, 0xE9);
            _hexWriter.AddByte(24, 0xCF);
            _hexWriter.AddByte(23, 0xBE);
            _hexWriter.AddByte(22, 0x1F);
            _hexWriter.AddByte(21, 0x24);
            _hexWriter.AddByte(20, 0x11);
            _hexWriter.AddByte(19, 0xC0);
            _hexWriter.AddByte(18, 0x06);
            _hexWriter.AddByte(17, 0xC0);
            _hexWriter.AddByte(16, 0x07);

            // Non-fuseable sequence
            _hexWriter.AddByte(48, 0x90);
            _hexWriter.AddByte(49, 0x40);
            _hexWriter.AddByte(50, 0xE1);
            _hexWriter.AddByte(51, 0xF7);
            _hexWriter.AddByte(52, 0x00);
            _hexWriter.AddByte(53, 0xC0);
            _hexWriter.AddByte(54, 0x00);
            _hexWriter.AddByte(55, 0x00);
            _hexWriter.AddByte(56, 0xC0);
            _hexWriter.AddByte(57, 0x9A);
            _hexWriter.AddByte(58, 0x2F);
            _hexWriter.AddByte(59, 0xEF);
            _hexWriter.AddByte(60, 0x8D);
            _hexWriter.AddByte(61, 0xE5);
            _hexWriter.AddByte(62, 0x9A);
            _hexWriter.AddByte(63, 0xE1);

            // Incomplete line
            _hexWriter.AddByte(80, 0xFF);
            _hexWriter.AddByte(81, 0xCF);


            var result = _hexWriter.ToString();

            var expectedResult = @":1000000009C00EC00DC00CC00BC00AC009C008C09A
:1000100007C006C011241FBECFE9CDBF02D017C054
:100030009040E1F700C00000C09A2FEF8DE59AE1F3
:02005000FFCFE0
:00000001FF
";

            Assert.AreEqual(expectedResult, result);
        }


        /// <summary>
        /// Test raw data list to file write
        /// </summary>
        [Test]
        public void TestListWriteToString()
        {
            var data = new List<byte>()
            {
                0x09, 0xC0, 0x0E, 0xC0, 0x0D, 0xC0, 0x0C, 0xC0, 0x0B, 0xC0, 0x0A, 0xC0, 0x09, 0xC0, 0x08, 0xC0,
                0x07, 0xC0, 0x06, 0xC0, 0x11, 0x24, 0x1F, 0xBE, 0xCF, 0xE9, 0xCD, 0xBF, 0x02, 0xD0, 0x17, 0xC0
            };

            _hexWriter.LoadFromList(0, data);

            var result = _hexWriter.ToString();

            var expectedResult = @":1000000009C00EC00DC00CC00BC00AC009C008C09A
:1000100007C006C011241FBECFE9CDBF02D017C054
:00000001FF
";

            Assert.AreEqual(expectedResult, result);
        }
    }
}
