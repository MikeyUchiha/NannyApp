using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public enum ConnectionType
    {
        Caretaker = 1,
        [Display(Name = "Family Member")]
        FamilyMember,
        Other
    }
}
