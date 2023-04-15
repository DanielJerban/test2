using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace BPMS.Application.Services;

public class UsefulLinksService : IUsefulLinksService
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IDistributedCacheHelper _cacheHelper;

    public UsefulLinksService(IUnitOfWork unitOfWork, IDistributedCacheHelper cacheHelper)
    {
        _unitOfWork = unitOfWork;
        _cacheHelper = cacheHelper;
    }

    public void AddOrUpdateUsefullLinks(UsefulLinksViewModel usefulLinks)
    {
        var usefulLink = new UsefulLinks()
        {
            Name = usefulLinks.Name,
            Url = usefulLinks.Url,
            Description = usefulLinks.Description,
            IsExternalLink = usefulLinks.IsExternalLink,
        };
        if (usefulLinks.Id == Guid.Empty)
        {
            _unitOfWork.UsefulLinks.AddUsefulLink(usefulLink);
        }
        else
        {
            usefulLink.Id = usefulLinks.Id;
            _unitOfWork.UsefulLinks.UpdateUsefulLink(usefulLink);
        }
        _cacheHelper.Remove(CacheKeyHelper.GetUsefulLinksCacheKey());

    }

    public bool RemoveUsefulLink(Guid Id)
    {
        bool result = _unitOfWork.UsefulLinks.RemoveUsefulLink(Id);
        if (result)
        {
            _cacheHelper.Remove(CacheKeyHelper.GetUsefulLinksCacheKey());
        }
        return result;
    }

    public DataSourceResult GetUsefulLinkList(DataSourceRequest request)
    {
        var usefulLinks = _unitOfWork.UsefulLinks.GetAllUsefulLinks();
        var result = usefulLinks.Select(s => new UsefulLinksViewModel
        {
            Id = s.Id,
            Name = s.Name,
            Url = s.Url,
            Description = s.Description,
            IsExternalLink = s.IsExternalLink
        });
        return result.ToDataSourceResult(request);
    }

    public IEnumerable<UsefulLinksViewModel> GetUsefulLinkForCartable()
    {
        var result = new List<UsefulLinksViewModel>();
        result = _cacheHelper.GetObject<List<UsefulLinksViewModel>>(CacheKeyHelper.GetUsefulLinksCacheKey());
        if (result == null)
        {
            var Links = _unitOfWork.UsefulLinks.GetAllUsefulLinks();
            result = Links.Select(s => new UsefulLinksViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Url = s.Url,
                Description = s.Description,
                IsExternalLink = s.IsExternalLink
            }).ToList();
            _cacheHelper.SetObject(CacheKeyHelper.GetUsefulLinksCacheKey(), result, TimeSpan.FromDays(30));
        }
        return result;
    }
}