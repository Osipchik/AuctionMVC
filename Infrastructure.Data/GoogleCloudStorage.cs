using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Domain.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
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

        public async Task<string> UploadFileAsync(Stream source, string filename, string contentType)
        {
            // await using var memoryStream = new MemoryStream();
            // await imageFile.CopyToAsync(memoryStream);
            
            var dataObject = await _storageClient.UploadObjectAsync(
                    _bucketName, 
                    filename, 
                    contentType, 
                    source);

            return Regex.Replace(dataObject.MediaLink, "(download.*?b/)|(/o)|(\\?.*)", "");
        }

        public async Task DeleteFileAsync(string imageUrl)
        {
            var filename = imageUrl.Substring(imageUrl.LastIndexOf("/") + 1);

            try
            {
                await _storageClient.DeleteObjectAsync(_bucketName, filename);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public string CreateFileName(string fileName, string userId)
        {
            return $"{userId}-{Guid.NewGuid()}{Path.GetExtension(fileName)}";
        }
    }
}