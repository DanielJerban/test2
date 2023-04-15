using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BPMS.Application.Repositories;

public class WorkFlowDetailPatternRepository : Repository<WorkflowDetailPattern>, IWorkFlowDetailPatternRepository
{
    public WorkFlowDetailPatternRepository(BpmsDbContext context) : base(context)
    {
    }

    public BpmsDbContext _dbContext => Context;

    public void AddPattern(string patternName, List<(Guid, string, int)> items)
    {
        var newPattern = new WorkflowDetailPattern()
        {
            Name = patternName
        };
        var AddingItems = new List<WorkflowDetailPatternItem>();
        items.ForEach(i =>
        {
            AddingItems.Add(new WorkflowDetailPatternItem()
            {
                Index = i.Item3,
                LookupOrganizationPostId = i.Item1,
                WorkflowDetailPatternId = newPattern.Id
            });
        });
        _dbContext.WorkflowDetailPatternItems.AddRange(AddingItems);
        _dbContext.SaveChanges();
    }


    public Result DeletePattern(Guid id)
    {
        var result = new Result();
        var pattern = _dbContext.WorkflowDetailPatterns.FirstOrDefault(i => i.Id == id);
        if (pattern != null)
        {
            var workFlowDetails = _dbContext.WorkFlowDetails.Where(w => w.WorkflowDetailPatternId == id).ToList();
            if (workFlowDetails.Any())
            {
                foreach (var workFlowDetail in workFlowDetails)
                {
                    var flow = _dbContext.Flows.Where(f => f.WorkFlowDetailId == workFlowDetail.Id).ToList();
                    if (flow.Any())
                    {
                        result.IsValid = false;
                        result.Message = "به دلیل استفاده از این الگو در گردش کار، قابل حذف نیست";
                        return result;
                    }

                }
            }

            _dbContext.WorkflowDetailPatterns.Remove(pattern);
            _dbContext.SaveChanges();
            result.IsValid = true;
            result.Message = "با موفقیت حذف شد ";
        }
        else
        {
            result.IsValid = false;
            result.Message = " الگو یافت نشد ";
        }

        return result;
    }

    public Result EditPattern(EditPatternViewModel model)
    {
        var result = new Result();
        if (model.selectedPosts == null)
        {
            result.IsValid = false;
            result.Message = "پستی انتخاب نشده است";
            return result;
        }

        var current = _dbContext.WorkflowDetailPatterns.Include("WorkflowPatternItems")
            .FirstOrDefault(i => i.Id == model.Id);
        if (current != null)
        {
            var oldItems = current.WorkflowPatternItems.ToList();
            oldItems?.ForEach(i => { _dbContext.WorkflowDetailPatternItems.Remove(i); });

            if (model.patternName != current.Name)
                current.Name = model.patternName;

            var newWorkflowPatternItems = mapTitlesToPatternItems(model.selectedPosts, model.Id);
            var editConditionsPassed = checkEditConditions(oldItems, newWorkflowPatternItems, model.Id);
            if (editConditionsPassed.IsValid)
            {
                newWorkflowPatternItems.ForEach(item => { _dbContext.WorkflowDetailPatternItems.Add(item); });

                current.WorkflowPatternItems = newWorkflowPatternItems;
                _dbContext.WorkflowDetailPatterns.Update(current);
                _dbContext.SaveChanges();
                result.IsValid = true;
                result.Message = "با موفقیت ویرایش شد ";
            }
            else
            {
                result.IsValid = false;
                result.Message = editConditionsPassed.Message;
            }

        }
        else
        {
            result.IsValid = false;
            result.Message = " الگو یافت نشد ";
        }
        return result;

    }

    public Guid GetIdByName(string name)
    {
        return _dbContext.LookUps.Where(l => l.Type == "OrganizationPostType" && l.IsActive && l.Title == name)
            .Select(s => s.Id).FirstOrDefault();

    }
    private List<WorkflowDetailPatternItem> mapTitlesToPatternItems(List<string> selectedPosts, Guid patternId)
    {
        var organizationalItems = new List<(Guid, string, int)>();
        int index = 1;
        selectedPosts.ForEach(i =>
        {
            var organizationId = GetIdByName(i);
            organizationalItems.Add((organizationId, i, index++));
        });
        var AddingItems = new List<WorkflowDetailPatternItem>();
        organizationalItems.ForEach(i =>
        {
            AddingItems.Add(new WorkflowDetailPatternItem()
            {
                Index = i.Item3,
                LookupOrganizationPostId = i.Item1,
                WorkflowDetailPatternId = patternId
            });
        });
        return AddingItems;
    }

    private Result checkEditConditions(List<WorkflowDetailPatternItem> oldItems, List<WorkflowDetailPatternItem> newItems, Guid patternId)
    {
        var result = new Result();
        result.IsValid = true;

        var pattern = _dbContext.WorkflowDetailPatterns.FirstOrDefault(i => i.Id == patternId);
        if (pattern != null)
        {
            var workFlowDetails = _dbContext.WorkFlowDetails.Where(w => w.WorkflowDetailPatternId == patternId).ToList();
            if (workFlowDetails.Any())
            {

                foreach (var workFlowDetail in workFlowDetails)
                {
                    var flow = _dbContext.Flows.Where(f => f.WorkFlowDetailId == workFlowDetail.Id).ToList();
                    if (flow.Any())
                    {
                        var listNewPosts = newItems.Select(i => i.LookupOrganizationPostId);
                        oldItems.ForEach(i =>
                        {
                            var newItemsHasThisItem = listNewPosts.Contains(i.LookupOrganizationPostId);
                            if (!newItemsHasThisItem)
                            {
                                result.IsValid = false;
                                result.Message = "به دلیل استفاده در جریان کار نمی توانید پستی را کم کنید";

                            }

                        });

                    }
                }
            }
        }

        return result;
    }

}