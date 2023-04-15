using BPMS.Infrastructure.Services;
using Microsoft.AspNetCore.Http;

namespace BPMS.Application.Services;

public class FileService : IFileService
{
    public void Upload(IFormFile file, string path)
    {
        if (file.Length <= 0) return;
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string filePath = Path.Combine(path, file.FileName);
        using Stream fileStream = new FileStream(filePath, FileMode.Create);
        file.CopyTo(fileStream);
    }

    public void RemoveDirectory(string path)
    {
        if (!Directory.Exists(path)) return;

        var di = new DirectoryInfo(path);
        foreach (var file in di.GetFiles())
        {
            file.Delete();
        }
        Directory.Delete(path);
    }
}