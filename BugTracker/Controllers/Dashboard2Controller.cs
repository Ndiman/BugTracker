using BugTracker.Models;
using BugTracker.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class Dashboard2Controller : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketsHelper ticketsHelper = new TicketsHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();
        
        public ActionResult Index()
        {
            //create an instance of DashboardVM
            var data = new DashboardVM();
            
            //load up all the TableData
            data.TableData.Projects = db.Projects.OrderByDescending(p => p.Id).Take(5).ToList();
            data.TableData.Tickets = db.Tickets.OrderByDescending(t => t.Created).Take(5).ToList();
            data.TableData.TicketNotifications = db.TicketNotifications.OrderByDescending(t => t.Id).Take(5).ToList();
            data.TableData.TicketAttachments = db.TicketAttachments.OrderByDescending(t => t.Created).Take(5).ToList();
            data.TableData.TicketComments = db.TicketComments.OrderByDescending(t => t.Created).Take(5).ToList();
            data.TableData.TicketHistories = db.TicketHistories.OrderByDescending(t => t.Changed).Take(5).ToList();


            ////Load up all the Ticket Dashboard Data
            //data.TicketData.TicketCnt = db.Tickets.Count();
            //data.TicketData.TicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "Unassigned").Count();
            //data.TicketData.UnassignedTicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "In Progress").Count();
            //data.TicketData.InProgressTicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "On Hold").Count();
            //data.TicketData.OnHoldTicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "Resolved").Count();
            //data.TicketData.ResolvedTicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "CLosed").Count();


            //load up all My Tickets
            var myTickets = ticketsHelper.GetMyTicketsByRole(User.Identity.GetUserId());
            data.TicketData.TicketCnt = myTickets.Count();
            //status
            data.TicketData.UnassignedTicketCnt = myTickets.Where(t => t.TicketStatus.Name == "Unassigned").Count();
            data.TicketData.InProgressTicketCnt = myTickets.Where(t => t.TicketStatus.Name == "In Progress").Count();
            data.TicketData.OnHoldTicketCnt = myTickets.Where(t => t.TicketStatus.Name == "Assigned/On Hold").Count();
            data.TicketData.ResolvedTicketCnt = myTickets.Where(t => t.TicketStatus.Name == "Resolved").Count();
            data.TicketData.ClosedTicketCnt = myTickets.Where(t => t.TicketStatus.Name == "Closed").Count();
            //priority
            data.TicketData.ImmediateTicketCnt = myTickets.Where(t => t.TicketPriority.Name == "Immediate").Count();

            //load up all My Projects
            var myProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            data.ProjectData.MyProjectCnt = myProjects.Count();


            data.TicketData.TicketNotificationCnt = db.TicketNotifications.Count();
            data.TicketData.TicketAttachmentCnt = db.TicketAttachments.Count();
            data.TicketData.TicketCommentCnt = db.TicketComments.Count();
            data.TicketData.TicketHistoryCnt = db.TicketHistories.Count();

            //load up al the Project Dashboard Data
            data.ProjectData.ProjectCnt = db.Projects.Count();

            return View(data);
        }
    }
}