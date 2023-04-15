using BPMS.Domain.Entities;

namespace BPMS.Infrastructure.Repositories;

public interface IEmailLogRepository
{
    void AddEmailLog(EmailLog model);
    IQueryable<EmailLog> GetAllEmailLogs();
}