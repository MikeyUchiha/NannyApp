using NannyApp.ViewModels.API.Families;
using NannyApp.ViewModels.API.Users;
using System;
using System.Collections.Generic;

namespace NannyApp.ViewModels.Users
{
    public class UserViewModel
    {
        public string Username { get; internal set; }
        public string Email { get; internal set; }
        public string FirstName { get; internal set; }
        public string LastName { get; internal set; }
        public string Country { get; internal set; }
        public DateTime JoinDate { get; internal set; }
        public DateTime LastLoginDate { get; internal set; }
        public ProfilePhotoViewModel ProfilePhoto { get; internal set; }
    }
}
