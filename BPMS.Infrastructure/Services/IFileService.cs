using Microsoft.AspNetCore.Http;

namespace BPMS.Infrastructure.Services;

public interface IFileService
{
    void Upload(IFormFile file, string path);
    void RemoveDirectory(string path);
}