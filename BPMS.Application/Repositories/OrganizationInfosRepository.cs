using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using System.Collections;
using System.Reflection;

namespace BPMS.Application.Repositories;

public class OrganizationInfosRepository : Repository<OrganiztionInfo>, IOrganizationInfoRepository
{
    public OrganizationInfosRepository(BpmsDbContext context) : base(context)
    {
    }
    public BpmsDbContext _dbContext => Context;

    public IEnumerable<ChartTreeViewModel> GetChartTreeViewModel(Guid id)
    {
        var staffList = _dbContext.OrganiztionInfos.Where(o => o.ChartId == id && o.IsActive && o.Staff.EngType.Code == 1).Select(org => new ChartTreeViewModel()
        {
            Id = org.Id,
            Name = org.Staff.FName,
            Family = org.Staff.LName,
            PostTitle = org.OrganiztionPost.Title,
            PersonalCode = org.Staff.PersonalCode,
            MainPost = org.Priority,
            staffType = org.Staff.StaffType.Title,
            ImagePath = org.Staff.ImagePath.Replace("/", @"\")
        }).ToList();
        return staffList;
    }
    public void HaveAnyMainPost(Guid staffId, bool isMain)
    {
        var mainPostForStaff = _dbContext.OrganiztionInfos.Where(g => g.StaffId == staffId && g.Priority && g.IsActive).ToList();
        if (mainPostForStaff.Any() && isMain)
        {
            throw new CustomAttributeFormatException(
                "با ایجاد این پست، پست سازمانی اصلی قبلی غیر فعال خواهد شد، آیا ادامه می دهید ؟");
        }
    }

    public void OldMainPostDisinfect(Guid staffId)
    {
        var mainPostForStaff = _dbContext.OrganiztionInfos.Where(g => g.StaffId == staffId && g.Priority).ToList();
        var items = mainPostForStaff;
        foreach (var item in items)
        {
            var itemToChange = mainPostForStaff.FirstOrDefault(i => i.Id == item.Id);
            if (itemToChange != null)
            {
                itemToChange.Priority = false;
            }
        }
    }
    public IEnumerable<StaffPostViewModel> GetPostsToFillGrid(Guid staffId)
    {
        var staffpostList = new List<StaffPostViewModel>();
        var model = from organizationInfo in _dbContext.OrganiztionInfos.ToList()
                    join lookup in _dbContext.LookUps.ToList()
                        on organizationInfo.OrganiztionPostId equals lookup.Id
                    where organizationInfo.StaffId == staffId
                    select new { organizationInfo, lookup };
        foreach (var item in model)
        {
            var postTypeId = Guid.Parse(_dbContext.LookUps
                .Where(l => l.Id == item.organizationInfo.OrganiztionPostId)
                .Select(o => o.Aux).FirstOrDefault());
            var PostTypeTitle = _dbContext.LookUps.Where(k => k.Id == postTypeId).Select(u => u.Title)
                .FirstOrDefault();
            var data = new StaffPostViewModel()
            {
                OrgId = item.organizationInfo.Id,
                ChartId = item.organizationInfo.ChartId,
                ChartTitle = item.organizationInfo.Chart.Title,
                PostTitle = item.organizationInfo.OrganiztionPost.Title,
                Status = item.organizationInfo.IsActive,
                MainPost = item.organizationInfo.Priority,
                PostId = item.organizationInfo.OrganiztionPostId.ToString(),
                PostType = PostTypeTitle
            };
            staffpostList.Add(data);
        }
        return staffpostList;
    }
    public IEnumerable<StaffViewModel> GetAllStaff()
    {
        return from staff in _dbContext.Staffs
               join user in _dbContext.Users on staff.Id equals user.StaffId
               join eng in _dbContext.LookUps on staff.EngTypeId equals eng.Id
               where user.IsActive && eng.Code == 1
               select new StaffViewModel
               {
                   Id = staff.Id,
                   FName = staff.FName,
                   LName = staff.LName,
                   PersonalCode = staff.PersonalCode,
                   PhoneNumber = staff.PhoneNumber,
                   FullName = staff.FName + " " + staff.LName,
                   EmailAddress = staff.Email,
                   StaffType = staff.StaffType.Title,
                   UserName = user.UserName
               };
    }

    public IEnumerable<StaffViewModel> GetStaffbyIds(List<Guid> ids)
    {
        return from staff in _dbContext.Staffs
               join user in _dbContext.Users on staff.Id equals user.StaffId
               join eng in _dbContext.LookUps on staff.EngTypeId equals eng.Id
               where user.IsActive && eng.Code == 1 && ids.Contains(staff.Id)
               select new StaffViewModel
               {
                   Id = staff.Id,
                   FName = staff.FName,
                   LName = staff.LName,
                   PersonalCode = staff.PersonalCode,
                   PhoneNumber = staff.PhoneNumber,
                   FullName = staff.FName + " " + staff.LName,
                   EmailAddress = staff.Email,
                   StaffType = staff.StaffType.Title,
                   UserName = user.UserName
               };
    }


    public IEnumerable FillTree(Guid id)
    {
        return from e in _dbContext.Charts
               where (e.ParentId == id && e.IsActive)
               select new
               {
                   id = e.Id,
                   e.Title,
                   e.IsActive,
                   hasChildren = true
               };
    }

    public List<LookUp> GetLookUpsByType()
    {
        return _dbContext.LookUps.Where(l => l.Type == "OrganizationPostType" && l.IsActive).ToList();
    }

    public Guid ExistsInWorkFlowDetail(Guid staffId)
    {
        var notStarted = _dbContext.LookUps.Single(c => c.Code == 1 && c.Type == "RequestStatus").Id;
        var recInFlow = _dbContext.Flows
            .Where(f => f.StaffId == staffId && (f.FlowStatusId == notStarted))
            .ToList().Select(w => w.WorkFlowDetailId).FirstOrDefault();
        var recInDetail = _dbContext.WorkFlowDetails.Where(r => r.Id == recInFlow)
            .Select(w => w.OrganizationPostTitleId).FirstOrDefault();
        if (recInDetail != null)
        {
            return recInDetail.Value;
        }
        return Guid.Empty;
    }

    public Result CheckRedundantPostForSamePerson(Guid chartId, Guid staffId, Guid organizationPostId, Guid postId)
    {
        var result = new Result() { IsValid = true, Message = "" };
        var currentPost = _dbContext.OrganiztionInfos.FirstOrDefault(
            o => o.ChartId == chartId && o.StaffId == staffId && o.OrganiztionPostId == organizationPostId);
        if (currentPost != null)
        {
            if (postId == Guid.Empty)
            {
                result.IsValid = false;
                result.Message = "این پست قبلا برای این شخص ثبت شده است";

            }
            else
            {
                if (currentPost.Id != postId)
                {
                    result.IsValid = false;
                    result.Message = "این پست قبلا برای این شخص ثبت شده است";
                }
            }
        }

        return result;
    }

    public void CheckForPostsCapacity(Guid chartId, Guid organizationPostId, Guid staffId)
    {
        var postTypeId = Guid.Empty;
        var samePostsInChart =
            _dbContext.OrganiztionInfos.Where(c => c.ChartId == chartId &&
                                                   c.OrganiztionPostId == organizationPostId);
        var auxAsPostTypeId = _dbContext.LookUps.Where(o => o.Id == organizationPostId).Select(k => k.Aux)
            .FirstOrDefault();
        if (auxAsPostTypeId != null)
        {
            postTypeId = Guid.Parse(auxAsPostTypeId);
        }
        var validCount = _dbContext.LookUps.Where(u => u.Id == postTypeId && u.IsActive).ToList().Select(p => p.Aux2).FirstOrDefault();
        if (validCount != null)
        {
            if (samePostsInChart.Count() >= int.Parse(validCount))
            {
                var isExist = samePostsInChart.Any(s => s.StaffId == staffId);
                if (!isExist)
                {
                    throw new ArgumentException("حداکثر ظرفیت این پست سازمانی پر شده است");
                }
            }
        }

    }

    public void CheckForActiveFlowForCurrentStaff(Guid staffId, Guid organizationPostId)
    {
        var notStarted = _dbContext.LookUps.Single(c => c.Code == 1 && c.Type == "FlowStatus").Id;
        var recInFlow = _dbContext.Flows
            .Where(f => f.StaffId == staffId && (f.FlowStatusId == notStarted))
            .ToList().Select(w => w.WorkFlowDetailId).FirstOrDefault();
        var recInDetail = _dbContext.WorkFlowDetails.Where(r => r.Id == recInFlow)
            .Select(w => w.OrganizationPostTitleId).FirstOrDefault();
        if (recInDetail == organizationPostId)
        {
            throw new ArgumentException("پرسنل باید یک پست اصلی و فعال داشته باشند");
        }
    }

    public void CheckForAtLeastOnePost(Guid staffId, bool mainPost, bool isActive)
    {
        //var posts = _dbContext.OrganiztionInfos.Where(o => o.StaffId == staffId).ToList();
        //if (!posts.Any())
        //{
        //    if ((!isActive && !mainPost) || (isActive && !mainPost) ||
        //        (!isActive && mainPost))
        //    {
        //        throw new ArgumentException("پرسنل باید یک پست اصلی و فعال داشته باشند");
        //    }
        //}
    }

    public void CheckForActiveMainPostInEdit(Guid staffId, bool mainPost, bool active, Guid postId)
    {
        var selectedStaffMainPost =
            _dbContext.OrganiztionInfos.FirstOrDefault(o => o.StaffId == staffId && o.IsActive && o.Priority);
        if (selectedStaffMainPost != null)
        {
            var postEdited = _dbContext.OrganiztionInfos.FirstOrDefault(i => i.Id == postId);
            if (postEdited != null)
            {
                if (mainPost && selectedStaffMainPost.Id != postId)
                {
                    if ((!postEdited.IsActive && !postEdited.Priority && !active) || (!active && postEdited.IsActive && !postEdited.Priority))
                    {
                        throw new ArgumentException(
                            "در حال اصلی کردن یک پست غیرفعال هستید");

                    }

                    throw new CustomAttributeFormatException(
                        "با انتخاب این گزینه پست سازمانی اصلی قبلی غیر فعال خواهد شد، آیا ادامه می دهید ؟");

                }

                if (selectedStaffMainPost.Id == postId)
                {
                    if ((!active && !mainPost) || (active && !mainPost) ||
                        (!active && mainPost))
                    {
                        throw new ArgumentException("پرسنل باید یک پست اصلی و فعال داشته باشند");
                    }
                }
            }
        }

    }

    public IEnumerable<OrganizationInfoDto> GetAllOrganizationInfo()
    {
        var org = from orginfo in _dbContext.OrganiztionInfos
                  join chart in _dbContext.Charts on orginfo.ChartId equals chart.Id
                  join staff in _dbContext.Staffs on orginfo.StaffId equals staff.Id
                  join look in _dbContext.LookUps on orginfo.OrganiztionPostId equals look.Id
                  select new OrganizationInfoDto()
                  {
                      Id = orginfo.Id,
                      StaffId = orginfo.StaffId,
                      Staff = staff.FName + " " + staff.LName,
                      IsActive = orginfo.IsActive,
                      OrganiztionPostId = orginfo.OrganiztionPostId,
                      Chart = chart.Title,
                      ChartId = orginfo.ChartId,
                      Priority = orginfo.Priority,
                      OrganiztionPost = look.Title
                  };
        return org.ToList();
    }

    public void UpdateModifeidOrganizationInfos(OrganizationInfoPhpDto model)
    {
        switch (model.Action)
        {
            case "Insert":
                {

                    var organizationPostTitle = _dbContext.LookUps.FirstOrDefault(l => l.Type == "OrganizationPostTitle" && l.Code == model.OrganizationPostId && l.IsActive);
                    if (organizationPostTitle == null)
                        throw new ArgumentException("عنوان پست سازمانی وجود ندارد.");

                    var chart = _dbContext.Charts.FirstOrDefault(c => c.PhpId == model.ChartId && c.IsActive);
                    if (chart == null)
                        throw new ArgumentException("چارت وجود ندارد.");

                    var staff = _dbContext.Staffs.FirstOrDefault(c => c.PersonalCode == model.PersonalCode);
                    if (staff == null)
                        throw new ArgumentException("پرسنل وجود ندارد.");

                    var org = new OrganiztionInfo()
                    {
                        IsActive = model.IsActive == "active",
                        OrganiztionPostId = organizationPostTitle.Id,
                        ChartId = chart.Id,
                        Priority = model.Priority == "1",
                        StaffId = staff.Id,
                        PhpId = model.PhpId
                    };
                    _dbContext.OrganiztionInfos.Add(org);

                    break;
                }
            case "Update":
                {
                    var org = _dbContext.OrganiztionInfos.FirstOrDefault(c => c.PhpId == model.PhpId);
                    if (org == null)
                    {
                        throw new ArgumentException("پست سازمانی پیدا نشد.");
                    }

                    if (!string.IsNullOrWhiteSpace(model.Priority))
                    {
                        org.Priority = model.Priority == "1";
                    }

                    var request = _dbContext.Requests.Where(r => r.OrganizationPostTitleId == org.OrganiztionPostId
                                                                 && r.StaffId == org.StaffId
                                                                 && (r.RequestStatus.Code == 1 || r.RequestStatus.Code == 2)
                                                                 && r.IgnoreOrgInfChange == false).ToList();


                    if (!string.IsNullOrWhiteSpace(model.IsActive))
                    {
                        if (model.IsActive != "active" && request.Any())
                            throw new ArgumentException("امکان غیرفعال کردن پست به دلیل وجود درخواست اقدام نشده یا درحال اقدام نیست." + model.PersonalCode);

                        org.IsActive = model.IsActive == "active";
                    }

                    if (!string.IsNullOrWhiteSpace(model.PersonalCode))
                    {
                        var staff = _dbContext.Staffs.FirstOrDefault(c => c.PersonalCode == model.PersonalCode);

                        if (staff == null)
                            throw new ArgumentException("پرسنل وجود ندارد.");

                        org.StaffId = staff.Id;
                    }

                    if (model.OrganizationPostId != null)
                    {
                        var organizationPostTitle = _dbContext.LookUps.FirstOrDefault(l => l.Type == "OrganizationPostTitle" && l.Code == model.OrganizationPostId && l.IsActive);

                        if (organizationPostTitle == null)
                            throw new ArgumentException("عنوان پست سازمانی وجود ندارد.");

                        if (org.OrganiztionPostId != organizationPostTitle.Id && request.Any())
                            throw new ArgumentException("امکان تغییر پست به دلیل وجود درخواست اقدام نشده یا درحال اقدام نیست.");


                        var thisUserRequests = _dbContext.Requests.Where(r => r.OrganizationPostTitleId == org.OrganiztionPostId
                                                                              && r.StaffId == org.StaffId
                                                                              && (r.RequestStatus.Code == 1 || r.RequestStatus.Code == 2)).ToList();

                        org.OrganiztionPostId = organizationPostTitle.Id;

                        foreach (var req in thisUserRequests)
                        {
                            req.OrganizationPostTitleId = org.OrganiztionPostId;
                            _dbContext.Requests.Update(req);
                        }
                    }

                    if (model.ChartId != null)
                    {
                        var chart = _dbContext.Charts.FirstOrDefault(c => c.PhpId == model.ChartId && c.IsActive);

                        if (chart == null)
                            throw new ArgumentException("چارت وجود ندارد.");

                        if (org.ChartId != chart.Id && request.Any())
                            throw new ArgumentException("امکان تغییر چارت به دلیل وجود درخواست اقدام نشده یا درحال اقدام نیست.");

                        org.ChartId = chart.Id;
                    }

                    _dbContext.OrganiztionInfos.Update(org);

                    break;
                }
            case "Delete":
                {

                    var org = _dbContext.OrganiztionInfos.FirstOrDefault(s => s.PhpId == model.PhpId);
                    if (org == null)
                    {
                        throw new ArgumentException("پست سازمانی پیدا نشد.");

                    }
                    var request = _dbContext.Requests.Where(r => r.OrganizationPostTitleId == org.OrganiztionPostId
                                                                 && r.StaffId == org.StaffId
                                                                 && (r.RequestStatus.Code == 1 || r.RequestStatus.Code == 2)
                                                                 && r.IgnoreOrgInfChange == false).ToList();

                    if (request.Any())
                        throw new ArgumentException("امکان حذف پست سازمانی  به دلیل وجود درخواست اقدام نشده یا درحال اقدام نیست.");

                    _dbContext.OrganiztionInfos.Remove(org);
                    break;
                }

            default:
                throw new ArgumentException("خطا در ارسال درخواست");
        }
    }

    public OrganiztionInfo GetOrgInfoByStaffId(Guid staffId)
    {
        return _dbContext.OrganiztionInfos.FirstOrDefault(o => o.StaffId == staffId && o.Priority);
    }

    public List<OrganiztionInfo> GetOrganizationInfosTypeEngTypeBystaffIdAndChartId(Guid chartId, WorkflowDetailPatternItem patternItem, Guid staffId)
    {
        return _dbContext.OrganiztionInfos
            .Where(l => l.ChartId == chartId && l.IsActive)
            .Where(p => p.OrganiztionPost.Aux.ToUpper() == patternItem.LookupOrganizationPostId.ToString().ToUpper()
                        && p.StaffId != staffId
                        && p.Staff.Users.FirstOrDefault().IsActive
                        && p.Staff.EngType.Code == 1
                        && p.Staff.EngType.Type == "EngType"
            ).ToList();

    }

    public List<OrganiztionInfo> GetOrganizationInfosBystaffIdAndChartId(Guid chartId, WorkFlowDetail nextWorkFlowDetail, Guid staffId)
    {
        return _dbContext.OrganiztionInfos
            .Where(l => l.ChartId == chartId && l.IsActive)
            .Where(p => p.OrganiztionPost.Aux.ToUpper() ==
                        nextWorkFlowDetail.OrganizationPostTypeId.ToString().ToUpper()
                        && p.StaffId != staffId
                        && p.Staff.Users.FirstOrDefault().IsActive
                        && p.Staff.EngType.Code == 1
            ).ToList();
    }
    public List<OrganiztionInfo> GetOrgInfos()
    {

        return _dbContext.OrganiztionInfos.ToList();
    }

    public OrganiztionInfo GetOrgByStaffIdAndOrgPostId(Guid staffId, Guid organizationPostId)
    {
        return _dbContext.OrganiztionInfos.FirstOrDefault(o => o.StaffId == staffId &&
                                                               o.OrganiztionPostId == organizationPostId &&
                                                               o.IsActive);
    }
}