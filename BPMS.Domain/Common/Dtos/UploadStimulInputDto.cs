using Microsoft.AspNetCore.Http;

namespace BPMS.Domain.Common.Dtos;

public class UploadStimulInputDto
{
    public IFormFile File { get; set; }
}