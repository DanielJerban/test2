using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Text;

namespace BPMS.Application.Repositories;

public class LookUpRepository : Repository<LookUp>, ILookUpRepository
{
    public LookUpRepository(BpmsDbContext context) : base(context)
    {
    }

    public BpmsDbContext DbContext => Context;

    public IEnumerable<LookUpViewModel> GetAllLookUpTypeTitles()
    {
        var query = from lookup in DbContext.LookUps
                    where lookup.Type == "LookUpTypeTitle"
                    select new LookUpViewModel()
                    {
                        Id = lookup.Id,
                        Aux = lookup.Aux,
                        Aux2 = lookup.Aux2,
                        Title = lookup.Title,
                        Code = lookup.Code,
                        Type = lookup.Type,
                        IsActive = lookup.IsActive

                    };
        return query;
    }

    public IEnumerable<LookUpViewModel> GetActiveLookUpByType(string type)
    {

        return DbContext.LookUps.Where(l => l.Type == type && l.IsActive).Select(p => new LookUpViewModel()
        {
            Title = p.Title,
            Id = p.Id,
            Type = p.Type,
            Aux = p.Aux,
            Aux2 = p.Aux2,
            IsActive = p.IsActive,
            Code = p.Code
        });


    }

    public IEnumerable<LookUpViewModel> GetLookUpByType(string type)
    {

        return DbContext.LookUps.Where(l => l.Type == type).OrderBy(c => c.Code).Select(p => new LookUpViewModel()
        {
            Title = p.Title,
            Id = p.Id,
            Type = p.Type,
            Aux = p.Aux,
            Aux2 = p.Aux2,
            IsActive = p.IsActive,
            Code = p.Code
        });
    }

    public IEnumerable<LookUpViewModel> GetAllLookUpsByTypeTitle(Guid? id, string type)
    {
        if (id == null)
        {
            var model = DbContext.LookUps.Where(l => l.Type == type).Select(p => new LookUpViewModel()
            {
                Title = p.Title,
                Id = p.Id,
                Type = p.Type,
                Aux = p.Aux,
                Aux2 = p.Aux2,
                IsActive = p.IsActive,
                Code = p.Code
            });
            return model;
        }
        else
        {
            var lookid = id.ToString();
            var model = DbContext.LookUps.Where(l => l.Type == type && l.Aux == lookid).Select(p => new LookUpViewModel()
            {
                Title = p.Title,
                Id = p.Id,
                Type = p.Type,
                Aux = p.Aux,
                Aux2 = p.Aux2,
                IsActive = p.IsActive,
                Code = p.Code
            });
            return model;
        }
    }

    public Guid GetLookUpByTypeAndCode(int code, string type)
    {
        return DbContext.LookUps.Where(l => l.Type == type && l.Code == code).Select(p => p.Id).FirstOrDefault();
    }

    public IEnumerable<LookUpViewModel> GetAllStaffinOneChart(Guid lookupid)
    {
        var res = DbContext.LookUps.Where(o => o.Id == lookupid).ToList().Select(org => new LookUpViewModel()
        {
            Id = org.Id,
            Title = org.Title,
            Code = org.Code,
            IsActive = org.IsActive,
            Aux2 = org.Aux2
        });
        return res;
    }

    public IEnumerable<LookUpViewModel> GetRecordsForGrid(string aux)
    {
        return DbContext.LookUps.Where(o => o.Type == aux).ToList().Select(org => new LookUpViewModel()
        {
            Id = org.Id,
            Title = org.Title,
            Code = org.Code,
            IsActive = org.IsActive,
            Aux2 = org.Aux2
        });
    }

    public IEnumerable<LookUpDto> GetOrganizationPostType()
    {
        return DbContext.LookUps.Where(l => l.Type == "OrganizationPostType" && l.IsActive).Select(s => new LookUpDto()
        {
            Id = s.Id,
            Title = s.Title,
            Type = s.Type,
            Aux = s.Aux,
            IsActive = s.IsActive,
            Aux2 = s.Aux2,
            Code = s.Code
        }).ToList();
    }

