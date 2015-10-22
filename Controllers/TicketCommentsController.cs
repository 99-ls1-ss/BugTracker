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
    public class TicketCommentsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketComments
        public async Task<ActionResult> Index() {
            var ticketCommentsModels = db.TicketCommentsModels.Include(t => t.Ticket);
            return View(await ticketCommentsModels.ToListAsync());
        }

        // GET: TicketComments/Details/5
        public async Task<ActionResult> Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketCommentsModel ticketCommentsModel = await db.TicketCommentsModels.FindAsync(id);
            if (ticketCommentsModel == null) {
                return HttpNotFound();
            }
            return View(ticketCommentsModel);
        }

        // GET: TicketComments/Create
        public ActionResult Create() {
            ViewBag.TicketId = new SelectList(db.TicketsModels, "Id", "Title");
            return View();
        }

        // POST: TicketComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Comment,CreatedDate,TicketId,UserId")] TicketCommentsModel ticketCommentsModel) {
            if (ModelState.IsValid) {
                db.TicketCommentsModels.Add(ticketCommentsModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TicketId = new SelectList(db.TicketsModels, "Id", "Title", ticketCommentsModel.TicketId);
            return View(ticketCommentsModel);
        }

        // GET: TicketComments/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketCommentsModel ticketCommentsModel = await db.TicketCommentsModels.FindAsync(id);
            if (ticketCommentsModel == null) {
                return HttpNotFound();
            }
            ViewBag.TicketId = new SelectList(db.TicketsModels, "Id", "Title", ticketCommentsModel.TicketId);
            return View(ticketCommentsModel);
        }

        // POST: TicketComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Comment,CreatedDate,TicketId,UserId")] TicketCommentsModel ticketCommentsModel) {
            if (ModelState.IsValid) {
                db.Entry(ticketCommentsModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.TicketId = new SelectList(db.TicketsModels, "Id", "Title", ticketCommentsModel.TicketId);
            return View(ticketCommentsModel);
        }

        // GET: TicketComments/Delete/5
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketCommentsModel ticketCommentsModel = await db.TicketCommentsModels.FindAsync(id);
            if (ticketCommentsModel == null) {
                return HttpNotFound();
            }
            return View(ticketCommentsModel);
        }

        // POST: TicketComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            TicketCommentsModel ticketCommentsModel = await db.TicketCommentsModels.FindAsync(id);
            db.TicketCommentsModels.Remove(ticketCommentsModel);
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
