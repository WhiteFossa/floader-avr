using LibIntelHex.Implementations;
using LibIntelHex.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHexTests
{
    /// <summary>
    /// Tests for checksumm processor
    /// </summary>
    [TestFixture]
    public class ChecksummProcessorTests
    {
        private IChecksummProcessor _checksummProcessor;

        [SetUp]
        public void SetUp()
        {
            _checksummProcessor = new ChecksummProcessor();
        }

        /// <summary>
        /// Testing checksum generation
        /// </summary>
        /// <param name="testCase"></param>
        [TestCaseSource(nameof(TestChecksumCalculationSource))]
        public void TestChecksumCalculation(Tuple<List<byte>, byte> testCase)
        {
            Assert.AreEqual(testCase.Item2, _checksummProcessor.CalculateChecksumm(testCase.Item1));
        }

        public static IEnumerable<Tuple<List<byte>, byte>> TestChecksumCalculationSource()
        {
            yield return new Tuple<List<byte>, byte>(new List<byte>() { 0x00, 0x00, 0x00, 0x01 }, 0xFF);
            yield return new Tuple<List<byte>, byte>(new List<byte>() { 0x10, 0x00, 0x20, 0x00, 0xEF, 0xCF, 0xB8, 0x9A, 0xC0, 0x98, 0x2F, 0xEF,
                0x8D, 0xEE, 0x92, 0xE0, 0x21, 0x50, 0x80, 0x40 }, 0x2C);
        }
    }
}
