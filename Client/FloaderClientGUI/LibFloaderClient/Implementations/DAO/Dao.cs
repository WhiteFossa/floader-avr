using System;
using System.Data.SQLite;
using Dapper;
using LibFloaderClient.Interfaces.DAO;

namespace LibFloaderClient.Implementations.DAO
{
    public class Dao : IDao
    {
        /// <summary>
        /// Path from executable directory to devices database
        /// </summary>
        private const string DbFilename = @"\DevDB.sqlite";

        public string GetVendorName(int vendorId)
        {
            using (var connection = GetDbConnection())
            {
                connection.Open();

                return connection
                    .QueryFirst<string>($"select Name from vendors_names where Id={ vendorId }");
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