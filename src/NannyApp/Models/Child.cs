using System.Collections.Generic;

namespace NannyApp.Models
{
    public class Child
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public string Color { get; set; }
        public string Allergies { get; set; }
        public string ChildNotes { get; set; }
        public FilePath ChildPhoto { get; set; }

        public ICollection<Activity> Activities { get; set; }
    }
}