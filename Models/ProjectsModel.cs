using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;

namespace BugTracker.Models {
    public class ProjectsModel {

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> User { get; set; }

    }
}