using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models {
    public class TicketPrioritiesModel {

        public int Id { get; set; }
        [Display(Name = "Ticket Priority")]
        public string Name { get; set; }

    }
}