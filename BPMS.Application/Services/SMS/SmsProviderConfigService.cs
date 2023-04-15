using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using BPMS.Infrastructure.Services.SMS;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BPMS.Application.Services.SMS;

public class SmsProviderConfigService : ISmsProviderConfigService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCacheHelper _cacheHelper;
    private readonly ISystemSettingService _systemSettingService;
    public SmsProviderConfigService(IUnitOfWork unitOfWork, ISystemSettingService systemSettingService, IServiceProvider serviceProvider)
    {
        _unitOfWork = unitOfWork;
        _systemSettingService = systemSettingService;
        _cacheHelper = serviceProvider.GetRequiredService<IDistributedCacheHelper>();
    }

    public SystemSetting AddSMSConfig(object data, string username, SystemSettingType type)
    {
        var added = _systemSettingService.CreateSystemSetting(data, username, type);
        resetActiveProviderConfigCache();
        return added;
    }
    public List<SmsConfigGetListViewModel> GetProviderList()
    {
        return _cacheHelper.GetOrSet<List<SmsConfigGetListViewModel>>(CacheKeyHelper.GetSystemSettingCacheKey(SystemSettingType.SMS), () => GetAllSMSSetting(), TimeSpan.FromDays(1));

    }
    public void ActivateProvider(Guid id)
    {
        var activeItems = GetAllSMSSetting().Where(p => p.IsActive);
        foreach (var item in activeItems)
        {
            var currentSystemSetting = GetById(item.SettingId);
            var currentData = JsonConvert.DeserializeObject<SmsConfigGetListViewModel>(currentSystemSetting.Data);
            currentData.IsActive = false;
            var newData = new SmsProvider()
            {

                SmsSendType = (int)currentData.SmsSendType,
                Name = currentData.Name,
                ProviderNumber = currentData.ProviderNumber,
                UserName = currentData.UserName,
                Password = currentData.Password,
                ApiKey = currentData.ApiKey,
                Uri = currentData.Uri,
                IsActive = currentData.IsActive,
                GsmPort = currentData.GsmPort,
                GsmPortRate = currentData.GsmPortRate,
                Id = currentData.Id
            };
            currentSystemSetting.Data = JsonConvert.SerializeObject(newData);
            _unitOfWork.SystemSetting.Update(currentSystemSetting);
        }

        var provider = GetById(id);
        var data = JsonConvert.DeserializeObject<SmsConfigGetListViewModel>(provider.Data);
        if (provider != null)
        {
            data.IsActive = true;
            var bindedData = new SmsProvider()
            {

                SmsSendType = (int)data.SmsSendType,
                Name = data.Name,
                ProviderNumber = data.ProviderNumber,
                UserName = data.UserName,
                Password = data.Password,
                ApiKey = data.ApiKey,
                Uri = data.Uri,
                IsActive = data.IsActive,
                GsmPort = data.GsmPort,
                GsmPortRate = data.GsmPortRate,
                Id = data.Id
            };
            provider.Data = JsonConvert.SerializeObject(bindedData);
            _unitOfWork.SystemSetting.Update(provider);
        }
        _unitOfWork.Complete();
        _systemSettingService.ResetSystemSettingCache(SystemSettingType.SMS);
    }
    public SystemSetting GetById(Guid id)
    {
        return _unitOfWork.SystemSetting.Where(i => i.Id == id).FirstOrDefault();
    }
    public SmsConfigGetListViewModel GetActiveProviderConfig()
    {
        return _cacheHelper.GetOrSet<SmsConfigGetListViewModel>(CacheKeyHelper.GetSystemSettingActiveProviderConfig(), () =>
        {
            var config = GetAllSMSSetting().Where(p => p.IsActive).FirstOrDefault();
            if (config == null)
            {
                throw new Exception("خدمات دهنده اس ام اس فعال یافت نشد");
            }
            return config;
        }, TimeSpan.FromDays(365));
    }
    public (bool, string) RemoveSMSConfig(Guid Id)
    {
        var result = DeleteProviderConfige(Id);
        if (result.Item1)
        {
            _systemSettingService.ResetSystemSettingCache(SystemSettingType.SMS);
            resetActiveProviderConfigCache();
        }
        return result;

    }
    private List<SmsConfigGetListViewModel> GetAllSMSSetting()
    {
        var systemSettings = _systemSettingService.GetSmsSettings();
        var list = new List<SmsConfigGetListViewModel>();

        if (systemSettings != null)
        {
            foreach (var systemSetting in systemSettings)
            {
                var data = new SmsConfigGetListViewModel()
                {
                    UserName = systemSetting.UserName,
                    Password = systemSetting.Password,
                    ApiKey = systemSetting.ApiKey,
                    ProviderNumber = systemSetting.ProviderNumber,
                    IsActive = systemSetting.IsActive,
                    Name = systemSetting.Name,
                    GsmPort = systemSetting.GsmPort,
                    GsmPortRate = systemSetting.GsmPortRate,
                    SettingId = systemSetting.Id,
                    SmsSenderType = systemSetting.SmsSenderType,
                    SmsSendType = systemSetting.SmsSenderType
                };
                list.Add(data);
            }

        }

        return list;
    }
    private (bool, string) DeleteProviderConfige(Guid id)
    {
        var all = _unitOfWork.SystemSetting.GetAll();
        var mustBeDeleted = new List<SystemSetting>();
        var cantBeDeleteedForBeingActive = new List<SmsConfigGetListViewModel>();
        foreach (var item in all)
        {
            var providerConfige = GetById(item.Id);
            if (providerConfige != null)
            {
                var data = JsonConvert.DeserializeObject<SmsConfigGetListViewModel>(providerConfige.Data);
                data.CreatedDate = item.CreatedDate;
                if (data.Id == id)
                {

                    cantBeDeleteedForBeingActive.Add(data);

                    mustBeDeleted.Add(item);
                }

            }

        }

        var deletecandidatation = cantBeDeleteedForBeingActive.GroupBy(i => i.Id).Select(g => g.OrderByDescending(r => r.CreatedDate).First()).FirstOrDefault();
        if (deletecandidatation.IsActive)
        {
            return (false, "IsActive");
        }
        _unitOfWork.SystemSetting.RemoveRange(mustBeDeleted);

        _unitOfWork.Complete();
        return (true, "Removed");
    }
    private void resetActiveProviderConfigCache()
    {
        _cacheHelper.Remove(CacheKeyHelper.GetSystemSettingActiveProviderConfig());
    }

}