using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using Dapper;
using LibFloaderClient.Implementations.Helpers;
using LibFloaderClient.Interfaces.DAO;
using LibFloaderClient.Models.DAO;

namespace LibFloaderClient.Implementations.DAO
{
    public class Dao : IDao
    {
        /// <summary>
        /// Devices database name
        /// </summary>
        private const string DbFilename = "DevDB.sqlite";

        public VendorDBO GetVendorData(int vendorId)
        {
            using (var connection = GetDbConnection())
            {
                connection.Open();

                return connection
                    .QueryFirst<VendorDBO>(
                        ResourcesHelper.GetResourceAsString(typeof(LibFloaderClient.Implementations.DAO.Dao),
                            "LibFloaderClient.Implementations.DAO.Queries.GetVendorData.sql"),
                        new { vendorId });
            }
        }

        /// <summary>
        /// Returns full path to device DB
        /// </summary>
        private string GetFullDbPath()
        {
            return Path.Combine(Environment.CurrentDirectory, DbFilename);
        }

        /// <summary>
        /// Returns ready to use device DB connection
        /// </summary>
        private SQLiteConnection GetDbConnection()
        {
            return new SQLiteConnection($"Data Source={ GetFullDbPath() }");
        }
    }
}