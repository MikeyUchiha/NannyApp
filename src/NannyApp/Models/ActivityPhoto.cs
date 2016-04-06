using NannyApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public class ActivityPhoto : FilePath
    {
        public Activity Activity { get; set; }

        public ActivityPhoto()
        {
            FileType = FileType.ActivityPhoto;
        }
    }
}
