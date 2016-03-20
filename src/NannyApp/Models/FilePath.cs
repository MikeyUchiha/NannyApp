using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public class FilePath
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public FileType FileType { get; set; }
        public DateTime DateUploaded { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? FamilyId { get; set; }
        public Family Family { get; set; }

        public int? ChildId { get; set; }
        public Child Child { get; set; }

        public int? ActivityId { get; set; }
        public Activity Activity { get; set; }
    }
}
