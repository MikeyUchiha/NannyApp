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
        ProfilePhoto = 1,
        [Display(Name = "Family Photo")]
        FamilyPhoto,
        [Display(Name = "Child Photo")]
        ChildPhoto,
        [Display(Name = "Activity Photo")]
        ActivityPhoto
    }
}
