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
    public class ChecksumProcessorTests
    {
        private IChecksumProcessor _checksumProcessor;

        [SetUp]
        public void SetUp()
        {
            _checksumProcessor = new ChecksumProcessor();
        }

        /// <summary>
        /// Testing checksum generation
        /// </summary>
        [TestCaseSource(nameof(TestChecksumCalculationSource))]
        public void TestChecksumCalculation(Tuple<List<byte>, byte> testCase)
        {
            Assert.AreEqual(testCase.Item2, _checksumProcessor.CalculateChecksum(testCase.Item1));
        }

        public static IEnumerable<Tuple<List<byte>, byte>> TestChecksumCalculationSource()
        {
            yield return new Tuple<List<byte>, byte>(new List<byte>() { 0x00, 0x00, 0x00, 0x01 }, 0xFF);

            yield return new Tuple<List<byte>, byte>(new List<byte>() { 0x10, 0x00, 0x20, 0x00, 0xEF, 0xCF, 0xB8, 0x9A, 0xC0, 0x98, 0x2F, 0xEF,
                0x8D, 0xEE, 0x92, 0xE0, 0x21, 0x50, 0x80, 0x40 }, 0x2C);
        }

        /// <summary>
        /// Testing checksum verification
        /// </summary>
        [TestCaseSource(nameof(TestChecksumVerificationSource))]
        public void TestChecksumVerification(Tuple<List<byte>, bool> testCase)
        {
            Assert.AreEqual(testCase.Item2, _checksumProcessor.VerifyChecksum(testCase.Item1));
        }

        public static IEnumerable<Tuple<List<byte>, bool>> TestChecksumVerificationSource()
        {
            yield return new Tuple<List<byte>, bool>(new List<byte>() { 0x00, 0x00, 0x00, 0x01, 0xFF }, true);

            yield return new Tuple<List<byte>, bool>(new List<byte>() { 0x10, 0x00, 0x20, 0x00, 0xEF, 0xCF, 0xB8, 0x9A, 0xC0, 0x98, 0x2F, 0xEF,
                0x8D, 0xEE, 0x92, 0xE0, 0x21, 0x50, 0x80, 0x40, 0x2C }, true);

            yield return new Tuple<List<byte>, bool>(new List<byte>() { 0x10, 0x20, 0x20, 0x00, 0xEF, 0xCF, 0xB8, 0x9A, 0xC0, 0x98, 0x2F, 0xEF,
                0x8D, 0xEE, 0x92, 0xE0, 0x21, 0x50, 0x80, 0x40, 0x2C }, false);
        }
    }
}
