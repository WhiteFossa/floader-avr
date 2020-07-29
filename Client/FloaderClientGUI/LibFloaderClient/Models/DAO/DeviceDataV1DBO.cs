namespace LibFloaderClient.Models.DAO
{
    /// <summary>
    /// Information about device inner constitution for V1 protocol
    /// </summary>
    public class DeviceDataV1DBO
    {
        /// <summary>
        /// Vendor ID
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Model ID
        /// </summary>
        public int ModelId { get; set; }

        /// <summary>
        /// Total amount of device FLASH pages
        /// </summary>
        public int FlashPagesAll { get; set; }

        /// <summary>
        /// Amount of writeable (lower) device FLASH pages
        /// </summary>
        public int FlashPagesWriteable { get; set; }

        /// <summary>
        /// FLASH page size in bytes
        /// </summary>
        public int FlashPageSize { get; set; }

        /// <summary>
        /// EEPROM size in bytes
        /// </summary>
        public int EepromSize { get; set; }
    }
}
