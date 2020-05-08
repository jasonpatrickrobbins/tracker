using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Tracker.Models;
using Tracker.DAL;


namespace Tracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BugDAL bugDal;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            this.bugDal = new BugDAL();
        }

        [HttpGet]
        public IActionResult Index()
        {

            if (User.Identity.IsAuthenticated)
            {
                //List<Bug> closedBugList = this.bugDal.GetOpenBugs(User.Identity.Name);
                this.bugDal.GetOpenBugs(User.Identity.Name).Clear();
                return View(this.bugDal.GetOpenBugs(User.Identity.Name));
            }
            else
            {
                return View();
            }
        }

        /// <summary>
        /// Creates a Bug object from information recieved from the Add Bug Form.
        /// </summary>
        [HttpPost]
        public IActionResult Index(string software, string name, string description)
        {
            Bug bug = new Bug
            {
                BugId = 0,
                SoftwareName = software,
                BugName = name,
                Description = description,
                Author = User.Identity.Name,
                DateOpened = DateTime.Now,
                DateClosed = DateTime.Now
            };

            bool success = this.bugDal.AddBugToDatabase(bug);

            if (success)
            {
                TempData["Referrer"] = "SaveRegister";
                this.bugDal.GetOpenBugs(User.Identity.Name).Clear();
                return View(this.bugDal.GetOpenBugs(User.Identity.Name));
            }
            else
            {
                TempData["Referrer"] = "NO";
                this.bugDal.GetOpenBugs(User.Identity.Name).Clear();
                return View(this.bugDal.GetOpenBugs(User.Identity.Name));
            }
        }

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