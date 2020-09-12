using LibIntelHex.Enums;
using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;

namespace LibIntelHex.Models
{
    /// <summary>
    /// Record for End of File
    /// </summary>
    public class EndOfFileRecord : RecordBase
    {
        /// <summary>
        /// EoF record must have this data length
        /// </summary>
        private const int ExpectedDataLength = 0;

        /// <summary>
        /// EoF record must have this address inside
        /// </summary>
        private const int ExpectedAddress = 0;

        /// <summary>
        /// Constructs EndOfFileRecord from base record and trying to validate it
        /// </summary>
        /// <param name="baseRecord"></param>
        public EndOfFileRecord(RecordBase baseRecord, IRecordFormatter formatter = null) : base(baseRecord.Address, baseRecord.Type, baseRecord.Data, formatter)
        {
            if (!Validate(baseRecord))
            {
                throw new ArgumentException("Given base record is not a End of File record.", nameof(baseRecord));
            }
        }

        public EndOfFileRecord(IRecordFormatter recordFormatter) : base(0, RecordType.EndOfFile, new List<byte>(), recordFormatter)
        {

        }

        /// <summary>
        /// Returns true if this base record is a valid End of File record.
        /// </summary>
        public static bool Validate(RecordBase baseRecord)
        {
            if (baseRecord.Type != RecordType.EndOfFile)
            {
                return false;
            }

            if (baseRecord.DataLength != ExpectedDataLength)
            {
                return false;
            }

            if (baseRecord.Address != ExpectedAddress)
            {
                return false;
            }

            return true;
        }
    }
}
