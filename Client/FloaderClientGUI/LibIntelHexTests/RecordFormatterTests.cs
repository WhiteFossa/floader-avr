using LibIntelHex.Implementations;
using LibIntelHex.Interfaces;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHexTests
{
    [TestFixture]
    public class RecordFormatterTests
    {
        private IRecordFormatter _recordFormatter;

        [SetUp]
        public void SetUp()
        {
            _recordFormatter = new RecordFormatter(
                new ChecksumProcessor(),
                new BytesReaderWriter());
        }

        /// <summary>
        /// Testing raw record formatting
        /// </summary>
        [TestCaseSource(nameof(TestRawRecordFormattingSource))]
        public void TestRawRecordFormatting(Tuple<List<byte>, string> testCase)
        {
            Assert.AreEqual(testCase.Item2, _recordFormatter.FormatRawRecord(testCase.Item1));
        }

        public static IEnumerable<Tuple<List<byte>, string>> TestRawRecordFormattingSource()
        {
            yield return new Tuple<List<byte>, string>(new List<byte>() { 0x00, 0x00, 0x00, 0x01 }, $":00000001FF{ Environment.NewLine }");
            yield return new Tuple<List<byte>, string>(new List<byte>() { 0x10, 0x00, 0x40, 0x00, 0x21, 0x50, 0x80, 0x40, 0x90, 0x40, 0xE1, 0xF7, 0x00, 0xC0, 0x00, 0x00, 0xEB, 0xCF, 0xF8, 0x94 },
                $":10004000215080409040E1F700C00000EBCFF894D1{ Environment.NewLine }");
        }


    }
}
