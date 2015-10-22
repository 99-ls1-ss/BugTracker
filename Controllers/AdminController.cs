using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using BugTracker.Helpers;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using System.IO;
using System.Net;


namespace BugTracker.Controllers {
    public class AdminController : Controller {

        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index() {

            ViewBag.Roles = db.Roles.ToDictionary(k => k.Id, v => v.Name);
            return View(db.Users.ToList());
        }

        // GET: UserRoles
        public ActionResult EditUser(string id) {

            var user = db.Users.Find(id);
            var selected = user.Roles.Select(r => r.RoleId);

            AdminUserViewModel adminModel = new AdminUserViewModel() {
                Roles = new MultiSelectList(db.Roles, "Id", "Name", selected),
                UserList = new SelectList(db.Users, "Id", "Name", selected),
                User = user
            };

            return View(adminModel);
        }

        // POST: UserRoles
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditUser(string id, AdminUserViewModel model) {

            if (id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(id);
            UserRolesHelper helper = new UserRolesHelper();

            foreach (var role in db.Roles.ToList()) {
                //If selected but user doesn't have, add
                if (model.SelectedRoles.Contains(role.Id)) {
                    helper.AddUserToRole(id, role.Name);
                }
                //If not selected but user *does* have, remove
                else if (!model.SelectedRoles.Contains(role.Id)) {
                    helper.RemoveUserFromRole(id, role.Name);
                }
            }

            return RedirectToAction("Index", new { id });
        }
    }
}