    public IEnumerable<LookUpDto> GetOrganizationPostTitle()
    {
        //var mylist = new List<LookUpDto>();
        //var orgType = _dbContext.LookUps.Where(l => l.Type == "OrganizationPostType" && l.IsActive && l.Aux2 == "1").ToList();
        //foreach (var item in orgType)
        //{
        //    var id = item.Id.ToString();
        //    var orgTitle = _dbContext.LookUps.Where(l => l.Type == "OrganizationPostTitle" && l.IsActive && l.Aux == id).Select(s => new LookUpDto()
        //    {
        //        Id = s.Id,
        //        Title = s.Title,
        //        Type = s.Type,
        //        Aux = s.Aux,
        //        IsActive = s.IsActive,
        //        Aux2 = s.Aux2,
        //        Code = s.Code
        //    }).ToList();
        //    mylist.AddRange(orgTitle);
        //}
        //return mylist;
        return DbContext.LookUps.Where(l => l.Type == "OrganizationPostTitle" && l.IsActive).OrderBy(d => d.Title).Select(s => new LookUpDto()
        {
            Id = s.Id,
            Title = s.Title,
            Type = s.Type,
            Aux = s.Aux,
            IsActive = s.IsActive,
            Aux2 = s.Aux2,
            Code = s.Code
        });
    }

    public IEnumerable<LookUpViewModel> GetLookUpsByAux(string Aux)
    {
        var recordsindetailgrd = DbContext.LookUps.Where(k => k.Type == Aux).ToList().Select(
            u => new LookUpViewModel()
            {
                Id = u.Id,
                Code = u.Code,
                Type = u.Type,
                Title = u.Title,
                Aux = u.Aux,
                Aux2 = u.Aux2,
                IsActive = u.IsActive
            });
        return recordsindetailgrd;
    }

    public IEnumerable<LookUpViewModel> GetLookupsByIdBasedAux(Guid id)
    {
        var subAuxForSecondGrid = DbContext.LookUps.Where(l => l.Id == id).Select(c => c.Aux).FirstOrDefault();
        return DbContext.LookUps.Where(l => l.Type == subAuxForSecondGrid).ToList().Select(d => new LookUpViewModel()
        {
            Id = d.Id,
            Code = d.Code,
            Type = d.Type,
            Title = d.Title,
            Aux = d.Aux,
            Aux2 = d.Aux2,
            IsActive = d.IsActive,
            SubAux = subAuxForSecondGrid
        });
    }

