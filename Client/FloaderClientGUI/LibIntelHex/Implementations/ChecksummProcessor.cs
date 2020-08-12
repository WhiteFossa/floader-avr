using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHex.Implementations
{
    public class ChecksummProcessor : IChecksummProcessor
    {
        public byte CalculateChecksumm(List<byte> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            byte checksumm = 0;
            foreach (var dataByte in data)
            {
                checksumm += dataByte;
            }

            return (byte)((0 - checksumm) & 0xFF);
        }

        public bool CheckChecksumm(List<byte> data)
        {
            throw new NotImplementedException();
        }
    }
}
