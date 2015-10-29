using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace BugTracker.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM dd yyyy}")]
        public DateTimeOffset CreatedDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM dd yyyy}")]
        public Nullable<DateTimeOffset> ModifiedDate { get; set; }

        public ApplicationUser() {
            this.Ticket = new HashSet<TicketsModel>();
        }

        public virtual ICollection<TicketsModel> Ticket { get; set; }
        public virtual ICollection<ProjectsModel> Projects { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketAttachmentsModel> AttachmentData { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.ProjectsModel> ProjectsData { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketsModel> TicketsData { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketPrioritiesModel> TicketPrioritiesData { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketStatusesModel> TicketStatusesData { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketTypesModel> TicketTypesData { get; set; }

        public System.Data.Entity.DbSet<BugTracker.Models.TicketCommentsModel> TicketCommentsData { get; set; }

    }
}