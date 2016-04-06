using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public ConnectionType ConnectionType { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int FamilyId { get; set; }
        public Family Family { get; set; }
    }
}
