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
using System.IO;
using BugTracker.Helpers;

namespace BugTracker.Controllers {
    public class TicketsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index() {
            var tickets = db.TicketsData.Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            var userId = User.Identity.GetUserId();
            var user = db.Users.Include(p=>p.Projects).FirstOrDefault(u=> u.Id == userId);

            if (User.IsInRole("Admin")) {
                tickets = tickets;
            }
            else if ((User.IsInRole("ProjectManager")) || (User.IsInRole("Developer"))) {

                tickets = user.Projects.SelectMany(p=>p.Tickets).AsQueryable();
            }
            else if (User.IsInRole("Submitter")) {
                tickets = db.TicketsData.Where(t=>t.OwnerUserId == user.Id);
            }

            return View(tickets.ToList());
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
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName");
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName");

            ViewBag.ProjectId = new SelectList(db.ProjectsData, "Id", "Name");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPrioritiesData, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatusesData, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypesData, "Id", "Name");
            ViewBag.TicketAttachmentId = new SelectList(db.AttachmentData, "Id", "FileUrl");
            return View(ticketsModel);
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Description,CreatedDate,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,AssignedToUserId")] TicketsModel ticketsModel, HttpPostedFileBase file) {

            if (file != null && file.ContentLength > 0) {
                //Check the file name to make sure it's a image
                var ext = Path.GetExtension(file.FileName).ToLower();

                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && 
                    ext != ".bmp" && ext != ".txt" && ext != ".doc" && ext != ".docx" && 
                    ext != ".pdf" && ext != ".rtf" && ext != ".wps")
                    ModelState.AddModelError("image", "Invalid Image Type.");
            }

            TicketAttachmentsModel attachments = new TicketAttachmentsModel();
            if (ModelState.IsValid) {

                if (file != null && file.ContentLength > 0) {
                    var filePath = "/Uploads/";
                    var absPath = Server.MapPath("~" + filePath);
                    attachments.FileUrl = filePath + file.FileName;
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                }

                ticketsModel.CreatedDate = DateTimeOffset.Now;
                ticketsModel.UpdatedDate = null;
                attachments.CreatedDate = DateTimeOffset.Now;
                attachments.UserId = ticketsModel.OwnerUserId;

                db.AttachmentData.Add(attachments);
                db.TicketsData.Add(ticketsModel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.ProjectsData, "Id", "Name", ticketsModel.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPrioritiesData, "Id", "Name", ticketsModel.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatusesData, "Id", "Name", ticketsModel.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypesData, "Id", "Name", ticketsModel.TicketTypeId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticketsModel.OwnerUserId);
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticketsModel.AssignedToUserId);
            ViewBag.Comments = new SelectList(db.Users, "Id", "Comment", ticketsModel.Comments);

            return View(ticketsModel);
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int id) {

            TicketsModel ticketsModel = db.TicketsData.Find(id);
            if (ticketsModel == null) {
                return HttpNotFound();
            }
            var projectId = db.ProjectsData.Find(ticketsModel.ProjectId);
            var projectUser = projectId.Users.ToList();
            var projUserList = new List<ApplicationUser>();

            List<ApplicationUser> projdevs = new List<ApplicationUser>();
            UserRolesHelper helper = new UserRolesHelper();

            foreach (var user in projectId.Users) {
                if (helper.IsUserInRole(user.Id, "Developer")) {
                    projdevs.Add(user);
                }
            }

            ViewBag.ProjectId = new SelectList(db.TicketPrioritiesData, "Id", "Name", ticketsModel.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPrioritiesData, "Id", "Name", ticketsModel.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatusesData, "Id", "Name", ticketsModel.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypesData, "Id", "Name", ticketsModel.TicketTypeId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticketsModel.OwnerUserId);
            ViewBag.AssignedToUserId = new SelectList(projdevs.ToList(), "Id", "DisplayName", ticketsModel.AssignedToUserId);
            ViewBag.Comments = new SelectList(db.Users, "Id", "Comment", ticketsModel.Comments);

            return View(ticketsModel);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketsModel ticketsModel) {
           

            if (ModelState.IsValid) {

                ticketsModel.UpdatedDate = DateTimeOffset.Now;

                db.TicketsData.Attach(ticketsModel);
                
                db.Entry(ticketsModel).Property(p => p.AssignedToUserId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.Description).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.Title).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.TicketTypeId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.TicketStatusId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.TicketPriorityId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.ProjectId).IsModified = true;
                //db.Entry(ticketsModel).Property(p => p.OwnerUserId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.UpdatedDate).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerUserId = new SelectList(db.Users,"Id","DisplayName",ticketsModel.OwnerUserId);
            ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName", ticketsModel.AssignedToUserId);
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

        [HttpPost]
        public ActionResult AddComment(TicketCommentsModel newComment) {
            if (ModelState.IsValid) {
                newComment.CreatedDate = System.DateTimeOffset.Now;
                newComment.UpdatedDate = null;
                newComment.UserId = User.Identity.GetUserId();
                db.TicketCommentsData.Add(newComment);
                db.SaveChanges();
            }
            return RedirectToAction("Details",new { id = newComment.TicketId });
        }


        [HttpPost]
        public ActionResult EditComment(TicketCommentsModel editComment) {

            if (ModelState.IsValid) {
                // the Comments refers to the Model IdentityModels - public DbSet<Comment> Comments { get; set; }.
                if (!db.TicketCommentsData.Local.Any(c => c.Id == editComment.Id))
                    db.TicketCommentsData.Attach(editComment);

                db.Entry(editComment).Property(p => p.Comment).IsModified = true;
                editComment.UpdatedDate = System.DateTimeOffset.Now;

                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = editComment.TicketId });
        }

        [HttpPost]
        public ActionResult DeleteComment(TicketCommentsModel deleteComment) {

            if (ModelState.IsValid) {
                if (!db.TicketCommentsData.Local.Any(c => c.Id == deleteComment.Id))
                    db.TicketCommentsData.Attach(deleteComment);

                db.TicketCommentsData.Remove(deleteComment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = deleteComment.TicketId });
        }


        //Add Attachment
        [HttpPost]
        public ActionResult AddAttachment(TicketAttachmentsModel newAttachment, HttpPostedFileBase file) {

            if (file != null && file.ContentLength > 0) {
                var ext = Path.GetExtension(file.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && 
                    ext != ".bmp" && ext != ".txt" && ext != ".doc" && ext != ".docx" && 
                    ext != ".pdf" && ext != ".rtf" && ext != ".wps")
                    ModelState.AddModelError("image", "Invalid Image Type.");
            }
            if (ModelState.IsValid) {
                if (file != null && file.ContentLength > 0) {
                    var filePath = "/Uploads/";
                    var absPath = Server.MapPath("~" + filePath);
                    newAttachment.FileUrl = filePath + file.FileName;
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                }
                //db.AttachmentData.Add(newAttachment);

                newAttachment.CreatedDate = System.DateTimeOffset.Now;
                newAttachment.UpdatedDate = null;
                newAttachment.UserId = User.Identity.GetUserId();
                db.AttachmentData.Add(newAttachment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = newAttachment.TicketId });
        }


        [HttpPost]
        public ActionResult EditAttachment(TicketAttachmentsModel editAttachment, HttpPostedFileBase file) {
            
            if(!db.AttachmentData.Local.Any(c => c.Id == editAttachment.Id))
                db.AttachmentData.Attach(editAttachment);

            if (ModelState.IsValid) {
                if(db.Entry(editAttachment).Property("FileUrl").IsModified == true) {

                    if(file != null && file.ContentLength > 0) {
                        var ext = Path.GetExtension(file.FileName).ToLower();

                        if(ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" &&
                            ext != ".bmp" && ext != ".txt" && ext != ".doc" && ext != ".docx" &&
                            ext != ".pdf" && ext != ".rtf" && ext != ".wps")
                            ModelState.AddModelError("image","Invalid Image Type.");
                    }

                    if(file != null && file.ContentLength > 0) {
                        var filePath = "/Uploads/";
                        var absPath = Server.MapPath("~" + filePath);
                        editAttachment.FileUrl = filePath + file.FileName;
                        file.SaveAs(Path.Combine(absPath,file.FileName));
                    }

                }                

                if(editAttachment.FileUrl == null) {
                    editAttachment.CreatedDate = System.DateTimeOffset.Now;
                }                

                db.Entry(editAttachment).Property("Description").IsModified = true;
                db.Entry(editAttachment).Property("FileUrl").IsModified = true;
                editAttachment.UpdatedDate = System.DateTimeOffset.Now;                
                editAttachment.UserId = User.Identity.GetUserId();                
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = editAttachment.TicketId });
        }


        [HttpPost]
        public ActionResult DeleteAttachment(TicketAttachmentsModel deleteAttachment) {

            if (ModelState.IsValid) {
                if (!db.AttachmentData.Local.Any(c => c.Id == deleteAttachment.Id))
                    db.AttachmentData.Attach(deleteAttachment);

                db.AttachmentData.Remove(deleteAttachment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = deleteAttachment.TicketId });
        }
    }
}
