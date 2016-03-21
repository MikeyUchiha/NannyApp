using System.Collections.Generic;

namespace NannyApp.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<Category> Categories { get; set; }
    }
}