using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using BugTracker.Helpers;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private TicketsHelper ticketsHelper = new TicketsHelper();

        // GET: Tickets
        public ActionResult Index()
        {
            return View(db.Tickets.ToList());
            //var myTickets = new List<Ticket>();
            //var userId = User.Identity.GetUserId();
            //if (User.IsInRole("Admin"))
            //{
            //    myTickets = db.Tickets.ToList();
            //}
            //else if (User.IsInRole("Developer"))
            //{

            //    myTickets = db.Tickets.Where(t => t.AssignedToUserId == userId).ToList();
            //}
            //else if (User.IsInRole("Submitter"))
            //{
            //    myTickets = db.Tickets.Where(t => t.OwnerUserId == userId).ToList();
            //}
            //else
            //{
            //    //I must be a project Manager
            //    //First I need to get a list of all the projects I'm on
            //    //Then I need to get all the tickets for each of these projects
            //}


            //var tickets = db.Tickets.Include(t => t.AssignedToUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            //return View(tickets.ToList());
        }

        public ActionResult MyTickets()
        {
            var userId = User.Identity.GetUserId();
            return View(ticketsHelper.GetMyTicketsByRole(userId));
        }

        public ActionResult MyProjTickets()
        {
            var userId = User.Identity.GetUserId();
            var myProjects = projHelper.ListUserProjects(userId).ToList();
            return View(myProjects);
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Developer") && ticket.AssignedToUserId == User.Identity.GetUserId() ||
                    (User.IsInRole("Submitter") && ticket.OwnerUserId == User.Identity.GetUserId()) ||
                    (User.IsInRole("ProjectManager")) ||
                    (User.IsInRole("Admin")))
            {
                return View(ticket);
            }
            return RedirectToAction("MyTickets");
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create()
        {
            var myProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            ViewBag.ProjectId = new SelectList(myProjects, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.OwnerUserId = User.Identity.GetUserId();
                ticket.AssignedToUserId = null;
                ticket.TicketStatusId = 1;
                ticket.Created = DateTimeOffset.Now;

                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("MyTickets");
            }

            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            if (User.IsInRole("Developer") && ticket.AssignedToUserId == User.Identity.GetUserId() ||
                    (User.IsInRole("Submitter") && ticket.OwnerUserId == User.Identity.GetUserId()) ||
                    (User.IsInRole("ProjectManager")) ||
                    (User.IsInRole("Admin")))
            {
                var projDevs = new List<ApplicationUser>();
                var projUsers = projHelper.UsersOnProject(ticket.ProjectId);
                foreach (var user in projUsers)
                {
                    if (rolesHelper.IsUserInRole(user.Id, "Developer"))
                        projDevs.Add(user);
                }
                ViewBag.AssignedToUserId = new SelectList(projDevs, "Id", "DisplayName", ticket.AssignedToUserId);

                ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticket.OwnerUserId);
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
                ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);


                return View(ticket);
            }
            return RedirectToAction("MyTickets");
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {

                ticket.Updated = DateTimeOffset.Now;

                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property(x => x.Title).IsModified = true;
                db.Entry(ticket).Property(x => x.Description).IsModified = true;
                db.Entry(ticket).Property(x => x.TicketTypeId).IsModified = true;
                db.Entry(ticket).Property(x => x.TicketPriorityId).IsModified = true;
                db.Entry(ticket).Property(x => x.TicketStatusId).IsModified = true;
                db.Entry(ticket).Property(x => x.AssignedToUserId).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedToUserId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "FirstName", ticket.OwnerUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("Developer") && ticket.AssignedToUserId == User.Identity.GetUserId() ||
                    (User.IsInRole("Submitter") && ticket.OwnerUserId == User.Identity.GetUserId()) ||
                    (User.IsInRole("ProjectManager")) ||
                    (User.IsInRole("Admin")))
            {
                return View(ticket);
            }
            return RedirectToAction("MyTickets");
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Details");
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
