// -----------------------------------------------------------
// Copyright(c) Coalition of the Good-Hearted Engineers
// ======= FREE TO USE FOR THE WORLD =======
// -----------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace SqlQueryProtect
{
    internal class Database
    {
        private readonly string connectionString;

        public Database()
        {
            this.connectionString = GetDbConnection();
        }

        public IEnumerable<dynamic> ExecuteQuery(string sqlQuery)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(connectionString);
                
                return connection.Query(sqlQuery);

            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            return Enumerable.Empty<dynamic>();
        }

        private static string GetDbConnection()
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            string connection = builder.Build().GetConnectionString("SqlDevConnection");

            return connection;
        }
    }
}