using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using System.Web.Mvc;

namespace BugTracker.Models {
    public class ProjectUsersModel {

        
        public ProjectsModel Project { get; set; }
        public MultiSelectList Users { get; set; }
        public string[] SelectedUsers { get; set; }
        public string[] userRoles { get; set; }

    }
}