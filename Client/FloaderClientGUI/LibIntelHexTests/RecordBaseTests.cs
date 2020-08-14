using LibIntelHex.Enums;
using LibIntelHex.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibIntelHexTests
{
    /// <summary>
    /// Tests for base records class
    /// </summary>
    [TestFixture]
    public class RecordBaseTests
    {
        /// <summary>
        /// Testing record conversion into bytes list
        /// </summary>
        [TestCaseSource(nameof(TestToDataListConversionSource))]
        public void TestToDataListConversion(Tuple<int, RecordType, List<byte>, List<byte>> testCase)
        {
            var record = new RecordBase(testCase.Item1, testCase.Item2, testCase.Item3);
            Assert.AreEqual(true, record
                .ToDataList()
                .SequenceEqual(testCase.Item4));
        }

        public static IEnumerable<Tuple<int, RecordType, List<byte>, List<byte>>> TestToDataListConversionSource()
        {
            yield return new Tuple<int, RecordType, List<byte>, List<byte>>(
                0x0, // Address
                RecordType.EndOfFile, // Type
                new List<byte>(), // Empty data for End of File
                new List<byte>() { 0x00, 0x00, 0x00, 0x01 } // Expected result
                );

            yield return new Tuple<int, RecordType, List<byte>, List<byte>>(
                0x0040,
                RecordType.Data,
                new List<byte>() { 0x21, 0x50, 0x80, 0x40, 0x90, 0x40, 0xE1, 0xF7, 0x00, 0xC0, 0x00, 0x00, 0xEB, 0xCF, 0xF8, 0x94 },
                new List<byte>() { 0x10, 0x00, 0x40, 0x00, 0x21, 0x50, 0x80, 0x40, 0x90, 0x40, 0xE1, 0xF7, 0x00, 0xC0, 0x00, 0x00, 0xEB, 0xCF, 0xF8, 0x94 }
                );

            yield return new Tuple<int, RecordType, List<byte>, List<byte>>(
                0xA55A,
                RecordType.Data,
                new List<byte>() { 0x21, 0x50, 0x80, 0x40, 0x90, 0x40, 0xE1, 0xF7, 0x00, 0xC0, 0x00, 0x00, 0xEB, 0xCF, 0xF8, 0x94 },
                new List<byte>() { 0x10, 0xA5, 0x5A, 0x00, 0x21, 0x50, 0x80, 0x40, 0x90, 0x40, 0xE1, 0xF7, 0x00, 0xC0, 0x00, 0x00, 0xEB, 0xCF, 0xF8, 0x94 }
                );
        }
    }
}
