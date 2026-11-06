

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace FoodOrdering.Infrastructure.Services
{
public class ImageManagementService : IImageManagementService
{
    private readonly IFileProvider _fileProvider;

    public ImageManagementService(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public async Task<string> AddImageAsync(IFormFile file, string src)
    {
        if (file == null || file.Length == 0)
            return string.Empty;

        return await SaveImageToFile(file, src);
    }

    public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
    {
        var saveImageSrc = new List<string>();

        foreach (var item in files)
        {
            var path = await AddImageAsync(item, src); 
            if (!string.IsNullOrEmpty(path))
            {
                saveImageSrc.Add(path);
            }
        }
        return saveImageSrc;
    }

    private async Task<string> SaveImageToFile(IFormFile file, string src)
    {
        var imageDirectory = Path.Combine("wwwroot", "Images", src);

        if (!Directory.Exists(imageDirectory))
        {
            Directory.CreateDirectory(imageDirectory);
        }

        var imageName = $"{Guid.NewGuid()}_{file.FileName}";
        var imageSrc = $"/Images/{src}/{imageName}";
        var root = Path.Combine(imageDirectory, imageName);

        using (FileStream stream = new FileStream(root, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return imageSrc;
    }
        
    public void DeleteImageAsync(string src)
    {
        var info = _fileProvider.GetFileInfo(src);
        if (info.Exists)
        {
            var root = info.PhysicalPath;
            if (File.Exists(root)) File.Delete(root);
        }
    }
}
}
