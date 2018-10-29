using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Classes;
using Microsoft.AspNet.Identity;
using PagedList;

namespace BugTracker.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {
            return View(db.Tickets.ToList());
        }

        // GET: Tickets
        [Authorize(Roles = "Developer")]
        public ActionResult Index2()
        {
            var userId = User.Identity.GetUserId();
            return View(db.Tickets.Where(t => t.AssigneeId == userId).ToList());
        }

        // GET: Tickets
        [Authorize(Roles = "Submitter")]
        public ActionResult Index3()
        {
            var userId = User.Identity.GetUserId();
            return View(db.Tickets.Where(t => t.AuthorId == userId).ToList());
        }
        //GET: Tickets
        [Authorize(Roles = "Project Manager, Developer")]
        public ActionResult Index4()
        {
            var userId = User.Identity.GetUserId();
            var userProjects = db.Projects.Where(p => p.Users.Any(u => u.Id == userId)).Select(i => i.Id);
            return View(db.Tickets.Where(t => userProjects.Contains(t.Id)).ToList());
        }

        //POST: Attachments
        public ActionResult PostAttachment([Bind(Include ="Id, TicketId, FilePath, Description")] TicketAttachment attachment, HttpPostedFileBase fileUpload)
        {
            if (ModelState.IsValid)
            {
                
                var dbTicket = db.Tickets.First(p => p.Id == attachment.TicketId);

                var user = db.Users.FirstOrDefault(p => p.Id == dbTicket.AssigneeId);
                var userEmail = user.Email;

                var body = "An attachment has been added to a ticket (Ticket ID " + dbTicket.Id + ".). Please log in now.";
                var from = "mack.hatcher1@outlook.com";
                var email = new MailMessage(from,
                                    ConfigurationManager.AppSettings["emailto"])
                {
                    Subject = "Attachment added to ticket",
                    Body = body,
                    IsBodyHtml = true
                };

                var svc = new PersonalEmail();
                svc.Send(email);

                var fileName = Path.GetFileName(fileUpload.FileName);
                fileUpload.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                attachment.FilePath = "/Uploads/" + fileName;

                attachment.Created = DateTimeOffset.Now;
                attachment.UserId = User.Identity.GetUserId();
                db.TicketAttachments.Add(attachment);
                db.SaveChanges();
                return RedirectToAction("Details", "Tickets", new { id = attachment.TicketId });
            }

            return View(attachment);
            
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }
        [Authorize(Roles = "Submitter")]
        // GET: Tickets/Create
        public ActionResult Create()
        {
            var user = User.Identity.GetUserId();
            
            ViewBag.ProjectId = new SelectList(db.Projects.Where(u => u.Users.Any(p => p.Id == user)), "Id", "Name");
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            
            return View();
        }
        [Authorize(Roles = "Submitter")]
        // POST: Tickets/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name, Body, ProjectId, TicketPriorityId, TicketTypeId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.TicketStatusId = 1;
                ticket.Created = DateTimeOffset.Now;
                ticket.AuthorId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorId);
            return View(ticket);
        }

        [Authorize(Roles = "Project Manager, Developer")]
        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            var userId = User.Identity.GetUserId();
            ticket.Project.Users.Any(p => p.Id == userId);

            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
        }
        [Authorize(Roles = "Project Manager, Developer")]
        // POST: Tickets/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id, Name, Body, ProjectId, TicketPriorityId, TicketTypeId, TicketStatusId, AuthorId, Created, Updated")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {

                var dateChanged = DateTimeOffset.Now;
                var changes = new List<TicketHistory>();
                
                var dbTicket = db.Tickets.First(p => p.Id == ticket.Id);

                var user = db.Users.FirstOrDefault(p => p.Id == dbTicket.AssigneeId);
                var userEmail = user.Email;
                
                var body = "A change has been made to a ticket (Ticket ID " + dbTicket.Id + ".). Please log in now.";
                var from = "mack.hatcher1@outlook.com";
                var email = new MailMessage(from,
                                    ConfigurationManager.AppSettings["emailto"])
                {
                    Subject = "Ticket has been modified",
                    Body = body,
                    IsBodyHtml = true
                };

                var svc = new PersonalEmail();
                svc.Send(email);

                dbTicket.Name = ticket.Name;
                dbTicket.Body = ticket.Body;
                
                dbTicket.TicketPriorityId = ticket.TicketPriorityId;
                dbTicket.TicketStatusId = ticket.TicketStatusId;
                
                

                var originalValues = db.Entry(dbTicket).OriginalValues;
                var currentValues = db.Entry(dbTicket).CurrentValues;

                foreach(var property in originalValues.PropertyNames)
                {
                    var originalValue = originalValues[property]?.ToString();
                    var currentValue = currentValues[property]?.ToString();

                    if (originalValue != currentValue)
                    {
                        var history = new TicketHistory();
                        history.Changed = dateChanged;
                        history.NewValue = GetValueFromKey(property, currentValue);
                        history.OldValue = GetValueFromKey(property, originalValue); ;
                        history.Property = property;
                        history.TicketId = dbTicket.Id;
                        history.UserId = User.Identity.GetUserId();
                        changes.Add(history);
                    }
                }
                db.TicketHistories.AddRange(changes);

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssigneeId = new SelectList(db.Users, "Id", "Name", ticket.AssigneeId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");

            return View(ticket);
        }

        private string GetValueFromKey(string propertyName, string key)
        {
            if (propertyName == "TicketTypeId")
            {
                return db.TicketTypes.Find(Convert.ToInt32(key)).Name;
            }
            return key;
        }

        [Authorize(Roles = "Project Manager")]
        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }
        [Authorize(Roles = "Project Manager")]
        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Project Manager")]
        //GET: Tickets/AssignUsers

        public ActionResult AssignUsers(int id)
        {
            var model = new TicketAssignViewModel();

            model.Id = id;

            var ticket = db.Tickets.FirstOrDefault(p => p.Id == id);
            var users = db.Users.ToList();
            
            model.UserList = new MultiSelectList(users, "Id", "Name");   
            
            return View(model);
        }

        [Authorize(Roles = "Project Manager")]
        //POST: Ticket/AssignUsers

        [HttpPost]
        public ActionResult AssignUsers(TicketAssignViewModel model)
        {

            var ticket = db.Tickets.FirstOrDefault(p => p.Id == model.Id);

            ticket.AssigneeId = model.SelectedUser;
            
            var user = db.Users.FirstOrDefault(p => p.Id == ticket.AssigneeId);
            
            var userEmail = user.Email;




            var body = "You have a newly assigned ticket. Please log in now.";
            var from = "mack.hatcher1@outlook.com";
            var email = new MailMessage(from,
                                ConfigurationManager.AppSettings["emailto"])
                                 {
                        Subject = "New ticket notification",
                        Body = body,
                        IsBodyHtml = true
                    };

            var svc = new PersonalEmail();
            svc.Send(email);

            db.SaveChanges();
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
