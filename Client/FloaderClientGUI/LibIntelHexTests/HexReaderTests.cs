using LibIntelHex.Implementations;
using LibIntelHex.Implementations.Reader;
using LibIntelHex.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
