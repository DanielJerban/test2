using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Application.Services;

public class RecieverPatternService : IRecieverPatternService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDistributedCacheHelper _cacheHelper;
    private readonly IServiceProvider _serviceProvider;

    public RecieverPatternService(IUnitOfWork unitOfWork, IDistributedCacheHelper cacheHelper, IServiceProvider serviceProvider)
    {
        _unitOfWork = unitOfWork;
        _cacheHelper = cacheHelper;
        _serviceProvider = serviceProvider;
    }

    public IWorkFlowDetailPatternRepository WorkFlowDetailPatternRepository =>
        _serviceProvider.GetRequiredService<IWorkFlowDetailPatternRepository>();
   
    public void AddPattern(string patternName, List<string> selectedPosts)
    {
        var organizationalItems = new List<(Guid, string, int)>();
        int index = 1;
        selectedPosts.ForEach(i =>
        {
            var organizationId = WorkFlowDetailPatternRepository.GetIdByName(i);
            organizationalItems.Add((organizationId, i, index++));
        });
        var newPattern = new WorkflowDetailPattern()
        {
            Name = patternName
        };
        _unitOfWork.WorkFlowDetailPattern.Add(newPattern);
        var addingItems = new List<WorkflowDetailPatternItem>();
        organizationalItems.ForEach(i =>
        {
            addingItems.Add(new WorkflowDetailPatternItem()
            {
                Index = i.Item3,
                LookupOrganizationPostId = i.Item1,
                WorkflowDetailPatternId = newPattern.Id
            });
        });
        _unitOfWork.WorkFlowDetailPatternItem.AddRange(addingItems);
        _unitOfWork.Complete();

    }


    public Result EditPattern(EditPatternViewModel model)
    {
        var result = WorkFlowDetailPatternRepository.EditPattern(model);
        return new Result() { IsValid = result.IsValid, Message = result.Message };

    }
    public EditPatternDto GetPatternById(Guid id)
    {
        var pattern = _unitOfWork.WorkFlowDetailPattern.Include(i => i.WorkflowPatternItems).FirstOrDefault(i => i.Id == id);
        var postId = pattern.WorkflowPatternItems.Where(p => p.WorkflowDetailPatternId == pattern.Id)
            .OrderBy(i => i.Index)
            .Select(i => i.LookupOrganizationPostId);
        var items = new List<PatternItemDto>();
        foreach (var guid in postId)
        {
            items.Add(new PatternItemDto() { ItemId = guid, ItemTitle = GetPostNameById(guid) });
        }

        var postsWhichNotSelected = GetRemainedPosts(items);
        return new EditPatternDto()
        {
            Title = pattern.Name,
            PatternId = pattern.Id,
            Items = items,
            OrganizationPosts = postsWhichNotSelected
        };
    }

    private List<string> GetRemainedPosts(List<PatternItemDto> selectedPostsDto)
    {
        var allPosts = GetPosts();
        var selectedPosts = selectedPostsDto.Select(i => i.ItemTitle).ToList();

        var unselectedPosts = allPosts.Where(p => !selectedPosts.Contains(p)).ToList();

        return unselectedPosts;
    }
    public Result DeletePattern(Guid id)
    {
        var result = WorkFlowDetailPatternRepository.DeletePattern(id);
        return new Result() { IsValid = result.IsValid, Message = result.Message };
    }
    public List<PatternDto> GetAllPatterns()
    {
        var result = new List<PatternDto>();
        var allPattern = _unitOfWork.WorkFlowDetailPattern.Include(i => i.WorkflowPatternItems);

        foreach (var pattern in allPattern)
        {
            var items = new List<PatternItemDto>();
            var postId = pattern.WorkflowPatternItems.Where(p => p.WorkflowDetailPatternId == pattern.Id).Select(i => i.LookupOrganizationPostId);
            foreach (var guid in postId)
            {
                items.Add(new PatternItemDto() { ItemId = guid, ItemTitle = GetPostNameById(guid) });
            }

            result.Add(new PatternDto()
            {
                Title = pattern.Name,
                PatternId = pattern.Id,
                Items = items
            });
        }
        if (result.Count > 0)
            return result.OrderByDescending(i => i.Title).ToList();

        return result;
    }

    public string GetPostNameById(Guid Id)
    {
        return _unitOfWork.LookUps.Where(i => i.Id == Id).Select(i => i.Title).FirstOrDefault();
    }
    public List<string> GetPosts()
    {
        return _cacheHelper.GetOrSet(CacheKeyHelper.GetPostsCacheKey(), () => GetOrganizationPostType()/*GetAllPosts()*/, TimeSpan.FromDays(45));
    }
    public void SetPoststoCache(List<string> posts)
    {
        _cacheHelper.SetObject(CacheKeyHelper.GetPostsCacheKey(), posts, TimeSpan.FromDays(45));
    }



    public List<string> GetPostsFromCache()
    {
        return _cacheHelper.GetObject<List<string>>(CacheKeyHelper.GetPostsCacheKey());
    }
    public void ResetPostsCache()
    {
        _cacheHelper.Remove(CacheKeyHelper.GetPostsCacheKey());
    }
    private List<string> GetAllPosts()
    {
        return _unitOfWork.OrganizationInfos.Where(i => i.IsActive).Select(i => i.OrganiztionPost.Title).ToList();
    }
    public List<string> GetOrganizationPostType()
    {
        var lookup = _unitOfWork.LookUps.GetOrganizationPostType().Select(i => i.Title).ToList();
        return lookup;
    }
}