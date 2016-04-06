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
        public bool HasSubCategory { get; set; }
        public string Group { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string ActitivityNotes { get; set; }

        public ICollection<ActivityPhoto> ActivityPhotos { get; set; }
    }
}