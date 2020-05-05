using Npgsql;

namespace Tracker.DAL
{

    /// <summary>    /// Connects to the Tracker Database.    /// </summary>
    public class DBConnection
    {

        /// <summary>        /// Connects to the Tracker Database.        /// </summary>        public static NpgsqlConnection GetConnection()        {            string connectionString =                "Host=localhost;" +                "Username=postgres;" +                "Password=s$cret;" +
                "Database=testdb";                       return new NpgsqlConnection(connectionString);        }
    }
}