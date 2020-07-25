using LibFloaderClient.Models.DAO;

namespace LibFloaderClient.Interfaces.DAO
{
    /// <summary>
    /// DAO interface
    /// </summary>
    public interface IDao
    {
        /// <summary>
        /// Returns vendor data for given ID or null if vendor not found.
        /// </summary>
        VendorDBO GetVendorData(int vendorId);
    }
}