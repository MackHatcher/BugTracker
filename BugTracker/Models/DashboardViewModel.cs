using BugTracker.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            this.Tickets = new HashSet<Ticket>();
            this.Projects = new HashSet<Project>();
        }

        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}