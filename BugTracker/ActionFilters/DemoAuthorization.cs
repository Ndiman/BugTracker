using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Web.Routing;

namespace BugTracker.ActionFilters
{
    public class DemoAuthorization : ActionFilterAttribute
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var user = HttpContext.Current.User.Identity.GetUserId();
            var userName = db.Users.Find(user).FirstName;

            if (user == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "GeneralError" } });
            }
            else if (userName == "Demo")
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "GeneralError" } });
            }

            base.OnActionExecuting(filterContext);
        }
    }
}