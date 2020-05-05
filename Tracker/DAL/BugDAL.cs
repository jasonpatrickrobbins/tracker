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
               "INSERT INTO Bug (" +
                    "bugName, " +
                    "softwareName, " +
                    "author" +
                    "assignedEngineer, " +
                    "openDate, " +
               "VALUES((@bugName), " +
                    "(@softwareName), " +
                    "(@assignedEngineer), " +
                    "(@openDate)";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(insertStatement, connection);

            command.Parameters.AddWithValue("@bugName", bug.BugName);
            command.Parameters.AddWithValue("@softwareName", bug.SoftwareName);
            command.Parameters.AddWithValue("@author", bug.Author);
            command.Parameters.AddWithValue("@assignedEngineer", bug.Description);
            command.Parameters.AddWithValue("@openDate", bug.DateOpened);

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





