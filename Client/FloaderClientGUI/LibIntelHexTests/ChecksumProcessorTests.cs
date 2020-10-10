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
using System;
using System.Collections.Generic;

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
