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
            var user = db.Users.Include(p => p.Projects).FirstOrDefault(u => u.Id == userId);

            if (User.IsInRole("Admin")) {
                tickets = tickets;
            }
            else if ((User.IsInRole("ProjectManager")) || (User.IsInRole("Developer"))) {
                tickets = user.Projects.SelectMany(p => p.Tickets).AsQueryable();
            }
            else if (User.IsInRole("Submitter")) {
                tickets = db.TicketsData.Where(t => t.OwnerUserId == user.Id);
            }

            return View(tickets.ToList());
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id) {

            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketsModel ticketsModel = db.TicketsData.Find(id);
            if (ticketsModel == null) {
                return HttpNotFound();
            }
            return View(ticketsModel);
        }


        // GET: Tickets/Create
        public ActionResult Create() {
            TicketsModel ticketsModel = new TicketsModel();
            if (ticketsModel.AssignedToUserId == null) {
                ViewBag.AssignedToUserId = "Not Assigned Yet";
            }
            else {
                ViewBag.AssignedToUserId = new SelectList(db.Users, "Id", "DisplayName");
            }
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

            TicketHistoriesModel histories = new TicketHistoriesModel();
            TicketAttachmentsModel attachments = new TicketAttachmentsModel();
            if (ModelState.IsValid) {

                if (file != null && file.ContentLength > 0) {
                    var filePath = "/Uploads/";
                    var absPath = Server.MapPath("~" + filePath);
                    attachments.FileUrl = filePath + file.FileName;
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                    attachments.CreatedDate = DateTimeOffset.Now;
                    //attachments.Description = new at
                    attachments.UserId = ticketsModel.OwnerUserId;
                    db.AttachmentData.Add(attachments);
                }

                ticketsModel.CreatedDate = DateTimeOffset.Now;
                ticketsModel.UpdatedDate = null;
                if (ticketsModel.AssignedToUserId == null) {
                    //if ticket is unassigned when it is created it is assigned to the Project Manager
                    ticketsModel.AssignedToUserId = "43c6b827-3faf-4afa-bb24-7b937e623052";
                }
                
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
            ViewBag.Historys = new SelectList(db.TicketHistoriesData, "Id", "Property", ticketsModel.History);

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

            ViewBag.ProjectId = new SelectList(db.ProjectsData, "Id", "Name", ticketsModel.ProjectId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPrioritiesData, "Id", "Name", ticketsModel.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatusesData, "Id", "Name", ticketsModel.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypesData, "Id", "Name", ticketsModel.TicketTypeId);
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticketsModel.OwnerUserId);
            ViewBag.AssignedToUserId = new SelectList(projdevs.ToList(), "Id", "DisplayName", ticketsModel.AssignedToUserId);
            ViewBag.Comments = new SelectList(db.Users, "Id", "Comment", ticketsModel.Comments);
            ViewBag.Historys = new SelectList(db.TicketHistoriesData, "Id", "Property", ticketsModel.History);

            return View(ticketsModel);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TicketsModel ticketsModel) {

            if (ModelState.IsValid) {

                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var dateChanged = System.DateTimeOffset.Now;
                var oldTicket = db.TicketsData.AsNoTracking().FirstOrDefault(t => t.Id == ticketsModel.Id);
                var assignedUser = db.Users.Find(ticketsModel.AssignedToUserId);

                //Title Notification
                if (oldTicket.Title != ticketsModel.Title) {
                    TicketHistoriesModel histTitle = new TicketHistoriesModel {
                        TicketId = ticketsModel.Id,
                        Property = "Title",
                        OldValue = oldTicket.Title,
                        NewValue = ticketsModel.Title,
                        ChangedDate = dateChanged,
                        UserId = userId
                    };
                    db.TicketHistoriesData.Add(histTitle);
                }

                //Ticket Title Description
                if (oldTicket.Description != ticketsModel.Description) {
                    TicketHistoriesModel histDescription = new TicketHistoriesModel {
                        TicketId = ticketsModel.Id,
                        Property = "Description",
                        OldValue = oldTicket.Description,
                        NewValue = ticketsModel.Description,
                        ChangedDate = dateChanged,
                        UserId = userId
                    };
                    db.TicketHistoriesData.Add(histDescription);
                }

                //Ticket Assigned To Notification
                if (oldTicket.AssignedToUserId != ticketsModel.AssignedToUserId) {
                    TicketHistoriesModel histAssignedTo = new TicketHistoriesModel {
                        TicketId = ticketsModel.Id,
                        Property = "Assigned To",
                        OldValue = db.Users.Find(oldTicket.AssignedToUserId).DisplayName,
                        NewValue = db.Users.Find(ticketsModel.AssignedToUserId).DisplayName,
                        ChangedDate = dateChanged,
                        UserId = userId
                    };
                    db.TicketHistoriesData.Add(histAssignedTo);

                    //Send Notification for Assigned To Change
                    TicketNotificationsModel ticketNotification = new TicketNotificationsModel {
                        TicketId = ticketsModel.Id,
                        UserId = User.Identity.GetUserId(),
                        Sender = user.Email,
                        Recipient = assignedUser.Email,
                        NotificationSubject = "Ticket \"" + ticketsModel.Title + "\" has been changed.",
                        NotificationMessage = "The Ticket <b>" + ticketsModel.Title + "</b> was reassigned to " + user.DisplayName + ".<br /><br />" +
                            "Previous Assignee: " + histAssignedTo.OldValue + "<br />" +
                            "New Assignee: " + histAssignedTo.NewValue + ".<br /><br />" +
                            "This change was made by " + user.DisplayName + ".",
                        SentDate = DateTimeOffset.Now
                    };
                    db.TicketNotificationsData.Add(ticketNotification);
                    assignedUser.SendNotification(ticketNotification.NotificationSubject, ticketNotification.NotificationMessage);
                }

                //Ticket Type Notification
                if (oldTicket.TicketTypeId != ticketsModel.TicketTypeId) {
                    TicketHistoriesModel histTicketType = new TicketHistoriesModel {
                        TicketId = ticketsModel.Id,
                        Property = "Ticket Type",
                        OldValue = db.TicketTypesData.Find(oldTicket.TicketTypeId).Name,
                        NewValue = db.TicketTypesData.Find(ticketsModel.TicketTypeId).Name,
                        ChangedDate = dateChanged,
                        UserId = userId
                    };
                    db.TicketHistoriesData.Add(histTicketType);
                }

                //Ticket Priority Notification
                if (oldTicket.TicketPriorityId != ticketsModel.TicketPriorityId) {

                    TicketHistoriesModel histPriority = new TicketHistoriesModel {
                        TicketId = ticketsModel.Id,
                        Property = "Ticket Priority",
                        OldValue = db.TicketPrioritiesData.Find(oldTicket.TicketPriorityId).Name,
                        NewValue = db.TicketPrioritiesData.Find(ticketsModel.TicketPriorityId).Name,
                        ChangedDate = dateChanged,
                        UserId = userId
                    };
                    db.TicketHistoriesData.Add(histPriority);

                    //Send Notification for Priority Change
                    TicketNotificationsModel ticketNotification = new TicketNotificationsModel {
                        TicketId = ticketsModel.Id,
                        UserId = User.Identity.GetUserId(),
                        Sender = user.Email,
                        Recipient = assignedUser.Email,
                        NotificationSubject = "The Priority for the Ticket titled \"" + ticketsModel.Title + "\" has been changed",
                        NotificationMessage = "The Ticket titled <b>" + ticketsModel.Title + "</b> has had its Priority changed. <br /><br />" +
                            "Previous Priority: " + histPriority.OldValue + "<br />" +
                            "New Priority: " + histPriority.NewValue + ".<br /><br />" +
                            "The change was made by " + user.DisplayName + ".",
                        SentDate = DateTimeOffset.Now
                    };
                    db.TicketNotificationsData.Add(ticketNotification);
                    assignedUser.SendNotification(ticketNotification.NotificationSubject, ticketNotification.NotificationMessage);
                }

                //Ticket Status Notification
                if (oldTicket.TicketStatusId != ticketsModel.TicketStatusId) {
                    TicketHistoriesModel histStatus = new TicketHistoriesModel {
                        TicketId = ticketsModel.Id,
                        Property = "Ticket Status",
                        OldValue = db.TicketStatusesData.Find(oldTicket.TicketStatusId).Name,
                        NewValue = db.TicketStatusesData.Find(ticketsModel.TicketStatusId).Name,
                        ChangedDate = dateChanged,
                        UserId = userId
                    };
                    db.TicketHistoriesData.Add(histStatus);

                    //Send Notification for Priority Change
                    TicketNotificationsModel ticketNotification = new TicketNotificationsModel {
                        TicketId = ticketsModel.Id,
                        UserId = User.Identity.GetUserId(),
                        Sender = user.Email,
                        Recipient = assignedUser.Email,
                        NotificationSubject = "The Status for the Ticket titled \"" + ticketsModel.Title + "\" has been changed",
                        NotificationMessage = "The Ticket titled <b>" + ticketsModel.Title + "</b> has had its Status changed. <br /><br />" +
                                    "Previous Status: " + histStatus.OldValue + "<br />" +
                                    "New Status: " + histStatus.NewValue + ".<br /><br />" +
                                    "The change was made by " + user.DisplayName + ".",
                        SentDate = DateTimeOffset.Now
                    };
                    db.TicketNotificationsData.Add(ticketNotification);
                    assignedUser.SendNotification(ticketNotification.NotificationSubject, ticketNotification.NotificationMessage);

                    if ((db.Entry(ticketsModel).Property(p => p.AssignedToUserId).IsModified == true) && (assignedUser.Roles.Any(r => r.RoleId == "c8d38989-258c-484b-9606-6f733c273059"))) { 
                        assignedUser.SendNotification(
                            "You have been assigned a Ticket",
                            "The Ticket titled <b>" + ticketsModel.Title + "</b> has been assigned to you. <br /><br />" +
                            "The change was made by " + user.DisplayName + "."
                            );
                    }
                }

                ticketsModel.UpdatedDate = DateTimeOffset.Now;

                db.TicketsData.Attach(ticketsModel);
                db.Entry(ticketsModel).Property(p => p.AssignedToUserId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.Description).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.Title).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.TicketTypeId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.TicketStatusId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.TicketPriorityId).IsModified = true;
                db.Entry(ticketsModel).Property(p => p.UpdatedDate).IsModified = true;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerUserId = new SelectList(db.Users, "Id", "DisplayName", ticketsModel.OwnerUserId);
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
            return RedirectToAction("Details", new { id = newComment.TicketId });
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
                    editAttachment.FileUrl = filePath + file.FileName;
                    file.SaveAs(Path.Combine(absPath, file.FileName));
                }

                editAttachment.UpdatedDate = System.DateTimeOffset.Now;
                editAttachment.UserId = User.Identity.GetUserId();
                db.AttachmentData.Attach(editAttachment);
                db.Entry(editAttachment).Property("Description").IsModified = true;
                db.Entry(editAttachment).Property("FileUrl").IsModified = true;
                db.Entry(editAttachment).Property("UpdatedDate").IsModified = true;
                db.Entry(editAttachment).Property("UserId").IsModified = true;

                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = editAttachment.TicketId });
        }


        [HttpPost]
        public ActionResult DeleteAttachment(TicketAttachmentsModel deleteAttachment, HttpPostedFileBase file) {

            if (ModelState.IsValid) {
                db.AttachmentData.Attach(deleteAttachment);
                db.AttachmentData.Remove(deleteAttachment);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = deleteAttachment.TicketId });
        }
    }
}
