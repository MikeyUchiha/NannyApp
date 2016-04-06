using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Models.Interfaces
{
    public interface IFilePath
    {
        int Id { get; set; }
        string FileName { get; set; }
        string FileUrl { get; set; }
        FileType FileType { get; set; }
        DateTime DateUploaded { get; set; }
    }
}