    public void UpdateModifeidLookup(LookUpPhpDto model)
    {
        if (model.Type != "ChartLevel" && model.Type != "OrganizationPostType" &&
            model.Type != "OrganizationPostTitle")
        {
            throw new ArgumentException("نوع وارد شده صحیح نمی باشد.");
        }
        switch (model.Action)
        {
            case "Insert":
                {
                    string aux = null;
                    if (model.Type == "OrganizationPostTitle")
                    {

                        if (string.IsNullOrWhiteSpace(model.Aux))
                        {
                            throw new ArgumentException("سمت مربوط به این عنوان وجود ندارد.");
                        }
                        var phpId = int.Parse(model.Aux);
                        var parrent = DbContext.LookUps.FirstOrDefault(l => l.Type == "OrganizationPostType" && l.Code == phpId && l.IsActive);
                        if (parrent == null)
                        {
                            throw new ArgumentException("سمت مربوط به این عنوان وجود ندارد.");
                        }
                        aux = parrent.Id.ToString();
                    }
                    var look = new LookUp()
                    {
                        Code = model.Code,
                        Type = model.Type,
                        Title = model.Title,
                        IsActive = model.IsActive == "active",
                        Aux = aux
                    };
                    DbContext.LookUps.Add(look);
                    break;
                }
            case "Update":
                {
                    string aux = null;
                    if (model.Type == "OrganizationPostTitle")
                    {

                        if (!string.IsNullOrWhiteSpace(model.Aux))
                        {

                            var phpId = int.Parse(model.Aux);
                            var parrent = DbContext.LookUps.FirstOrDefault(l => l.Type == "OrganizationPostType" && l.Code == phpId && l.IsActive);

                            if (parrent == null)
                            {
                                throw new ArgumentException("سمت مربوط به این عنوان وجود ندارد.");
                            }
                            aux = parrent.Id.ToString();
                        }
                    }
                    var look = DbContext.LookUps.FirstOrDefault(l => l.Type == model.Type && l.Code == model.Code);
                    if (look == null)
                    {
                        throw new ArgumentException("رکورد پیدا نشد.");
                    }
                    if (!string.IsNullOrWhiteSpace(model.IsActive))
                    {
                        look.IsActive = model.IsActive == "active";
                    }

                    if (!string.IsNullOrWhiteSpace(model.Title))
                    {
                        look.Title = model.Title;
                    }

                    if (!string.IsNullOrWhiteSpace(model.Aux))
                    {
                        look.Aux = aux;
                    }
                    DbContext.LookUps.Update(look);
                    break;
                }
            case "Delete":
                {

                    var look = DbContext.LookUps.FirstOrDefault(l => l.Type == model.Type && l.Code == model.Code);
                    if (look == null)
                    {
                        throw new ArgumentException("رکورد پیدا نشد.");

                    }
                    if (model.Type == "OrganizationPostType")
                    {
                        var id = look.Id.ToString();
                        var d = DbContext.LookUps.Where(l => l.Type == "OrganizationPostTitle" && l.Aux == id).ToList();
                        if (d.Any())
                        {
                            throw new ArgumentException("به دلیل داشتن زیر مجموعه قابل حذف نمی باشد.");
                        }

                        var wfd = DbContext.WorkFlowDetails.Where(w => w.OrganizationPostTypeId == look.Id)
                            .ToList();
                        if (wfd.Any())
                        {
                            throw new ArgumentException("به دلیل استفاده در فرآیند قابل حذف نمی باشد.");
                        }
                    }
                    if (model.Type == "OrganizationPostTitle")
                    {

                        var wfd = DbContext.WorkFlowDetails.Where(w => w.OrganizationPostTitleId == look.Id)
                            .ToList();
                        if (wfd.Any())
                        {
                            throw new ArgumentException("به دلیل استفاده در فرآیند قابل حذف نمی باشد.");
                        }
                        var req = DbContext.Requests.Where(w => w.OrganizationPostTitleId == look.Id)
                            .ToList();
                        if (req.Any())
                        {
                            throw new ArgumentException("به دلیل استفاده در درخواست ها قابل حذف نمی باشد.");
                        }
                        var org = DbContext.OrganiztionInfos.Where(o => o.OrganiztionPostId == look.Id).ToList();
                        if (org.Any())
                        {
                            throw new ArgumentException("به دلیل استفاده در پست سازمانی قابل حذف نمی باشد.");
                        }
                    }
                    if (model.Type == "ChartLevel")
                    {

                        var chart = DbContext.Charts.Where(w => w.ChartLevelId == look.Id)
                            .ToList();
                        if (chart.Any())
                        {
                            throw new ArgumentException("به دلیل استفاده در چارت سازمانی قابل حذف نمی باشد.");
                        }


                    }
                    DbContext.LookUps.Remove(look);
                    break;
                }

            default:
                throw new ArgumentException("خطا در ارسال درخواست");
        }
    }



