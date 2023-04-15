using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using BPMS.Infrastructure.Services.Email;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BPMS.Application.Services.Email;

public class EmailConfigService : IEmailConfigService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IServiceProvider _serviceProvider;

    private IDistributedCacheHelper CacheHelper => _serviceProvider.GetRequiredService<IDistributedCacheHelper>();
    public EmailConfigService(IUnitOfWork unitOfWork, ISystemSettingService systemSettingService, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _unitOfWork = unitOfWork;
    }

    public EmailConfigsViewModel GetActiveConfig()
    {
        EmailConfigsViewModel activeConfig = null;
        var configs = GetAllEmailSetting();
        if (configs != null)
        {
            activeConfig = configs.Where(a => a.IsActive).FirstOrDefault();
        }
        return activeConfig;
    }
    public List<EmailConfigsViewModel> GetAllEmailSetting()
    {
        var systemSetting = GetLastSetting(SystemSettingType.Email);
        return systemSetting == null ? null : JsonConvert.DeserializeObject<List<EmailConfigsViewModel>>(systemSetting.Data);
    }

    private SystemSetting GetLastSetting(SystemSettingType type)
    {
        return CacheHelper.GetOrSet(CacheKeyHelper.GetSystemSettingCacheKey(type), () => getLastSystemSetting(type), TimeSpan.FromDays(1));
    }

    private SystemSetting getLastSystemSetting(SystemSettingType type)
    {
        var result = _unitOfWork.SystemSetting.Where(c => c.Type == type).OrderByDescending(c => c.CreatedDate).FirstOrDefault();

        return result;
    }
}