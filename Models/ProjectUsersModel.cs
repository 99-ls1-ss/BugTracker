using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.Models {
    public class ProjectUsersModel {

        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }

        public virtual ProjectsModel Project { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}