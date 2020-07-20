using LibFloaderClient.Implementations.Enums.Device;

namespace LibFloaderClient.Models.Device
{
    /// <summary>
    /// Data, identifying connected device
    /// </summary>
    public class DeviceIdentificator
    {
        /// <summary>
        /// Identification result
        /// </summary>
        public DeviceIdentificationStatus Status { get; private set; }

        /// <summary>
        /// Bootloader protocol version
        /// </summary>
        public int Version { get; private set; }

        /// <summary>
        /// Vendor ID
        /// </summary>
        public int VendorId { get; private set; }

        /// <summary>
        /// Device model ID
        /// </summary>
        public int ModelId { get; private set; }

        /// <summary>
        /// Device serial number
        /// </summary>
        public long Serial { get; private set; }

        public DeviceIdentificator(DeviceIdentificationStatus status, int version, int vendorId, int modelId, long serial)
        {
            Status = status;
            Version = version;
            VendorId = vendorId;
            ModelId = modelId;
            Serial = serial;
        }
    }
}