using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using Dapper;
using LibFloaderClient.Interfaces.DAO;
using LibFloaderClient.Models.DAO;

namespace LibFloaderClient.Implementations.DAO
{
    public class Dao : IDao
    {
        /// <summary>
        /// Path from executable directory to devices database
        /// </summary>
        private const string DbFilename = @"\DevDB.sqlite";

        public VendorDBO GetVendorData(int vendorId)
        {
            using (var connection = GetDbConnection())
            {
                connection.Open();

                var assembly = typeof(LibFloaderClient.Implementations.DAO.Dao).GetTypeInfo().Assembly;
                var resourceStream = assembly.GetManifestResourceStream("LibFloaderClient.Implementations.DAO.Queries.GetVendorData.sql");
                string queryText;
                using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
                {
                    queryText = reader.ReadToEnd();
                }


                return connection
                    .QueryFirst<VendorDBO>(queryText, new { vendorId });
            }
        }

        /// <summary>
        /// Returns full path to device DB
        /// </summary>
        private string GetFullDbPath()
        {
            return $"{ Environment.CurrentDirectory }{ DbFilename }";
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