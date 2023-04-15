using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class ExternalApiService : IExternalApiService
{
    private readonly IUnitOfWork _unitOfWork;
    public ExternalApiService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public SystemApiResultViewModel TestApiById(Guid externalApiId, dynamic work, string webRootPath, bool useInServiceTask = false)
    {
        return _unitOfWork.ExternalApis.TestApiById(externalApiId, work, webRootPath, useInServiceTask);
    }
}