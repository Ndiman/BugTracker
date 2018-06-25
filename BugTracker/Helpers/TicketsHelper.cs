using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers
{
    public class TicketsHelper
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ProjectsHelper projHelper = new ProjectsHelper();
        private UserRolesHelper roleHelper = new UserRolesHelper();

        public ICollection<Ticket> GetProjectTickets(string pmId)
        {
            //This List of Tickets will hold all Tickets assigned to a given PM
            var myTickets = new List<Ticket>();
            foreach (var project in projHelper.ListUserProjects(pmId))
            {
                myTickets.AddRange(db.Tickets.Where(t => t.ProjectId == project.Id).ToList());
            }
            return myTickets;
        }

        public ICollection<Ticket> GetProjectTicketsUsingLinq(string userId)
        {
            return db.Users.Find(userId).Projects.SelectMany(t => t.Tickets).ToList();
        }

        public bool IsTicketOnMyProjects(string userId, int id)
        {
            return db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId).Projects.SelectMany(t => t.Tickets).Select(t => t.Id).Contains(id);
        }

        public ICollection<Ticket> GetMyTicketsByRole (string userId)
        {
            var mytickets = new List<Ticket>();
            var myRole = roleHelper.ListUserRoles(userId).FirstOrDefault();
            switch(myRole)
            {
                case "Admin":
                    mytickets.AddRange(db.Tickets.ToList());
                    break;
                case "ProjectManager":
                    mytickets.AddRange(GetProjectTickets(userId));
                    break;
                case "Developer":
                    mytickets.AddRange(db.Tickets.Where(t => t.AssignedToUserId == userId).ToList());
                    break;
                case "Submitter":
                    mytickets.AddRange(db.Tickets.Where(t => t.OwnerUserId == userId).ToList());
                    break;
            }
            return mytickets;
        }
    }
}