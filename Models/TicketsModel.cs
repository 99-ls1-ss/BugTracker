using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models {
    public class TicketsModel {

        public TicketsModel() {
            this.Comment = new HashSet<TicketCommentsModel>();
            this.Attachment = new HashSet<TicketAttachmentsModel>();
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
        public int ProjectId { get; set; }
        public int TicketTypeId { get; set; }
        public int TicketPriorityId { get; set; }
        public int TicketStatusId { get; set; }
        public int OwnerUserId { get; set; }
        public int AssignedToUserId { get; set; }


        public virtual ProjectsModel Project { get; set; }
        public virtual TicketTypesModel TicketType { get; set; }
        public virtual TicketPrioritiesModel TicketPriority { get; set; }
        public virtual TicketStatusesModel TicketStatus { get; set; }
        public virtual ApplicationUser OwnerUser { get; set; }
        public virtual ApplicationUser AssignedToUser { get; set; }


        public virtual ICollection<TicketCommentsModel> Comment { get; set; }
        public virtual ICollection<TicketAttachmentsModel> Attachment { get; set; }
        public virtual ICollection<TicketHistoriesModel> History { get; set; }
        public virtual ICollection<TicketNotificationsModel> Notification { get; set; }

    }
}