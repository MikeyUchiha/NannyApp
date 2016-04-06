using NannyApp.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models
{
    public class FilePath : IFilePath
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public FileType FileType { get; set; }
        public DateTime DateUploaded { get; set; }
    }
}
