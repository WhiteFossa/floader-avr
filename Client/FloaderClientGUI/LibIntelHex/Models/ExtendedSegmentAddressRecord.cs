using LibIntelHex.Enums;
using LibIntelHex.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibIntelHex.Models
{
    public class ExtendedSegmentAddressRecord : RecordBase
    {
        /// <summary>
        /// ESA record must have this data length
        /// </summary>
        private const int ExpectedDataLength = 2;

        public ExtendedSegmentAddressRecord(IRecordFormatter recordFormatter) : base(0, RecordType.ExtendedSegmentAddress, new List<byte>(), recordFormatter)
        {

        }

        /// <summary>
        /// Constructs ExtendedSegmentAddressRecord from base record and trying to validate it
        /// </summary>
        public ExtendedSegmentAddressRecord(RecordBase baseRecord, IRecordFormatter formatter = null) : base(baseRecord.Address, baseRecord.Type, baseRecord.Data, formatter)
        {
            if (!Validate(baseRecord))
            {
                throw new ArgumentException("Given base record is not a Extended Segment Address record.", nameof(baseRecord));
            }
        }

        /// <summary>
        /// Returns true if this base record is a valid End of File record.
        /// </summary>
        public static bool Validate(RecordBase baseRecord)
        {
            if (baseRecord.Type != RecordType.ExtendedSegmentAddress)
            {
                return false;
            }

            if (baseRecord.DataLength != ExpectedDataLength)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Returns segment base address from record
        /// </summary>
        /// <returns></returns>
        public int GetSegmentBaseAddress()
        {
            return Address << 4; // As in Intel HEX specification
        }

    }
}
