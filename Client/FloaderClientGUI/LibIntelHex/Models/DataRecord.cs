using LibIntelHex.Enums;
using LibIntelHex.Interfaces;
using System.Collections.Generic;

namespace LibIntelHex.Models
{
    /// <summary>
    /// Record with data.
    /// </summary>
    public class DataRecord : RecordBase
    {
        public DataRecord(int address, List<byte> data, IRecordFormatter recordFormatter) : base(address, RecordType.Data, data, recordFormatter)
        {

        }
    }
}
