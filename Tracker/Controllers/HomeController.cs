using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tracker.Models;
using Tracker.DAL;
using Microsoft.AspNetCore.Http;

namespace Tracker.Controllers
{
    /// <summary>
    /// Constructor for the HomeController.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BugDAL bugDal;

        public Object Session { get; private set; }

        /// <summary>
        /// Constructor for the HomeController.
        /// </summary>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            this.bugDal = new BugDAL();
        }

        /// <summary>
        /// Main index method. Returns the view with a List<Bug> if a user is authenticated.
        /// </summary>
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                TempData["id"] = -1;
                TempData["1"] = 0;

                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View(this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Creates a Bug object from information recieved from the Add Bug Form and then returns the view.
        /// </summary>
        [HttpPost]
        public IActionResult NewBug(string software, string name, string description)
        {
            TempData["id"] = -1;
            TempData["1"] = 0;
            
            Bug newBug = new Bug
            {
                BugId = 0,
                SoftwareName = software,
                BugName = name,
                Description = description,
                Author = User.Identity.Name,
                AssignedEngineer = "Not Assigned",
                DateOpened = DateTime.Now,
                DateClosed = DateTime.Now
            };

            bool success = this.bugDal.AddBugToDatabase(newBug);
            if (success)
            {
                foreach (Bug bug in this.bugDal.GetAllBugsFromDatabase(User.Identity.Name))
                {
                    if (bug.BugName.Equals(newBug.BugName) &&
                        bug.SoftwareName.Equals(newBug.SoftwareName) &&
                        bug.Description.Equals(newBug.Description) &&
                        bug.Author.Equals(newBug.Author) &&
                        bug.DateOpened.ToString().Equals(newBug.DateOpened.ToString()))
                    {
                        this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                        return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
                    }
                }

                TempData["id"] = -1;
                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }
            else
            {
                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }
        }

        /// <summary>
        /// Performs the search. Iterates through a List</Bug> and returns the appropriate bug.
        /// </summary>
        [HttpGet]
        public IActionResult Search(string id)
        {
            TempData["id"] = -1;
            TempData["1"] = 0;

            bool success = false;
            bool intResultTryParse = int.TryParse(id, out int idInt);
            if (intResultTryParse == true && User.Identity.IsAuthenticated)
            {
                foreach (Bug bug in this.bugDal.GetAllBugsFromDatabase(User.Identity.Name))
                {
                    if (bug.BugId == idInt)
                    {
                        success = true;
                        TempData["id"] = bug.BugId;
                        TempData["1"] = 1;
                        return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
                    }
                }
            }
            else if (intResultTryParse == false && User.Identity.IsAuthenticated)
            {
                TempData["1"] = 2;
                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }

            if (success == false && User.Identity.IsAuthenticated)
            {
                TempData["1"] = 3;
                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }
            else if (success == false && !User.Identity.IsAuthenticated)
            {
                return View("Index");
            }

            return View("Index");
        }

        /// <summary>
        /// Reads the data from the UpdateCloseAndDelete form and calls the corresponding method.
        /// </summary>
        [HttpPost]
        public IActionResult UpdateCloseAndDelete(string id, string engineer, string description, string submit)
        {
            TempData["id"] = -1;
            TempData["1"] = 0;

            Bug oldBug = new Bug();
            bool intResultTryParse = int.TryParse(id, out int idInt);
            if (intResultTryParse == true && User.Identity.IsAuthenticated)
            {
                foreach (Bug bug in this.bugDal.GetAllBugsFromDatabase(User.Identity.Name))
                {
                    if (bug.BugId == idInt)
                    {
                        oldBug = bug;
                        oldBug.BugId = bug.BugId;
                        oldBug.BugName = bug.BugName;
                        oldBug.SoftwareName = bug.SoftwareName;
                        oldBug.Description = bug.Description;
                        oldBug.Author = bug.Author;
                        oldBug.AssignedEngineer = bug.AssignedEngineer;
                        oldBug.DateOpened = bug.DateOpened;
                        oldBug.DateClosed = bug.DateClosed;
                    }
                }
            }

            //~~~~~~~~UPDATE BUG~~~~~~~~//
            if (submit.Equals("update"))
            {
                string updatedDescription;
                if (string.IsNullOrEmpty(description))
                {
                    updatedDescription = oldBug.Description;
                }
                else
                {
                    updatedDescription = oldBug.Description + " " + "<< UPDATE >> Submitted on " + DateTime.Now + ": " + description;
                }

                Bug newBug = new Bug
                {
                    BugId = idInt,
                    AssignedEngineer = engineer,
                    Description = updatedDescription,
                    Author = User.Identity.Name,
                };

                bool success = this.bugDal.UpdateBug(newBug, oldBug, User.Identity.Name);
                if (success)
                {
                    TempData["id"] = idInt;
                    TempData["1"] = 1;
                    return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
                }
                else
                {
                    return View();
                }
            }
            //~~~~~~~~CLOSE BUG~~~~~~~~//
            else if (submit.Equals("close"))
            {
                bool success = this.bugDal.CloseBug(oldBug);
                if (success)
                {
                    TempData["id"] = idInt;
                    TempData["1"] = 1;
                    return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
                }
                else
                {
                    return View();
                }
            }
            //~~~~~~~~DELETE BUG~~~~~~~~//
            else if (submit.Equals("delete"))
            {
                this.bugDal.DeleteBug(oldBug);
            }

            TempData["id"] = idInt;
            TempData["1"] = 1;
            return View("Index", this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
        }

        /// <summary>
        /// Calls the privacy view.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}