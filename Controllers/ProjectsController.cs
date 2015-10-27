using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Helpers;

namespace BugTracker.Controllers {
    public class ProjectsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public async Task<ActionResult> Index() {
            return View(await db.ProjectsData.ToListAsync());
        }

        // GET: Projects/Details/5
        // the "?" within the () is to allow you to catch any errors so you can give a friendly error instead.  
        // it tells the request to go through the action anyway
        public async Task<ActionResult> Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectsModel projectsModel = await db.ProjectsData.FindAsync(id);

            ViewBag.AssignedUsers = new MultiSelectList(db.Users, "Id", "DisplayName", projectsModel.Users.Select(p => p.Id));

            if (projectsModel == null) {
                return HttpNotFound();
            }
            return View(projectsModel);
        }

        // GET: Projects/Create
        public ActionResult Create() {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Create is the action and everything in the () is what values you are expecting use
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] ProjectsModel projectsModel) {

            if (ModelState.IsValid) {

                //projectsModel.Users.Clear();

                //db.Users.ToList();

                //ViewBag.AssignedProjects = new SelectList(db.Users, "Id", "DisplayName", projectsModel.Id);

                db.ProjectsData.Add(projectsModel);
                
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(projectsModel);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var project = db.ProjectsData.Find(id);
            var selected = project.Users.Select(u => u.Id);
            //var userRole = selected()
            //var userRole = project.Users.Select(r => r.Roles);

            ProjectUsersModel projectUsersModel = new ProjectUsersModel() {
                Users = new MultiSelectList(db.Users, "Id", "DisplayName", selected),
                Project = project
            };
            return View(projectUsersModel);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProjectUsersModel projectUsersModel) {

            var project = db.ProjectsData.Find(projectUsersModel.Project.Id);

            UserProjectRolesHelper helper = new UserProjectRolesHelper();
            project.Users.Clear();

            foreach (var UserId in projectUsersModel.SelectedUsers) {

                    helper.AddUserToProject(UserId, project.Id);              
            }

            return RedirectToAction("Index");
        }


        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(int? id) {

            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProjectsModel projectsModel = await db.ProjectsData.FindAsync(id);
            if (projectsModel == null) {
                return HttpNotFound();
            }

            return View(projectsModel);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            ProjectsModel projectsModel = await db.ProjectsData.FindAsync(id);
            db.ProjectsData.Remove(projectsModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {

            if (disposing) {
                db.Dispose();
            }

            base.Dispose(disposing);
        }

        public int projectId { get; set; }
    }
}
