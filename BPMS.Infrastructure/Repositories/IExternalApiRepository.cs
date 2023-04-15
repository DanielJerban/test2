using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;
using RestSharp;

namespace BPMS.Infrastructure.Repositories;

public interface IExternalApiRepository : IRepository<ExternalApi>
{
    SystemApiResultViewModel TestApi(ExternalApiViewModel model, string webRootPath, bool useInServiceTask = false);
    SystemApiResultViewModel TestApiById(Guid externalApiId, dynamic work, string webRootPath, bool useInServiceTask = false);
    string ExecuteRequest(RestResponse result, bool useInGrid);
    IEnumerable<ExternalApiViewModel> GetTableData();
    List<DropdownViewModel<Guid>> GetAllExternalApiAsDropdown();
}