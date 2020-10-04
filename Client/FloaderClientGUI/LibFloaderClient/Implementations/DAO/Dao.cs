using Dapper;
using LibFloaderClient.Implementations.Helpers;
using LibFloaderClient.Interfaces.DAO;
using LibFloaderClient.Models.DAO;
using LibFloaderClient.Models.DAO.Versioned.V1;
using System;
using System.Data.SQLite;

namespace LibFloaderClient.Implementations.DAO
{
    public class Dao : IDao
    {
        /// <summary>
        /// Devices database name
        /// </summary>
        private const string DbFilename = "DevDB.sqlite";

        /// <summary>
        /// Current assembly
        /// </summary>
        private readonly Type CurrentAssembly = typeof(LibFloaderClient.Implementations.DAO.Dao);

        /// <summary>
        /// Returns ready to use device DB connection
        /// </summary>
        private SQLiteConnection GetDbConnection()
        {
            return new SQLiteConnection($"Data Source={ DbFilename }");
        }

        public VendorDBO GetVendorNameData(int vendorId)
        {
            using (var connection = GetDbConnection())
            {
                connection.Open();

                return connection
                    .QueryFirstOrDefault<VendorDBO>(
                        ResourcesHelper.GetResourceAsString(CurrentAssembly, "LibFloaderClient.Implementations.DAO.Queries.GetVendorNameData.sql"),
                        new
                        {
                            VendorId = vendorId
                        });
            }
        }

        public DeviceNameDBO GetDeviceNameData(int vendorId, int modelId)
        {
            using (var connection = GetDbConnection())
            {
                connection.Open();

                return connection
                    .QueryFirstOrDefault<DeviceNameDBO>(
                        ResourcesHelper.GetResourceAsString(CurrentAssembly, "LibFloaderClient.Implementations.DAO.Queries.GetDeviceNameData.sql"),
                        new
                        {
                            VendorId = vendorId,
                            ModelId = modelId
                        });
            }
        }

        public DeviceDataV1DBO GetDeviceDataV1(int vendorId, int modelId)
        {
            using (var connection = GetDbConnection())
            {
                connection.Open();

                return connection
                    .QueryFirstOrDefault<DeviceDataV1DBO>(
                        ResourcesHelper.GetResourceAsString(CurrentAssembly, "LibFloaderClient.Implementations.DAO.Queries.GetDeviceDataV1.sql"),
                        new
                        {
                            VendorId = vendorId,
                            ModelId = modelId
                        });
            }
        }
    }
}