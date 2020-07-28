namespace LibFloaderClient.Models.DAO
{
    /// <summary>
    /// Device name - database object
    /// </summary>
    public class DeviceNameDBO
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
        /// Device model name
        /// </summary>
        public string Name { get; set; }
    }
}
