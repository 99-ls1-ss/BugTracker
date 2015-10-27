using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker.Helpers {
    public class UserProjectRolesHelper {

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsOnProject(string userId, int projectId) {
            if (db.ProjectsData.Find(projectId).Users.Contains(db.Users.Find(userId))) {
                return true;
            }
            return false;
        }


        public void AddUserToProject(string userId, int projectId) {
            if (!IsOnProject(userId, projectId)) {
                var project = db.ProjectsData.Find(projectId);
                project.Users.Add(db.Users.Find(userId));
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public ICollection<ApplicationUser> ListUsersOnProject(int projectId) {
            return db.ProjectsData.Find(projectId).Users;
        }

        public ICollection<ProjectsModel> ListProjectsForUsers(string userId) {
            return db.Users.Find(userId).Projects;
        }

        public ICollection<ApplicationUser> ListUsersNotOnProjects(int projectId) {
            return db.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToList();
        }

    }
}