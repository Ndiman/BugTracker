using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;

namespace BugTracker.Controllers
{
    public class AttachmentTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AttachmentTypes
        public ActionResult Index()
        {
            return View(db.AttachmentTypes.ToList());
        }

        // GET: AttachmentTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttachmentType attachmentType = db.AttachmentTypes.Find(id);
            if (attachmentType == null)
            {
                return HttpNotFound();
            }
            return View(attachmentType);
        }

        // GET: AttachmentTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttachmentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Type,MediaUrl")] AttachmentType attachmentType, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    attachmentType.MediaUrl = "/Uploads/" + fileName;
                }

                db.AttachmentTypes.Add(attachmentType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(attachmentType);
        }

        // GET: AttachmentTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttachmentType attachmentType = db.AttachmentTypes.Find(id);
            if (attachmentType == null)
            {
                return HttpNotFound();
            }
            return View(attachmentType);
        }

        // POST: AttachmentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Type,MediaUrl")] AttachmentType attachmentType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attachmentType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(attachmentType);
        }

        // GET: AttachmentTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AttachmentType attachmentType = db.AttachmentTypes.Find(id);
            if (attachmentType == null)
            {
                return HttpNotFound();
            }
            return View(attachmentType);
        }

        // POST: AttachmentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AttachmentType attachmentType = db.AttachmentTypes.Find(id);
            db.AttachmentTypes.Remove(attachmentType);
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
