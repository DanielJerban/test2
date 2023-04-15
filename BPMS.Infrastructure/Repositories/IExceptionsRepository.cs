using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using Kendo.Mvc.UI;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IExceptionsRepository : IRepository<Exceptions>
{
    Task<DataSourceResult> GetByPaging(DataSourceRequest request);
    IEnumerable<ExceptionsViewModel> GetAllExp();
}