﻿/*
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
using LibIntelHex.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Testing end of file records generation
        /// </summary>
        [Test]
        public void TestEndOfFileRecordGeneration()
        {
            var record = new EndOfFileRecord(_recordFormatter);
            Assert.AreEqual($":00000001FF{ Environment.NewLine }", record.ToString());
        }

        /// <summary>
        /// Testing data records generation
        /// </summary>
        [TestCaseSource(nameof(TestDataRecordGenerationSource))]
        public void TestDataRecordGeneration(Tuple<int, List<byte>, string> testCase)
        {
            var record = new DataRecord(testCase.Item1, testCase.Item2, _recordFormatter);
            Assert.AreEqual(testCase.Item3, record.ToString());
        }

        public static IEnumerable<Tuple<int, List<byte>, string>> TestDataRecordGenerationSource()
        {
            yield return new Tuple<int, List<byte>, string>(
                0x0040,
                new List<byte>() { 0x21, 0x50, 0x80, 0x40, 0x90, 0x40, 0xE1, 0xF7, 0x00, 0xC0, 0x00, 0x00, 0xEB, 0xCF, 0xF8, 0x94 },
                $":10004000215080409040E1F700C00000EBCFF894D1{ Environment.NewLine }"
                );
        }
    }
}
