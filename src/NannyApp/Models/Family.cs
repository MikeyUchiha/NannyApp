using System.Collections.Generic;

namespace NannyApp.Models
{
    public class Family
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GeneralNotes { get; set; }
        public FilePath FamilyPhoto { get; set; }

        public ICollection<Connection> Connections { get; set; }
        public ICollection<Child> Children { get; set; }
    }
}