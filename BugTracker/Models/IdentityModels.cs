using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using BugTracker.Models.Classes;
using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }

        public ApplicationUser()
        {
            Projects = new HashSet<Project>();
            CreatedTickets = new HashSet<Ticket>();
            AssignedTickets = new HashSet<Ticket>();
            Attachments = new HashSet<TicketAttachment>();
            Histories = new HashSet<TicketHistory>();
        }

        public virtual ICollection<Project> Projects { get; set; }

        [InverseProperty("Assignee")]
        public virtual ICollection<Ticket> AssignedTickets { get; set; }
        [InverseProperty("Author")]
        public virtual ICollection<Ticket> CreatedTickets { get; set; }

        public virtual ICollection<TicketAttachment> Attachments { get; set; }

        public virtual ICollection<TicketHistory> Histories { get; set; }



        public string Name { get; internal set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BugTracker.Models.Classes.Project> Projects { get; set; }
        public System.Data.Entity.DbSet<BugTracker.Models.Classes.TicketPriority> TicketPriorities { get; set; }
        public System.Data.Entity.DbSet<BugTracker.Models.Classes.TicketStatus> TicketStatuses { get; set; }
        public System.Data.Entity.DbSet<BugTracker.Models.Classes.TicketType> TicketTypes { get; set; }
        public System.Data.Entity.DbSet<BugTracker.Models.Classes.TicketAttachment> TicketAttachments { get; set; }
        public System.Data.Entity.DbSet<BugTracker.Models.Classes.TicketHistory> TicketHistories { get; set; }
        public System.Data.Entity.DbSet<BugTracker.Models.Ticket> Tickets { get; set; }
        
        public System.Data.Entity.DbSet<BugTracker.Models.Classes.Comment> Comments { get; set; }
        public object TicketPriority { get; internal set; }
    }
}