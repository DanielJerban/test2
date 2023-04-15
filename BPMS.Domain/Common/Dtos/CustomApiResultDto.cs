namespace BPMS.Domain.Common.Dtos;

public class CustomApiResultDto
{
    public bool Success { get; set; } = false;
    public object Data { get; set; }
    public string Message { get; set; }
}