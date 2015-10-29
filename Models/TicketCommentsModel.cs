using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models {
    public class TicketCommentsModel {

        public int Id { get; set; }

        public string AuthorId { get; set; }

        [Display(Name = "Ticket Comment")]
        public string Comment { get; set; }

        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "{0:MMM dd yyyy}")]
        public DateTimeOffset CreatedDate { get; set; }

        [Display(Name = "Date Updated")]
        [DisplayFormat(DataFormatString = "{0:MMM dd yyyy}")]
        public Nullable<DateTimeOffset> UpdatedDate { get; set; }

        public int TicketId { get; set; }
        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual TicketsModel Ticket { get; set; }

    }
}