using LibIntelHex.Implementations;
using LibIntelHex.Interfaces;
using LibIntelHex.Models;
using NUnit.Framework;
using System;

namespace LibIntelHexTests
{
    [TestFixture]
    public class RecordsTests
    {
        private IRecordFormatter _recordFormatter;

        [SetUp]
        public void SetUp()
        {
            _recordFormatter = new RecordFormatter(
                new ChecksumProcessor(),
                new BytesReaderWriter());
        }

        [Test]
        public void TestEndOfFileRecordGeneration()
        {
            var record = new EndOfFileRecord(_recordFormatter);
            Assert.AreEqual($":00000001FF{ Environment.NewLine }", record.ToString());
        }
    }
}
