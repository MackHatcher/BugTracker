namespace BugTracker.Migrations
{
    using BugTracker.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
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
            
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }

            if (!context.Roles.Any(r => r.Name == "Project Manager"))
            {
                roleManager.Create(new IdentityRole { Name = "Project Manager" });
            }

            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }

            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }


            ApplicationUser adminUser = null;
            ApplicationUser managerUser = null;
            ApplicationUser devUser = null;
            ApplicationUser subUser = null;

            if (!context.Users.Any(p => p.UserName == "admin@myblogapp.com"))
            {
                adminUser = new ApplicationUser();
                adminUser.UserName = "admin@myblogapp.com";
                adminUser.Email = "admin@myblogapp.com";
                adminUser.FirstName = "Admin";
                adminUser.LastName = "User";
                adminUser.DisplayName = "Admin User";

                userManager.Create(adminUser, "Password-1");
            }
            else
            {
                adminUser = context.Users.Where(p => p.UserName == "admin@myblogapp.com")
                    .FirstOrDefault();
            }


            if (!context.Users.Any(p => p.UserName == "manager@myblogapp.com"))
            {
                managerUser = new ApplicationUser();
                managerUser.UserName = "manager@myblogapp.com";
                managerUser.Email = "manager@myblogapp.com";
                managerUser.FirstName = "Project";
                managerUser.LastName = "Manager";
                managerUser.DisplayName = "Project Manager";

                userManager.Create(managerUser, "Password-2");
            }
            else
            {
                managerUser = context.Users.Where(p => p.UserName == "manager@myblogapp.com")
                    .FirstOrDefault();
            }

            if (!context.Users.Any(p => p.UserName == "developer@myblogapp.com"))
            {
                devUser = new ApplicationUser();
                devUser.UserName = "developer@myblogapp.com";
                devUser.Email = "developer@myblogapp.com";
                devUser.FirstName = "Web";
                devUser.LastName = "Dev";
                devUser.DisplayName = "Developer";

                userManager.Create(devUser, "Password-3");
            }
            else
            {
                devUser = context.Users.Where(p => p.UserName == "developer@myblogapp.com")
                    .FirstOrDefault();
            }
            if (!context.Users.Any(p => p.UserName == "submitter@myblogapp.com"))
            {
                subUser = new ApplicationUser();
                subUser.UserName = "submitter@myblogapp.com";
                subUser.Email = "submitter@myblogapp.com";
                subUser.FirstName = "Project";
                subUser.LastName = "Submitter";
                subUser.DisplayName = "Submitter";

                userManager.Create(subUser, "Password-4");
            }
            else
            {
                subUser = context.Users.Where(p => p.UserName == "submitter@myblogapp.com")
                    .FirstOrDefault();
            }

            
            if (!userManager.IsInRole(adminUser.Id, "Admin"))
            {
                userManager.AddToRole(adminUser.Id, "Admin");
            }
            if (!userManager.IsInRole(managerUser.Id, "Project Manager"))
            {
                userManager.AddToRole(managerUser.Id, "Project Manager");
            }
            if (!userManager.IsInRole(devUser.Id, "Developer"))
            {
                userManager.AddToRole(devUser.Id, "Developer");
            }
            if (!userManager.IsInRole(subUser.Id, "Submitter"))
            {
                userManager.AddToRole(subUser.Id, "Submitter");
            }
        }
    }
}
