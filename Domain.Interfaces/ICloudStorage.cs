using System.IO;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ICloudStorage
    {
        Task<string> UploadFileAsync(Stream source, string filename, string contentType);
        
        Task DeleteFileAsync(string imageUrl);
        
        string CreateFileName(string fileName, string userId);
    }
}