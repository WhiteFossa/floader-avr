namespace LibFloaderClient.Implementations.Enums.Device
{
    /// <summary>
    /// Device identification status
    /// </summary>
    public enum DeviceIdentificationStatus
    {
        /// <summary>
        /// Device successfully identified
        /// </summary>
        Identified,

        /// <summary>
        /// Device answer indicates that it's not our bootloader
        /// </summary>
        WrongSignature,

        /// <summary>
        /// Timeout while awaiting for response
        /// </summary>
        Timeout
    }
}