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

namespace BugTracker.Controllers {
    public class ProjectsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public async Task<ActionResult> Index() {
            return View(await db.ProjectsModels.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectsModel projectsModel = await db.ProjectsModels.FindAsync(id);
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
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] ProjectsModel projectsModel) {
            if (ModelState.IsValid) {

                db.ProjectsModels.Add(projectsModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(projectsModel);
        }

        // GET: Projects/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectsModel projectsModel = await db.ProjectsModels.FindAsync(id);
            if (projectsModel == null) {
                return HttpNotFound();
            }
            return View(projectsModel);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] ProjectsModel projectsModel) {

       
        var projectUser = new ApplicationUser();

            if (ModelState.IsValid) {
                db.Entry(projectsModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(projectsModel);
        }

        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectsModel projectsModel = await db.ProjectsModels.FindAsync(id);
            if (projectsModel == null) {
                return HttpNotFound();
            }
            return View(projectsModel);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            ProjectsModel projectsModel = await db.ProjectsModels.FindAsync(id);
            db.ProjectsModels.Remove(projectsModel);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
