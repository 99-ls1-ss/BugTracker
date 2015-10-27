using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using BugTracker.Models;

namespace BugTracker.Controllers {
    public class TicketsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public async Task<ActionResult> Index() {
            var ticketsModels = db.TicketsData.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);

            var ticket = new List<TicketsModel>();
            var user = db.Users.Find(User.Identity.GetUserId());
            if (User.IsInRole("ProjectManager")) {
                ticket = user.Projects.SelectMany(p => p.Tickets).ToList();
            }
            else if (User.IsInRole("Developer")) {
                ticket = user.Projects.SelectMany(p => p.Tickets).ToList();
            }
            else if (User.IsInRole("Submitter"))  {
                ticket = user.Projects.SelectMany(p => p.Tickets).ToList();
            } 

            return View(await ticketsModels.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<ActionResult> Details(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketsModel ticketsModel = await db.TicketsData.FindAsync(id);
            if (ticketsModel == null) {
                return HttpNotFound();
            }
            return View(ticketsModel);
        }

        // GET: Tickets/Create
        public ActionResult Create() {
            TicketsModel ticketsModel = new TicketsModel();
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Name");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "Name");

            ViewBag.ProjectId = new SelectList(db.ProjectsData, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPrioritiesData, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatusesData, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypesData, "Id", "Name");
            return View(ticketsModel);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Description,CreatedDate,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId")] TicketsModel ticketsModel) {
            if (ModelState.IsValid) {

                ticketsModel.CreatedDate = DateTimeOffset.Now;
                ticketsModel.UpdatedDate = null;
                db.TicketsData.Add(ticketsModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.ProjectsData, "Id", "Name", ticketsModel.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPrioritiesData, "Id", "Name", ticketsModel.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatusesData, "Id", "Name", ticketsModel.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypesData, "Id", "Name", ticketsModel.TicketTypeId);
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "Name", ticketsModel.AssignedToUserId);
            return View(ticketsModel);
        }

        // GET: Tickets/Edit/5
        public async Task<ActionResult> Edit(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketsModel ticketsModel = await db.TicketsData.FindAsync(id);
            if (ticketsModel == null) {
                return HttpNotFound();
            }

            ViewBag.ProjectId = new SelectList(db.ProjectsData, "Id", "Name", ticketsModel.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPrioritiesData, "Id", "Name", ticketsModel.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatusesData, "Id", "Name", ticketsModel.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypesData, "Id", "Name", ticketsModel.TicketTypeId);
            return View(ticketsModel);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Title,Description,CreatedDate,UpdatedDate,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] TicketsModel ticketsModel) {
            if (ModelState.IsValid) {
                db.Entry(ticketsModel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.ProjectsData, "Id", "Name", ticketsModel.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPrioritiesData, "Id", "Name", ticketsModel.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatusesData, "Id", "Name", ticketsModel.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypesData, "Id", "Name", ticketsModel.TicketTypeId);
            return View(ticketsModel);
        }

        // GET: Tickets/Delete/5
        public async Task<ActionResult> Delete(int? id) {
            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketsModel ticketsModel = await db.TicketsData.FindAsync(id);
            if (ticketsModel == null) {
                return HttpNotFound();
            }
            return View(ticketsModel);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id) {
            TicketsModel ticketsModel = await db.TicketsData.FindAsync(id);
            db.TicketsData.Remove(ticketsModel);
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
