using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BugTracker.Models {
    public class TicketAttachmentsModel {

        public int Id { get; set; }
        public int TicketId { get; set; }
        [Display(Name = "File Path")]
        public string FilePath { get; set; }
        [Display(Name = "Attachment Description")]
        public string Description { get; set; }
        [Display(Name = "Date Added")]
        [DisplayFormat(DataFormatString = "{0:MMM dd yyyy}")]
        public DateTimeOffset CreatedDate { get; set; }
        public int UserId { get; set; }
        public string FileUrl { get; set; }

        public virtual TicketsModel Ticket { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}