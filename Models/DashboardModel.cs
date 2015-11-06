using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models {
    public class DashboardModel {

        public IList<TicketsModel> Tickets { get; set; }
        public IList<TicketAttachmentsModel> Attachments { get; set; }
        public IList<TicketCommentsModel> Comments { get; set; }
        public IList<TicketPrioritiesModel> Priorities { get; set; }
        public IList<TicketStatusesModel> Statuses { get; set; }
        public IList<TicketTypesModel> Types { get; set; }

    }
}