using BPMS.Domain.Common.Dtos;

namespace BPMS.Infrastructure.Services;

public interface IAutomationService
{
    List<LookUpPhpDto> GetLookUpsFromAutomation(string type, string baseUrl);
}