namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
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

            string userId = "";

            if (!context.Users.Any(u => u.Email == "DemoAdmin@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoAdmin@Mailinator.com",
                    Email = "DemoAdmin@Mailinator.com",
                    FirstName = "Demo",
                    LastName = "Admin",
                    DisplayName = "Admin",
                }, "Abc&123!");
            }

            userId = userManager.FindByEmail("DemoAdmin@Mailinator.com").Id;
            userManager.AddToRole(userId, "Admin");

            if (!context.Users.Any(u => u.Email == "DemoPM@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoPM@Mailinator.com",
                    Email = "DemoPM@Mailinator.com",
                    FirstName = "Demo",
                    LastName = "ProjectManager",
                    DisplayName = "Project Manager",
                }, "Abc&123!");
            }

            userId = userManager.FindByEmail("DemoPM@Mailinator.com").Id;
            userManager.AddToRole(userId, "ProjectManager");

            if (!context.Users.Any(u => u.Email == "DemoSubmitter@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoSubmitter@Mailinator.com",
                    Email = "DemoSubmitter@Mailinator.com",
                    FirstName = "Demo",
                    LastName = "Submitter",
                    DisplayName = "Submitter",
                }, "Abc&123!");
            }

            userId = userManager.FindByEmail("DemoSubmitter@Mailinator.com").Id;
            userManager.AddToRole(userId, "Submitter");

            if (!context.Users.Any(u => u.Email == "DemoDev@Mailinator.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "DemoDev@Mailinator.com",
                    Email = "DemoDev@Mailinator.com",
                    FirstName = "Demo",
                    LastName = "Developer",
                    DisplayName = "Developer",
                }, "Abc&123!");
            }

            userId = userManager.FindByEmail("DemoDev@Mailinator.com").Id;
            userManager.AddToRole(userId, "Developer");

            //Add records into any of my custom tables
            var projectStatusIds = new List<int> { 100, 200, 300 };
            if (context.ProjectStatuses.Count() == 0)
            {
                context.ProjectStatuses.AddOrUpdate(
                    p => p.Name,
                        new ProjectStatus { Id = projectStatusIds[0], Name = "Open" },
                        new ProjectStatus { Id = projectStatusIds[1], Name = "On Hold" },
                        new ProjectStatus { Id = projectStatusIds[2], Name = "Closed"}
                );
            }

            var ticketPriorityIds = new List<int> { 100, 200, 300, 400, 500 };
            if (context.TicketPriorities.Count() == 0)
            {
                context.TicketPriorities.AddOrUpdate(
                t => t.Name,
                    new TicketPriority { Id = ticketPriorityIds[0], Name = "Immediate" },
                    new TicketPriority { Id = ticketPriorityIds[1], Name = "High" },
                    new TicketPriority { Id = ticketPriorityIds[2], Name = "Medium" },
                    new TicketPriority { Id = ticketPriorityIds[3], Name = "Low" },
                    new TicketPriority { Id = ticketPriorityIds[4], Name = "Ignore" }
             );
            }

            var ticketStatusIds = new List<int> { 100, 200, 300, 400, 500, 600 };
            if (context.TicketStatuses.Count() == 0)
            {
                context.TicketStatuses.AddOrUpdate(
               t => t.Name,
                   new TicketStatus { Id = ticketStatusIds[0], Name = "Unassigned" },
                   new TicketStatus { Id = ticketStatusIds[1], Name = "Assigned" },
                   new TicketStatus { Id = ticketStatusIds[2], Name = "In Progress" },
                   new TicketStatus { Id = ticketStatusIds[3], Name = "On Hold" },
                   new TicketStatus { Id = ticketStatusIds[4], Name = "Resolved" },
                   new TicketStatus { Id = ticketStatusIds[5], Name = "Closed" }
            );
            }

            var ticketTypeIds = new List<int> { 100, 200, 300, 400 };
            if (context.TicketTypes.Count() == 0)
            {
                context.TicketTypes.AddOrUpdate(
               t => t.Name,
                   new TicketType { Id = ticketTypeIds[0], Name = "Bug Fix" },
                   new TicketType { Id = ticketTypeIds[1], Name = "Task" },
                   new TicketType { Id = ticketTypeIds[2], Name = "Documentation Request" },
                   new TicketType { Id = ticketTypeIds[3], Name = "Demo Video Request" }
            );
            }

            var projectIds = new List<int> { 100, 200, 300, 400, 500 };
            if (context.Projects.Count() == 0)
            {
                context.Projects.AddOrUpdate(
                    t => t.Name,
                        new Project { Id = projectIds[0], Name = "Honey Badger", Description = "", ProjectStatusId = projectStatusIds[0] },
                        new Project { Id = projectIds[0], Name = "Michigo", Description = "", ProjectStatusId = projectStatusIds[0] },
                        new Project { Id = projectIds[0], Name = "Raging Mongoose", Description = "", ProjectStatusId = projectStatusIds[0] },
                        new Project { Id = projectIds[0], Name = "Ugly Duckling", Description = "", ProjectStatusId = projectStatusIds[0] },
                        new Project { Id = projectIds[0], Name = "Black Mamba", Description = "", ProjectStatusId = projectStatusIds[0] }
                );
            }

            #region AttachmentTypes
            context.AttachmentTypes.AddOrUpdate(
                t => t.Type,
                    new AttachmentType { Type = "txt" },
                    new AttachmentType { Type = "doc" },
                    new AttachmentType { Type = "docx" },
                    new AttachmentType { Type = "pdf" },
                    new AttachmentType { Type = "xls" },
                    new AttachmentType { Type = "xlsx" },
                    new AttachmentType { Type = "jpg" },
                    new AttachmentType { Type = "jpeg" },
                    new AttachmentType { Type = "gif" },
                    new AttachmentType { Type = "png" },
                    new AttachmentType { Type = "tiff" }
            );
            #endregion

        }


    }
}
