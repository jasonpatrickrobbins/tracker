using System;

namespace Tracker.Models
{

    /// <summary>    /// Model class for Bugs.    /// </summary>
    public class Bug
    {
        public int BugId { get; set; }
        public string BugName { get; set; }        public string SoftwareName { get; set; }        public string Description { get; set; }        public string Author { get; set; }        public string AssignedEngineer { get; set; }        public DateTime DateOpened { get; set; }        public DateTime DateClosed { get; set; }
    }
}