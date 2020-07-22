using LibFloaderClient.Interfaces.DAO;

namespace LibFloaderClient.Implementations.DAO
{
    public class Dao : IDao
    {
        public string GetVendorName(int vendorId)
        {
            return "Fossa";
        }
    }
}