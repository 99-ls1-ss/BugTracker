using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models {
    public class TicketTypesModel {

        public int Id { get; set; }
        [Display(Name = "Ticket Type")]
        public string Name { get; set; }

    }
}