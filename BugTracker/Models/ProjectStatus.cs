using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class ProjectStatus
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        //Add a relationship to Projects
        public virtual ICollection<Project> Projects { get; set; }

        public ProjectStatus()
        {
            this.Projects = new HashSet<Project>();
        }
    }
}