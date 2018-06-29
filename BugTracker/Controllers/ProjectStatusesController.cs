using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    [RequireHttps]
    public class ProjectStatusesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ProjectStatuses
        public ActionResult Index()
        {
            return View(db.ProjectStatuses.ToList());
        }

        // GET: ProjectStatuses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectStatus projectStatus = db.ProjectStatuses.Find(id);
            if (projectStatus == null)
            {
                return HttpNotFound();
            }
            return View(projectStatus);
        }

        // GET: ProjectStatuses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProjectStatuses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] ProjectStatus projectStatus)
        {
            if (ModelState.IsValid)
            {
                db.ProjectStatuses.Add(projectStatus);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projectStatus);
        }

        // GET: ProjectStatuses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectStatus projectStatus = db.ProjectStatuses.Find(id);
            if (projectStatus == null)
            {
                return HttpNotFound();
            }
            return View(projectStatus);
        }

        // POST: ProjectStatuses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] ProjectStatus projectStatus)
        {
            if (ModelState.IsValid)
            {
                db.Entry(projectStatus).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projectStatus);
        }

        // GET: ProjectStatuses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectStatus projectStatus = db.ProjectStatuses.Find(id);
            if (projectStatus == null)
            {
                return HttpNotFound();
            }
            return View(projectStatus);
        }

        // POST: ProjectStatuses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectStatus projectStatus = db.ProjectStatuses.Find(id);
            db.ProjectStatuses.Remove(projectStatus);
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
