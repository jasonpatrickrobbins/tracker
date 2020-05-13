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
                    "name, " +
                    "software, " +
                    "description, " +
                    "author," +
                    "engineer," + 
                    "opened) " +
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

            string selectStatement = "SELECT id, name, software, description, engineer, opened, closed " +
                                     "FROM bug " +
                                     "WHERE author = @author " +
                                     "GROUP BY opened, software, engineer, id " +
                                     "ORDER BY software, opened DESC";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(selectStatement, connection);
            command.Parameters.AddWithValue("@author", author);
            connection.Open();            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["id"]);
                string name = reader["name"].ToString();
                string software = reader["software"].ToString();
                string description = reader["description"].ToString();
                string engineer;

                if (reader["engineer"] == null || reader["engineer"] == DBNull.Value)
                {
                    engineer = "";
                }
                else
                {
                    engineer = reader["engineer"].ToString();

                }

                DateTime opened = (DateTime)reader["opened"];
                DateTime closed;

                if (reader["closed"] == null || reader["closed"] == DBNull.Value)
                {
                    closed = new DateTime();
                }
                else
                {
                    closed = (DateTime)reader["closed"];
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
            string updateStatement =                    "UPDATE bug SET " +                        "description = @newDescription, " +                        "engineer = @newEngineer " +
                    "WHERE " +                        "id = @id AND " +                        "author = @author AND " +                        "description = @oldDescription AND " +                        "engineer = @oldEngineer ";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(updateStatement, connection);

            command.Parameters.AddWithValue("@oldDescription", oldBug.Description);
            command.Parameters.AddWithValue("@oldEngineer", oldBug.AssignedEngineer);
            command.Parameters.AddWithValue("@id", newBug.BugId);
            command.Parameters.AddWithValue("@author", newBug.Author);
            command.Parameters.AddWithValue("@newDescription", newBug.Description);
            command.Parameters.AddWithValue("@newEngineer", newBug.AssignedEngineer);

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
        public bool CloseBug(Bug bugToClose)
        {
            string updateStatement =                    "UPDATE bug SET " +                        "closed = current_timestamp " +                    "WHERE " +                        "id = @id AND " +                        "name = @bugName AND " +                        "author = @author AND " +                        "description = @description AND " +                        "engineer != @notAssigned AND " +
                        "opened = @opened AND " +
                        "closed IS NULL ";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(updateStatement, connection);

            command.Parameters.AddWithValue("@id", bugToClose.BugId);
            command.Parameters.AddWithValue("@bugName", bugToClose.BugName);
            command.Parameters.AddWithValue("@author", bugToClose.Author);
            command.Parameters.AddWithValue("@description", bugToClose.Description);
            command.Parameters.AddWithValue("@notAssigned", "Not Assigned");
            command.Parameters.AddWithValue("@opened", bugToClose.DateOpened);

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

        /// <summary>        /// Deletes the selected bug.        /// </summary>
        public bool DeleteBug(Bug bugToDelete)
        {
            string deleteStatement =                    "DELETE FROM bug " +                                            "WHERE " +                        "id = @id AND " +                        "name = @bugName AND " +                        "author = @author AND " +                        "description = @description AND " +                        "engineer != @notAssigned AND " +
                        "opened = @opened AND " +
                        "closed IS NOT NULL ";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(deleteStatement, connection);

            command.Parameters.AddWithValue("@id", bugToDelete.BugId);
            command.Parameters.AddWithValue("@bugName", bugToDelete.BugName);
            command.Parameters.AddWithValue("@author", bugToDelete.Author);
            command.Parameters.AddWithValue("@description", bugToDelete.Description);
            command.Parameters.AddWithValue("@notAssigned", "Not Assigned");
            command.Parameters.AddWithValue("@opened", bugToDelete.DateOpened);

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





