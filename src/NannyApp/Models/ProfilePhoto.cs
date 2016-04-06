using NannyApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public class ProfilePhoto : FilePath
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ProfilePhoto()
        {
            FileType = FileType.ProfilePhoto;
        }
    }
}
