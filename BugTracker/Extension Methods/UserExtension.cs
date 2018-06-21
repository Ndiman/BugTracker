using BugTracker.Helpers;
using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Extension_Methods
{
    public static class UserExtension
    {
        public static ApplicationDbContext db = new ApplicationDbContext();
        public static UserRolesHelper rolesHelper = new UserRolesHelper();

        public static List<Ticket> MyTickets(this ApplicationUser user)
        {
            var myTickets = new List<Ticket>();

            var myRole = rolesHelper.ListUserRoles(user.Id).FirstOrDefault();
            switch(myRole)
            {
                case "Submitter":
                    myTickets = db.Tickets.Where(t => t.OwnerUserId == user.Id).ToList();
                    break;
                case "Developer":
                    myTickets = db.Tickets.Where(t => t.AssignedToUserId == user.Id).ToList();
                    break;
                case "ProjectManager":
                    myTickets = user.Projects.SelectMany(t => t.Tickets).ToList();
                    break;
                case "Admin":
                    myTickets = db.Tickets.ToList();
                    break;
                default:
                    break;
            }
            return myTickets;
        }
    }
}