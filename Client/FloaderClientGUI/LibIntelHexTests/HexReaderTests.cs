﻿/*
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
using LibIntelHex.Implementations.Reader;
using LibIntelHex.Interfaces;
using NUnit.Framework;
using System;

namespace LibIntelHexTests
{
    /// <summary>
    /// Tests for Intel HEX reader
    /// </summary>
    [TestFixture]
    public class HexReaderTests
    {
        private IHexReader _hexReader;

        [SetUp]
        public void SetUp()
        {
            _hexReader = new HexReader(new ChecksumProcessor(), new BytesReaderWriter());
        }

        /// <summary>
        /// Testing that data are readed correctly
        /// </summary>
        [Test]
        public void TestReadFromString()
        {
            var hexContent = @":1000000009C00EC00DC00CC00BC00AC009C008C09A
:1000100007C006C011241FBECFE9CDBF02D017C054
:10002000EFCFB89AC0982FEF8DEE92E0215080402C
:100030009040E1F700C00000C09A2FEF8DE59AE1F3
:10004000215080409040E1F700C00000EBCFF894D1
:02005000FFCFE0
:00000001FF
";

            var result = _hexReader.ReadFromString(hexContent);

            Assert.AreEqual(0x09, result[0x00]); // Address 0x00, value 0x09
            Assert.AreEqual(0xC0, result[0x01]);
            Assert.AreEqual(0x07, result[0x10]);
            Assert.AreEqual(0xC0, result[0x1F]);
            Assert.AreEqual(0xFF, result[0x50]);
            Assert.AreEqual(0xCF, result[0x51]);
        }

        /// <summary>
        /// Testing that data are readed correctly (data contains ESA record)
        /// </summary>
        [Test]
        public void TestReadFromStringExtendedSegmentAddress()
        {
            var hexContent = @":020001020000FB
:100000000C94B71B18951895189518950C94C20167
:020002020000FA
:1038A000E4DF01C0DEDF069618E38E3B910748F3A4
:0400000300000000F9
:00000001FF
";

            var result = _hexReader.ReadFromString(hexContent);

            Assert.AreEqual(0x0C, result[0x10]); // Address 0x10, value 0x0C
            Assert.AreEqual(0x94, result[0x11]);
            Assert.AreEqual(0xB7, result[0x12]);

            Assert.AreEqual(0xE4, result[0x38C0]); // Address 0x38C0, value 0xE4
            Assert.AreEqual(0xDF, result[0x38C1]);
            Assert.AreEqual(0x01, result[0x38C2]);
        }

        /// <summary>
        /// Testing for too short file detection
        /// </summary>
        [Test]
        public void TestForTooShortData()
        {
            var hexContent = @":1000000009C00EC00DC00CC00BC00AC009C008C09A
";
            var ex = Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
            Assert.IsTrue(ex.Message.Contains("HEX file must contain at least two lines (DATA + EoF records)"));
        }


        /// <summary>
        /// Test for incorrect strings.
        /// </summary>
        [TestCase(@":1000000009C00EC00DC00CC00BC00AC009C008C09A
1000100007C006C011241FBECFE9CDBF02D017C054")]
        [TestCase(@":1000000009C00EC00DC00CC00BC00AC009C008C09A
:YIFF100007C006C011241FBECFE9CDBF02D017C054")]
        [TestCase(@":1000000009C00EC00DC00CC00BC00AC009C008C09A
10001000")]
        public void TestForIncorrectStrings(string hexContent)
        {
            var ex = Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
            Assert.IsTrue(ex.Message.Contains("HEX file contains incorrect line"));
        }

        /// <summary>
        /// Test for checksums verifier.
        /// </summary>
        /// <param name="hexContent"></param>
        [TestCase(@":1000000009C00EC00DC00CC00BC00AC009C008C09B
:1000100007C006C011241FBECFE9CDBF02D017C054")]
        [TestCase(@":1000000009C00EC00DC00CC10BC00AC009C008C09A
:1000100007C006C011241FBECFE9CDBF02D017C054")]
        public void TestForChecksumErrors(string hexContent)
        {
            var ex = Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
            Assert.IsTrue(ex.Message.Contains("HEX file contains line with a wrong checksum."));
        }

        /// <summary>
        /// Test for data lenght checker.
        /// </summary>
        /// <param name="hexContent"></param>
        [TestCase(@":1000000009C00EC00DC00CC00BC00AC009C008C09A
:1100100007C006C011241FBECFE9CDBF02D017C053")]
        public void TestForDataLengthErrors(string hexContent)
        {
            var ex = Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
            Assert.IsTrue(ex.Message.Contains("Wrong data length for sequence"));
        }

        /// <summary>
        /// Test for a meaningless record type.
        /// </summary>
        /// <param name="hexContent"></param>
        [TestCase(@":10004000215080409040E1F700C00000EBCFF894D1
:020000141234A4
:02005000FFCFE0")]
        public void TestForUndefinedRecordTypes(string hexContent)
        {
            var ex = Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
            Assert.IsTrue(ex.Message.Contains("Undefined record type for sequence"));
        }

        /// <summary>
        /// Test for not allowed records.
        /// </summary>
        /// <param name="hexContent"></param>
        [TestCase(@":10004000215080409040E1F700C00000EBCFF894D1
:020000041234B4
:02005000FFCFE0")]
        public void TestForNotAllowedRecords(string hexContent)
        {
            var ex = Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
            Assert.IsTrue(ex.Message.Contains("HEX file contains not allowed record with type"));
        }

        /// <summary>
        /// Test for missing/duplicated EoF record
        /// </summary>
        /// <param name="hexContent"></param>
        [TestCase(@":10004000215080409040E1F700C00000EBCFF894D1
:1000100007C006C011241FBECFE9CDBF02D017C054")]
        [TestCase(@":10004000215080409040E1F700C00000EBCFF894D1
:00000001FF
:00000001FF")]
        public void TestForMissingOrDuplicatedEoFs(string hexContent)
        {
            var ex = Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
            Assert.IsTrue(ex.Message.Contains("HEX file must contain one and only one End of File record."));
        }

        /// <summary>
        /// Test for EoF position
        /// </summary>
        /// <param name="hexContent"></param>
        [TestCase(@":00000001FF
:10004000215080409040E1F700C00000EBCFF894D1")]
        public void TestForEoFPosition(string hexContent)
        {
            var ex = Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
            Assert.IsTrue(ex.Message.Contains("HEX file must end with End of File record."));
        }
    }
}
