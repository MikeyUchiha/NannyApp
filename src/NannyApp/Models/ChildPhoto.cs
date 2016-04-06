using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public class ChildPhoto : FilePath
    {
        public Child Child { get; set; }

        public ChildPhoto()
        {
            FileType = FileType.ChildPhoto;
        }
    }
}