    public void UpdateLookUpFromPhp(string type, List<LookUpPhpDto> lookups)
    {
        foreach (var lookup in lookups)
        {
            var look = DbContext.LookUps.Where(s => s.Code == lookup.Code && s.Type == type).ToList();
            lookup.Action = look.Any() ? "Update" : "Insert";
            UpdateModifeidLookup(lookup);
        }
        DbContext.SaveChanges();
        var lookUpRemove = DbContext.LookUps.Where(l => l.Type == type).ToList()
            .Where(c => lookups.All(u => u.Code != c.Code)).ToList();
        foreach (var lookup in lookUpRemove)
        {
            DbContext.LookUps.Remove(lookup);
            DbContext.SaveChanges();
        }
    }
    private IQueryable<Guid> GetRoleMapPostTypeAccessId(Guid staffId)
    {
        var userPostTypes = from orgInfo in DbContext.OrganiztionInfos
                            join orgPostType in DbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostType.Id
                            where orgInfo.StaffId == staffId && orgInfo.IsActive

                            select new { orgInfo, orgPostType };
        var userPostTypeIds = new List<Guid>();
        foreach (var item in userPostTypes)
        {
            var postTypeId = DbContext.LookUps.Where(l => l.Id == item.orgInfo.OrganiztionPostId)
                .Select(o => o.Aux).FirstOrDefault();
            if (postTypeId != null)
            {
                userPostTypeIds.Add(Guid.Parse(postTypeId));
            }
        }


        var roleMapPostTypeAccessId = from lookup in DbContext.LookUps
                                      join roleMapPostType in DbContext.RoleMapPostTypes
                                          on lookup.Id equals roleMapPostType.PostTypeId
                                      where userPostTypeIds.Contains(roleMapPostType.PostTypeId)
                                      select roleMapPostType.RoleId;

        return roleMapPostTypeAccessId;

    }

    private IQueryable<Guid> GetRoleMapPostTitleAccessId(Guid staffId)
    {

        var userPostTitleIds = new List<Guid>();

        var userPostTitles = from orgInfo in DbContext.OrganiztionInfos
                             join orgPostTitle in DbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostTitle.Id
                             where orgInfo.StaffId == staffId && orgInfo.IsActive

                             select new { orgInfo, orgPostTitle };

        foreach (var item in userPostTitles)
        {
            var postTitleId = DbContext.LookUps.Where(l => l.Id == item.orgInfo.OrganiztionPostId)
                .Select(o => o.Id).FirstOrDefault();
            userPostTitleIds.Add(postTitleId);

        }

        var roleMapPostTitleAccessId = from lookup in DbContext.LookUps
                                       join roleMapPostTitle in DbContext.RoleMapPostTitles
                                           on lookup.Id equals roleMapPostTitle.PostTitleId
                                       where userPostTitleIds.Contains(roleMapPostTitle.PostTitleId)
                                       select roleMapPostTitle.RoleId;

        return roleMapPostTitleAccessId;

    }
    public IEnumerable<SelectListItem> GetWidgetTypeByAccess(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;
        var staffId = user.StaffId;

        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartAccessIds = from organizationInfo in DbContext.OrganiztionInfos
                                    join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                                    join roleMapChart in DbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
                                    where organizationInfo.StaffId == staffId
                                    select roleMapChart.RoleId;


        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(roleMapChartAccessIds);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();


        return (from lookup in DbContext.LookUps
                join roleClaim in DbContext.RoleClaims on lookup.Id.ToString() equals roleClaim.ClaimValue
                join roleId in roleIds on roleClaim.RoleId equals roleId
                where lookup.IsActive && roleClaim.ClaimType == PermissionPolicyType.WidgetPermission
                select new SelectListItem()
                {
                    Text = lookup.Title,
                    Value = lookup.Id.ToString()
                }).AsEnumerable().DistinctBy(d => d.Value).OrderBy(d => d.Text);
    }

    public OneLevelLookUpViewModel BaseInfoOneLevel(string system)
    {
        var id = Guid.NewGuid();
        var title = string.Empty;
        var fatherPage = string.Empty;
        var level2 = string.Empty;
        switch (system)
        {
            case "BPMS":
                title = "گروه فرآیند";
                fatherPage = "فرآیندها";
                level2 = "اطلاعات پایه فرایند";
                break;
            case "HR":
                title = "اطلاعات پایه کاربران";
                fatherPage = "مدیریت کاربران";
                break;
            case "IT":
                title = "اطلاعات پایه بخش IT";
                fatherPage = "سامانه کارگزینی";
                break;
            case "Schedule":
                title = "اطلاعات پایه زمانبندی";
                fatherPage = "زمان بندی";
                break;
            case "Base":
                title = "اطلاعات پایه امور سیستم";
                fatherPage = "امور سیستم";
                break;
            case "BpmsForm":
                title = "اصلاح انتخابگر";
                fatherPage = "فرم ها";
                level2 = "اطلاعات پایه فرم";
                break;
            case "FormClassification":
                title = "اطلاعات پایه مدارک";
                fatherPage = "مدارک و مستندات";
                break;
        }

        return new OneLevelLookUpViewModel()
        {
            System = HashPassword.Encrypt(system, "d,m4do dUV21%3[B" + id),
            Title = title,
            FatherPage = fatherPage,
            Level2 = level2,
            Idenc = HashPassword.Encrypt(id.ToString(), "^%*sd9^%&[km sdmo")
        };
    }

