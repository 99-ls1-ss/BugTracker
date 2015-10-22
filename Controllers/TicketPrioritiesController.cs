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
    public class TicketPrioritiesController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketPriorities
        public async Task<ActionResult> Index() {
            return View(await db.TicketPrioritiesModels.ToListAsync());
        }

        // GET: TicketPriorities/Details/5
        public async Task<ActionResult> Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPrioritiesModel ticketPrioritiesModel = await db.TicketPrioritiesModels.FindAsync(id);
            if (ticketPrioritiesModel == null) {
                return HttpNotFound();
            }
            return View(ticketPrioritiesModel);
        }

        // GET: TicketPriorities/Create
        public ActionResult Create() {
            return View();
        }

        // POST: TicketPriorities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name")] TicketPrioritiesModel ticketPrioritiesModel) {
            if (ModelState.IsValid) {
                db.TicketPrioritiesModels.Add(ticketPrioritiesModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(ticketPrioritiesModel);
        }

        // GET: TicketPriorities/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPrioritiesModel ticketPrioritiesModel = await db.TicketPrioritiesModels.FindAsync(id);
            if (ticketPrioritiesModel == null) {
                return HttpNotFound();
            }
            return View(ticketPrioritiesModel);
        }

        // POST: TicketPriorities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name")] TicketPrioritiesModel ticketPrioritiesModel) {
            if (ModelState.IsValid) {
                db.Entry(ticketPrioritiesModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(ticketPrioritiesModel);
        }

        // GET: TicketPriorities/Delete/5
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPrioritiesModel ticketPrioritiesModel = await db.TicketPrioritiesModels.FindAsync(id);
            if (ticketPrioritiesModel == null) {
                return HttpNotFound();
            }
            return View(ticketPrioritiesModel);
        }

        // POST: TicketPriorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            TicketPrioritiesModel ticketPrioritiesModel = await db.TicketPrioritiesModels.FindAsync(id);
            db.TicketPrioritiesModels.Remove(ticketPrioritiesModel);
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