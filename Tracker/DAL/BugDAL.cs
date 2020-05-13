using System;
using System.Collections.Generic;
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
                    "assigned_engineer," + 
                    "open_date) " +
               "VALUES(" +
                    "@bugName, " +
                    "@softwareName, " +
                    "@description, " +
                    "@author, " +
                    "@assignedEngineer, " +
                    "current_timestamp)";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(insertStatement, connection);

            command.Parameters.AddWithValue("@bugName", bug.BugName);
            command.Parameters.AddWithValue("@softwareName", bug.SoftwareName);
            command.Parameters.AddWithValue("@description", bug.Description);
            command.Parameters.AddWithValue("@author", bug.Author);
            command.Parameters.AddWithValue("@assignedEngineer", bug.AssignedEngineer);

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

        /// <summary>        /// Returns the open bugs from the Bug table in the Tracker Database.        /// </summary>
        public List<Bug> GetAllBugsFromDatabase(string author)
        {
            List<Bug> openBugList = new List<Bug>();

            string selectStatement = "SELECT id, bug_name, software_name, description, assigned_engineer, open_date, close_date " +
                                     "FROM bug " +
                                     "WHERE author = @author " +
                                     "GROUP BY open_date, software_name, assigned_engineer, id " +
                                     "ORDER BY software_name, open_date DESC";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(selectStatement, connection);
            command.Parameters.AddWithValue("@author", author);
            connection.Open();            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["id"]);
                string name = reader["bug_name"].ToString();
                string software = reader["software_name"].ToString();
                string description = reader["description"].ToString();
                string engineer;

                if (reader["assigned_engineer"] == null || reader["assigned_engineer"] == DBNull.Value)
                {
                    engineer = "";
                }
                else
                {
                    engineer = reader["assigned_engineer"].ToString();

                }

                DateTime opened = (DateTime)reader["open_date"];
                DateTime closed;

                if (reader["close_date"] == null || reader["close_date"] == DBNull.Value)
                {
                    closed = new DateTime();
                }
                else
                {
                    closed = (DateTime)reader["close_date"];
                }

                Bug bug = new Bug
                {
                    BugId = id,
                    BugName = name,
                    SoftwareName = software,
                    Description = description,
                    Author = author,
                    AssignedEngineer = engineer,
                    DateOpened = opened,
                    DateClosed = closed,
                };

                openBugList.Add(bug);
            }

            return openBugList;
        }

        /// <summary>        /// Updates and returns the selected bug.        /// </summary>
        public bool UpdateBug(Bug newBug, Bug oldBug, string author)
        {
            string updateStatement =                    "UPDATE bug SET " +                        "description = @newDescription, " +                        "assigned_engineer = @newEngineer " +
                    "WHERE " +                        "id = @id AND " +                        "author = @author AND " +                        "description = @oldDescription AND " +                        "assigned_engineer = @oldEngineer ";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(updateStatement, connection);

            command.Parameters.AddWithValue("@oldDescription", oldBug.Description);
            command.Parameters.AddWithValue("@oldEngineer", oldBug.AssignedEngineer);

            command.Parameters.AddWithValue("@newDescription", newBug.Description);
            command.Parameters.AddWithValue("@newEngineer", newBug.AssignedEngineer);

            command.Parameters.AddWithValue("@id", newBug.BugId);
            command.Parameters.AddWithValue("@author", newBug.Author);

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

        /// <summary>        /// Closes and returns the selected bug.        /// </summary>
        public bool CloseBug(int id, string author)
        {
            List<Bug> closedBugList = new List<Bug>();

            return true;
        }

        /// <summary>        /// Deletes the selected bug.        /// </summary>
        public bool DeleteBug(int id, string author)
        {
            List<Bug> closedBugList = new List<Bug>();

            return true;
        }
    }
}





