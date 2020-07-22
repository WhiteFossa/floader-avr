namespace LibFloaderClient.Interfaces.DAO
{
    /// <summary>
    /// DAO interface
    /// </summary>
    public interface IDao
    {
        /// <summary>
        /// Returns vendor name for given ID or null if vendor not found.
        /// </summary>
        string GetVendorName(int vendorId);
    }
}