﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Models.Classes;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "Admin,Project Manager")]
        // GET: Projects
        public ActionResult Index()
        {
            return View(db.Projects.ToList());
        }
        [Authorize(Roles = "Admin,Project Manager, Developer, Submitter")]
        // GET: Projects
        public ActionResult Index2()
        {
            var userId = User.Identity.GetUserId();
            return View(db.Projects.Where(p => p.Users.Any(u => u.Id == userId)).ToList());

        }
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }
        [Authorize(Roles = "Admin,Project Manager")]
        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }
        [Authorize(Roles = "Admin,Project Manager")]
        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }
        [Authorize(Roles = "Admin,Project Manager")]
        // POST: Projects/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
        [Authorize(Roles = "Admin,Project Manager")]
        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }
        [Authorize(Roles = "Admin,Project Manager")]
        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //GET: Assign Users
        [Authorize(Roles = "Admin,Project Manager")]
        public ActionResult AssignUsers(int id)
        {
            var model = new ProjectAssignViewModel();

            model.Id = id;

            var project = db.Projects.FirstOrDefault(p => p.Id == id);
            var users = db.Users.ToList();
            var userIdsAssignedToProject = project.Users
                .Select(p => p.Id).ToList();

            model.UserList = new MultiSelectList(users, "Id", "Name", userIdsAssignedToProject);


            return View(model);
        }
        //POST: Assign Users
        [Authorize(Roles = "Admin,Project Manager")]
        [HttpPost]
        public ActionResult AssignUsers(ProjectAssignViewModel model)
        {
            //STEP 1: Find the project
            var project = db.Projects.FirstOrDefault(p => p.Id == model.Id);

            //STEP 2: Remove all assigned users from this project
            var assignedUsers = project.Users.ToList();

            foreach (var user in assignedUsers)
            {
                project.Users.Remove(user);
            }

            //STEP 3: Assign users to the project
            if (model.SelectedUsers != null)
            {
                foreach (var userId in model.SelectedUsers)
                {
                    var user = db.Users.FirstOrDefault(p => p.Id == userId);

                    project.Users.Add(user);
                }
            }

            //STEP 4: Save changes to the database
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