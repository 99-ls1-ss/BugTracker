using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models {
    public class TicketStatusesModel {

        public int Id { get; set; }
        [Display(Name = "Ticket Status")]
        public string Name { get; set; }

    }
}