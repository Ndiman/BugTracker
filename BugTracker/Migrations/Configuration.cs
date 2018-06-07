namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            var RoleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if(!context.Roles.Any(r => r.Name == "Admin"))
            {
                RoleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if(!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                RoleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                RoleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                RoleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "TestAdmin@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "TestAdmin@Mailinator.com",
                    Email = "TestAdmin@Mailinator.com",
                    FirstName = "Test",
                    LastName = "Admin",
                    DisplayName = "Admin",
                }, "Abc&123!");
            }

            var userId = userManager.FindByEmail("TestAdmin@Mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Users.Any(u => u.Email == "TestPM@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "TestPM@Mailinator.com",
                    Email = "TestPM@Mailinator.com",
                    FirstName = "Test",
                    LastName = "ProjectManager",
                    DisplayName = "ProjectManager",
                }, "Abc&123!");
            }

            userId = userManager.FindByEmail("TestPM@Mailinator.com").Id;
            userManager.AddToRole(userId, "ProjectManager");

            if (!context.Users.Any(u => u.Email == "TestSubmitter@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "TestSubmitter@Mailinator.com",
                    Email = "TestSubmitter@Mailinator.com",
                    FirstName = "Test",
                    LastName = "Submitter",
                    DisplayName = "Submitter",
                }, "Abc&123!");
            }

            userId = userManager.FindByEmail("TestSubmitter@Mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            if (!context.Users.Any(u => u.Email == "TestDev@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "TestDev@Mailinator.com",
                    Email = "TestDev@Mailinator.com",
                    FirstName = "Test",
                    LastName = "Developer",
                    DisplayName = "Developer",
                }, "Abc&123!");
            }

            userId = userManager.FindByEmail("TestDev@Mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            //Add records into any of my custom tables
            context.TicketPriorities.AddOrUpdate(
                t => t.Name,
                    new TicketPriority { Id = 100, Name = "Immediate" },
                    new TicketPriority { Id = 200, Name = "High" },
                    new TicketPriority { Id = 300, Name = "Medium" },
                    new TicketPriority { Id = 400, Name = "Low" },
                    new TicketPriority { Id = 500, Name = "Ignore" }
             );
            context.TicketStatuses.AddOrUpdate(
               t => t.Name,
                   new TicketStatus { Id = 100, Name = "Unassigned" },
                   new TicketStatus { Id = 200, Name = "In Progress" },
                   new TicketStatus { Id = 300, Name = "Assigned/On Hold" },
                   new TicketStatus { Id = 400, Name = "Resolved" },
                   new TicketStatus { Id = 500, Name = "Closed" }
            );
            context.TicketTypes.AddOrUpdate(
               t => t.Name,
                   new TicketType { Id = 100, Name = "Bug" },
                   new TicketType { Id = 200, Name = "Call for documentation" },
                   new TicketType { Id = 300, Name = "ScreenCast Demo Request" }
            );
            //context.Projects.AddOrUpdate(
            //   t => t.Name,
            //       new Project { Id = 100, Name = "", Description = "" },
            //       new Project { Id = 200, Name = "", Description = "" },
            //       new Project { Name = "", Description = "" }
            //);
            //context.Tickets.AddOrUpdate(
            //    t => t.Title,
            //        new Ticket { ProjectId = 100,
            //        TicketPriorityId = 300,
            //        TicketStatusId = 100,
            //        TicketTypeId = 100,
            //        Title = "",
            //        Created = DateTimeOffSet.Now }

            //);
        }
    }
}
