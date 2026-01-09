using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Randevu365.Application.Interfaces;

namespace Randevu365.Infrastructure.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> UploadFileAsync(IFormFile file, string folderPath)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file");

        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", folderPath);
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return Path.Combine("uploads", folderPath, uniqueFileName).Replace("\\", "/");
    }

    public async Task<bool> DeleteFileAsync(string filePath)
    {
        var fullPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, filePath);
        
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return await Task.FromResult(true);
        }
        
        return await Task.FromResult(false);
    }
}
