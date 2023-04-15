using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class FormClassificationCreatorsRepository : Repository<FormClassificationCreators>, IFormClassificationCreatorsRepository
{
    public FormClassificationCreatorsRepository(BpmsDbContext context) : base(context)
    {
    }

}