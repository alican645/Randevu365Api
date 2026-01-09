using Microsoft.AspNetCore.Http;

namespace Randevu365.Application.Interfaces;

public interface IFileService
{
    Task<string> UploadFileAsync(IFormFile file, string folderPath);
    Task<bool> DeleteFileAsync(string filePath);
}
