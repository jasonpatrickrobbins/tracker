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

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Creates a Bug object from information recieved from the Add Bug Form.
        /// </summary>
        [HttpPost]
        public IActionResult Index(Bug bug)
        {

            bug.BugId = 0;
            bug.Author = User.Identity.Name;
            bug.DateOpened = DateTime.Now;
            bug.DateClosed = DateTime.Now;

            bool success = this.bugDal.AddBugToDatabase(bug);

            if (success)
            {
                TempData["Referrer"] = "SaveRegister";
                return View(bug);
            }
            else
            {
                TempData["Referrer"] = "NO";
                return View(bug);
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