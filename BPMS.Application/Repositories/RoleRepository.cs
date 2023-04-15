using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Repositories;
using Kendo.Mvc.UI;

namespace BPMS.Application.Repositories;

public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(BpmsDbContext context) : base(context)
    {
    }

    public BpmsDbContext _dbContext
    {
        get { return Context as BpmsDbContext; }
    }

    private static Lazy<IEnumerable<ControllerDescription>> _controllerList =
        new Lazy<IEnumerable<ControllerDescription>>();

    private static Lazy<IEnumerable<ControllerPolicyDTO>> _controllerPolicies =
        new Lazy<IEnumerable<ControllerPolicyDTO>>();

    public IEnumerable<RoleViewModel> GetRolesToFillGrid()
    {
        return _dbContext.Roles.Select(role => new RoleViewModel
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Dsr
        });
    }

    public IEnumerable<UserViewModel> GetUsers()
    {
        return _dbContext.Users.Where(c => c.IsActive).Select(user => new UserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            PersonelCode = user.Staff.PersonalCode,
            FullName = user.Staff.FName + " " + user.Staff.LName,
            IsActive = user.IsActive
        });

    }
    public IEnumerable<UserViewModel> UsersInSpecificRole(Guid? id)
    {
        var users = from dbRoleAccess in _dbContext.RoleAccesses.ToList()
                    join dbusers in _dbContext.Users.ToList() on dbRoleAccess.UserId equals dbusers.Id
                    where dbRoleAccess.RoleId == id
                    select new UserViewModel()
                    {
                        Id = dbusers.Id,
                        FullName = dbusers.Staff.FName + " " + dbusers.Staff.LName,
                        PersonelCode = dbusers.Staff.PersonalCode,
                        UserName = dbusers.UserName,
                        RoleAccessId = dbRoleAccess.Id
                    };
        return users;
    }

    public IEnumerable<ChartViewModel> ChartsInSpecificRole(Guid? id)
    {
        var charts = from dbRoleMapCharts in _dbContext.RoleMapCharts
                     join dbcharts in _dbContext.Charts on dbRoleMapCharts.ChartId equals dbcharts.Id
                     where dbRoleMapCharts.RoleId == id
                     select new ChartViewModel()
                     {
                         Id = dbcharts.Id,
                         Title = dbcharts.Title,
                         ParentTitle = dbcharts.ChartParent.Title
                     };
        return charts;
    }

    public IEnumerable<PostTitleViewModel> PostTitleInSpecificRole(Guid? id)
    {
        var postTitles = from dbRoleMapPostTitles in _dbContext.RoleMapPostTitles
                         join dblookup in _dbContext.LookUps on dbRoleMapPostTitles.PostTitleId equals dblookup.Id
                         where dbRoleMapPostTitles.RoleId == id
                         select new PostTitleViewModel()
                         {
                             Title = dblookup.Title,
                             Id = dblookup.Id

                         };
        return postTitles;
    }

    public List<Guid> GetUserRoles(string username)
    {
        var user = _dbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;

        var roleAccessesId = from ra in _dbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartAccessId = from organizationInfo in _dbContext.OrganiztionInfos
                                   join chart in _dbContext.Charts on organizationInfo.ChartId equals chart.Id
                                   join roleMapChart in _dbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
                                   where organizationInfo.StaffId == user.StaffId
                                   select roleMapChart.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(user.StaffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(user.StaffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(roleMapChartAccessId);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        return roleIds;
    }

    public IEnumerable<PostTypeViewModel> PostTypeInSpecificRole(Guid? id)
    {
        var postTypes = from dbRoleMapPostTypes in _dbContext.RoleMapPostTypes
                        join dblookup in _dbContext.LookUps on dbRoleMapPostTypes.PostTypeId equals dblookup.Id
                        where dbRoleMapPostTypes.RoleId == id
                        select new PostTypeViewModel()
                        {
                            Title = dblookup.Title,
                            Id = dblookup.Id

                        };
        return postTypes;
    }

    public IEnumerable<WorkFlowViewModel> DiagramsInSpecificRoleByPolicy(Guid? id)
    {
        var diagrams = from roleClaim in _dbContext.RoleClaims
                       join workflow in _dbContext.Workflows on roleClaim.ClaimValue equals workflow.RequestTypeId.ToString()
                       join lookup in _dbContext.LookUps on workflow.RequestTypeId equals lookup.Id
                       where roleClaim.ClaimType == PermissionPolicyType.WorkFlowPermission
                             && roleClaim.RoleId == id
                       select new WorkFlowViewModel()
                       {
                           Id = workflow.Id,
                           RequestTypeId = workflow.RequestTypeId,
                           RequestType = lookup.Title,
                           Staff = workflow.Staff.FName + " " + workflow.Staff.LName
                       };

        return diagrams.GroupBy(d => new { d.RequestTypeId }).Select(s => s.FirstOrDefault()).ToList();
    }

    public IEnumerable<UserViewModel> UsersInRoles(List<Guid> id)
    {
        return from dbusers in _dbContext.Users
               join iddd in id
                   on dbusers.Id equals iddd

               select new UserViewModel()
               {
                   Id = dbusers.Id,
                   FullName = dbusers.Staff.FName + " " + dbusers.Staff.LName,
                   PersonelCode = dbusers.Staff.PersonalCode,
                   UserName = dbusers.UserName,
               };

    }

    public IEnumerable<ChartViewModel> ChartsInRoles(List<Guid> id)
    {
        var model = from dbcharts in _dbContext.Charts
                    join ids in id
                        on dbcharts.Id equals ids

                    select new ChartViewModel()
                    {
                        Id = dbcharts.Id,
                        Title = dbcharts.Title,
                        ParentTitle = dbcharts.ChartParent.Title,
                    };
        return model;
    }

    public IEnumerable<WorkFlowViewModel> DiagramsInRole(List<Guid> id)
    {
        var model = from dbworkflows in _dbContext.Workflows
                    join ids in id
                        on dbworkflows.RequestTypeId equals ids

                    select new WorkFlowViewModel()
                    {
                        Id = dbworkflows.Id,
                        RequestTypeId = dbworkflows.RequestTypeId,
                        RequestType = dbworkflows.RequestType.Title,
                        Staff = dbworkflows.Staff.FName + " " + dbworkflows.Staff.LName
                    };
        return model;
    }

    public IEnumerable<UserViewModel> UsersInRoles2(Guid[] id)
    {
        return from dbusers in _dbContext.Users.ToList()
               where dbusers.IsActive == true
               select new UserViewModel()
               {
                   Id = dbusers.Id,
                   FullName = dbusers.Staff.FName + " " + dbusers.Staff.LName,
                   PersonelCode = dbusers.Staff.PersonalCode,
                   UserName = dbusers.UserName,
               };
    }

    public IEnumerable<ReportAccessViewModel> GetAccessesForReports()
    {
        return from user in _dbContext.Users
               join roleaccess in _dbContext.RoleAccesses on user.Id equals roleaccess.UserId
               join role in _dbContext.Roles on roleaccess.RoleId equals role.Id
               join roleaction in _dbContext.RoleActions on role.Id equals roleaction.RoleId
               select new ReportAccessViewModel()
               {
                   UserId = user.Id,
                   RoleId = role.Id,
                   UserName = user.UserName,
                   FullName = user.Staff.FName + " " + user.Staff.LName,
                   IsActive = user.IsActive,
                   RoleName = role.Name,
                   RoleDescription = role.Dsr,
                   PersonelCode = user.Staff.PersonalCode,
                   PhoneNumber = user.Staff.PhoneNumber,
                   ControlerName = roleaction.Controller,
                   ActionName = roleaction.Action


               };
    }

    public IEnumerable<ChartViewModel> GetCharts()
    {
        var charts = from chart in _dbContext.Charts
                     join parentchart in _dbContext.Charts on chart.Id equals parentchart.ParentId
                     where chart.IsActive
                     select new ChartViewModel()
                     {
                         Id = parentchart.Id,
                         Title = chart.Title,
                         ParentTitle = parentchart.Title,

                     };
        return charts;
    }

    public IEnumerable<WorkFlowViewModel> GetDiagrams()
    {
        var diagrams = from workflows in _dbContext.Workflows
                       join requesttypes in _dbContext.LookUps on workflows.RequestTypeId equals
                           requesttypes.Id
                       join staff in _dbContext.Staffs on workflows.StaffId equals staff.Id
                       where workflows.IsActive == true && workflows.SubProcessId == null
                       select new WorkFlowViewModel()
                       {
                           Id = workflows.Id,
                           RequestTypeId = workflows.RequestTypeId,
                           RequestType = workflows.RequestType.Title,
                           Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                           Staff = staff.FName + " " + staff.LName
                       };
        return diagrams;

    }

    private static IEnumerable<ControllerPolicyDTO> GetControllersPolicies()
    {
        if (_controllerPolicies.IsValueCreated)
            return _controllerPolicies.Value;

        // todo: uncomment later 
        //_controllerPolicies =
        //    new Lazy<IEnumerable<ControllerPolicyDTO>>(() =>
        //        new ControllerHelper().GetControllersNameAndPolicy());
        return _controllerPolicies.Value;
    }

    public IEnumerable<TreeViewItemModel> GetTreeAccessPolicyBase(Guid? id)
    {
        var treeViewList = new List<TreeViewItemModel>();
        var controllers = GetControllersPolicies().OrderBy(p => p.Name);

        foreach (var controllerItem in controllers)
        {
            if (!controllerItem.ActionPolicies.Any()) continue;

            var controller = new TreeViewItemModel
            {
                Text = controllerItem.Name,
                Id = controllerItem.Policy
            };

            foreach (var actionItem in controllerItem.ActionPolicies.OrderBy(p => p.Name))
            {
                var action = new TreeViewItemModel
                {
                    Text = actionItem.Name,
                    Id = actionItem.Policy
                };

                controller.Items.Add(action);

                if (id == null) continue;

                var rolePolicy = _dbContext.RoleClaims.FirstOrDefault(c =>
                    c.RoleId == id && c.ClaimValue == actionItem.Policy &&
                    c.ClaimType == PermissionPolicyType.RoutePermission);

                if (rolePolicy != null)
                {
                    action.Checked = true;
                }
            }

            treeViewList.Add(controller);

        }

        return treeViewList;
    }

    public List<TreeViewItemModel> GetTreeWidget(Guid? id)
    {
        var list = new List<TreeViewItemModel>();
        var root = _dbContext.LookUps.Where(p => p.Type == "WidgetGroup").ToList();
        var roleWidgets = _dbContext.RoleClaims.Where(c => c.RoleId == id).Select(c => c.ClaimValue).ToList();
        foreach (var widgetGroup in root)
        {
            var widgetGroupId = widgetGroup.Id.ToString();
            var widgets = _dbContext.LookUps.Where(p => p.Aux == widgetGroupId).ToList();
            var listWidget = new List<TreeViewItemModel>();
            foreach (var widget in widgets)
            {
                var check = false;
                if (id != null)
                {
                    check = roleWidgets.Any(d => d == widget.Id.ToString());
                }
                listWidget.Add(new TreeViewItemModel()
                {
                    Text = widget.Title,
                    Id = widget.Id.ToString(),
                    Checked = check
                });
            }

            list.Add(new TreeViewItemModel()
            {
                Text = widgetGroup.Title,
                Id = "widgetGroup",
                Items = listWidget
            });

        }

        return list;
    }

    public void ModifyRoleMapChart(List<Chart> modelCharts, Guid roleId)
    {
        var chartAdd = from chart in modelCharts
                       where !_dbContext.RoleMapCharts.Any(raa => raa.ChartId == chart.Id && raa.RoleId == roleId)
                       select chart.Id;
        foreach (var item in chartAdd.ToList())
        {
            var rolemapchart = new RoleMapChart()
            {
                RoleId = roleId,
                ChartId = item
            };
            _dbContext.RoleMapCharts.Add(rolemapchart);
        }

        var chartsRemove = _dbContext.RoleMapCharts.Where(r => r.RoleId == roleId).ToList()
            .Where(chart => modelCharts.All(u => u.Id != chart.ChartId));
        _dbContext.RoleMapCharts.RemoveRange(chartsRemove);
    }

    public void ModifyRoleMapPostTitle(List<LookUp> modelPostTitles, Guid roleId)
    {
        var postTitleAdd = from postTitle in modelPostTitles
                           where !_dbContext.RoleMapPostTitles.Any(raa => raa.PostTitleId == postTitle.Id && raa.RoleId == roleId)
                           select postTitle.Id;
        foreach (var item in postTitleAdd.ToList())
        {
            var rolemappostitle = new RoleMapPostTitle()
            {
                RoleId = roleId,
                PostTitleId = item
            };
            _dbContext.RoleMapPostTitles.Add(rolemappostitle);
        }

        var postTitleRemove = _dbContext.RoleMapPostTitles.Where(r => r.RoleId == roleId).ToList()
            .Where(postTitle => modelPostTitles.All(u => u.Id != postTitle.PostTitleId));
        _dbContext.RoleMapPostTitles.RemoveRange(postTitleRemove);
    }
    public void ModifyRoleMapPostType(List<LookUp> modelPostTypes, Guid roleId)
    {
        var postTypeAdd = from postType in modelPostTypes
                          where !_dbContext.RoleMapPostTypes.Any(raa => raa.PostTypeId == postType.Id && raa.RoleId == roleId)
                          select postType.Id;
        foreach (var item in postTypeAdd.ToList())
        {
            var rolemappostType = new RoleMapPostType()
            {
                RoleId = roleId,
                PostTypeId = item
            };
            _dbContext.RoleMapPostTypes.Add(rolemappostType);
        }

        var postTypeRemove = _dbContext.RoleMapPostTypes.Where(r => r.RoleId == roleId).ToList()
            .Where(postType => modelPostTypes.All(u => u.Id != postType.PostTypeId));
        _dbContext.RoleMapPostTypes.RemoveRange(postTypeRemove);
    }

    public void CreateNewRole(RoleViewModel model)
    {
        if (model.Id == Guid.Empty || model.Id == null)
        {
            if (model.Description == "sysadmin" || model.Name == "sysadmin")
            {
                throw new ArgumentException("نمی توانید گروهی به این نام ایجاد کنید.");
            }
            var role = new Role()
            {
                Dsr = model.Description,
                Name = model.Name

            };

            _dbContext.Roles.Add(role);
        }
        else
        {
            var role = _dbContext.Roles.Find(model.Id);
            if (role != null)
            {
                if (model.Description == "sysadmin" || model.Name == "sysadmin")
                {
                    throw new ArgumentException("نمی توانید نام گروه را به این نام تغییر دهید.");
                }
                role.Name = model.Name;
                role.Dsr = model.Description;

            }
        }
    }

    public void DeleteRole(Guid id)
    {
        var role = _dbContext.Roles.Find(id);
        if (role == null)
        {
            // Response.StatusCode = (int)HttpStatusCode.NotFound;
            throw new ArgumentException("گروهی برای حذف انتخاب نشده است");
        }
        var roleAccesses = _dbContext.RoleAccesses.Where(r => r.RoleId == id);
        if (roleAccesses.Any())
        {
            throw new ArgumentException("امکان حذف گروه با کاربر فعال وجود ندارد");
        }
        var rolemapcharts = _dbContext.RoleMapCharts.Where(r => r.RoleId == id);
        if (rolemapcharts.Any())
        {
            throw new ArgumentException("امکان حذف گروه  مورد استفاده در چارت سازمانی وجود ندارد");
        }
        var roleMapPostTypes = _dbContext.RoleMapPostTypes.Where(r => r.RoleId == id);
        if (roleMapPostTypes.Any())
        {
            throw new ArgumentException("امکان حذف گروه  مورد استفاده در چارت سازمانی وجود ندارد");
        }
        var roleMapPostTitles = _dbContext.RoleMapPostTitles.Where(r => r.RoleId == id);
        if (roleMapPostTitles.Any())
        {
            throw new ArgumentException("امکان حذف گروه  مورد استفاده در چارت سازمانی وجود ندارد");
        }

        if (role.Dsr == "sysadmin" || role.Name == "sysadmin")
        {
            throw new ArgumentException("امکان حذف گروه sysadmin وجود ندارد.");
        }

        var roleClaim = _dbContext.RoleClaims.Where(r => r.RoleId == id);
        _dbContext.RoleClaims.RemoveRange(roleClaim);
        _dbContext.Roles.Remove(role);
    }

    public bool CheckUserIsAdmin(Guid userId)
    {
        var access = from user in _dbContext.Users
                     join roleAccess in _dbContext.RoleAccesses on user.Id equals roleAccess.UserId
                     join role in _dbContext.Roles on roleAccess.RoleId equals role.Id
                     where role.Name == "sysadmin" && user.Id == userId
                     select role;
        return access.Any();
    }

    public IEnumerable<WorkFlowViewModel> GetProcess()
    {
        var proccess = (from workflows in _dbContext.Workflows
                        join requesttypes in _dbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                        join staff in _dbContext.Staffs on workflows.StaffId equals staff.Id
                        where workflows.SubProcessId == null
                        select new WorkFlowViewModel()
                        {
                            Id = workflows.Id,
                            RequestTypeId = requesttypes.Id,
                            RequestType = requesttypes.Title,
                            Staff = staff.FName + " " + staff.LName
                        }).DistinctBy(r => r.RequestTypeId).ToList();
        return proccess;
    }
    public List<WorkFlowViewModel> GetProcessByIds(List<Guid> ids)
    {
        var process = from workflow in _dbContext.Workflows
                      join requestType in _dbContext.LookUps on workflow.RequestTypeId equals requestType.Id
                      join staff in _dbContext.Staffs on workflow.StaffId equals staff.Id
                      where workflow.SubProcessId == null && ids.Contains(requestType.Id)
                      select new WorkFlowViewModel()
                      {
                          Id = workflow.Id,
                          RequestTypeId = requestType.Id,
                          RequestType = requestType.Title,
                          Staff = staff.FName + " " + staff.LName
                      };
        return process.DistinctBy(r => r.RequestTypeId).ToList();
    }

    public List<WorkFlowViewModel> GetProcessesList()
    {
        var processes = from workflow in _dbContext.Workflows
                        join requestType in _dbContext.LookUps on workflow.RequestTypeId equals requestType.Id
                        join staff in _dbContext.Staffs on workflow.StaffId equals staff.Id
                        where workflow.IsActive == true && workflow.SubProcessId == null
                        select new WorkFlowViewModel()
                        {
                            RequestTypeId = requestType.Id,
                            RequestType = requestType.Title,
                            Staff = staff.FName + " " + staff.LName
                        };
        return processes.ToList();
    }

    public List<WorkFlowViewModel> ProcessBpmnInSpecificRoleByPolicy(Guid? roleId)
    {
        var bpmn = from roleClaim in _dbContext.RoleClaims
                   join workFlow in _dbContext.Workflows on roleClaim.ClaimValue equals workFlow.RequestTypeId.ToString()
                   join lookUp in _dbContext.LookUps on workFlow.RequestTypeId equals lookUp.Id
                   where roleClaim.RoleId == roleId && roleClaim.ClaimType == PermissionPolicyType.WorkFlowPreviewPermission
                   select new WorkFlowViewModel()
                   {
                       Id = workFlow.Id,
                       RequestTypeId = workFlow.RequestTypeId,
                       RequestType = lookUp.Title,
                       Staff = workFlow.Staff.FName + " " + workFlow.Staff.LName

                   };

        return bpmn.GroupBy(d => new { d.RequestTypeId }).Select(s => s.FirstOrDefault()).ToList();
    }

    public List<WorkFlowViewModel> WorkFlowIndexAccessByPolicy(Guid? roleId)
    {
        var processWidget = from roleClaim in _dbContext.RoleClaims
                            join workflow in _dbContext.Workflows on roleClaim.ClaimValue equals workflow.RequestTypeId.ToString()
                            join lookUp in _dbContext.LookUps on workflow.RequestTypeId equals lookUp.Id
                            where roleClaim.RoleId == roleId && roleClaim.ClaimType == PermissionPolicyType.WorkFlowIndexPermission
                            select new WorkFlowViewModel
                            {
                                Id = workflow.Id,
                                RequestTypeId = workflow.RequestTypeId,
                                RequestType = lookUp.Title,
                                Staff = workflow.Staff.FName + " " + workflow.Staff.LName

                            };

        return processWidget.GroupBy(d => new { d.RequestTypeId }).Select(s => s.FirstOrDefault()).ToList();
    }

    public List<WorkFlowFormViewModel> FormsInSpecificRoleByPolicy(Guid? id)
    {
        var forms = from roleClaim in _dbContext.RoleClaims
                    join workFlowForm in _dbContext.WorkFlowForms on roleClaim.ClaimValue equals workFlowForm.Id.ToString()
                    where roleClaim.RoleId == id && roleClaim.ClaimType == PermissionPolicyType.WorkFlowFormPreviewPermission
                    select new WorkFlowFormViewModel()
                    {
                        Id = workFlowForm.Id,
                        PName = workFlowForm.PName,
                        Staff = workFlowForm.Staff.FName + " " + workFlowForm.Staff.LName,
                        SecondaryVersion = workFlowForm.SecondaryVersion,
                        OrginalVersion = workFlowForm.OrginalVersion
                    };

        return forms.ToList();
    }

    public List<DynamicChartViewModel> DynamicChartsInSpecificRoleByPolicy(Guid? id)
    {
        return (from roleClaim in _dbContext.RoleClaims
                join dynamicChart in _dbContext.DynamicCharts on roleClaim.ClaimValue equals dynamicChart.Id.ToString()
                join widgetType in _dbContext.LookUps on dynamicChart.WidgetTypeId equals widgetType.Id
                join creator in _dbContext.Staffs on dynamicChart.CreatorId equals creator.Id
                where roleClaim.RoleId == id && roleClaim.ClaimType == PermissionPolicyType.DynamicChartReportPermission
                select new DynamicChartViewModel()
                {
                    Id = dynamicChart.Id,
                    WidgetType = widgetType.Title,
                    Creator = creator.FName + " " + creator.LName
                }).ToList();
    }

    public List<WorkFlowFormListViewModel> WorkflowFormListInSpecificRoleByPolicy(Guid? id)
    {
        return (from roleClaim in _dbContext.RoleClaims
                join workFlowFormList in _dbContext.WorkFlowFormList on roleClaim.ClaimValue equals workFlowFormList.Id.ToString()
                where roleClaim.RoleId == id && roleClaim.ClaimType == PermissionPolicyType.WorkFlowFormListPermission
                select new WorkFlowFormListViewModel()
                {
                    Id = workFlowFormList.Id,
                    Title = workFlowFormList.Title
                }).ToList();
    }

    public List<ReportViewModel> GeneratedReportsInSpecificRolePolicy(Guid? id)
    {
        return (from roleClaim in _dbContext.RoleClaims
                join report in _dbContext.Reports on roleClaim.ClaimValue equals report.Id.ToString()
                where roleClaim.RoleId == id && roleClaim.ClaimType == PermissionPolicyType.ReportPermission
                select new ReportViewModel()
                {
                    Id = report.Id,
                    Title = report.Title,
                    Creator = report.Creator.FName + " " + report.Creator.LName
                }).ToList();
    }

    public List<WorkFlowViewModel> ProcessStatusInSpecificRolePolicy(Guid? id)
    {
        var processStatus = from roleClaim in _dbContext.RoleClaims
                            join workflow in _dbContext.Workflows on roleClaim.ClaimValue equals workflow.RequestTypeId.ToString()
                            join lookUp in _dbContext.LookUps on workflow.RequestTypeId equals lookUp.Id
                            where roleClaim.RoleId == id && roleClaim.ClaimType == PermissionPolicyType.WorkFlowStatusPermission
                            select new WorkFlowViewModel
                            {
                                Id = workflow.Id,
                                RequestTypeId = workflow.RequestTypeId,
                                RequestType = lookUp.Title,
                                Staff = workflow.Staff.FName + " " + workflow.Staff.LName

                            };

        return processStatus.GroupBy(d => new { d.RequestTypeId }).Select(s => s.FirstOrDefault()).ToList();
    }


    private IQueryable<Guid> GetRoleMapPostTypeAccessId(Guid staffId)
    {
        var userPostTypes = from orgInfo in _dbContext.OrganiztionInfos
                            join orgPostType in _dbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostType.Id
                            where orgInfo.StaffId == staffId && orgInfo.IsActive

                            select new { orgInfo, orgPostType };
        var userPostTypeIds = new List<Guid>();
        foreach (var item in userPostTypes)
        {
            var postTypeId = _dbContext.LookUps.Where(l => l.Id == item.orgInfo.OrganiztionPostId)
                .Select(o => o.Aux).FirstOrDefault();
            if (postTypeId != null)
            {
                userPostTypeIds.Add(Guid.Parse(postTypeId));
            }
        }


        var roleMapPostTypeAccessId = from lookup in _dbContext.LookUps
                                      join roleMapPostType in _dbContext.RoleMapPostTypes
                                          on lookup.Id equals roleMapPostType.PostTypeId
                                      where userPostTypeIds.Contains(roleMapPostType.PostTypeId)
                                      select roleMapPostType.RoleId;

        return roleMapPostTypeAccessId;

    }

    private IQueryable<Guid> GetRoleMapPostTitleAccessId(Guid staffId)
    {

        var userPostTitleIds = new List<Guid>();

        var userPostTitles = from orgInfo in _dbContext.OrganiztionInfos
                             join orgPostTitle in _dbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostTitle.Id
                             where orgInfo.StaffId == staffId && orgInfo.IsActive

                             select new { orgInfo, orgPostTitle };

        foreach (var item in userPostTitles)
        {
            var postTitleId = _dbContext.LookUps.Where(l => l.Id == item.orgInfo.OrganiztionPostId)
                .Select(o => o.Id).FirstOrDefault();
            userPostTitleIds.Add(postTitleId);

        }

        var roleMapPostTitleAccessId = from lookup in _dbContext.LookUps
                                       join roleMapPostTitle in _dbContext.RoleMapPostTitles
                                           on lookup.Id equals roleMapPostTitle.PostTitleId
                                       where userPostTitleIds.Contains(roleMapPostTitle.PostTitleId)
                                       select roleMapPostTitle.RoleId;

        return roleMapPostTitleAccessId;

    }
}