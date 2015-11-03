using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Helpers {
    public static class Extensions {

        public static void SendNotification(this ApplicationUser user, string subject, string body) {
            EmailService es = new EmailService();
            IdentityMessage im = new IdentityMessage {
                Destination = user.Email,
                Subject = subject,
                Body = body
            };
            es.SendAsync(im);
        }
    }
}