using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace BugTracker.Models.Classes
{
    public class Project
    {

            public int Id { get; set; }
            public string Name { get; set; }
            public string Body { get; set; }
            public DateTimeOffset Created { get; set; }
            public DateTimeOffset? Updated { get; set; }
            public bool Completed { get; set; }

        public Project()
            {
                Users = new HashSet<ApplicationUser>();
            }

            public virtual ICollection<ApplicationUser> Users { get; set; }
        }
    
}