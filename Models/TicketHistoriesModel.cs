using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models {
    public class TicketHistoriesModel {

        public int Id { get; set; }
        public int TicketId { get; set; }
        [Display(Name = "Field Name")]
        public string Property { get; set; }
        [Display(Name = "Previous Value")]
        public string OldValue { get; set; }
        [Display(Name = "New Value")]
        public string NewValue { get; set; }
        [Display(Name = "Date Changed")]
        [DisplayFormat(DataFormatString = "{0:MMM dd yyyy}")]
        public DateTimeOffset ChangedDate { get; set; }
        public int UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual TicketsModel Ticket { get; set; }

    }
}