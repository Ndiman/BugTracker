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
using BugTracker.ActionFilters;
using BugTracker.Extension_Methods;
using System.Threading.Tasks;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private TicketsHelper ticketsHelper = new TicketsHelper();

        // GET: Tickets
        public ActionResult Index(string status)
        {
            if (string.IsNullOrEmpty(status))
                return View(db.Tickets.ToList());
            else
                return View(db.Tickets.Where(t => t.TicketStatus.Name == status).ToList());
            //return View(db.Tickets.ToList());
        }

        public ActionResult MyTickets()
        {
            var userId = User.Identity.GetUserId();
            return View(db.Users.Find(userId).MyTickets());
        }

        public ActionResult MyProjTickets()
        {
            var userId = User.Identity.GetUserId();
            var myProjects = projHelper.ListUserProjects(userId).ToList();
            return View(myProjects);
        }

        // GET: Tickets/Details/5
        [TicketAuthorization]
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
            return View(ticket);
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
                ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(t => t.Name == "Unassigned").Id;
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
        [TicketAuthorization]
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

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,Created,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,AssignedToUserId")] Ticket ticket)
        {
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);

            if (ModelState.IsValid)
            {

                ticket.Updated = DateTimeOffset.Now;

                db.Tickets.Attach(ticket);
                db.Entry(ticket).Property(x => x.Title).IsModified = true;
                db.Entry(ticket).Property(x => x.Description).IsModified = true;
                db.Entry(ticket).Property(x => x.ProjectId).IsModified = true;
                db.Entry(ticket).Property(x => x.TicketTypeId).IsModified = true;
                db.Entry(ticket).Property(x => x.TicketPriorityId).IsModified = true;
                db.Entry(ticket).Property(x => x.TicketStatusId).IsModified = true;
                db.Entry(ticket).Property(x => x.AssignedToUserId).IsModified = true;

                //If we are going from an unassigned Ticket to an Assigned Ticket, the Ticket Status needs to change
                if (string.IsNullOrEmpty(oldTicket.AssignedToUserId) && !string.IsNullOrEmpty(ticket.AssignedToUserId))
                {
                    ticket.TicketStatusId = db.TicketStatuses.FirstOrDefault(t => t.Name == "Assigned").Id;
                }

                db.SaveChanges();

                ticket.RecordChanges(oldTicket);
           
                //only should trigger if developer is changed
                if (oldTicket.AssignedToUserId != ticket.AssignedToUserId)
                {
                    await ticket.TriggerNotifications(oldTicket);
                }
                //Using a Ticket Extension method
                

                ////Using a helper method
                //var notificationData = new NotificationData
                //{
                //    RecipientId = ticket.AssignedToUserId,
                //    Body = NotificationHelper.BuildNotificationBody(ticket),
                //    Subject = "There was a change in assignment",
                //    TicketId = ticket.Id,
                //    Created = DateTimeOffset.Now
                //};

                //await NotificationHelper.TriggerNotification(notificationData, false);

                return RedirectToAction("Details", new { id = ticket.Id});
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
        [TicketAuthorization]
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
                return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("MyTickets");
            }
            
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
