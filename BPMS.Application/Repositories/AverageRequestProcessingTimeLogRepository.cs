using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class AverageRequestProcessingTimeLogRepository : Repository<AverageRequestProcessingTimeLog>, IAverageRequestProcessingTimeLogRepository
{
    public AverageRequestProcessingTimeLogRepository(BpmsDbContext context) : base(context)
    {
    }
}