    public IEnumerable<LookUpViewModel> GetLookUpBySystem(string bpmsform)
    {
        return DbContext.LookUps.Where(l => l.Type == "LookUpTypeTitle" && l.Aux2 == bpmsform).OrderBy(d => d.Title).Select(s => new LookUpViewModel()
        {
            Id = s.Id,
            Aux2 = s.Aux2,
            IsActive = s.IsActive,
            Type = s.Type,
            Aux = s.Aux,
            Code = s.Code,
            Title = s.Title
        });

    }

    public void CheckForDeleteInOneLevel(string id, string type, string system)
    {
        var itemToDelete = Guid.Parse(id);
        switch (type)
        {
            case "TaskType" when DbContext.Schedules.Any(s => s.TaskTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در زمانبندی وجود ندارد");
            case "RequestType" when DbContext.Workflows.Any(c => c.RequestTypeId == itemToDelete) || DbContext.WorkFlowConfermentAuthority.Any(o => o.RequestTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در فرآیندها وجود ندارد");
            case "StaffType" when DbContext.Staffs.Any(t => t.StaffTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در جدول پرسنل وجود ندارد.");
            case "EngType" when DbContext.Staffs.Any(t => t.EngTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در جدول پرسنل وجود ندارد.");
            case "Building" when DbContext.Staffs.Any(t => t.StaffTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در جدول پرسنل وجود ندارد.");
            case "ChartLevel" when DbContext.Charts.Any(t => t.ChartLevelId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در چارت سازمانی وجود ندارد.");

            case "Widget" when DbContext.DynamicCharts.Any(i => i.WidgetTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در نمودار گزارش وجود ندارد");
            case "Widget" when DbContext.WorkFlowIndicators.Any(i => i.WidgetTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در شاخص وجود ندارد");
            case "StandardType" when DbContext.FormClassifications.Any(i => i.StandardTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در شاخص وجود ندارد");
            case "FormType" when DbContext.FormClassifications.Any(i => i.FormTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در شاخص وجود ندارد");
            case "FormStatus" when DbContext.FormClassifications.Any(i => i.FormStatusId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در شاخص وجود ندارد");
            case "AccessType" when DbContext.FormClassifications.Any(i => i.AccessTypeId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در شاخص وجود ندارد");
            case "ConfidentialLevelId" when DbContext.FormClassifications.Any(i => i.ConfidentialLevelId == itemToDelete):
                throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در مستندات وجود ندارد");
        }

        if (system == "BpmsForm")
        {
            var encoding = new UnicodeEncoding();
            var requests = DbContext.Requests;
            if (requests == null)
                return;

            foreach (var request in requests)
            {
                if (request.Value != null)
                {
                    var obj = JObject.Parse(encoding.GetString(request.Value));

                    if (obj.ToString().Contains(id))
                        throw new ArgumentException("امکان حذف رکورد به دلیل استفاده در گردش کار وجود ندارد.");
                }
            }
        }
    }

    public void DeleteVirtualTable(Guid id)
    {
        var lookUp = DbContext.LookUps.SingleOrDefault(l => l.Id == id);
        if (lookUp.Code == 65)
        {
            throw new ArgumentException("امکان حذف رکورد گروه پاسخ دهنده وجود ندارد.");
        }
        if (lookUp == null)
        {
            throw new ArgumentException("رکورد مورد نظر پیدا نشد");
        }

        var forms = DbContext.WorkFlowForms;
        foreach (var form in forms)
        {
            var encoding = new UnicodeEncoding();
            var jsonFile = JObject.Parse(encoding.GetString(form.Content));

            var fileds = jsonFile["fields"];
            foreach (var filed in fileds)
            {
                var fli = filed.First.ToObject<JObject>();

                var type = (string)fli["meta"]["id"];
                if (type != "lookup") continue;
                var value = (string)fli["attrs"]["value"];
                if (value == id.ToString())
                {
                    throw new ArgumentException("امکان حذف این رکورد به دلیل استفاده در فرم ها وجود ندارد.");
                }
            }
        }
        DbContext.LookUps.Remove(lookUp);
    }
    public IEnumerable<LookUpViewModel> GetRequestTypeHasWorkflow(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var staffId = user.StaffId;

        var userId = DbContext.Users.Single(d => d.StaffId == staffId).Id;

        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartAccessIds = from organizationInfo in DbContext.OrganiztionInfos
                                    join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                                    join roleMapChart in DbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
                                    where organizationInfo.StaffId == staffId
                                    select roleMapChart.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(roleMapChartAccessIds);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var proccessStatusWithRequest = (from workflow in DbContext.Workflows
                                         join requestType in DbContext.LookUps on workflow.RequestTypeId equals requestType.Id
                                         join request in DbContext.Requests on workflow.Id equals request.WorkFlowId
                                         join roleClaim in DbContext.RoleClaims on requestType.Id.ToString() equals roleClaim.ClaimValue
                                         join roleId in roleIds on roleClaim.RoleId equals roleId
                                         where workflow.SubProcessId == null && roleClaim.ClaimType == PermissionPolicyType.WorkFlowStatusPermission
                                         select new ProccessStatusViewModel
                                         {
                                             RequestId = request.Id,
                                             RequestTypeId = requestType.Id,
                                             RequestTypeTitle = requestType.Title,
                                             T = 1
                                         }).DistinctBy(r => r.RequestId);

        var proccessStatusWithoutRequest = from workflow in DbContext.Workflows
                                           join requestType in DbContext.LookUps on workflow.RequestTypeId equals requestType.Id
                                           join roleClaim in DbContext.RoleClaims on requestType.Id.ToString() equals roleClaim.ClaimValue
                                           join roleId in roleIds on roleClaim.RoleId equals roleId
                                           where workflow.SubProcessId == null && roleClaim.ClaimType == PermissionPolicyType.WorkFlowStatusPermission
                                           select new ProccessStatusViewModel
                                           {
                                               RequestTypeId = requestType.Id,
                                               RequestTypeTitle = requestType.Title
                                           };

        var final = proccessStatusWithRequest.Union(proccessStatusWithoutRequest);
        IEnumerable<LookUpViewModel> data = final.GroupBy(d => new { d.RequestTypeId, d.RequestTypeTitle }).Select(s => new LookUpViewModel()
        {
            Id = s.Key.RequestTypeId,
            Count = s.Sum(d => d.T),
            Title = s.Key.RequestTypeTitle
        }).OrderBy(d => d.Title).ToList();

        return data;
    }

    public string GetEventsOfDay(DateTime date)
    {
        var events = "";
        var sh = date.ToString("MM/dd");
        var shamsi = DbContext.LookUps.FirstOrDefault(d => d.Type == "SolarCalendarEvents" && d.Title == sh);
        if (shamsi != null)
        {
            events = shamsi.Aux2;
        }
        var hc = new HijriCalendar();
        var gh = hc.GetMonth(date).ToString("D2") + "/" + hc.GetDayOfMonth(date.AddDays(-1)).ToString("D2");
        var ghamari = DbContext.LookUps.FirstOrDefault(d => d.Type == "LunarCalendarEvents" && d.Title == gh);
        if (ghamari != null)
        {
            if (!string.IsNullOrWhiteSpace(events))
                events += " - ";
            events += ghamari.Aux2;
        }

        var gc = new GregorianCalendar();
        var mi = gc.GetMonth(date).ToString("D2") + "/" + gc.GetDayOfMonth(date).ToString("D2");
        var miladi = DbContext.LookUps.FirstOrDefault(d => d.Type == "GregorianCalendarEvents" && d.Title == mi);
        if (miladi != null)
        {
            if (!string.IsNullOrWhiteSpace(events))
                events += " - ";
            events += miladi.Aux2;
        }
        return events;
    }

    public void ChangeLoginPictureForSchedule()
    {
        // login qoute
        var currentPic = DbContext.LookUps.Single(d => d.Type == "LoginPicQoute" && d.IsActive);
        currentPic.IsActive = false;
        var number = int.Parse(currentPic.Title) + 1;
        var nextPic = DbContext.LookUps.FirstOrDefault(d => d.Type == "LoginPicQoute" && d.Title == number.ToString());
        if (nextPic == null)
        {
            nextPic = DbContext.LookUps.Single(d => d.Type == "LoginPicQoute" && d.Title == "1");
        }
        nextPic.IsActive = true;

        // login back
        var currentback = DbContext.LookUps.Single(d => d.Type == "LoginPicBackground" && d.IsActive);
        currentback.IsActive = false;
        var numberback = int.Parse(currentback.Title) + 1;
        var nextback = DbContext.LookUps.FirstOrDefault(d => d.Type == "LoginPicBackground" && d.Title == numberback.ToString());
        if (nextback == null)
        {
            nextback = DbContext.LookUps.Single(d => d.Type == "LoginPicBackground" && d.Title == "1");
        }
        nextback.IsActive = true;

    }

    public IEnumerable<LookUpViewModel> GetOneLevelLookup(string sys)
    {
        List<int> hiddenCodes = new List<int> { 30, 41, 52, 53, 63, 65 };
        IQueryable<LookUpViewModel> lvl;
        lvl = from look in DbContext.LookUps
              where look.Aux2 == sys && look.Type == "LookUpTypeTitle" && !hiddenCodes.Contains(look.Code)
              select new LookUpViewModel()
              {
                  Id = look.Id,
                  Title = look.Title,
                  Type = look.Type,
                  IsActive = look.IsActive,
                  Aux = look.Aux,
                  Aux2 = look.Aux,
                  Code = look.Code
              };
        return lvl;
    }

    public void CreateOneLevelLookup(IEnumerable<LookUpViewModel> model, string subAux)
    {
        if (model == null) return;
        var maxCode = GetNewCode(subAux);
        foreach (var item in model)
        {
            if (string.IsNullOrWhiteSpace(item.Title))
                throw new Exception("عنوان وارد نشده است");
            var entity = new LookUp
            {
                Code = maxCode,
                Title = item.Title,
                Aux = item.Aux,
                Aux2 = item.Aux2,
                IsActive = item.IsActive
            };
            if (subAux == "ResponseGroup")
                entity.Type = "ResponseGroup";
            else
                entity.Type = item.Type;
            DbContext.LookUps.Add(entity);
            maxCode++;
        }
    }

    public Guid CreateResponseGroup(LookUpViewModel model, string subAux)
    {
        if (model == null) return Guid.Empty;
        var maxCode = GetNewCode(subAux);

        if (string.IsNullOrWhiteSpace(model.Title))
            throw new Exception("عنوان وارد نشده است");
        var entity = new LookUp
        {
            Code = maxCode,
            Title = model.Title,
            Aux = model.Aux,
            Aux2 = model.Aux2,
            IsActive = model.IsActive
        };
        if (subAux == "ResponseGroup")
            entity.Type = "ResponseGroup";
        else
            entity.Type = model.Type;
        DbContext.LookUps.Add(entity);

        return entity.Id;
    }


    public string EditOneLevelLookup(IEnumerable<LookUpViewModel> model, string system)
    {
        if (model == null) return null;
        var warning = new StringBuilder();
        var requests = DbContext.Requests.Where(d => d.RequestStatus.Code == 1 || d.RequestStatus.Code == 2);
        foreach (var item in model.ToList())
        {
            var check = CheckValidationForEdit(item, system, requests);
            if (string.IsNullOrWhiteSpace(check))
            {
                var lookUp = DbContext.LookUps.Find(item.Id.Value);
                lookUp.Title = item.Title;
                lookUp.Aux = item.Aux;
                lookUp.Aux2 = item.Aux2;
                lookUp.IsActive = item.IsActive;
            }
            else
                warning.AppendLine(check);
        }

        return warning.ToString();
    }

    private string CheckValidationForEdit(LookUpViewModel model, string system, IQueryable<Request> requests)
    {
        var result = new StringBuilder();
        if (system == "BpmsForm")
        {
            if (model.IsActive) return null;
            var encoding = new UnicodeEncoding();

            foreach (var request in requests)
            {
                if (request.Value == null) continue;
                var obj = JObject.Parse(encoding.GetString(request.Value));
                foreach (var item in obj)
                {
                    if (item.Value.ToString() == model.Id.Value.ToString())
                    {
                        result.AppendLine($"{model.Title} استفاده شده در درخواست {request.RequestNo}");
                    }
                }
            }
        }
        return result.ToString();
    }

    public void DeleteOneLevelLookup(IEnumerable<Guid> model, List<Assingnment> assingnments = null)
    {
        if (model == null) return;
        if (assingnments != null)
            DbContext.Assingnments.RemoveRange(assingnments);

        foreach (var item in model)
        {
            var itemToDelete = DbContext.LookUps.Find(item);
            if (itemToDelete != null)
            {
                try
                {
                    var requests = DbContext.Requests;
                    if (requests == null) return;

                    bool orgPostIsUsed = (from request in DbContext.Requests
                                          where request.OrganizationPostTitleId == itemToDelete.Id
                                          select request.Id).Any();

                    bool reqStatIsUsed = (from request in DbContext.Requests
                                          where request.RequestStatusId == itemToDelete.Id
                                          select request.Id).Any();

                    if (orgPostIsUsed | reqStatIsUsed)
                        throw new ArgumentException();

                    bool flowTypeIsUsed = (from wf in DbContext.Workflows
                                           where wf.FlowTypeId == itemToDelete.Id
                                           select wf.Id).Any();

                    bool reqGrpTypeIsUsed = (from wf in DbContext.Workflows
                                             where wf.RequestGroupTypeId == itemToDelete.Id
                                             select wf.Id).Any();

                    bool reqTypeIsUsed = (from wf in DbContext.Workflows
                                          where wf.RequestTypeId == itemToDelete.Id
                                          select wf.Id).Any();

                    if (flowTypeIsUsed | reqGrpTypeIsUsed | reqTypeIsUsed)
                        throw new ApplicationException();

                    DbContext.LookUps.Remove(itemToDelete);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException($@"امکان حذف به دلیل استفاده در گردش کار وجود ندارد.
                                                    نام رکورد : ""{itemToDelete.Title}""");
                }
                catch (ApplicationException)
                {
                    throw new ArgumentException($@"امکان حذف به دلیل استفاده در طراحی فرآیند وجود ندارد.
                                                    نام رکورد : ""{itemToDelete.Title}""");
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }
    }

    public int GetNewCode(string type)
    {
        int maxcodeRequestType;
        if (DbContext.LookUps.Local.Any(l => l.Type == type))
        {
            var code = DbContext.LookUps.Local.Where(l => l.Type == type).Select(c => c.Code).ToList();
            maxcodeRequestType = code.Max() + 1;
        }
        else
        {
            var code = DbContext.LookUps.Where(l => l.Type == type).Select(c => c.Code);
            if (code.Count() != 0)
            {
                maxcodeRequestType = code.Max() + 1;
            }
            else
            {
                maxcodeRequestType = 1;
            }
        }

        return maxcodeRequestType;
    }


    public LookUp GetByTypeAndCode(string type, int code)
    {
        return DbContext.LookUps.SingleOrDefault(d => d.Type == type && d.Code == code);
    }
}