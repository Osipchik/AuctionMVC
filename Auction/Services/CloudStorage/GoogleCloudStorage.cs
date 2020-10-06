using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Auction.Services.CloudStorage
{
    public class GoogleCloudStorage : ICloudStorage
    {
        private const int MaxFileSize = 20 * 1024 * 1024;
        private readonly string[] AllowedFileExtensions = {".JPG", ".JPEG", ".WEBP", ".PNG", ".GIF", ".TIFF"};
        
        private readonly StorageClient _storageClient;
        private readonly string _bucketName;

        public GoogleCloudStorage(IConfiguration configuration)
        {
            var googleCredential = GoogleCredential.FromFile(configuration.GetValue<string>("GoogleCredentialFile"));
            _storageClient = StorageClient.Create(googleCredential);
            _bucketName = configuration.GetValue<string>("GoogleCloudStorageBucket");
        }

        public async Task<string> UploadFileAsync(IFormFile imageFile, string filename)
        {
            await using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);
            var dataObject = await _storageClient.UploadObjectAsync(
                    _bucketName, 
                    filename, 
                    imageFile.ContentType, 
                    memoryStream);

            return Regex.Replace(dataObject.MediaLink, "(download.*?b/)|(/o)|(\\?.*)", "");
        }

        public async Task DeleteFileAsync(string imageUrl)
        {
            var filename = imageUrl.Substring(imageUrl.LastIndexOf("/") + 1);
            await _storageClient.DeleteObjectAsync(_bucketName, filename);
        }

        public bool IsFileValid(IFormFile imageFile)
        {
            var sizeIsValid = imageFile.Length <= MaxFileSize;
            var fileExtension = Path.GetExtension(imageFile.FileName).ToUpper();

            return sizeIsValid && AllowedFileExtensions.Contains(fileExtension);
        }

        public string CreateFileName(IFormFile file, string userId)
        {
            return $"{userId}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        }
    }
}