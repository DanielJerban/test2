using BPMS.Domain.Common.ViewModels;

namespace BPMS.Infrastructure.Services;

public interface IExternalApiService
{
    SystemApiResultViewModel TestApiById(Guid externalApiId, dynamic work, string webRootPath, bool useInServiceTask = false);
}