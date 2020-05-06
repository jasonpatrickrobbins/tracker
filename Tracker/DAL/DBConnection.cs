using Npgsql;

namespace Tracker.DAL
{

    /// <summary>    /// Connects to the Tracker Database.    /// </summary>
    public class DBConnection
    {

        /// <summary>        /// Connects to the Tracker Database.        /// </summary>        public static NpgsqlConnection GetConnection()        {            string connectionString =                "Server = 127.0.0.1; " +
                "Port = 5432; " +
                "Database = tracker;";

            return new NpgsqlConnection(connectionString);        }
    }
}