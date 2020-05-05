using System;

namespace Tracker.Models
{

    /// <summary>    /// Model class for Bugs.    /// </summary>
    public class Bug
    {

        public int BugId { get; }
        public string BugName { get; }        public string SoftwareName { get; }        public string Description { get; }        public string Author { get; }        public string AssignedEngineer { get; }        public DateTime DateOpened { get; }        public DateTime DateClosed { get; }        
        /// <summary>        /// Constructor for the Bug class. Creates a new Bug.        /// </summary>
        public Bug(int bugId,                   string bugName,                   string softwareName,                   string description,                   string author,                   string assignedEngineer,                   DateTime dateOpened,                   DateTime dateClosed)
        {

            if (string.IsNullOrEmpty(bugName) ||                string.IsNullOrEmpty(softwareName) ||                string.IsNullOrEmpty(description) ||                string.IsNullOrEmpty(author))                            {                throw new ArgumentException("Fields cannot be null or empty");            }            this.BugId = bugId;            this.BugName = bugName;            this.SoftwareName = softwareName;            this.Description = description;            this.Author = author;            this.AssignedEngineer = assignedEngineer;            this.DateOpened = dateOpened;            this.DateClosed = dateClosed;
        }
    }
}