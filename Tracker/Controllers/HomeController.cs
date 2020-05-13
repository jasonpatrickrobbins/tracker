using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tracker.Models;
using Tracker.DAL;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Tracker.Controllers
{
    /*
    /// <summary>
    /// Flags an Action Method valid for any incoming request only if all, any or none of the given HTTP parameter(s) are set,
    /// enabling the use of multiple Action Methods with the same name (and different signatures) within the same MVC Controller.
    /// </summary>
    public class RequireParameterAttribute : ActionMethodSelectorAttribute
    {
        public RequireParameterAttribute(string valueName)
        {
            ValueName = valueName;
        }
        public override bool IsValidForRequest(ControllerContext controllerContext, MethodInfo methodInfo)
        {
            return controllerContext.HttpContext.Request[ValueName] != null;
        }

        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            throw new NotImplementedException();
        }

        public string ValueName { get; private set; }
    }
    */
    

    /// <summary>
    /// Constructor for the HomeController.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BugDAL bugDal;
        private Bug bug;

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
                TempData["2"] = 0;
                TempData["3"] = 0;

                //List<Bug> closedBugList = this.bugDal.GetOpenBugs(User.Identity.Name);
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
        public IActionResult Index(string software, string name, string description)
        {
            TempData["id"] = -1;
            TempData["1"] = 0;
            TempData["2"] = 0;
            TempData["3"] = 0;

            Bug bug = new Bug
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

            bool success = this.bugDal.AddBugToDatabase(bug);

            if (success)
            {
                TempData["Referrer"] = "SaveRegister";
                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View(this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }
            else
            {
                TempData["Referrer"] = "NO";
                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View(this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }
        }

        /// <summary>
        /// Performs the search. Iterates through a List</Bug> and returns the appropriate bug.
        /// </summary>
        [HttpGet]
        public virtual ActionResult Index(string id)
        {
            TempData["id"] = -1;
            TempData["1"] = 0;
            TempData["2"] = 0;
            TempData["3"] = 0;

            bool success = false;
            bool intResultTryParse = int.TryParse(id, out int idInt);

            if (intResultTryParse == true && User.Identity.IsAuthenticated)
            {
                foreach (var bug in this.bugDal.GetAllBugsFromDatabase(User.Identity.Name))
                {
                    if (bug.BugId == idInt)
                    {
                        success = true;
                        
                        TempData["id"] = bug.BugId;
                        TempData["search"] = bug.BugId;
                        TempData["1"] = 1;
                        return View(this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
                    }
                }
            }
            else if (intResultTryParse == false && User.Identity.IsAuthenticated)
            {
                TempData["2"] = 2;
                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View(this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }

            if (success == false && User.Identity.IsAuthenticated)
            {
                TempData["3"] = 3;
                this.bugDal.GetAllBugsFromDatabase(User.Identity.Name).Clear();
                return View(this.bugDal.GetAllBugsFromDatabase(User.Identity.Name));
            }
            else if (success == false && !User.Identity.IsAuthenticated)
            {
                return View();
            }

            return View();
        }

        /// <summary>
        /// Reads the data from the UpdateCloseAndDelete form and calls the corresponding method.
        /// </summary>
        [HttpPost]
        public IActionResult UpdateCloseAndDelete(string id, string engineer, string description, string update, string close, string delete)
        {
            TempData["id"] = -1;
            TempData["1"] = 0;
            TempData["2"] = 0;
            TempData["3"] = 0;

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
                    }
                }
            }

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

            if (!string.IsNullOrEmpty(update))
            {
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

            else if (!string.IsNullOrEmpty(close))
            {
                this.bugDal.CloseBug(idInt, User.Identity.Name);
            }
            else if (!string.IsNullOrEmpty(delete))
            {
                this.bugDal.DeleteBug(idInt, User.Identity.Name);
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