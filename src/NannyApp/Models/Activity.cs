using System;
using System.Collections.Generic;

namespace NannyApp.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool HasStartTime { get; set; }
        public bool HasEndTime { get; set; }
        public string ActitivityNotes { get; set; }

        public ICollection<FilePath> Photos { get; set; }
    }
}