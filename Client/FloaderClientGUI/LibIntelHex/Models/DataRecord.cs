using LibIntelHex.Enums;
using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;

namespace LibIntelHex.Models
{
    /// <summary>
    /// Record with data.
    /// </summary>
    public class DataRecord : RecordBase
    {
        /// <summary>
        /// Constructs data record from base record and trying to validate it
        /// </summary>
        /// <param name="baseRecord"></param>
        /// <param name="formatter"></param>
        public DataRecord(RecordBase baseRecord, IRecordFormatter formatter = null) : base(baseRecord.Address, baseRecord.Type, baseRecord.Data, formatter)
        {
            if (!Validate(baseRecord))
            {
                throw new ArgumentException("Given base record is not valid Data record.", nameof(baseRecord));
            }
        }

        public DataRecord(int address, List<byte> data, IRecordFormatter recordFormatter) : base(address, RecordType.Data, data, recordFormatter)
        {

        }

        /// <summary>
        /// Returns true if giben base record is a valid Data Record.
        /// </summary>
        /// <param name="recordBase"></param>
        /// <returns></returns>
        public static bool Validate(RecordBase recordBase)
        {
            if (recordBase.Type != RecordType.Data)
            {
                return false;
            }

            return true;
        }
    }
}
