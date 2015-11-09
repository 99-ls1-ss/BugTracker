namespace BugTracker.Migrations {
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using BugTracker.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.AspNet.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<BugTracker.Models.ApplicationDbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(BugTracker.Models.ApplicationDbContext context) {
            //  This method will be called after migrating to the latest version.

            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin")) {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            };
            if (!context.Roles.Any(r => r.Name == "ProjectManager")) {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            };
            if (!context.Roles.Any(r => r.Name == "Developer")) {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            };
            if (!context.Roles.Any(r => r.Name == "Submitter")) {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            };
            if (!context.Roles.Any(r => r.Name == "Guest")) {
                roleManager.Create(new IdentityRole { Name = "Guest" });
            };


            if (!context.Users.Any(u => u.Email == "brandon@navicamls.net")) {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            };
            if (!context.Users.Any(u => u.Email == "moderator@coderfoundry.com")) {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            };
            if (!context.Users.Any(u => u.Email == "branpayne69@gmail.com")) {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            };
            if (!context.Users.Any(u => u.Email == "guest@coderfoundry")) {
                roleManager.Create(new IdentityRole { Name = "Guest" });
            };

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));

            if (!context.Users.Any(u => u.Email == "brandon@navicamls.net")) {
                userManager.Create(new ApplicationUser {
                    UserName = "brandon@navicamls.net",
                    Email = "brandon@navicamls.net",
                    FirstName = "Brandon",
                    LastName = "Payne",
                    DisplayName = "Brandon Payne",
                    EmailConfirmed = true
                },
                    "billybob");
            };
            
            if (!context.Users.Any(u => u.Email == "branpayne69@gmail.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "branpayne69@gmail.com",
                    Email = "branpayne69@gmail.com",
                    FirstName = "Brandon",
                    LastName = "Payne",
                    DisplayName = "Brandon Payne",
                    EmailConfirmed = true
                },
                    "Password-1");
            };
            if (!context.Users.Any(u => u.Email == "moderator@coderfoundry.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "moderator@coderfoundry.com",
                    Email = "moderator@coderfoundry.com",
                    FirstName = "Coder",
                    LastName = "Foundry",
                    DisplayName = "Coder Foundry",
                    EmailConfirmed = true
                },
                    "Password-1");
            };

            if (!context.Users.Any(u => u.Email == "guest@coderfoundry.com")) {
                userManager.Create(new ApplicationUser {
                    UserName = "guest@coderfoundry.com",
                    Email = "guest@coderfoundry.com",
                    FirstName = "Guest",
                    LastName = "User",
                    DisplayName = "Guest User",
                    EmailConfirmed = true
                },
                    "Password-1");
            };

            var userIdAdmin = userManager.FindByEmail("brandon@navicamls.net").Id;
            userManager.AddToRole(userIdAdmin, "Admin");
            var userIdMod = userManager.FindByEmail("moderator@coderfoundry.com").Id;
            userManager.AddToRole(userIdMod, "ProjectManager");
            var userIdDev = userManager.FindByEmail("branpayne69@gmail.com").Id;
            userManager.AddToRole(userIdDev, "Developer");
            var userIdGuest = userManager.FindByEmail("guest@coderfoundry.com").Id;
            userManager.AddToRole(userIdGuest, "Guest");
        }
    }
}
