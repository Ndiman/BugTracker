using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helpers;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin, ProjectManager")]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

        //public ActionResult UserInfo()
        //{
        //    return View();
        //}

        // GET: Users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,DisplayName,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName")] ApplicationUser applicationUser)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(applicationUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(applicationUser);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }

            //set up a selectlist so I can select the role I want my user to occupy
            var occupiedRole = rolesHelper.ListUserRoles(id).FirstOrDefault();
            ViewBag.Roles = new SelectList(db.Roles, "Name", "Name", occupiedRole);

            var myProjectIds = new List<int>();
            var myProjects = projHelper.ListUserProjects(id);
            foreach (var project in myProjects)
                myProjectIds.Add(project.Id);

            ViewBag.Projects = new MultiSelectList(db.Projects, "Id", "Name", myProjectIds);

            return View(applicationUser);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,DisplayName,Email")] ApplicationUser applicationUser, string Roles, List<int> Projects)
        {
            if (ModelState.IsValid)
            {
                applicationUser.UserName = applicationUser.Email;
                //Assign the user to the selected role
                //Determine if the user currently occupies a role and if so remove them from it

                if (User.IsInRole("Admin"))
                {
                    foreach (var role in rolesHelper.ListUserRoles(applicationUser.Id))
                    {
                        rolesHelper.RemoveUserFromRole(applicationUser.Id, role);
                    }

                    //Add them to the selected role
                    if (Roles != null)
                        rolesHelper.AddUserToRole(applicationUser.Id, Roles);
                }
                //Remove user from all projects
                foreach (var project in projHelper.ListUserProjects(applicationUser.Id))
                {
                    projHelper.RemoveUserFromProject(applicationUser.Id, project.Id);
                }

                //Add them to the selected projects
                if (Projects != null)
                    foreach (var projectId in Projects)
                        projHelper.AddUserToProject(applicationUser.Id, projectId);

                //Because we have removed several fields from the User Edit Form, we have to tell the SQL that only a few specific properties are eligible for modification
                //db.Entry(applicationUser).State = EntityState.Modified; <-- This tells SQL that ALL properties are eligible for modification
                db.Users.Attach(applicationUser);
                db.Entry(applicationUser).Property(x => x.FirstName).IsModified = true;
                db.Entry(applicationUser).Property(x => x.LastName).IsModified = true;
                db.Entry(applicationUser).Property(x => x.DisplayName).IsModified = true;
                db.Entry(applicationUser).Property(x => x.Email).IsModified = true;
                db.Entry(applicationUser).Property(x => x.UserName).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(applicationUser);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ApplicationUser applicationUser = db.Users.Find(id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
