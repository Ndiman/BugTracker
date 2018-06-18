using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class AttachmentType
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string MediaUrl { get; set; }
    }
}