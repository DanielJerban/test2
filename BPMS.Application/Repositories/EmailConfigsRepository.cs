using BPMS.Application.Repositories.Base;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;

namespace BPMS.Application.Repositories;

public class EmailConfigsRepository : Repository<EmailConfigs>, IEmailConfigsRepository
{
    public EmailConfigsRepository(BpmsDbContext context) : base(context)
    {
    }

    BpmsDbContext _context => Context;

    public void AddConfig(EmailConfigs configs)
    {
        _context.EmailConfigurations.Add(configs);
        _context.SaveChanges();
    }

    public IEnumerable<EmailConfigs> GetConfigs()
    {
        return _context.EmailConfigurations;
    }

    public void Remove(Guid id)
    {
        var entity = _context.EmailConfigurations.SingleOrDefault(a => a.Id == id);
        _context.EmailConfigurations.Remove(entity);
        _context.SaveChanges();
    }

    public void UpdateConfig(EmailConfigs configs)
    {
        var result = _context.EmailConfigurations.SingleOrDefault(a => a.Id == configs.Id);
        if (result != null)
        {
            result.SmtpServerUrl = configs.SmtpServerUrl;
            result.PortNumber = configs.PortNumber;
            result.Username = configs.Username;
            result.Password = configs.Password;
            result.SslRequired = configs.SslRequired;
            result.IsActive = configs.IsActive;
        }
        _context.SaveChanges();
    }

    public void DeactivePrevious()
    {
        if (_context.EmailConfigurations.Any())
        {
            _context.EmailConfigurations.Where(a => a.IsActive).ToList().ForEach(a => a.IsActive = false);
        }
    }

    public EmailConfigs GetActiveConfig()
    {
        return _context.EmailConfigurations.FirstOrDefault(a => a.IsActive);
    }
}