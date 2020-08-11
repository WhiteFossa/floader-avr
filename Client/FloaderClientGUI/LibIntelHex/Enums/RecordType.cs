namespace LibIntelHex.Enums
{
    /// <summary>
    /// Possible (not all of them are supported by this library) record types
    /// </summary>
    public enum RecordType
    {
        Data = 0,

        EndOfFile = 1,

        ExtendedSegmentAddress = 2,

        StartSegmentAddress = 3,

        ExtendedLinearAddress = 4,

        StartLinearAddress = 5
    }
}
