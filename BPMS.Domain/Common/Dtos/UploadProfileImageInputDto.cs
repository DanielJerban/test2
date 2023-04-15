using Microsoft.AspNetCore.Http;

namespace BPMS.Domain.Common.Dtos;

public class UploadProfileImageInputDto
{
    public IFormFile File { get; set; }
}