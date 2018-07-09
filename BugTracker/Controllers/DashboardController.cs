using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Helpers;
using BugTracker.Models;
using BugTracker.ViewModels;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace BugTracker.Controllers
{
    [RequireHttps]
    [Authorize]
    public class DashboardController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TicketsHelper ticketsHelper = new TicketsHelper();
        private ProjectsHelper projHelper = new ProjectsHelper();

        public ActionResult Index()
        {
            //create an instance of DashboardVM
            var data = new DashboardVM();
            var myProjTickets = ticketsHelper.GetProjectTicketsUsingLinq(User.Identity.GetUserId());
            var myTickets = ticketsHelper.GetMyTicketsByRole(User.Identity.GetUserId());
            //load up all the TableData
            data.TableData.Projects = db.Projects.OrderByDescending(p => p.Id).ToList();
            if(User.IsInRole("Admin"))
            {
                data.TableData.Tickets = db.Tickets.OrderByDescending(t => t.Created).ToList();
            }
            else if (User.IsInRole("ProjectManager"))
            {
                data.TableData.Tickets = myProjTickets.OrderByDescending(t => t.Created).ToList();
            }
            else
            {
                data.TableData.Tickets = myTickets.OrderByDescending(t => t.Created).ToList();
            }
            data.TableData.TicketNotifications = db.TicketNotifications.OrderByDescending(t => t.Id).ToList();
            data.TableData.TicketAttachments = db.TicketAttachments.OrderByDescending(t => t.Created).ToList();
            data.TableData.TicketComments = db.TicketComments.OrderByDescending(t => t.Created).ToList();
            data.TableData.TicketHistories = db.TicketHistories.OrderByDescending(t => t.Changed).ToList();
            


            ////Load up all the Ticket Dashboard Data
            //data.TicketData.TicketCnt = db.Tickets.Count();
            //data.TicketData.TicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "Unassigned").Count();
            //data.TicketData.UnassignedTicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "In Progress").Count();
            //data.TicketData.InProgressTicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "On Hold").Count();
            //data.TicketData.OnHoldTicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "Resolved").Count();
            //data.TicketData.ResolvedTicketCnt = db.Tickets.Where(t => t.TicketStatus.Name == "CLosed").Count();


            //load up all My Tickets
            
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

            if (User.IsInRole("Admin"))
                return View("Admin", data);
            else if (User.IsInRole("ProjectManager"))
                return View("ProjectManager", data);
            else if (User.IsInRole("Submitter"))
                return View("Submitter", data);
            else if (User.IsInRole("Developer"))
                return View("Developer", data);
            else
                return View();

        }

        public ActionResult Unassigned_Data()
        {
            var chartVM = new ChartVM();
            
            var projects = new List<string>
            {
                "Baratheon",
                "Tyrell",
                "Stark",
                "Lannister",
                "Targaryen",
                "Dorne"
            };

            foreach (var entry in projects)
            {
                chartVM.Data.Add(new ChartData
                {
                    Label = entry,
                    Value = db.Tickets.Where(t => t.Project.Name == entry && t.AssignedToUserId == null).Count().ToString()
                });
            }
            return Content(JsonConvert.SerializeObject(chartVM), "application/json");
        }

        public ActionResult TicketProj_Data()
        {
            var chartVM = new ChartVM();

            var projects = new List<string>
            {
                "Baratheon",
                "Tyrell",
                "Stark",
                "Lannister",
                "Targaryen",
                "Dorne"
            };

            foreach (var entry in projects)
            {
                chartVM.Data.Add(new ChartData
                {
                    Label = entry,
                    Value = db.Tickets.Where(t => t.Project.Name == entry).Count().ToString()

                });
            }
            return Content(JsonConvert.SerializeObject(chartVM), "application/json");
        }

        public ActionResult MyUnassigned_Data()
        {
            var ChartVM = new ChartVM();

            var myProjTickets = ticketsHelper.GetProjectTickets(User.Identity.GetUserId());
            //var myProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            var projects = new List<string>
            {
               "Baratheon",
                "Tyrell",
                "Stark",
                "Lannister",
                "Targaryen",
                "Dorne"
            };

            foreach (var entry in projects)
            {
                ChartVM.Data.Add(new ChartData
                {
                    Label = entry,
                    Value = myProjTickets.Where(t => t.Project.Name == entry && t.AssignedToUserId == null).Count().ToString()
                });
            }
            return Content(JsonConvert.SerializeObject(ChartVM), "application/json");
        }

        public ActionResult MyTicketProj_Data()
        {
            var chartVM = new ChartVM();
            var myProjTickets = ticketsHelper.GetProjectTickets(User.Identity.GetUserId());
            var projects = new List<string>
            {
                "Baratheon",
                "Tyrell",
                "Stark",
                "Lannister",
                "Targaryen",
                "Dorne"
            };

            foreach (var entry in projects)
            {
                chartVM.Data.Add(new ChartData
                {
                    Label = entry,
                    Value = myProjTickets.Where(t => t.Project.Name == entry).Count().ToString()

                });
            }
            return Content(JsonConvert.SerializeObject(chartVM), "application/json");
        }

        public ActionResult MyImmediate_Data()
        {
            var ChartVM = new ChartVM();

            var myTickets = ticketsHelper.GetMyTicketsByRole(User.Identity.GetUserId());
            //var myProjects = projHelper.ListUserProjects(User.Identity.GetUserId());
            var projects = new List<string>
            {
               "Baratheon",
                "Tyrell",
                "Stark",
                "Lannister",
                "Targaryen",
                "Dorne"
            };

            foreach (var entry in projects)
            {
                ChartVM.Data.Add(new ChartData
                {
                    Label = entry,
                    Value = myTickets.Where(t => t.Project.Name == entry && t.TicketPriority.Name == "Immediate").Count().ToString()
                });
            }
            return Content(JsonConvert.SerializeObject(ChartVM), "application/json");
        }
    }
}