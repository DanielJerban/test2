using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories.Base;

namespace BPMS.Infrastructure.Repositories;

public interface IFormLookUp2NRepository : IRepository<FormLookUp2N>
{
    void CreateNew(FormLookup2NViewModel model);
    void RemoveFormLookUp2N(Guid id);
}