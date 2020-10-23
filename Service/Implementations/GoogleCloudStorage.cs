using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;

namespace Service.Implementations
{
    public class GoogleCloudStorage : ICloudStorage
    {
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

        public string CreateFileName(IFormFile file, string userId)
        {
            return $"{userId}-{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        }
    }
}