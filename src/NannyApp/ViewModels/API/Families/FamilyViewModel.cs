using NannyApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.ViewModels.API.Families
{
    public class FamilyViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GeneralNotes { get; set; }
        public FamilyPhotoViewModel FamilyPhoto { get; set; }

        public IEnumerable<FamilyConnectionViewModel> Connections { get; set; }
    }
}
