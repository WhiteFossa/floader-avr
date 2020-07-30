namespace LibFloaderClient.Models.Device.Versioned
{
    /// <summary>
    /// Information about device inner constitution for V1 protocol
    /// </summary>
    public class DeviceDataV1
    {
        /// <summary>
        /// Vendor ID
        /// </summary>
        public int VendorId { get; }

        /// <summary>
        /// Model ID
        /// </summary>
        public int ModelId { get; }

        /// <summary>
        /// Total amount of device FLASH pages
        /// </summary>
        public int FlashPagesAll { get; }

        /// <summary>
        /// Amount of writeable (lower) device FLASH pages
        /// </summary>
        public int FlashPagesWriteable { get; }

        /// <summary>
        /// FLASH page size in bytes
        /// </summary>
        public int FlashPageSize { get; }

        /// <summary>
        /// EEPROM size in bytes
        /// </summary>
        public int EepromSize { get; }

        public DeviceDataV1(int vendorId, int modelId, int flashPagesAll, int flashPagesWriteable, int flashPageSize, int eepromSize)
        {
            VendorId = vendorId;
            ModelId = modelId;
            FlashPagesAll = flashPagesAll;
            FlashPagesWriteable = flashPagesWriteable;
            FlashPageSize = flashPageSize;
            EepromSize = eepromSize;
        }
    }
}
