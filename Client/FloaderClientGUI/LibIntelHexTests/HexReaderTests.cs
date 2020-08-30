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
            _hexReader = new HexReader();
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
        }

        /// <summary>
        /// Testing for too short file detection
        /// </summary>
        [Test]
        public void TestForTooShortData()
        {
            var hexContent = @":1000000009C00EC00DC00CC00BC00AC009C008C09A
";
            Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
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
            Assert.Throws<ArgumentException>(() => _hexReader.ReadFromString(hexContent));
        }
    }
}
