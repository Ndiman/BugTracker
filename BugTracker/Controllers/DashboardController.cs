using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class DashboardController : Controller
    {
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
                return View("Admin");
            else if (User.IsInRole("ProjectManager"))
                return View("ProjectManager");
            else if (User.IsInRole("Submitter"))
                return View("Submitter");
            else
                return View("Developer");

        }
    }
}