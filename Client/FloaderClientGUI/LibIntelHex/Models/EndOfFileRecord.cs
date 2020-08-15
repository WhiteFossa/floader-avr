using LibIntelHex.Enums;
using LibIntelHex.Interfaces;
using System.Collections.Generic;

namespace LibIntelHex.Models
{
    /// <summary>
    /// Record for End of File
    /// </summary>
    public class EndOfFileRecord : RecordBase
    {
        public EndOfFileRecord(IRecordFormatter recordFormatter) : base(0, RecordType.EndOfFile, new List<byte>(), recordFormatter)
        {

        }
    }
}
