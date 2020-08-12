using LibIntelHex.Implementations;
using LibIntelHex.Interfaces;
using NUnit.Framework;

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
        /// <param name="expected"></param>
        /// <param name="b"></param>
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
    }
}