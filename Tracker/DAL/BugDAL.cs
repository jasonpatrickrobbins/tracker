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

        /// <summary>        /// Returns the open bugs from the Bug table in the Tracker Database.        /// </summary>
        public List<Bug> GetOpenBugs(string author)
        {
            List<Bug> openBugList = new List<Bug>();

            string selectStatement = "SELECT id, bug_name, software_name, assigned_engineer, open_date " +
                                     "FROM bug " +
                                     "WHERE author = @author AND close_date IS NULL " +
                                     "GROUP BY open_date, software_name, assigned_engineer, id " +
                                     "ORDER BY open_date DESC";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(selectStatement, connection);
            command.Parameters.AddWithValue("@author", author);
            connection.Open();            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["id"]);
                string name = reader["bug_name"].ToString();
                string software = reader["software_name"].ToString();
                string engineer = "";

                if (reader["assigned_engineer"] == null)
                {
                    engineer = "";

                }
                else
                {
                    engineer = reader["assigned_engineer"].ToString();
                }

                DateTime opened = (DateTime)reader["open_date"];

                Bug bug = new Bug
                {
                    BugId = id,
                    BugName = name,
                    SoftwareName = software,
                    Author = author,
                    AssignedEngineer = engineer,
                    DateOpened = opened,
                    DateClosed = DateTime.Now
                };

                openBugList.Add(bug);
            }

            return openBugList;
        }


        /// <summary>        /// Returns the closed bugs from the Bug table in the Tracker Database.        /// </summary>
        public List<Bug> GetClosedBugs(string author)
        {
            List<Bug> closedBugList = new List<Bug>();

            string selectStatement = "SELECT id, bug_name, software_name, assigned_engineer, close_date " +
                                     "FROM bug " +
                                     "WHERE author = @author AND close_date IS NOT NULL " +
                                     "GROUP BY close_date, software_name, assigned_engineer, id " +
                                     "ORDER BY close_date DESC";

            using NpgsqlConnection connection = DBConnection.GetConnection();
            NpgsqlCommand command = new NpgsqlCommand(selectStatement, connection);
            command.Parameters.AddWithValue("@author", author);
            connection.Open();            using NpgsqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                int id = Convert.ToInt32(reader["id"]);
                string name = reader["bug_name"].ToString();
                string software = reader["software_name"].ToString();
                string engineer = "";

                if (reader["assigned_engineer"] == null)
                {
                    engineer = "";

                }
                else
                {
                    engineer = reader["assigned_engineer"].ToString();
                }

                DateTime opened = (DateTime)reader["open_date"];

                Bug bug = new Bug
                {
                    BugId = id,
                    BugName = name,
                    SoftwareName = software,
                    Author = author,
                    AssignedEngineer = engineer,
                    DateOpened = opened,
                    DateClosed = new DateTime()
                };

                closedBugList.Add(bug);
            }

            return closedBugList;
        }
    }
}





