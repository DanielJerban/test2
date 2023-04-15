using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class FormClassificationAccessRepository : Repository<FormClassificationAccess>, IFormClassificationAccessRepository
{
    public FormClassificationAccessRepository(BpmsDbContext context) : base(context)
    {
    }

    public List<Guid> GetChartByFormClassificationId(Guid id)
    {
        return Context.FormClassificationAccess.Where(d => d.FormClassificationId == id && d.Type == "Chart").Select(d => d.AccessId).ToList();
    }
}