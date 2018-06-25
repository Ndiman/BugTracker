using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace BugTracker.Helpers
{
    public class NotificationHelper
    {
        public static ApplicationDbContext db = new ApplicationDbContext();

        //I need data to generate a Notification, any other decisions should have
        //been made already before reaching this point
        public static async Task TriggerNotification(NotificationData data, bool sendEmail)
        {
            var notification = new TicketNotification
            {
                Body = data.Body,
                RecipientId = data.RecipientId,
                TicketId = data.TicketId,
                Subject = data.Subject,
                Created = data.Created
            };
            db.TicketNotifications.Add(notification);
            db.SaveChanges();

            //if this is a notification that I want an email to be generated for
            if(sendEmail)
            {
                


                //Generate 1 email to the new Developer letting them know they have been assigned
                var email = new IdentityMessage()
                {
                    Subject = data.Subject,
                    Body = data.Body,
                    Destination = db.Users.Find(data.RecipientId).Email
                };

                var svc = new EmailService();
                await svc.SendAsync(email);
            }
        }

        public static string BuildNotificationBody(Ticket ticket)
        {
            //Compose the body of the email
            var body = new StringBuilder();
            body.AppendFormat("<p>Email From: <bold>{0}</bold>({1})</p>", "Administrator", WebConfigurationManager.AppSettings["emailfrom"]);
            body.AppendLine("<br/><p><u><b>Message:</b></u></p>");
            body.AppendFormat("<p><b>Project Name:</b> {0}</p>", db.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId).Name);
            body.AppendFormat("<p><b>Ticket Title:</b> {0}</p>", ticket.Title, ticket.Id);
            body.AppendFormat("<p><b>Ticket Created:</b> {0}</p>", ticket.Created);
            body.AppendFormat("<p><b>Ticket Type:</b> {0}</p>", db.TicketTypes.Find(ticket.TicketTypeId).Name);
            body.AppendFormat("<p><b>Ticket Status:</b> {0}</p>", db.TicketStatuses.Find(ticket.TicketStatusId).Name);
            body.AppendFormat("<p><b>Ticket Priority:</b> {0}</p>", db.TicketPriorities.Find(ticket.TicketPriorityId).Name);
            return body.ToString();
        }

        //public static string BuildSubject(Ticket ticket)
        //{
        //    var subject = "";

        //    //Let's begin with notifications for Ticket Assignment/Unassignment
        //    var newAssignment = (ticket.AssignedToUserId != null && oldTicket.AssignedToUserId == null);
        //    var unAssignment = (ticket.AssignedToUserId == null && oldTicket.AssignedToUserId != null);
        //    var reAssignment = ((ticket.AssignedToUserId != null && oldTicket.AssignedToUserId != null) &&
        //                        (ticket.AssignedToUserId != oldTicket.AssignedToUserId));
        //    if (newAssignment)
        //        subject = string.Format("You have been assigned to Ticket Id {0}", ticket.Id);
        //    if(unAssignment)
        //        subject = string.Format("You have been removed from Ticket Id {0}", ticket.Id);
        //    if(reAssignment)
        //        subject = "There has been an assigment change for Ticket Id {0}", ticket.Id);

        //    return subject;
        //}
    }

    public class NotificationData
    {
        public string RecipientId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public int TicketId { get; set; }
        public DateTimeOffset Created { get; set; }
    }
}