namespace BPMS.Domain.Common.Dtos;

public class ServiceResultDTO
{
    public bool Success { get; set; } = false;
    public string Message { get; set; }
    public object Data { get; set; }
}