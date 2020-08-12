using LibIntelHex.Implementations;
using LibIntelHex.Interfaces;
using NUnit.Framework;
using System;

namespace LibIntelHexTests
{
    public class Tests
    {
        private IBytesReaderWriter _bytesReaderWriter;

        [SetUp]
        public void Setup()
        {
            _bytesReaderWriter = new BytesReaderWriter();
        }

        /// <summary>
        /// Testing byte to hex
        /// </summary>
        [TestCase("00", 0)]
        [TestCase("01", 1)]
        [TestCase("0A", 10)]
        [TestCase("7F", 127)]
        [TestCase("80", 128)]
        [TestCase("C8", 200)]
        [TestCase("FF", 255)]
        public void TestByteToHex(string expected, byte b)
        {
            Assert.AreEqual(expected, _bytesReaderWriter.ToHex(b));
        }

        /// <summary>
        /// Testing hex to byte on normal data
        /// </summary>
        [TestCase(0, "00")]
        [TestCase(1, "01")]
        [TestCase(10, "0A")]
        [TestCase(127, "7F")]
        [TestCase(127, "7f")]
        [TestCase(128, "80")]
        [TestCase(200, "C8")]
        [TestCase(255, "fF")]
        public void TextHexToByteNormal(byte expected, string line)
        {
            Assert.AreEqual(expected, _bytesReaderWriter.FromHex(line));
        }

        /// <summary>
        /// Feeding FromHex with incorrect strings
        /// </summary>
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("OwO")]
        [TestCase("UU")]
        public void TestHexToByteWrongInput(string line)
        {
            Assert.Throws<ArgumentException>(() => _bytesReaderWriter.FromHex(line));
        }
    }
}