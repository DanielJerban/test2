using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class DocumentsAccessTypeRepository : Repository<DocumentsAccessType>, IDocumentsAccessTypeRepository
{
    public DocumentsAccessTypeRepository(BpmsDbContext context) : base(context)
    {
    }

    public BpmsDbContext DbContext => Context;

    //private static Lazy<IEnumerable<ControllerDescription>> _controllerList =
    //    new Lazy<IEnumerable<ControllerDescription>>();

    //private static Lazy<IEnumerable<ControllerPolicyDTO>> _controllerPolicies =
    //    new Lazy<IEnumerable<ControllerPolicyDTO>>();
}