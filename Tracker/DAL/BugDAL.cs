using System;
using Npgsql;
using Tracker.Models;

namespace Tracker.DAL
{

    /// <summary>    /// Queries the Bug table in the Tracker Database.    /// </summary>
    public class BugDAL
    {

        /// <summary>        /// Adds a bug to the Bug table in the Tracker Database.        /// </summary>
        public bool AddBugToDatabase(Bug bug)
        {

            string insertStatement =
               "INSERT INTO bug(" +
                    "bug_name, " +
                    "software_name, " +
                    "description, " +
                    "author," +
                    "open_date) " +
               "VALUES(" +
                    "@bugName, " +
                    "@softwareName, " +
                    "@description, " +
                    "@author, " +
                    "current_timestamp)";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(insertStatement, connection);

            command.Parameters.AddWithValue("@bugName", bug.BugName);
            command.Parameters.AddWithValue("@softwareName", bug.SoftwareName);
            command.Parameters.AddWithValue("@description", bug.Description);
            command.Parameters.AddWithValue("@author", bug.Author);

            connection.Open();
            int count = command.ExecuteNonQuery();

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            };
        }
    }
}





