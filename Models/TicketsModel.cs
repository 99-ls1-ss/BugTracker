using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models {
    public class TicketsModel {

        public TicketsModel() {
            this.Comments = new HashSet<TicketCommentsModel>();
            this.History = new HashSet<TicketHistoriesModel>();
            this.Notification = new HashSet<TicketNotificationsModel>();
        }

        public int Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        [Required]
        [AllowHtml]
        [Display(Name = "Description")]
        public string Description { get; set; }
        [Display(Name = "Date Created")]
        public DateTimeOffset CreatedDate { get; set; }
        [Display(Name = "Date Updated")]
        public Nullable<DateTimeOffset> UpdatedDate { get; set; }
        [Display(Name = "Project ID")]
        public int ProjectId { get; set; }
        [Display(Name = "Ticket Type")]
        public int TicketTypeId { get; set; }
        [Display(Name = "Ticket Priority")]
        public int TicketPriorityId { get; set; }
        [Display(Name = "Ticket Status")]
        public int TicketStatusId { get; set; }
        [Display(Name = "Ticket Owner")]
        public string OwnerUserId { get; set; }
        [Display(Name = "Assigned To")]
        public string AssignedToUserId { get; set; }


        public virtual ProjectsModel Project { get; set; }
        public virtual TicketTypesModel TicketType { get; set; }
        public virtual TicketPrioritiesModel TicketPriority { get; set; }
        public virtual TicketStatusesModel TicketStatus { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }
        public virtual ICollection<TicketCommentsModel> Comments { get; set; }
        public virtual ICollection<TicketHistoriesModel> History { get; set; }
        public virtual ICollection<TicketNotificationsModel> Notification { get; set; }

    }
}