using NannyApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public class FamilyPhoto : FilePath
    {
        public Family Family { get; set; }

        public FamilyPhoto(Family family)
        {
            FileType = FileType.FamilyPhoto;
            Family = family;
        }
    }
}
