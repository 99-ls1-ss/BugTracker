using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BugTracker.Models;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models {
    public class TicketNotificationsModel {

        public int Id { get; set; }
        public int TicketId { get; set; }

        public string UserId { get; set; }

        [Display(Name = "Sender")]
        public string Sender { get; set; }

        [Display(Name = "Recipient")]
        public string Recipient { get; set; }

        [Display(Name = "Notification Subject")]
        public string NotificationSubject { get; set; }

        [Display(Name = "Notification Message")]
        public string NotificationMessage { get; set; }

        [Display(Name = "Sent Date")]
        [DisplayFormat(DataFormatString = "{0:MMM dd yyyy hh:mm tt}")]
        public DateTimeOffset SentDate { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual TicketsModel Ticket { get; set; }

    }
}