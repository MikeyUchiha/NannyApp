using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;
using Microsoft.Net.Http.Headers;

namespace NannyApp.Services
{
    public class FileService : IFileService
    {
        private CloudStorageAccount storageAccount;
        private ILogger _logger;

        public FileService(IOptions<FileServiceOptions> options, ILogger<FileService> logger)
        {
            Options = options.Value;
            storageAccount = CloudStorageAccount.Parse(Options.StorageConnectionString);
            _logger = logger;
        }

        public FileServiceOptions Options { get; private set; }

        public async void CreateAndConfigureAsync(string container, bool givePublicAccess)
        {
            try
            {
                // Create a blob client and retrieve a reference to the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);

                // Create the container if it doesn't already exist.
                if(await blobContainer.CreateIfNotExistsAsync())
                {
                    // Enable public access if needed.
                    if(givePublicAccess)
                    {
                        await blobContainer.SetPermissionsAsync(
                            new BlobContainerPermissions
                            {
                                PublicAccess = BlobContainerPublicAccessType.Blob
                            });

                        _logger.LogInformation("Successfully created Blob Storage Container and made it public");
                    }
                    else
                    {
                        _logger.LogInformation("Successfully created Blob Storage Container with default settings");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failure to Create or Configure images container in Blob Storage Service", ex);
            }
        }

        public string UploadFile(IFormFile fileToUpload, string container, string fileName, string contentType = null)
        {
            if (fileToUpload == null || fileToUpload.Length == 0)
            {
                return null;
            }

            string fullPath = null;
            Stopwatch timespan = Stopwatch.StartNew();

            try
            {
                // Create a blob client and retrieve a reference to the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);

                // Retrieve reference to blob
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);
                blockBlob.Properties.CacheControl = "public, no-cache";
                if (contentType == null)
                {
                    blockBlob.Properties.ContentType = fileToUpload.ContentType;
                }
                else
                {
                    blockBlob.Properties.ContentType = contentType;
                }
                try
                {
                    blockBlob.UploadFromStream(fileToUpload.OpenReadStream());
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unable to upload file to blob", ex);
                }

                var uriBuilder = new UriBuilder(blockBlob.Uri);
                uriBuilder.Scheme = "https";
                fullPath = uriBuilder.ToString();

                timespan.Stop();
                _logger.LogInformation(String.Format("Blob Service, FileService.UploadFile lasted {0} on file {1}", timespan.Elapsed, fullPath));
            }
            catch (Exception ex)
            {
                _logger.LogError("Error uploading file blob to storage.", ex);
            }

            return fullPath;
        }

        public void DeleteFile(string container, string fileName)
        {
            try
            {
                // Create a blob client and retrieve a reference to the container
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);

                // Retrieve reference to blob
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);
                try
                {
                    blockBlob.Delete();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unable to delete file from blob", ex);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting file blob from storage.", ex);
            }
        }
    }
}
