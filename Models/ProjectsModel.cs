using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models {
    public class ProjectsModel {

        public int Id { get; set; }
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<TicketsModel> Tickets { get; set; }

    }
}