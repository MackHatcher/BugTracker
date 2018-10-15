using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
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
        public ActionResult Index2()
        {
            var userId = User.Identity.GetUserId();
            return View(db.Tickets.Where(t => t.AssigneeId == userId).ToList());
        }

        // GET: Tickets
        public ActionResult Index3()
        {
            var userId = User.Identity.GetUserId();
            return View(db.Tickets.Where(t => t.AuthorId == userId).ToList());
        }

        public ActionResult Index4()
        {
            var userId = User.Identity.GetUserId();
            var userProjects = db.Projects.Where(p => p.Users.Any(u => u.Id == userId)).Select(i => i.Id);
            return View(db.Tickets.Where(t => userProjects.Contains(t.Id)).ToList());
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
        //[Authorize(Roles ="Submitter")]
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
            return View(ticket);
        }
        [Authorize(Roles = "Project Manager, Developer")]
        // POST: Tickets/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Body,AuthorId,Created,Updated,UpdateReason,Priority")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorId = new SelectList(db.Users, "Id", "FirstName", ticket.AuthorId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View(ticket);
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
            var user = ticket.Assignee.Roles.Select(t => t.RoleId);
            
            model.UserList = new MultiSelectList(user, "Id", "Name");
            
            return View(model);
        }

        [Authorize(Roles = "Project Manager")]
        //POST: Ticket/AssignUsers

        [HttpPost]
        public ActionResult AssignUsers(TicketAssignViewModel model)
        {
            //STEP 1: Find the ticket
            var ticket = db.Tickets.FirstOrDefault(p => p.Id == model.Id);
            ticket.AssigneeId = model.SelectedUser;
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
