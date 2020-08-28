using LibIntelHex.Implementations.Helpers;
using LibIntelHex.Implementations.Writer;
using LibIntelHex.Interfaces;
using LibIntelHex.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LibIntelHex.Implementations
{
    public class HexWriter : IHexWriter
    {
        /// <summary>
        /// Split blocks into records of this length
        /// </summary>
        private const int HexFileRecordLenght = 16;

        private readonly IRecordFormatter _recordFormatter;

        /// <summary>
        /// Here we stored sorted data bytes
        /// </summary>
        private SortedDictionary<int, byte> _sortedData = new SortedDictionary<int, byte>();

        /// <summary>
        /// Data blocks
        /// </summary>
        private List<DataBlock> _dataBlocks = new List<DataBlock>();

        public HexWriter(IRecordFormatter recordFormatter)
        {
            _recordFormatter = recordFormatter;
        }

        public void AddByte(int address, byte data)
        {
            ValidationHelper.ValidateAddress(address);

            try
            {
                _sortedData.Add(address, data);
            }
            catch (ArgumentException ex)
            {
                throw new InvalidOperationException($"Byte with address { address } was already added to writer.");
            }
        }

        public void LoadFromList(int baseAddress, List<byte> data)
        {
            ValidationHelper.ValidateAddress(baseAddress);
            Reset();

            var address = baseAddress;
            foreach(var dataByte in data)
            {
                AddByte(address, dataByte);
                address ++;
            }
        }

        public void Reset()
        {
            _sortedData.Clear();
            _dataBlocks.Clear();
        }

        public override string ToString()
        {
            // Stage 1 - Growing data blocks
            foreach (var pair in _sortedData)
            {
                var dataAddress = pair.Key;
                var dataByte = pair.Value;

                // Can we append?
                var blocksToAppend = _dataBlocks
                    .Where(db => db.IsByteAppendable(dataAddress));

                if (blocksToAppend.Any())
                {
                    blocksToAppend
                        .First()
                        .AppendByte(dataAddress, dataByte);

                    continue;
                }

                // Creating new block
                _dataBlocks.Add(new DataBlock(dataAddress, dataByte));
            }

            // Stage 2 - Generating data records
            // Because input data is sorted, blocks are sorted too
            var dataRecords = new List<DataRecord>();
            foreach (var block in _dataBlocks)
            {
                // Splitting block data into records
                var sequences = block.Data
                    .Select((b, i) => new { Index = i, Value = b })
                    .GroupBy(el => el.Index / HexFileRecordLenght)
                    .Select(el => el.Select(v => v.Value).ToList())
                    .ToList();

                var recordBaseAddress = block.BaseAddress;
                foreach(var sequence in sequences)
                {
                    dataRecords.Add(new DataRecord(recordBaseAddress, sequence, _recordFormatter));

                    recordBaseAddress += sequence.Count();
                }
            }

            // Stage 3 - Writing records to string
            var sb = new StringBuilder();

            foreach (var rec in dataRecords)
            {
                sb.Append(rec.ToString());
            }

            // EoF
            sb.Append(new EndOfFileRecord(_recordFormatter).ToString());

            return sb.ToString();
        }

        public void WriteToFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path must not be null or empty", nameof(path));
            }

            File.WriteAllText(path, ToString());
        }
    }
}
