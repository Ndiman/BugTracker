using BugTracker.Helpers;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        //Class level members go here
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper roleHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();

        // GET: Admin
        public ActionResult AssignPMs()
        {
            //Set up a MultiSelectList to display all the projects
            ViewBag.Projects = new MultiSelectList(db.Projects, "Id", "Name");
            
            //Set up a SelectList to display all the PM's
            var projectManagers = roleHelper.UsersInRole("ProjectManager");
            ViewBag.PMs = new SelectList(projectManagers, "Id", "DisplayName");

            return View();
        }

        [HttpPost]
        public ActionResult AssignPMs(List<int> Projects, string PMs)
        {
            if (ModelState.IsValid)
            {
                //Remove user from all projects
                foreach (var project in projHelper.ListUserProjects(PMs))
                {
                    projHelper.RemoveUserFromProject(PMs, project.Id);
                }

                //Add them to the selected projects
                if (Projects != null)
                    foreach (var projectId in Projects)
                        projHelper.AddUserToProject(PMs, projectId);
                
                return RedirectToAction("Index", "Users");
            }
            return View();
        }
    }
}