using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var userProjects = db.Projects.Where(p => p.Users.Any(u => u.Id == userId)).Select(i => i.Id);
            var userTicketsList = db.Tickets.Where(t => userProjects.Contains(t.Id)).ToList();
            var userProjectList = db.Projects.Where(p => p.Users.Any(u => u.Id == userId)).ToList();

            var dashboard = new DashboardViewModel();

            dashboard.Projects = userProjectList;
            dashboard.Tickets = userTicketsList;
            
            return View(dashboard);

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            SendEmail model = new SendEmail();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(SendEmail model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold> ({ 1})</ p >< p > Message:</ p >< p >{ 2}</ p > ";
                    var from = "MyPortfolio<example@email.com>";
                    

                    var email = new MailMessage(from,
                                ConfigurationManager.AppSettings["emailto"])
                    {
                        Subject = "New ticket notification",
                        Body = string.Format(body, model.FromName, model.FromEmail,
                                             model.Body),
                        IsBodyHtml = true
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);

                    return View(new SendEmail());
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }
            return View(model);
        }

    }
}