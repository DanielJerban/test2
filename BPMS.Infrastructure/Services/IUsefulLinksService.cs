using BPMS.Domain.Common.ViewModels;
using Kendo.Mvc.UI;

namespace BPMS.Infrastructure.Services;

public interface IUsefulLinksService
{
    void AddOrUpdateUsefullLinks(UsefulLinksViewModel usefulLinks);
    bool RemoveUsefulLink(Guid id);
    DataSourceResult GetUsefulLinkList(DataSourceRequest request);
    IEnumerable<UsefulLinksViewModel> GetUsefulLinkForCartable();
}