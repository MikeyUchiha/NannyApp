using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace NannyApp.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public DateTime BirthDate { get; internal set; }
        public string Country { get; internal set; }
        public DateTime EmailLinkDate { get; internal set; }
        public string FirstName { get; internal set; }
        public DateTime JoinDate { get; internal set; }
        public DateTime LastLoginDate { get; internal set; }
        public string LastName { get; internal set; }

        public FilePath ProfilePhoto { get; set; }
    }
}
