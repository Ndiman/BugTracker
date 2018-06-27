using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class DashboardVM
    {
        //for filling in Badges/Labels
        public TicketDashboardData TicketData { get; set; }
        public ProjectDashboardData ProjectData { get; set; }

        //displaying recent records
        public TableDashboardData TableData { get; set; }

        public DashboardVM()
        {
            TableData = new TableDashboardData();
            TicketData = new TicketDashboardData();
            ProjectData = new ProjectDashboardData();
        }
    }

    public class ProjectDashboardData
    {
        public int ProjectCnt { get; set; }
        public int MyProjectCnt { get; set; }
    }

    public class TicketDashboardData
    {
        public int TicketCnt { get; set; }
        //status
        public int UnassignedTicketCnt { get; set; }
        public int InProgressTicketCnt { get; set; }
        public int OnHoldTicketCnt { get; set; }
        public int ResolvedTicketCnt { get; set; }
        public int ClosedTicketCnt { get; set; }

        public int TicketNotificationCnt { get; set; }
        public int TicketCommentCnt { get; set; }
        public int TicketAttachmentCnt { get; set; }
        public int TicketHistoryCnt { get; set; }
        //priority
        public int ImmediateTicketCnt { get; set; }
    }

    public class TableDashboardData
    {
        public List<Project> Projects { get; set; }
        public List<Ticket> Tickets { get; set; }
        public List<TicketNotification> TicketNotifications { get; set; }
        public List<TicketAttachment> TicketAttachments { get; set; }
        public List<TicketComment> TicketComments { get; set; }
        public List<TicketHistory> TicketHistories { get; set; }

        public TableDashboardData()
        {
            this.Tickets = new List<Ticket>();
            this.Projects = new List<Project>();
            this.TicketNotifications = new List<TicketNotification>();
            this.TicketAttachments = new List<TicketAttachment>();
            this.TicketComments = new List<TicketComment>();
            this.TicketHistories = new List<TicketHistory>();
        }
    }
}
