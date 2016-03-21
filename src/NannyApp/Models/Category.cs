using System.Collections.Generic;

namespace NannyApp.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int GroupId { get; set; }
        public Group Group { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; }
    }
}