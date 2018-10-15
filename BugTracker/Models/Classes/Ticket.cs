using BugTracker.Models.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class Ticket
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string AssigneeId { get; set; }
        public string Body { get; set; }
        public string AuthorId { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }

        public Ticket()
        {
            this.Comments = new HashSet<Comment>();
        }

        public virtual Project Project { get; set; }
        public virtual ApplicationUser Author { get; set; }
        public virtual ApplicationUser Assignee { get; set; }
        public virtual TicketType TicketType { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }
        public virtual TicketStatus TicketStatus { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

    }
}