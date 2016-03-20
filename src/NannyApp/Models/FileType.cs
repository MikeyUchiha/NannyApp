using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public enum FileType
    {
        [Display(Name = "Profile Photo")]
        Profile_Photo = 1,
        Photo
    }
}
