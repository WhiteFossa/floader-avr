using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;

namespace LibIntelHex.Implementations
{
    public class ChecksumProcessor : IChecksumProcessor
    {
        public byte CalculateChecksum(List<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte checksum = 0;
            foreach (var dataByte in data)
            {
                checksum += dataByte;
            }

            return (byte)((0 - checksum) & 0xFF);
        }

        public bool VerifyChecksum(List<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte checksum = 0;
            foreach (var dataByte in data)
            {
                checksum += dataByte;
            }

            return checksum == 0;
        }
    }
}
