using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace NannyApp.Services
{
    public interface IFileService
    {
        void CreateAndConfigureAsync(string container, bool givePublicAccess);
        string UploadFile(IFormFile fileToUpload, string container, string fileName, string contentType = null);
        void DeleteFile(string container, string fileName);
    }
}
