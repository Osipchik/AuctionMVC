using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Service
{
    public interface ICloudStorage
    {
        Task<string> UploadFileAsync(IFormFile imageFile, string filename);
        
        Task DeleteFileAsync(string imageUrl);
        
        string CreateFileName(IFormFile imageFile, string userId);
    }
}