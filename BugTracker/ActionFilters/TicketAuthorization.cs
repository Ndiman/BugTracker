using BugTracker.Helpers;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BugTracker.ActionFilters
{
    public class TicketAuthorization : ActionFilterAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserRolesHelper rolesHelper = new UserRolesHelper();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ticketId = Convert.ToInt32(filterContext.ActionParameters.SingleOrDefault(p => p.Key == "id").Value);
            var ticket = db.Tickets.Find(ticketId);
            string userId = HttpContext.Current.User.Identity.GetUserId();
            var myRole = rolesHelper.ListUserRoles(userId).FirstOrDefault();

            if (userId == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Error" } });
            }
            else if (ticket == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Error" } });

            }
            else if (myRole == "Developer" && ticket.AssignedToUserId != userId ||
                    (myRole == "Submitter" && ticket.OwnerUserId != userId))
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Error" } });
            }
            else if (myRole == "ProjectManager")
            {
                var myProjectIds = db.Users.Find(userId).Projects.Select(p => p.Id).ToList();
                if(!myProjectIds.Contains(ticket.ProjectId))
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Error" } });
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}