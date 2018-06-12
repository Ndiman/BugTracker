using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.ViewModels
{
    public class MyProjectsViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}