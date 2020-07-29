using LibFloaderClient.Models.DAO;

namespace LibFloaderClient.Interfaces.DAO
{
    /// <summary>
    /// DAO interface
    /// </summary>
    public interface IDao
    {
        /// <summary>
        /// Returns vendor name data for given ID
        /// </summary>
        VendorDBO GetVendorNameData(int vendorId);

        /// <summary>
        /// Get human-readable device model name
        /// </summary>
        DeviceNameDBO GetDeviceNameData(int vendorId, int modelId);

        /// <summary>
        /// Get device data for V1 protocol
        /// </summary>
        DeviceDataV1DBO GetDeviceDataV1(int vendorId, int modelId);
    }
}