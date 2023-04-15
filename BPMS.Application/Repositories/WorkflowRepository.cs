using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Microsoft.Extensions.DependencyInjection;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace BPMS.Application.Repositories;

public class WorkflowRepository : Repository<Workflow>, IWorkflowRepository
{
    private readonly XNamespace _bpmn2 = "http://www.omg.org/spec/BPMN/20100524/MODEL";
    private readonly XNamespace _workflow = "http://magic";
    private readonly IServiceProvider _serviceProvider;
    public WorkflowRepository(BpmsDbContext context, IServiceProvider serviceProvider) : base(context)
    {
        _serviceProvider = serviceProvider;
    }

    public BpmsDbContext DbContext => Context;

    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();

    public IRoleRepository RoleRepository => _serviceProvider.GetRequiredService<IRoleRepository>();

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
    public IEnumerable<RequestTypeDropDownViewModel> RequestTypesByPolicy(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;

        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartAccessId = from organizationInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                                   join roleMapChart in DbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
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


        var checkAccessForRol = (from workflow in DbContext.Workflows
                                 join requestType in DbContext.LookUps on workflow.RequestTypeId equals requestType.Id
                                 join roleClaim in DbContext.RoleClaims on requestType.Id.ToString() equals roleClaim.ClaimValue
                                 join roleId in roleIds on roleClaim.RoleId equals roleId
                                 where workflow.IsActive && workflow.SubProcessId == null && workflow.RequestGroupType.IsActive && roleClaim.ClaimType == PermissionPolicyType.WorkFlowPermission
                                 select new RequestTypeDropDownViewModel()
                                 {
                                     Text = requestType.Title + " " + workflow.KeyWords,
                                     ValueTemplate = requestType.Title,
                                     Tag = workflow.KeyWords,
                                     Value = requestType.Id.ToString(),
                                     Group = workflow.RequestGroupType.Title
                                 }).OrderBy(o => o.Text).Distinct();

        var checkAccessForPerson = (from workflow in DbContext.Workflows
                                    join requestType in DbContext.LookUps on workflow.RequestTypeId equals requestType.Id
                                    join userClaim in DbContext.UserClaims on requestType.Id.ToString() equals userClaim.ClaimValue
                                    where workflow.IsActive && workflow.SubProcessId == null && workflow.RequestGroupType.IsActive && userClaim.ClaimType == PermissionPolicyType.WorkFlowPermission && userClaim.UserId == userId
                                    select new RequestTypeDropDownViewModel()
                                    {
                                        Text = requestType.Title + " " + workflow.KeyWords,
                                        ValueTemplate = requestType.Title,
                                        Tag = workflow.KeyWords,
                                        Value = requestType.Id.ToString(),
                                        Group = workflow.RequestGroupType.Title
                                    }).OrderBy(o => o.Text).Distinct();

        var final = checkAccessForRol.Union(checkAccessForPerson);
        return final;
    }

    public IEnumerable<RequestTypeDropDownViewModel> RequestTypes(Guid staffId, bool remote)
    {
        var userId = DbContext.Users.Single(d => d.StaffId == staffId).Id;

        var access = from user in DbContext.Users
                     join roleAccess in DbContext.RoleAccesses on user.Id equals roleAccess.UserId
                     join role in DbContext.Roles on roleAccess.RoleId equals role.Id
                     where role.Name == "sysadmin" && user.Id == userId
                     select role;
        if (access.Any())
        {
            return (from workflows in DbContext.Workflows
                    join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                    where workflows.IsActive && workflows.SubProcessId == null && (!remote || workflows.RemoteId != null)
                    select new RequestTypeDropDownViewModel()
                    {
                        Text = requesttypes.Title + " " + workflows.KeyWords,
                        ValueTemplate = requesttypes.Title,
                        Tag = workflows.KeyWords,
                        Value = requesttypes.Id.ToString(),
                        Group = workflows.RequestGroupType.Title
                    }).OrderBy(o => o.Text).Distinct();

        }


        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var rolemapchartaccessId = from organiztionInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organiztionInfo.ChartId equals chart.Id
                                   join rolemapchar in DbContext.RoleMapCharts on chart.Id equals rolemapchar.ChartId
                                   where organiztionInfo.StaffId == staffId
                                   select rolemapchar.RoleId;


        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleId = new List<Guid>();
        roleId.AddRange(roleMapPostTitleAccessId);
        roleId.AddRange(rolemapchartaccessId);
        roleId.AddRange(roleAccessesId);
        roleId.AddRange(roleMapPostTypeAccessId);
        roleId = roleId.Distinct().ToList();

        return (from workflows in DbContext.Workflows
                join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                join roleClaim in DbContext.RoleClaims on workflows.RequestTypeId.ToString() equals roleClaim.ClaimValue
                join roleid in roleId on roleClaim.RoleId equals roleid
                where workflows.IsActive && workflows.SubProcessId == null && (!remote || workflows.RemoteId != null)
                      && roleClaim.ClaimType == PermissionPolicyType.WorkFlowPermission
                select new RequestTypeDropDownViewModel()
                {
                    Text = requesttypes.Title + " " + workflows.KeyWords,
                    ValueTemplate = requesttypes.Title,
                    Tag = workflows.KeyWords,
                    Value = requesttypes.Id.ToString(),
                    Group = workflows.RequestGroupType.Title
                }).OrderBy(o => o.Text).Distinct();
    }

    public IEnumerable<SelectListItem> OrganizationPostTitle(Guid staffId)
    {
        return from orgInfo in DbContext.OrganiztionInfos
               join orgPostTitle in DbContext.LookUps on orgInfo.OrganiztionPostId equals orgPostTitle.Id
               where orgInfo.StaffId == staffId && orgInfo.IsActive
               orderby orgInfo.Priority descending
               select new SelectListItem()
               {
                   Text = orgPostTitle.Title + @"/" + orgInfo.Chart.Title,
                   Value = orgInfo.OrganiztionPostId.ToString()
                   //Value = orgInfo.Id.ToString()
               };
    }

    public List<int> CountOfEachTypeByUserName(string username)
    {
        var listCount = new List<int>();

        var currentUser = DbContext.Users.Single(c => c.UserName == username);

        var query1 = from w in DbContext.WorkFlowConfermentAuthority
                     join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                     join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                     join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                     join flow in DbContext.Flows on request.Id equals flow.RequestId
                     where wd.StaffId == currentUser.StaffId && flow.LookUpFlowStatus.Code == 1 && flow.StaffId == w.StaffId && (request.RegisterDate >= wd.FromDate && request.RegisterDate <= wd.ToDate)
                           && (!wd.OnlyOwnRequest || request.StaffId == currentUser.StaffId) && flow.IsActive
                     select flow;
        var query2 = from f in DbContext.Flows
                     where f.StaffId == currentUser.StaffId && f.LookUpFlowStatus.Code == 1 && f.IsActive
                     select f;

        var reqNotInProgressCount = query1.Union(query2).Count();

        listCount.Add(reqNotInProgressCount);

        var sendnotInProgress = from req in DbContext.Requests
                                join flow in DbContext.Flows
                                    on req.Id equals flow.RequestId
                                where req.RequestStatus.Code == 1 && req.StaffId == currentUser.StaffId
                                select req;
        var sendnotInProgressCount = sendnotInProgress.GroupBy(d => d.RequestNo).Select(d => d.FirstOrDefault()).Count();

        listCount.Add(sendnotInProgressCount);

        var sendInProgress = from req in DbContext.Requests
                             join flow in DbContext.Flows
                                 on req.Id equals flow.RequestId
                             where req.RequestStatus.Code == 2 && req.StaffId == currentUser.StaffId
                             select req;

        var sendInProgressCount = sendInProgress.GroupBy(d => d.RequestNo).Select(d => d.FirstOrDefault()).Count();

        listCount.Add(sendInProgressCount);

        return listCount;
    }


    public List<int> CountOfEachType(string personalCode)
    {
        return GetCountOfEachPersonal(personalCode);
    }
    private List<int> GetCountOfEachPersonal(string personalCode)
    {

        var listCount = new List<int>();
        var query1 = from w in DbContext.WorkFlowConfermentAuthority
                     join wd in DbContext.WorkFlowConfermentAuthorityDetails on w.Id equals wd.ConfermentAuthorityId
                     join workflows in DbContext.Workflows on w.RequestTypeId equals workflows.RequestTypeId
                     join request in DbContext.Requests on workflows.Id equals request.WorkFlowId
                     join flow in DbContext.Flows on request.Id equals flow.RequestId
                     where wd.Staffs.PersonalCode == personalCode && flow.LookUpFlowStatus.Code == 1 && flow.StaffId == w.StaffId && flow.IsActive
                           && (!wd.OnlyOwnRequest || request.Staff.PersonalCode == personalCode)
                     select flow;
        var query2 = from f in DbContext.Flows
                     where f.Staff.PersonalCode == personalCode && f.LookUpFlowStatus.Code == 1
                     select f;
        var reqNotInProgressCount = query1.Union(query2).Count();
        listCount.Add(reqNotInProgressCount);
        var sendnotInProgress = from req in DbContext.Requests
                                join flow in DbContext.Flows
                                    on req.Id equals flow.RequestId
                                where req.RequestStatus.Code == 1 && req.Staff.PersonalCode == personalCode && flow.IsActive
                                select flow;
        var sendnotInProgressCount = sendnotInProgress.Distinct().Count();
        listCount.Add(sendnotInProgressCount);
        var sendInProgress = from req in DbContext.Requests
                             join flow in DbContext.Flows
                                 on req.Id equals flow.RequestId
                             where req.RequestStatus.Code == 2 && req.Staff.PersonalCode == personalCode && flow.IsActive
                             select req;
        var sendInProgressCount = sendInProgress.Distinct().Count();
        listCount.Add(sendInProgressCount);
        return listCount;
    }
    public IEnumerable<WorkFlowViewModel> GetAllWorkflows()
    {
        var requestTypes = from workflows in DbContext.Workflows
                           join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals
                               requesttypes.Id
                           where workflows.SubProcessId == null
                           select new WorkFlowViewModel()
                           {

                               RequestType = workflows.RequestType.Title,
                               Dsr = workflows.Dsr,
                               Id = workflows.Id,
                               IsActive = workflows.IsActive,
                               StaffId = workflows.StaffId,
                               RequestTypeId = workflows.RequestTypeId,
                               Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                               RegisterDateTime = workflows.RegisterDate != 0 ? workflows.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.RegisterTime.Insert(2, ":") : "",
                               ModifiedDateTime = workflows.ModifideDate != 0 && workflows.ModifideDate != null ? workflows.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.ModifideTime.Insert(2, ":") : "",
                               Staff = workflows.Staff.FName + " " + workflows.Staff.LName,
                               Modifier = workflows.Modifier.FName + " " + workflows.Modifier.LName,
                               FlowType = workflows.FlowType.Title,
                               RequestGroupType = workflows.RequestGroupType.Title
                           };
        return requestTypes.OrderByDescending(i => i.RegisterDateTime);
    }

    public IEnumerable<WorkFlowViewModel> GetByStaffRegisterByPolicy(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;
        var staffId = user.StaffId;

        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartAccessId = from organizationInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                                   join roleMapChar in DbContext.RoleMapCharts on chart.Id equals roleMapChar.ChartId
                                   where organizationInfo.StaffId == staffId
                                   select roleMapChar.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(roleMapChartAccessId);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var checkProcessInAccess = from workflow in DbContext.Workflows
                                   join requestType in DbContext.LookUps on workflow.RequestTypeId equals requestType.Id
                                   join roleClaim in DbContext.RoleClaims on requestType.Id.ToString() equals roleClaim.ClaimValue
                                   join roleId in roleIds on roleClaim.RoleId equals roleId
                                   where workflow.SubProcessId == null && roleClaim.ClaimType == PermissionPolicyType.WorkFlowPreviewPermission
                                   select new WorkFlowViewModel()
                                   {

                                       RequestType = workflow.RequestType.Title,
                                       Dsr = workflow.Dsr,
                                       Id = workflow.Id,
                                       IsActive = workflow.IsActive,
                                       StaffId = workflow.StaffId,
                                       RequestTypeId = workflow.RequestTypeId,
                                       Version = workflow.OrginalVersion + "." + workflow.SecondaryVersion,
                                       RegisterDateTime = workflow.RegisterDate != 0 ? workflow.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflow.RegisterTime.Insert(2, ":") : "",
                                       ModifiedDateTime = workflow.ModifideDate != 0 && workflow.ModifideDate != null ? workflow.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflow.ModifideTime.Insert(2, ":") : "",
                                       Time = workflow.RegisterTime,
                                       SortingRegisterDate = workflow.RegisterDate,
                                       Staff = workflow.Staff.FName + " " + workflow.Staff.LName,
                                       Modifier = workflow.Modifier.FName + " " + workflow.Modifier.LName,
                                       FlowType = workflow.FlowType.Title,
                                       RequestGroupType = workflow.RequestGroupType.Title
                                   };



        var checkProcessForPerson = from workflows in DbContext.Workflows
                                    join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                                    where workflows.StaffId == staffId && workflows.SubProcessId == null
                                    select new WorkFlowViewModel()
                                    {

                                        RequestType = workflows.RequestType.Title,
                                        Dsr = workflows.Dsr,
                                        Id = workflows.Id,
                                        IsActive = workflows.IsActive,
                                        StaffId = workflows.StaffId,
                                        RequestTypeId = workflows.RequestTypeId,
                                        Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                                        RegisterDateTime = workflows.RegisterDate != 0 ? workflows.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.RegisterTime.Insert(2, ":") : "",
                                        ModifiedDateTime = workflows.ModifideDate != 0 && workflows.ModifideDate != null ? workflows.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.ModifideTime.Insert(2, ":") : "",
                                        Time = workflows.RegisterTime,
                                        SortingRegisterDate = workflows.RegisterDate,
                                        Staff = workflows.Staff.FName + " " + workflows.Staff.LName,
                                        Modifier = workflows.Modifier.FName + " " + workflows.Modifier.LName,
                                        FlowType = workflows.FlowType.Title,
                                        RequestGroupType = workflows.RequestGroupType.Title
                                    };
        var all = checkProcessForPerson.Union(checkProcessInAccess);
        var allDistinct = new List<WorkFlowViewModel>();
        foreach (var item in all.Distinct())
        {
            allDistinct.Add(new WorkFlowViewModel()
            {
                SortingRegisterTime = (item.Time != null && int.Parse(item.Time) != 0) ? int.Parse(item.Time) : 0,
                RequestType = item.RequestType,
                Dsr = item.Dsr,
                Id = item.Id,
                IsActive = item.IsActive,
                StaffId = item.StaffId,
                RequestTypeId = item.RequestTypeId,
                Version = item.Version,
                RegisterDateTime = item.RegisterDateTime,
                ModifiedDateTime = item.ModifiedDateTime,
                Time = item.Time,
                SortingRegisterDate = item.SortingRegisterDate,
                Staff = item.Staff,
                Modifier = item.Modifier,
                FlowType = item.FlowType,
                RequestGroupType = item.RequestGroupType

            });
        }

        return allDistinct.OrderByDescending(i => i.SortingRegisterDate).ThenByDescending(i => i.SortingRegisterTime);

    }

    public IEnumerable<WorkFlowViewModel> GetByStaffRegister(Guid staffId)
    {
        var userId = DbContext.Users.Single(d => d.StaffId == staffId).Id;

        var access = from user in DbContext.Users
                     join roleAccess in DbContext.RoleAccesses on user.Id equals roleAccess.UserId
                     join role in DbContext.Roles on roleAccess.RoleId equals role.Id
                     where role.Name == "sysadmin" && user.Id == userId
                     select role;
        if (access.Any())
        {
            return GetAllWorkflows();
        }


        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var rolemapchartaccessId = from organiztionInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organiztionInfo.ChartId equals chart.Id
                                   join rolemapchar in DbContext.RoleMapCharts on chart.Id equals rolemapchar.ChartId
                                   where organiztionInfo.StaffId == staffId
                                   select rolemapchar.RoleId;

        var roleMapPostTypeAccessId = GetRoleMapPostTypeAccessId(staffId);

        var roleMapPostTitleAccessId = GetRoleMapPostTitleAccessId(staffId);

        var roleIds = new List<Guid>();
        roleIds.AddRange(roleMapPostTitleAccessId);
        roleIds.AddRange(rolemapchartaccessId);
        roleIds.AddRange(roleAccessesId);
        roleIds.AddRange(roleMapPostTypeAccessId);
        roleIds = roleIds.Distinct().ToList();

        var checkProcessInAccess = from workflows in DbContext.Workflows
                                   join roleClaim in DbContext.RoleClaims on workflows.RequestTypeId.ToString() equals roleClaim.ClaimValue
                                   join roleId in roleIds on roleClaim.RoleId equals roleId
                                   where workflows.SubProcessId == null && roleClaim.ClaimType == PermissionPolicyType.WorkFlowPreviewPermission
                                   select new WorkFlowViewModel()
                                   {

                                       RequestType = workflows.RequestType.Title,
                                       Dsr = workflows.Dsr,
                                       Id = workflows.Id,
                                       IsActive = workflows.IsActive,
                                       StaffId = workflows.StaffId,
                                       RequestTypeId = workflows.RequestTypeId,
                                       Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                                       RegisterDateTime = workflows.RegisterDate != 0 ? workflows.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.RegisterTime.Insert(2, ":") : "",
                                       ModifiedDateTime = workflows.ModifideDate != 0 && workflows.ModifideDate != null ? workflows.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.ModifideTime.Insert(2, ":") : "",
                                       Staff = workflows.Staff.FName + " " + workflows.Staff.LName,
                                       Modifier = workflows.Modifier.FName + " " + workflows.Modifier.LName,
                                       FlowType = workflows.FlowType.Title,
                                       RequestGroupType = workflows.RequestGroupType.Title
                                   };
        var checkProcessForPerson = from workflows in DbContext.Workflows
                                    join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                                    where workflows.StaffId == staffId && workflows.SubProcessId == null
                                    select new WorkFlowViewModel()
                                    {

                                        RequestType = workflows.RequestType.Title,
                                        Dsr = workflows.Dsr,
                                        Id = workflows.Id,
                                        IsActive = workflows.IsActive,
                                        StaffId = workflows.StaffId,
                                        RequestTypeId = workflows.RequestTypeId,
                                        Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                                        RegisterDateTime = workflows.RegisterDate != 0 ? workflows.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.RegisterTime.Insert(2, ":") : "",
                                        ModifiedDateTime = workflows.ModifideDate != 0 && workflows.ModifideDate != null ? workflows.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.ModifideTime.Insert(2, ":") : "",
                                        Staff = workflows.Staff.FName + " " + workflows.Staff.LName,
                                        Modifier = workflows.Modifier.FName + " " + workflows.Modifier.LName,
                                        FlowType = workflows.FlowType.Title,
                                        RequestGroupType = workflows.RequestGroupType.Title
                                    };
        var all = checkProcessForPerson.Union(checkProcessInAccess);
        return all.Distinct();

    }
    public BpmnDiagramViewModel OpenBpmnPage(bool? isCopy, Guid? id, string filePath)
    {
        var grouptype = from requesttypes in DbContext.LookUps.ToList()
                        where requesttypes.Type == "RequestGroupType" && requesttypes.IsActive
                        select new SelectListItem()
                        {
                            Text = requesttypes.Title,
                            Value = requesttypes.Id.ToString()
                        };
        var flowType = from flowTypes in DbContext.LookUps.ToList()
                       where flowTypes.Type == "FlowType" && flowTypes.IsActive
                       select new SelectListItem()
                       {
                           Text = flowTypes.Title,
                           Value = flowTypes.Id.ToString()
                       };
        var activeStaff = from staff in DbContext.Staffs
                          join user in DbContext.Users on staff.Id equals user.StaffId
                          join eng in DbContext.LookUps on staff.EngTypeId equals eng.Id
                          where user.IsActive && eng.Code == 1
                          orderby staff.FName + " " + staff.LName
                          select new SelectListItem()
                          {
                              Text = staff.FName + @" " + staff.LName,
                              Value = user.UserName
                          };
        if (id == null)
        {
            //var tasktype = from requesttypes in _dbContext.LookUps.ToList()
            //               where requesttypes.Type == "RequestType" && requesttypes.IsActive && _dbContext.Workflows.All(w => w.RequestTypeId != requesttypes.Id)
            //               select new SelectListItem()
            //               {
            //                   Text = requesttypes.Title,
            //                   Value = requesttypes.Id.ToString()
            //               };

            var model = new BpmnDiagramViewModel()
            {
                //  RequestTypeListItem = tasktype.ToList(),
                FlowTypeListItem = flowType,
                RequestGroupTypeListItem = grouptype,
                StaffListItem = activeStaff,
                Workflow = new WorkFlowViewModel() { OrginalVersion = 1, SecondaryVersion = 0 },
                Charts = new List<TreeViewItemModel>()
            };

            return model;
        }
        if (isCopy.Value)
        {
            var query = DbContext.Workflows.Find(id.Value);
            UnicodeEncoding encoding = new UnicodeEncoding();
            var str = encoding.GetString(query.Content);
            var myxml = XDocument.Parse(str);
            XNamespace workflowXns = "http://magic";
            var workflowid = myxml.Descendants().ToList().Attributes(workflowXns + "WorkFlowDetailId");
            foreach (var xAttribute in workflowid)
            {
                xAttribute.Value = Guid.NewGuid().ToString();
            }
            str = myxml.ToString();
            var workFlow = new WorkFlowViewModel()
            {
                RequestType = query.RequestType.Title,
                Dsr = query.Dsr,
                About = query.About,
                Id = new Guid(),
                IdForCopy = query.Id,
                //IsActive = query.IsActive,
                FlowTypeId = query.FlowTypeId,
                StaffId = query.StaffId,
                RequestGroupTypeId = query.RequestGroupTypeId,
                RequestTypeId = query.RequestTypeId,
                OrginalVersion = query.OrginalVersion,
                SecondaryVersion = query.SecondaryVersion + 1,
                //RegisterDate = query.RegisterDate,
                Staff = query.Staff.FullName,
                XmlFile = HelperBs.EncodeUri(str),
                Content = query.Content,
                Owner = query.Owner,
                SubProcessId = query.SubProcessId,
                KeyWords = query.KeyWords
            };
            var model = new BpmnDiagramViewModel()
            {
                FlowTypeListItem = flowType,
                Workflow = workFlow,
                RequestGroupTypeListItem = grouptype,
                Charts = new List<TreeViewItemModel>(),
                StaffListItem = activeStaff
            };
            return model;
        }
        else
        {

            var query = DbContext.Workflows.Find(id.Value);
            UnicodeEncoding encoding = new UnicodeEncoding();
            var str = encoding.GetString(query.Content);

            string tutorialFileName = "";
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                tutorialFileName = Path.GetFileName(filePath);
            }

            var workFlow = new WorkFlowViewModel()
            {
                RequestType = query.RequestType.Title,
                Dsr = query.Dsr,
                About = query.About,
                Id = query.Id,
                IsActive = query.IsActive,
                FlowTypeId = query.FlowTypeId,
                RequestGroupTypeId = query.RequestGroupTypeId,
                StaffId = query.StaffId,
                RequestTypeId = query.RequestTypeId,
                OrginalVersion = query.OrginalVersion,
                SecondaryVersion = query.SecondaryVersion,
                // RegisterDate = query.RegisterDate,
                Staff = query.Staff.FullName,
                XmlFile = HelperBs.EncodeUri(str),
                Content = query.Content,
                Owner = query.Owner,
                KeyWords = query.KeyWords,
                SubProcessId = query.SubProcessId,
                ExternalId = query.ExternalId
            };
            var model = new BpmnDiagramViewModel()
            {
                FlowTypeListItem = flowType,
                Workflow = workFlow,
                Charts = new List<TreeViewItemModel>(),
                RequestGroupTypeListItem = grouptype,
                StaffListItem = activeStaff,
                TutorialFileName = tutorialFileName
            };
            return model;
        }
    }

    public void CheckVersion(Guid reqTypeId, int orgVersion, int secVersion)
    {
        var maxOrg = DbContext.Workflows.Where(r => r.RequestTypeId == reqTypeId).Max(l => l.OrginalVersion);
        var maxSec = DbContext.Workflows.Where(r => r.RequestTypeId == reqTypeId && r.OrginalVersion == maxOrg).Max(l => l.SecondaryVersion);
        if (orgVersion < maxOrg)
        {
            throw new ArgumentException("نسخه وارد شده کوچک تر یا مساوی نسخه فعلی وارد شده است.");
        }
        if (orgVersion == maxOrg && secVersion <= maxSec) throw new ArgumentException("نسخه وارد شده کوچک تر یا مساوی نسخه فعلی وارد شده است.");
    }
    public void DeleteWorkFlowById(Guid id)
    {
        var req = DbContext.Requests.Where(f => f.WorkFlowId == id).ToList();
        if (req.Any())
        {
            throw new ArgumentException("به دلیل استفاده این فرآیند در گردش کار قابل حذف نیست");
        }
        var workFlowDetails = DbContext.WorkFlowDetails.Where(w => w.WorkFlowId == id).ToList();
        foreach (var workFlowDetail in workFlowDetails)
        {
            var flow = DbContext.Flows.Where(f => f.WorkFlowDetailId == workFlowDetail.Id).ToList();
            if (flow.Any())
            {
                throw new ArgumentException("به دلیل استفاده این فرآیند در گردش کار قابل حذف نیست");
            }
            var nextstep = DbContext.WorkFlowNextSteps.Where(w => w.FromWfdId == workFlowDetail.Id || w.ToWfdId == workFlowDetail.Id).ToList();
            DbContext.WorkFlowNextSteps.RemoveRange(nextstep);
        }
        DbContext.WorkFlowDetails.RemoveRange(workFlowDetails);

        var workFlow = DbContext.Workflows.Find(id);

        DbContext.Workflows.Remove(workFlow);
        var subProcess = DbContext.Workflows.Where(d => d.SubProcessId == id).ToList();
        foreach (var workflow in subProcess)
        {
            DeleteWorkFlowById(workflow.Id);
        }
    }

    public Guid GetWorkflowIdForDetails(Guid requestTypeId)
    {
        Workflow workflow;
        workflow = DbContext.Workflows.FirstOrDefault(r => r.RequestTypeId == requestTypeId && r.IsActive);
        if (workflow == null)
        {
            workflow = DbContext.Workflows.Where(r => r.RequestTypeId == requestTypeId)
                .OrderByDescending(d => d.OrginalVersion).ThenByDescending(t => t.SecondaryVersion)
                .FirstOrDefault();
            if (workflow == null)
            {
                throw new ArgumentException("گردش کاری برای این فرآیند ایجاد نشده است. ");
            }
        }

        return workflow.Id;
    }

    public Stream DownloadPackage(Guid id)
    {
        var workflowArray = new JArray();
        var workflow = DbContext.Workflows.Find(id);
        if (workflow == null)
        {
            throw new ArgumentException("فرآیند وجود ندارد.");
        }

        if (workflow.FlowType.Code == 1)
        {
            throw new ArgumentException("فرآیند ثبت نهایی نشده است.");
        }
        AddWorkFlowFOrPackage(workflow, workflowArray);
        return Util.GenerateStreamFromString(workflowArray.ToString());
    }

    public string CheckForDownloadPackage(Guid id)
    {

        var workflow = DbContext.Workflows.Find(id);
        if (workflow == null)
        {
            return "فرآیند وجود ندارد.";
        }
        if (workflow.FlowType.Code == 1)
        {
            return "فرآیند ثبت نهایی نشده است.";
        }

        return "";
    }

    private void AddWorkFlowFOrPackage(Workflow workflow, JArray workflowArray)
    {
        var encoding = new UnicodeEncoding();
        var str = encoding.GetString(workflow.Content);
        //  var bpmn = JsonConvert.SerializeObject();
        var objBpmn = new JObject()
        {
            ["Id"] = workflow.Id,
            ["Content"] = workflow.Content,
            ["Dsr"] = workflow.Dsr,
            ["IsActive"] = workflow.IsActive,
            ["OrginalVersion"] = workflow.OrginalVersion,
            ["SecondaryVersion"] = workflow.SecondaryVersion,
            ["RemoteId"] = workflow.RemoteId,
            ["Owner"] = workflow.Owner,
            ["CodeId"] = workflow.CodeId,
            ["About"] = workflow.About,
            ["SubProcessId"] = workflow.SubProcessId,
            ["KeyWords"] = workflow.KeyWords,
            ["ModifiedId"] = workflow.ModifiedId,
            ["ModifideDate"] = workflow.ModifideDate,
            ["ModifideTime"] = workflow.ModifideTime,
            ["RegisterDate"] = workflow.RegisterDate,
            ["RegisterTime"] = workflow.RegisterTime,
        };

        var myxml = XDocument.Parse(str);
        XNamespace workflowXns = "http://magic";
        var formArray = new JArray();
        var workflowForms = myxml.Descendants().ToList().Attributes(workflowXns + "WorkFlowFormId");
        foreach (var xAttribute in workflowForms.DistinctBy(d => d.Value))
        {
            var workflowformId = Guid.Parse(xAttribute.Value);
            var workflowform = DbContext.WorkFlowForms.Find(workflowformId);
            if (workflowform != null)
            {
                AddFormForPackage(formArray, workflowform, encoding);
            }
        }

        var obj = new JObject()
        {
            ["bpmn"] = objBpmn,
            ["forms"] = formArray,
            ["RequestType"] = new JObject()
            {
                ["Id"] = workflow.RequestType.Id,
                ["Title"] = workflow.RequestType.Title,
                ["Type"] = workflow.RequestType.Type,
            }
        };
        if (workflow.RequestGroupTypeId != null)
        {
            obj["RequestGroupType"] = new JObject()
            {
                ["Id"] = workflow.RequestGroupType.Id,
                ["Title"] = workflow.RequestGroupType.Title,
                ["Type"] = workflow.RequestGroupType.Type,
            };
        }
        workflowArray.Add(obj);
        var subProcess = DbContext.Workflows.Where(d => d.SubProcessId == workflow.Id).ToList();
        foreach (var workflow1 in subProcess)
        {
            AddWorkFlowFOrPackage(workflow1, workflowArray);
        }
    }

    private void AddFormForPackage(JArray formArray, WorkFlowForm workflowform, UnicodeEncoding encoding)
    {

        formArray.Add(new JObject()
        {
            ["Id"] = workflowform.Id,
            ["PName"] = workflowform.PName,
            ["Jquery"] = workflowform.Jquery,
            ["AdditionalCssStyleCode"] = workflowform.AdditionalCssStyleCode,
            ["Content"] = workflowform.Content,
            ["OrginalVersion"] = workflowform.OrginalVersion,
            ["SecondaryVersion"] = workflowform.SecondaryVersion,
            ["DocumentCode"] = workflowform.DocumentCode,
            ["ModifiedId"] = workflowform.ModifiedId,
            ["RegisterDate"] = workflowform.RegisterDate,
            ["RegisterTime"] = workflowform.RegisterTime,
            ["ModifideDate"] = workflowform.ModifideDate,
            ["ModifideTime"] = workflowform.ModifideTime,

        });
        var json = encoding.GetString(workflowform.Content);

        var jobj = JObject.Parse(json);

        var fileds = jobj["fields"];
        foreach (var filed in fileds)
        {
            var fli = filed.First.ToObject<JObject>();
            var type = (string)fli["meta"]["id"];

            if (type == "loadform")
            {
                var value = (string)fli["attrs"]["value"];
                var workflowformId = Guid.Parse(value);
                var sub = DbContext.WorkFlowForms.Find(workflowformId);

                AddFormForPackage(formArray, sub, encoding);
            }
        }
    }

    public void UploadPackage(PackageViewModel model, string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);

        var maxcodeRequestType = LookUpRepository.GetNewCode(model.RequestType.Type);

        var reqType = new LookUp()
        {
            Id = model.RequestType.Id.Value,
            Title = model.RequestType.Title,
            Type = model.RequestType.Type,
            Code = maxcodeRequestType,
            IsActive = true
        };
        DbContext.LookUps.Update(reqType);

        LookUp gReqType = null;
        if (model.RequestGroupType != null)
        {
            var maxcodeRequestGroupType = LookUpRepository.GetNewCode(model.RequestGroupType.Type);
            gReqType = new LookUp()
            {
                Id = model.RequestGroupType.Id.Value,
                Title = model.RequestGroupType.Title,
                Type = model.RequestGroupType.Type,
                Code = maxcodeRequestGroupType,
                IsActive = true
            };
            DbContext.LookUps.Update(gReqType);
        }

        if (model.Forms != null)
        {
            foreach (var item in model.Forms.ToList())
            {
                if (DbContext.WorkFlowForms.Local.All(d => d.Id != item.Id))
                {
                    var f = new WorkFlowForm()
                    {
                        Id = item.Id,
                        Content = item.Content,
                        Jquery = item.Jquery,
                        AdditionalCssStyleCode = item.AdditionalCssStyleCode,
                        PName = item.PName,
                        StaffId = user.StaffId,
                        DocumentCode = item.DocumentCode,
                        OrginalVersion = item.OrginalVersion,
                        SecondaryVersion = item.SecondaryVersion,
                        ModifiedId = user.StaffId,
                        ModifideDate = null,
                        ModifideTime = null,
                        RegisterDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
                        RegisterTime = DateTime.Now.ToString("HHmm")
                    };
                    DbContext.WorkFlowForms.Update(f);
                }
            }
        }


        var workflow = new Workflow()
        {
            Id = model.Bpmn.Id,
            Content = model.Bpmn.Content,
            Dsr = model.Bpmn.Dsr,
            FlowTypeId = DbContext.LookUps.Single(d => d.Type == "FlowType" && d.Code == 1).Id,
            IsActive = false,
            RegisterDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
            RegisterTime = DateTime.Now.ToString("HHmm"),
            OrginalVersion = model.Bpmn.OrginalVersion,
            SecondaryVersion = model.Bpmn.SecondaryVersion,
            RemoteId = model.Bpmn.RemoteId,
            RequestGroupTypeId = gReqType?.Id,
            RequestTypeId = reqType.Id,
            StaffId = user.StaffId,
            CodeId = model.Bpmn.CodeId,
            About = model.Bpmn.About,
            Owner = model.Bpmn.Owner,
            SubProcessId = model.Bpmn.SubProcessId,
            KeyWords = model.Bpmn.KeyWords,
            ModifiedId = user.StaffId,
            ModifideDate = null,
            ModifideTime = null
        };
        DbContext.Workflows.Update(workflow);
    }

    public IEnumerable<WorkFlowViewModel> GetActiveWorkFlows(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;

        var roleAccessesId = from ra in DbContext.RoleAccesses
                             where ra.UserId == userId
                             select ra.RoleId;

        var roleMapChartAccessId = from organizationInfo in DbContext.OrganiztionInfos
                                   join chart in DbContext.Charts on organizationInfo.ChartId equals chart.Id
                                   join roleMapChart in DbContext.RoleMapCharts on chart.Id equals roleMapChart.ChartId
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


        var checkAccessForRol = (from workflows in DbContext.Workflows
                                 join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                                 join roleClaim in DbContext.RoleClaims on requesttypes.Id.ToString() equals roleClaim.ClaimValue
                                 join roleId in roleIds on roleClaim.RoleId equals roleId
                                 where workflows.SubProcessId == null && workflows.IsActive == true && roleClaim.ClaimType == PermissionPolicyType.WorkFlowPermission
                                 select new WorkFlowViewModel()
                                 {
                                     RequestType = workflows.RequestType.Title,
                                     Dsr = workflows.Dsr,
                                     Id = workflows.Id,
                                     IsActive = workflows.IsActive,
                                     StaffId = workflows.StaffId,
                                     RequestTypeId = workflows.RequestTypeId,
                                     Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                                     RegisterDateTime = workflows.RegisterDate >= 7 ? workflows.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.RegisterTime.Insert(2, ":") : "",
                                     ModifiedDateTime = workflows.ModifideDate >= 7 ? workflows.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.ModifideTime.Insert(2, ":") : "",
                                     Staff = workflows.Staff.FName + " " + workflows.Staff.LName,
                                     Modifier = workflows.Modifier.FName + " " + workflows.Modifier.LName,
                                     FlowType = workflows.FlowType.Title,
                                     RequestGroupType = workflows.RequestGroupType.Title
                                 }).Distinct();

        var checkAccessForPerson = (from workflows in DbContext.Workflows
                                    join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                                    join userClaim in DbContext.UserClaims on requesttypes.Id.ToString() equals userClaim.ClaimValue
                                    where workflows.SubProcessId == null && workflows.IsActive == true && userClaim.ClaimType == PermissionPolicyType.WorkFlowPermission && userClaim.UserId == userId
                                    select new WorkFlowViewModel()
                                    {
                                        RequestType = workflows.RequestType.Title,
                                        Dsr = workflows.Dsr,
                                        Id = workflows.Id,
                                        IsActive = workflows.IsActive,
                                        StaffId = workflows.StaffId,
                                        RequestTypeId = workflows.RequestTypeId,
                                        Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                                        RegisterDateTime = workflows.RegisterDate >= 7 ? workflows.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.RegisterTime.Insert(2, ":") : "",
                                        ModifiedDateTime = workflows.ModifideDate >= 7 ? workflows.ModifideDate.ToString().Insert(4, "/").Insert(7, "/") + " " + workflows.ModifideTime.Insert(2, ":") : "",
                                        Staff = workflows.Staff.FName + " " + workflows.Staff.LName,
                                        Modifier = workflows.Modifier.FName + " " + workflows.Modifier.LName,
                                        FlowType = workflows.FlowType.Title,
                                        RequestGroupType = workflows.RequestGroupType.Title
                                    }).Distinct();


        var final = checkAccessForRol.Union(checkAccessForPerson);
        return final;
    }

    public List<WorkflowTutorialDownloadViewModel> GetWorkflowForTutorialDownload(string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);
        var userId = user.Id;

        var roleIds = RoleRepository.GetUserRoles(username);

        var checkAccessForRol = (from workflows in DbContext.Workflows
                                 join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                                 join roleClaim in DbContext.RoleClaims on requesttypes.Id.ToString() equals roleClaim.ClaimValue
                                 join roleId in roleIds on roleClaim.RoleId equals roleId
                                 where workflows.SubProcessId == null && workflows.IsActive == true && roleClaim.ClaimType == PermissionPolicyType.WorkFlowPermission
                                 select new WorkflowTutorialDownloadViewModel()
                                 {
                                     RequestType = workflows.RequestType.Title,
                                     Id = workflows.Id,
                                     Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                                     RequestGroupType = workflows.RequestGroupType.Title
                                 }).Distinct();

        var checkAccessForPerson = (from workflows in DbContext.Workflows
                                    join requesttypes in DbContext.LookUps on workflows.RequestTypeId equals requesttypes.Id
                                    join userClaim in DbContext.UserClaims on requesttypes.Id.ToString() equals userClaim.ClaimValue
                                    where workflows.SubProcessId == null && workflows.IsActive == true &&
                                          userClaim.ClaimType == PermissionPolicyType.WorkFlowPermission && userClaim.UserId == userId
                                    select new WorkflowTutorialDownloadViewModel()
                                    {
                                        RequestType = workflows.RequestType.Title,
                                        Id = workflows.Id,
                                        Version = workflows.OrginalVersion + "." + workflows.SecondaryVersion,
                                        RequestGroupType = workflows.RequestGroupType.Title
                                    }).Distinct();


        return checkAccessForRol.Union(checkAccessForPerson).ToList();
    }

    public IEnumerable<WorkFlowViewModel> GetSubProcessOfWorkFlow(Guid id)
    {
        return DbContext.Workflows.Where(d => d.SubProcessId == id).Select(s => new WorkFlowViewModel()
        {
            Id = s.Id,
            About = s.About,
            RequestType = s.RequestType.Title,
            Version = s.OrginalVersion + "." + s.SecondaryVersion,
            RequestGroupType = s.RequestGroupType.Title,
            IsActive = s.IsActive,
            RegisterDateTime = s.RegisterDate != 0 ? s.RegisterDate.ToString().Insert(4, "/").Insert(7, "/") + " " + s.RegisterTime.Insert(2, ":") : ""
        });
    }

    public List<ExternalApiInWorkFlowsViewModel> ExternalApiInWorkFlows(Guid externalApiId)
    {
        return (from wfd in DbContext.WorkFlowDetails
                join wf in DbContext.Workflows on wfd.WorkFlowId equals wf.Id
                join lookUp in DbContext.LookUps on wf.RequestTypeId equals lookUp.Id
                where wfd.ExternalApiId == externalApiId
                select new ExternalApiInWorkFlowsViewModel
                {
                    WorkFlowName = lookUp.Title,
                    WorkFlowDetailName = wfd.Title
                }).ToList();
    }

    public Workflow GetActiveWorkflowByRemoteId(string remoteId)
    {
        return DbContext.Workflows.FirstOrDefault(p => p.RemoteId == remoteId && p.IsActive);
    }

    public Workflow FindById(Guid? Id)
    {
        return DbContext.Workflows.Find(Id);
    }

    #region Modeler Save

    public Guid GenerateEntityFromModel(XDocument myxml, BpmnDiagramViewModel model)
    {
        var gModel = new GenerateEntityViewModel();
        var uniqXml = HttpUtility.UrlDecode(model.Workflow.XmlFile, System.Text.Encoding.UTF8);
        UnicodeEncoding encoding = new UnicodeEncoding();
        byte[] bytes = encoding.GetBytes(uniqXml);

        string remoteid = null;
        string codeId = null;
        if (model.Workflow.FlowTypeId != DbContext.LookUps.Single(c => c.Type == "FlowType" && c.Code == 1).Id)
        {
            gModel = GenerateEntity(myxml);
            remoteid = CheckHasAttr(myxml, "RemoteId");
            codeId = CheckHasAttr(myxml, "CodeId");
        }

        // اضافه کردن RequestType
        var reqType = model.Workflow.RequestType.Trim();
        var look = DbContext.LookUps.FirstOrDefault(d => d.Type == "RequestType" && d.Title.Trim() == reqType);
        if (look == null)
        {
            var maxcode = LookUpRepository.GetNewCode("RequestType");

            look = new LookUp()
            {
                Code = maxcode,
                Title = reqType,
                IsActive = true,
                Type = "RequestType",
            };
            DbContext.LookUps.Add(look);
        }
        else
        {
            //غیر فعال کردن بقیه requestType ها در صورت فعال بودن این 
            if (model.Workflow.IsActive)
            {
                var work = DbContext.Workflows.FirstOrDefault(
                    w => w.RequestTypeId == look.Id && w.IsActive);
                if (work != null)
                {
                    work.IsActive = false;
                    DbContext.Workflows.Update(work);
                }
            }
        }
        //اضافه کرن workflow
        var workflow1 = new Workflow()
        {
            Content = bytes,
            Dsr = model.Workflow.Dsr,
            About = model.Workflow.About,
            IsActive = model.Workflow.IsActive,
            RegisterDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")),
            RegisterTime = DateTime.Now.ToString("HHmm"),
            RequestTypeId = look.Id,
            RequestGroupTypeId = model.Workflow.RequestGroupTypeId,
            StaffId = model.Workflow.StaffId,
            OrginalVersion = model.Workflow.OrginalVersion,
            SecondaryVersion = model.Workflow.SecondaryVersion,
            WorkflowDetails = gModel.WorkFlowDetails,
            FlowTypeId = model.Workflow.FlowTypeId,
            RemoteId = remoteid,
            CodeId = codeId,
            KeyWords = model.Workflow.KeyWords,
            Owner = model.Workflow.Owner,
            SubProcessId = model.Workflow.SubProcessId,
            ExternalId = model.Workflow.ExternalId
        };

        if (gModel.TimerStartEvent.StartDateTime != default(DateTime))
            workflow1.StartTimerEvent = gModel.TimerStartEvent;

        DbContext.Workflows.Add(workflow1);
        DbContext.WorkFlowNextSteps.AddRange(gModel.WorkFlowNextSteps);
        DbContext.WorkFlowBoundaries.AddRange(gModel.WorkFlowBoundary);
        DbContext.WorkflowEsbs.AddRange(gModel.WorkFlowEsb);
        //غیر فعال کردن بقیه requestType ها در صورت فعال بودن این 
        if (model.Workflow.IsActive)
        {
            var work = DbContext.Workflows.FirstOrDefault(
                w => w.RequestTypeId == model.Workflow.RequestTypeId && w.IsActive);
            if (work != null)
            {
                work.IsActive = false;
                DbContext.Workflows.Update(work);
            }
        }

        return workflow1.Id;
    }

    private string CheckHasAttr(XDocument myxml, string attrName)
    {
        var start = myxml.Descendants(_bpmn2 + "startEvent").ToList();
        var signal = start.Elements(_bpmn2 + "signalEventDefinition").ToList();
        if (signal.Any())
        {
            var rid = (string)start[0].Attribute(_workflow + attrName);
            return rid;
        }
        var message = start.Elements(_bpmn2 + "messageEventDefinition").ToList();
        if (message.Any())
        {
            var rid = (string)start[0].Attribute(_workflow + attrName);
            return rid;
        }
        return null;
    }

    public Tuple<Guid, string> UpdateEntityFromModel(XDocument myxml, BpmnDiagramViewModel model)
    {
        var gModel = new GenerateEntityViewModel();
        var workflow = DbContext.Workflows.Find(model.Workflow.Id);


        var encoding = new UnicodeEncoding();

        string remoteid = null;
        string codeId = null;
        if (model.Workflow.FlowTypeId != DbContext.LookUps.Single(c => c.Type == "FlowType" && c.Code == 1).Id)
        {
            gModel = GenerateEntity(myxml);
            remoteid = CheckHasAttr(myxml, "RemoteId");
            codeId = CheckHasAttr(myxml, "CodeId");
        }



        // Remove From WorkFlow
        var workFlowDetailRemove = DbContext.WorkFlowDetails.Where(w => w.WorkFlowId == model.Workflow.Id && w.Step != int.MaxValue && w.Step != int.MinValue).ToList()
            .Where(work => gModel.WorkFlowDetails.All(u => u.Id != work.Id)).ToList();

        foreach (var workFlowDetail in workFlowDetailRemove)
        {
            var flow = DbContext.Flows.Where(f => f.WorkFlowDetailId == workFlowDetail.Id);
            if (flow.Any())
            {
                throw new ArgumentException("مرحله ی " + workFlowDetail.Title + " به دلیل استفاده در گردش کار قابل حذف نیست.");
            }

        }
        DbContext.WorkFlowDetails.RemoveRange(workFlowDetailRemove);

        // update RequestType
        var reqType = DbContext.LookUps.Find(workflow.RequestTypeId);
        reqType.Title = model.Workflow.RequestType.Trim();

        //غیر فعال کردن بقیه requestType ها در صورت فعال بودن این 
        if (model.Workflow.IsActive)
        {
            var work = DbContext.Workflows.FirstOrDefault(
                w => w.RequestTypeId == reqType.Id && w.IsActive);
            if (work != null)
            {
                work.IsActive = false;
                DbContext.Workflows.Update(work);
            }
        }

        string errorMessage = string.Empty;
        //Update workflowDetail
        var workFlowDetailUpdate = DbContext.WorkFlowDetails.Where(w => w.WorkFlowId == model.Workflow.Id).ToList()
            .Where(work => gModel.WorkFlowDetails.Any(u => u.Id == work.Id));

        var str = encoding.GetString(workflow.Content);
        var dbxml = XDocument.Parse(str);

        foreach (var workFlowDetail in workFlowDetailUpdate)
        {

            var flow = DbContext.Flows.Where(f => f.WorkFlowDetailId == workFlowDetail.Id);
            var workfw = gModel.WorkFlowDetails.FirstOrDefault(w => w.Id == workFlowDetail.Id);
            var request = DbContext.Requests.Where(r => r.WorkFlowId == workflow.Id);

            if (flow.Any() || (workFlowDetail.Step == 0 && request.Any()))
            {

                var id = workFlowDetail.Id.ToString();
                var workflowdetail = myxml.Descendants().ToList().Single(d => (string)d.Attribute(_workflow + "WorkFlowDetailId") == id);
                var dbworkflowdetail = dbxml.Descendants().ToList().Single(d => (string)d.Attribute(_workflow + "WorkFlowDetailId") == id);

                //workflowformId
                var newForm = workflowdetail.Attribute(_workflow + "WorkFlowFormId");
                var dbFrom = dbworkflowdetail.Attribute(_workflow + "WorkFlowFormId");
                if (newForm?.Value != dbFrom?.Value)
                {
                    errorMessage += "فرم مرحله ی " + workFlowDetail.Title + "</br>";
                    var dbFromText = dbworkflowdetail.Attribute(_workflow + "WorkFlowFormText");
                    workflowdetail.SetAttributeValue(dbFrom.Name, dbFrom.Value);
                    workflowdetail.SetAttributeValue(dbFromText.Name, dbFromText.Value);
                }
                else
                {
                    workFlowDetail.WorkFlowFormId = workfw.WorkFlowFormId;
                }

                //PrintFileName
                var newPrint = workflowdetail.Attribute(_workflow + "PrintFileText");
                var dbPrint = dbworkflowdetail.Attribute(_workflow + "PrintFileText");
                if (newPrint?.Value != dbPrint?.Value)
                {
                    errorMessage += "فایل قالب گزارش مرحله ی " + workFlowDetail.Title + "</br>";
                    workflowdetail.SetAttributeValue(dbPrint.Name, dbPrint.Value);
                }
                else
                {
                    workFlowDetail.PrintFileName = workfw.PrintFileName;
                }
            }
            else
            {
                workFlowDetail.WorkFlowFormId = workfw.WorkFlowFormId;
                workFlowDetail.PrintFileName = workfw.PrintFileName;
            }


            workFlowDetail.Step = workfw.Step;
            workFlowDetail.Title = workfw.Title;
            workFlowDetail.BusinessAcceptorMethod = workfw.BusinessAcceptorMethod;
            workFlowDetail.ScriptTaskMethod = workfw.ScriptTaskMethod;
            workFlowDetail.IsMultiConfirmReject = workfw.IsMultiConfirmReject;
            workFlowDetail.IsOrLogic = workfw.IsOrLogic;
            workFlowDetail.NoReject = workfw.NoReject;
            workFlowDetail.OrganizationPostTitleId = workfw.OrganizationPostTitleId;
            workFlowDetail.OrganizationPostTypeId = workfw.OrganizationPostTypeId;
            workFlowDetail.RequesterAccept = workfw.RequesterAccept;
            workFlowDetail.StaffId = workfw.StaffId;
            workFlowDetail.ViewName = workfw.ViewName;
            workFlowDetail.BusinessAcceptor = workfw.BusinessAcceptor;
            workFlowDetail.SelectAcceptor = workfw.SelectAcceptor;

            workFlowDetail.WaitingTime = workfw.WaitingTime;
            workFlowDetail.EditableFields = workfw.EditableFields;
            workFlowDetail.HiddenFields = workfw.HiddenFields;
            workFlowDetail.Act = workfw.Act;
            workFlowDetail.Dsr = workfw.Dsr;
            workFlowDetail.WaithingTImeForAct = workfw.WaithingTImeForAct;

            workFlowDetail.ResponseGroupId = workfw.ResponseGroupId;
            workFlowDetail.WorkflowDetailPatternId = workfw.WorkflowDetailPatternId;
            workFlowDetail.SelectFirstPostPattern = workfw.SelectFirstPostPattern;
            workFlowDetail.SelectAllPostPattern = workfw.SelectAllPostPattern;

            workFlowDetail.ExiteMethod = workfw.ExiteMethod;
            workFlowDetail.CallProcessId = workfw.CallProcessId;
            workFlowDetail.Dsr = workfw.Dsr;
            workFlowDetail.IsAdHoc = workfw.IsAdHoc;
            workFlowDetail.AdHocWorkflowDetails = workfw.AdHocWorkflowDetails;

            workFlowDetail.StaffId = workfw.StaffId;
            workFlowDetail.IsServiceTask = workfw.IsServiceTask;
            workFlowDetail.ExternalApiId = workfw.ExternalApiId;
            workFlowDetail.ServiceTaskApiResponse = workfw.ServiceTaskApiResponse;
            workFlowDetail.IsManualTask = workfw.IsManualTask;
            workFlowDetail.IsScriptTask = workfw.IsScriptTask;
            workFlowDetail.Info = workfw.Info;
            workFlowDetail.HasSaveableForm = workfw.HasSaveableForm;

            DbContext.WorkFlowDetails.Update(workFlowDetail);
        }


        foreach (var boundary in gModel.WorkFlowBoundary)
        {
            DbContext.WorkFlowBoundaries.Update(boundary);
        }



        // check flowevent 
        //  var wfd = _dbContext.WorkFlowDetails.Where(w => w.WorkFlowId == model.Workflow.Id).ToList();

        var flowevents = DbContext.FlowEvents.Where(d => d.Flow.Request.WorkFlowId == model.Workflow.Id);
        if (flowevents.Any())
        {
            var sequenceFlows = myxml.Descendants(_bpmn2 + "sequenceFlow");
            foreach (var sequenceFlow in sequenceFlows)
            {
                var sequenceFlowId = sequenceFlow.Attribute("id")?.Value;
                //    var dbworkflowdetail = dbxml.Descendants().ToList().Single(d => (string)d.Attribute(_workflow + "WorkFlowDetailId") == id);
                var sequenceFlowDb = dbxml.Descendants().FirstOrDefault(d => (string)d.Attribute("id") == sequenceFlowId);

                sequenceFlowDb = new XElement(sequenceFlowDb);
                var tempSequenceFlow = new XElement(sequenceFlow);

                if (sequenceFlowDb?.Attribute(_workflow + "Exp") != null)
                {
                    sequenceFlowDb.Attribute(_workflow + "Exp")?.Remove();
                }

                if (tempSequenceFlow?.Attribute(_workflow + "Exp") != null)
                {
                    tempSequenceFlow.Attribute(_workflow + "Exp")?.Remove();
                }

                if (sequenceFlowDb == null || sequenceFlowDb.ToString() != tempSequenceFlow.ToString())
                {
                    throw new ArgumentException("به دلیل در گردش بودن فرآیند امکان تغییر وجود ندارد.");
                }
            }
            // باید خطایی در مورد عدم آپدیت رویداد داده شود و اطلاعات جیسون بازگردانده شود

            gModel.WorkFlowEsb.Clear();
            var wfesbs = DbContext.WorkflowEsbs.Where(d => d.WorkFlowNextStep.NextStepFromWfd.WorkFlowId == model.Workflow.Id).ToList();
            foreach (var wfesb in wfesbs)
            {
                var eventId = wfesb.EventId.Split('_')[1];
                var eventName = wfesb.EventId.Split('_')[0];
                var dbElement = dbxml.Descendants().ToList().Single(d => d.Attribute("id") != null && d.Attribute("id").Value.Contains(eventId));
                var newElement = myxml.Descendants().ToList().Single(d => d.Attribute("id") != null && d.Attribute("id").Value.Contains(eventId));

                if (dbElement.ToString() == newElement.ToString()) continue;
                if (eventName == "intermediateThrowEventMessage")
                {
                    dbElement = newElement;

                    dynamic obj = new JObject();
                    obj.EmailSubject = newElement.Attribute(_workflow + "EmailSubject") != null ? newElement.Attribute(_workflow + "EmailSubject").Value : null;
                    obj.EmailBody = newElement.Attribute(_workflow + "EmailText") != null ? newElement.Attribute(_workflow + "EmailText").Value : null;
                    obj.EmailRecieve = newElement.Attribute(_workflow + "EmailRecieve") != null ? newElement.Attribute(_workflow + "EmailRecieve").Value : null;
                    obj.SmsText = newElement.Attribute(_workflow + "SmsText") != null ? newElement.Attribute(_workflow + "SmsText").Value : null;
                    obj.SmsRecieve = newElement.Attribute(_workflow + "SmsRecieve") != null ? newElement.Attribute(_workflow + "SmsRecieve").Value : null;
                    obj.SendRemoteId = newElement.Attribute(_workflow + "SendRemoteId") != null ? newElement.Attribute(_workflow + "SendRemoteId").Value : null;
                    obj.DynamicGetCode = newElement.Attribute(_workflow + "DynamicGetCode") != null ? newElement.Attribute(_workflow + "DynamicGetCode").Value : null;
                    obj.FormIdForMessage = newElement.Attribute(_workflow + "FormIdForMessage") != null ? newElement.Attribute(_workflow + "FormIdForMessage").Value : null;

                    if (newElement.Attribute(_workflow + "SmsRequester") != null)
                        obj.SmsRequester = newElement.Attribute(_workflow + "SmsRequester").Value;
                    else
                        obj.SmsRequester = false;

                    if (newElement.Attribute(_workflow + "EmailRequester") != null)
                        obj.EmailRequester = newElement.Attribute(_workflow + "EmailRequester").Value;
                    else
                        obj.EmailRequester = false;

                    obj.Param = newElement.Attribute(_workflow + "Param") != null ? newElement.Attribute(_workflow + "Param").Value : null;

                    var info = JsonConvert.SerializeObject(obj);

                    var newWfesb = new WorkflowEsb()
                    {
                        Id = wfesb.Id,
                        EventId = wfesb.EventId,
                        WorkflowNextStepId = wfesb.WorkflowNextStepId,
                        Info = info,
                    };

                    DbContext.WorkflowEsbs.Update(newWfesb);

                }

                else
                {
                    foreach (var item in dbElement.Attributes())
                    {
                        newElement.SetAttributeValue(item.Name, item.Value);
                    }
                    errorMessage += "رویدادها </br>";
                }
            }

        }
        //Remove Exist WorkFlowNextStep
        else
        {
            var wfnsf = from nextStep in DbContext.WorkFlowNextSteps
                        join wfdFrom in DbContext.WorkFlowDetails on nextStep.FromWfdId equals wfdFrom.Id
                        where wfdFrom.WorkFlow.Id == model.Workflow.Id
                        select nextStep;
            var wfnst = from nextStep in DbContext.WorkFlowNextSteps
                        join wfdTo in DbContext.WorkFlowDetails on nextStep.ToWfdId equals wfdTo.Id
                        where wfdTo.WorkFlow.Id == model.Workflow.Id
                        select nextStep;
            var wfns = wfnsf.Union(wfnst).Distinct();
            DbContext.WorkFlowNextSteps.RemoveRange(wfns);

            // EndEvent Sync 
            var endEvent = gModel.WorkFlowDetails.SingleOrDefault(d => d.Step == int.MaxValue);
            if (endEvent != null)
            {
                var endEventDb = DbContext.WorkFlowDetails.SingleOrDefault(d => d.Step == int.MaxValue && d.WorkFlowId == model.Workflow.Id);
                if (endEventDb != null)
                {
                    foreach (var item in gModel.WorkFlowNextSteps.Where(d => d.ToWfdId == endEvent.Id))
                    {
                        item.ToWfdId = endEventDb.Id;
                    }
                }
                else
                {
                    workflow.WorkflowDetails.Add(gModel.WorkFlowDetails.Single(d => d.Step == int.MaxValue));
                }
            }

            var endEventTerminate = gModel.WorkFlowDetails.SingleOrDefault(d => d.Step == int.MinValue);
            if (endEventTerminate != null)
            {
                var endEventTerminateDb = DbContext.WorkFlowDetails.SingleOrDefault(d => d.Step == int.MinValue && d.WorkFlowId == model.Workflow.Id);
                if (endEventTerminateDb != null)
                {
                    foreach (var item in gModel.WorkFlowNextSteps.Where(d => d.ToWfdId == endEventTerminate.Id))
                    {
                        item.ToWfdId = endEventTerminateDb.Id;
                    }
                }
                else
                {
                    workflow.WorkflowDetails.Add(gModel.WorkFlowDetails.Single(d => d.Step == int.MinValue));
                }
            }


            DbContext.WorkFlowNextSteps.AddRange(gModel.WorkFlowNextSteps);
        }

        str = myxml.ToString();
        var bytes = encoding.GetBytes(HttpUtility.UrlDecode(str, System.Text.Encoding.UTF8));


        //add esb
        foreach (var workflowEsb in gModel.WorkFlowEsb)
        {
            DbContext.WorkflowEsbs.Update(workflowEsb);
        }

        if (gModel.TimerStartEvent.StartDateTime != default(DateTime))
        {
            gModel.TimerStartEvent.Id = workflow.Id;
            gModel.TimerStartEvent.WorkFlowId = workflow.Id;
            DbContext.StartTimerEvents.Update(gModel.TimerStartEvent);
        }
        else
        {
            var startTimer = DbContext.StartTimerEvents.FirstOrDefault(a => a.Id == workflow.Id);
            if (startTimer != null)
                DbContext.StartTimerEvents.Remove(startTimer);
        }

        //foreach (var workFlowDetail in gModel.WorkFlowDetails.Where(c => c.Title == null && c.Step == 0))
        //{
        //    workFlowDetail.Title = "Sub start";
        //}
        //Add To WorkFlow
        var workFlowDetailAdd = from workFlowDetail in gModel.WorkFlowDetails
                                where DbContext.WorkFlowDetails.ToList().All(w => w.Id != workFlowDetail.Id) && workFlowDetail.Step != int.MaxValue && workFlowDetail.Step != int.MinValue
                                select workFlowDetail;


        workflow.IsActive = model.Workflow.IsActive;
        workflow.Content = bytes;
        workflow.Dsr = model.Workflow.Dsr;
        workflow.About = model.Workflow.About;
        workflow.OrginalVersion = model.Workflow.OrginalVersion;
        workflow.SecondaryVersion = model.Workflow.SecondaryVersion;
        workflow.WorkflowDetails.AddRange(workFlowDetailAdd);
        workflow.FlowTypeId = model.Workflow.FlowTypeId;
        workflow.RemoteId = remoteid;
        workflow.CodeId = codeId;
        workflow.RequestGroupTypeId = model.Workflow.RequestGroupTypeId;

        workflow.Owner = model.Workflow.Owner;
        workflow.KeyWords = model.Workflow.KeyWords;

        workflow.ModifideDate = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
        workflow.ModifideTime = DateTime.Now.ToString("HHmm");
        workflow.ModifiedId = model.Workflow.StaffId;
        workflow.ExternalId = model.Workflow.ExternalId;
        DbContext.Workflows.Update(workflow);

        return new Tuple<Guid, string>(workflow.Id, errorMessage);
    }

    private void AddStartNode(XDocument myxml, List<WorkFlowDetail> workFlowDetailsList, StartTimerEvent tEvent)
    {
        var start = myxml.Descendants(_bpmn2 + "startEvent").ToList();
        var seq = myxml.Descendants(_bpmn2 + "sequenceFlow").ToList().FirstOrDefault(d => (string)d.Attribute("sourceRef") == (string)start[0].Attribute("id"));
        var starttarget = myxml.Descendants().ToList()
            .FirstOrDefault(d => (string)d.Attribute("id") == (string)seq.Attribute("targetRef"));

        var id = start.First().Elements(_bpmn2 + "timerEventDefinition").FirstOrDefault();
        if (id != null)
        {
            GenerateTimerStartEvent(start.First(), tEvent);
        }


        var workFlowDetailsStart = AddWorkFlowDetail(starttarget, 0);

        workFlowDetailsList.Add(workFlowDetailsStart);
    }

    private void GenerateTimerStartEvent(XElement element, StartTimerEvent timerEvent)
    {
        var startDate = element.Attribute("StartDate")?.Value?.Replace("/", "");
        var startTime = element.Attribute("StartTime")?.Value?.Replace(":", "");
        timerEvent.StartDateTime = HelperBs.ConvertShamsiToDateTime(startDate, startTime);

        var isSequential = element.Attribute("Repeating")?.Value;
        if (isSequential != null && isSequential == "true")
        {
            timerEvent.IsSequential = bool.Parse(isSequential);
            var waitingDate = element.Attribute("WaitingDate")?.Value;
            var waitingTime = element.Attribute("WaitingTime")?.Value;
            var nums = new int[2];
            if (waitingDate != null)
                nums[0] = int.Parse(waitingDate) * 24;
            if (waitingTime != null)
                nums[1] = int.Parse(waitingTime);
            timerEvent.IntervalHours = nums.Sum();
        }

        var hasExpireDate = element.Attribute("HasExpiry")?.Value;
        if (hasExpireDate != null && hasExpireDate == "true")
        {
            timerEvent.HasExpireDate = bool.Parse(hasExpireDate);
            var endDate = element.Attribute("EndDate")?.Value?.Replace("/", "");
            var endTime = element.Attribute("EndTime")?.Value?.Replace(":", "");
            timerEvent.ExpireDateTime = HelperBs.ConvertShamsiToDateTime(endDate, endTime);
        }
    }
    private WorkFlowDetail AddTerminateNode(XDocument myxml, List<WorkFlowDetail> workFlowDetailsList)
    {
        var workFlowDetailsTerminate = new WorkFlowDetail()
        {
            Step = int.MaxValue,
            IsMultiConfirmReject = false,
            IsOrLogic = false,
            RequesterAccept = false,
            Title = @"پایان فرآیند",
            BusinessAcceptor = false,
            Dsr = @"پایان فرآیند",
        };
        workFlowDetailsList.Add(workFlowDetailsTerminate);
        return workFlowDetailsTerminate;
    }
    private WorkFlowDetail AddEndNode(XDocument myxml, List<WorkFlowDetail> workFlowDetailsList)
    {
        var workFlowDetailsEnd = new WorkFlowDetail()
        {
            Step = int.MinValue,
            IsMultiConfirmReject = false,
            IsOrLogic = false,
            RequesterAccept = false,
            Title = @"پایان میانی",
            BusinessAcceptor = false,
            Dsr = @"پایان میانی",
        };
        workFlowDetailsList.Add(workFlowDetailsEnd);
        return workFlowDetailsEnd;
    }
    private bool SourceNodeIsAllowed(string sourceName)
    {
        bool isAllowed = sourceName.Contains("task") || sourceName.Contains("subprocess") ||
                         sourceName.Contains("callactivity") || sourceName.Contains("boundaryevent")
                         || sourceName.Contains("servicetask") || sourceName.Contains(BpmnNodeConstant.ManualTask);
        return isAllowed;
    }
    private Stack<string> GenerateFlowLine(XElement sequenceFlow, XElement task)
    {
        var flowLine = new Stack<string>();

        var condition = sequenceFlow.Elements(_bpmn2 + "conditionExpression").ToList();
        var defaultflow = (string)task.Attribute("default");
        var id = (string)sequenceFlow.Attribute("id");

        if ((string)sequenceFlow.Attribute("id") == defaultflow)
        {
            flowLine.Push("default" + id.Split('_')[1]);
        }
        else if (condition.Any())
        {
            flowLine.Push("condition" + id.Split('_')[1]);
        }
        else
        {
            flowLine.Push("sequence" + id.Split('_')[1]);
        }
        return flowLine;
    }
    private WorkFlowDetail GenerateWorkFlowDetail(XElement task, List<WorkFlowDetail> workFlowDetailsList, string sourceName)
    {
        var wfdId = Guid.Parse((string)task.Attribute(_workflow + "WorkFlowDetailId"));
        var existTask = workFlowDetailsList.FirstOrDefault(w => w.Id == wfdId);

        WorkFlowDetail workFlowDetailsFrom;

        if (existTask == null)
        {
            var wfd = AddWorkFlowDetail(task);
            workFlowDetailsList.Add(wfd);
            workFlowDetailsFrom = wfd;
        }
        else
        {
            workFlowDetailsFrom = existTask;
        }

        if (sourceName.Contains("servicetask") || sourceName.Contains(BpmnNodeConstant.ManualTask) || sourceName.Contains("scripttask"))
        {
            var systemStaffId = DbContext.Staffs.First(t => t.PersonalCode == SystemConstant.SystemUser).Id;
            workFlowDetailsFrom.StaffId = systemStaffId;
        }
        if (sourceName.Contains("servicetask"))
        {
            workFlowDetailsFrom.IsServiceTask = true;
            workFlowDetailsFrom.ExternalApiId = Guid.Parse((string)task.Attribute(_workflow + "requestApiId"));
            workFlowDetailsFrom.ServiceTaskApiResponse = (string)task.Attribute(_workflow + "responseApiId");
        }

        if (sourceName.Contains(BpmnNodeConstant.ManualTask))
        {
            workFlowDetailsFrom.IsManualTask = true;
        }

        if (sourceName.Contains("scripttask"))
        {
            workFlowDetailsFrom.IsScriptTask = true;
        }

        return workFlowDetailsFrom;
    }

    private GenerateEntityViewModel GenerateEntity(XDocument myxml)
    {
        var workFlowDetailsList = new List<WorkFlowDetail>();
        var workFlowNesxtStepList = new List<WorkFlowNextStep>();
        var workFlowBoundry = new List<WorkFlowBoundary>();
        var workFlowEsbList = new List<WorkflowEsb>();
        var timerStartEvent = new StartTimerEvent();

        AddStartNode(myxml, workFlowDetailsList, timerStartEvent);

        var workFlowDetailsTerminate = AddTerminateNode(myxml, workFlowDetailsList);

        var workFlowDetailsEnd = AddEndNode(myxml, workFlowDetailsList);

        //Add Other Node
        var sequenceFlows = myxml.Descendants(_bpmn2 + "sequenceFlow");

        foreach (var sequenceFlow in sequenceFlows)
        {
            var sourceId = sequenceFlow.Attribute("sourceRef")?.Value;
            var source = myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == sourceId);

            var targetId = sequenceFlow.Attribute("targetRef")?.Value;
            var target = myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == targetId);

            var workflowParam = new ModelerParamDto()
            {
                Path = new Stack<string>(),
                Exp = new Stack<string>(),
                Method = new Stack<string>(),
                Evt = new Stack<string>()
            };

            AddFlowInformation(sequenceFlow, workflowParam);

            string sourceName = source.Name.LocalName.ToLower();

            if (SourceNodeIsAllowed(sourceName))
            {
                XElement task;

                string boundaryName = null;
                if (sourceName.Contains("boundaryevent"))
                    task = AddWorkflowBoundry(myxml, source, workFlowBoundry, ref boundaryName);
                else
                    task = source;

                var flowLine = GenerateFlowLine(sequenceFlow, task);

                var workFlowDetail = GenerateWorkFlowDetail(task, workFlowDetailsList, sourceName);

                // fill modeler param
                workflowParam.WorkFlowDetailsFrom = workFlowDetail;
                workflowParam.WorkFlowNextStepId = Guid.NewGuid();
                workflowParam.Target = targetId;
                workflowParam.Myxml = myxml;
                workflowParam.WorkFlowDetailsEnd = workFlowDetailsEnd;
                workflowParam.WorkFlowDetailsTerminate = workFlowDetailsTerminate;
                workflowParam.WorkFlowDetailsList = workFlowDetailsList;
                workflowParam.WorkFlowNextStepsList = workFlowNesxtStepList;
                workflowParam.Esb = new Stack<string>();
                workflowParam.FlowLine = flowLine;
                workflowParam.Gateway = new Stack<string>();
                workflowParam.BoundryName = boundaryName;
                workflowParam.WorkflowEsbsList = workFlowEsbList;

                var targetName = target.Name.LocalName.ToLower();

                if (targetName.Contains("endevent"))
                {
                    EndEventTarget(workflowParam);
                }
                if (targetName.Contains("task") || targetName.Contains("subprocess") || targetName.Contains("callactivity")
                    || targetName.Contains(BpmnNodeConstant.ManualTask) || targetName.Contains("servicetask") || targetName.Contains("scripttask"))
                {
                    TaskTarget(workflowParam);
                }
                if (targetName.Contains("intermediate"))
                {
                    TraversIntermediateThrowMessage(workflowParam);
                }
                if (targetName.Contains("gateway"))
                {
                    TraversGatway(workflowParam);
                }
            }
        }
        var adHocs = myxml.Descendants(_bpmn2 + "adHocSubProcess");
        foreach (var adHoc in adHocs)
        {
            workFlowDetailsList.Add(AddWorkFlowDetail(adHoc));
        }
        return new GenerateEntityViewModel()
        {
            WorkFlowDetails = workFlowDetailsList,
            WorkFlowNextSteps = workFlowNesxtStepList,
            WorkFlowBoundary = workFlowBoundry,
            WorkFlowEsb = workFlowEsbList,
            TimerStartEvent = timerStartEvent
        };
    }

    private XElement AddWorkflowBoundry(XDocument myxml, XElement boundry, List<WorkFlowBoundary> workFlowBoundry, ref string boundaryName)
    {
        var taskId = (string)boundry.Attribute("attachedToRef");
        var boundryId = (string)boundry.Attribute("id");
        var task = myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == taskId);

        // boundary Error
        var errorEvent = boundry.Elements(_bpmn2 + "errorEventDefinition").ToList();
        if (errorEvent.Any())
        {
            var recriveErrorId = (string)boundry.Attribute(_workflow + "RecriveErrorId");
            var obj = new JObject { { "RecriveErrorId", recriveErrorId } };
            boundaryName = "BoundaryErrorEvent_" + boundryId.Split('_')[1];
            workFlowBoundry.Add(new WorkFlowBoundary
            {
                Id = Guid.Parse((string)boundry.Attribute(_workflow + "WorkFlowBoundaryId")),
                WorkflowDetailId = Guid.Parse((string)task.Attribute(_workflow + "WorkFlowDetailId")),
                BoundaryId = boundaryName,
                Info = obj.ToString()
            });
        }

        // boundary Timer
        var timerEvent = boundry.Elements(_bpmn2 + "timerEventDefinition").ToList();
        if (timerEvent.Any())
        {
            JObject obj = null;
            var act = (string)boundry.Attribute(_workflow + "Act") ?? "A";
            var waitingTimeForAct = Convert.ToInt32((string)boundry.Attribute(_workflow + "WaitingTimeForAct"));
            if (boundry.Attribute("cancelActivity") != null)
            {
                boundaryName = "NonInterruptingBoundaryTimerEvent_" + boundryId.Split('_')[1];
                obj = new JObject
                {
                    { "WaitingTimeForAct", waitingTimeForAct }
                };
            }
            else
            {
                boundaryName = "BoundaryTimerEvent_" + boundryId.Split('_')[1];
                obj = new JObject
                {
                    { "Act", act },
                    { "WaitingTimeForAct", waitingTimeForAct }
                };
            }
            workFlowBoundry.Add(new WorkFlowBoundary
            {
                Id = Guid.Parse((string)boundry.Attribute(_workflow + "WorkFlowBoundaryId")),
                WorkflowDetailId = Guid.Parse((string)task.Attribute(_workflow + "WorkFlowDetailId")),
                BoundaryId = boundaryName,
                Info = obj.ToString()
            });
        }


        return task;
    }

    private void TaskTarget(ModelerParamDto param)
    {
        //var taskTarget = myxml.Descendants(_bpmn2 + "task").FirstOrDefault(d => d.Attribute("id").Value == target);
        WorkFlowDetail workFlowDetailsNext;
        var taskTarget = param.Myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == param.Target);
        var wfdIdTarget = Guid.Parse((string)taskTarget.Attribute(_workflow + "WorkFlowDetailId"));
        var existTaskTarget = param.WorkFlowDetailsList.FirstOrDefault(w => w.Id == wfdIdTarget);
        if (existTaskTarget == null)
        {
            var wfd = AddWorkFlowDetail(taskTarget);
            param.WorkFlowDetailsList.Add(wfd);
            workFlowDetailsNext = wfd;
        }
        else
        {
            workFlowDetailsNext = existTaskTarget;
        }


        //   AddWorkFlowNextStep(_workFlowDetailsFrom.Id, _workFlowDetailsNext.Id);
        var wfns = AddWorkFlowNextStep(param.WorkFlowDetailsFrom.Id, workFlowDetailsNext.Id, param);
        param.WorkFlowNextStepsList.Add(wfns);
    }

    private WorkFlowNextStep AddWorkFlowNextStep(Guid from, Guid to, ModelerParamDto param)
    {
        var workFlowNesxtStep = new WorkFlowNextStep()
        {
            Id = param.WorkFlowNextStepId,
            FromWfdId = from,
            ToWfdId = to,
            Path = ConvertStackToString(param.Path),
            Exp = ConvertStackToString(param.Exp),
            Evt = ConvertStackToString(param.Evt),
            Gateway = ConvertStackToString(param.Gateway),
            Esb = ConvertStackToString(param.Esb),
            FlowLine = ConvertStackToString(param.FlowLine),
            Method = ConvertStackToString(param.Method),
            BoundaryName = param.BoundryName
        };
        return workFlowNesxtStep;
    }

    private void EndEventTarget(ModelerParamDto param)
    {
        var p = param.WorkFlowDetailsEnd;
        var end = param.Myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == param.Target);
        var endId = ((string)end.Attribute("id")).Split('_')[1];
        // تشخیص نوع ایونت
        var termin = end.Elements(_bpmn2 + "terminateEventDefinition").ToList();
        if (termin.Any())
        {
            p = param.WorkFlowDetailsTerminate;
        }
        var messageEvent = end.Elements(_bpmn2 + "messageEventDefinition").ToList();
        if (messageEvent.Any())
        {
            // چون بیزینس مشابه رویداد پیام میانی است لوکال نیم هم نام آن است
            var localName = "intermediateThrowEventMessage";
            var workflowEsb = AddWorkflowEsbForThrowMessage(param, end, localName, endId);
            param.WorkflowEsbsList.Add(workflowEsb);
            param.Esb.Push(localName + "_" + endId);
        }
        var errorEvent = end.Elements(_bpmn2 + "errorEventDefinition").ToList();
        if (errorEvent.Any())
        {
            var localName = "errorEvent";
            var errorId = (string)end.Attribute(_workflow + "ErrorId");
            var obj = new JObject { { "ErrorId", errorId } };
            var workflowEsb = new WorkflowEsb()
            {
                Info = obj.ToString(),
                EventId = localName + "_" + endId,
                WorkflowNextStepId = param.WorkFlowNextStepId
            };
            param.WorkflowEsbsList.Add(workflowEsb);
            param.Esb.Push(localName + "_" + endId);
        }
        var signalEvent = end.Elements(_bpmn2 + "signalEventDefinition").ToList();
        if (signalEvent.Any())
        {
            var localName = "intermediateThrowEventSignal";
            var sendRemoteId = (string)end.Attribute(_workflow + "SendRemoteId");
            var obj = new JObject { { "SendRemoteId", sendRemoteId } };
            var workflowEsb = new WorkflowEsb()
            {
                Info = obj.ToString(),
                EventId = localName + "_" + endId,
                WorkflowNextStepId = param.WorkFlowNextStepId
            };
            param.WorkflowEsbsList.Add(workflowEsb);
            param.Esb.Push(localName + "_" + endId);
        }
        //   AddWorkFlowNextStep(_workFlowDetailsFrom.Id, p.Id);
        var wfns = AddWorkFlowNextStep(param.WorkFlowDetailsFrom.Id, p.Id, param);
        param.WorkFlowNextStepsList.Add(wfns);
    }

    private void AddFlowInformation(XElement sequenceFlow, ModelerParamDto param)
    {
        var xAttribute = sequenceFlow.Attribute(_workflow + "Path");
        param.Path.Push(xAttribute?.Value ?? "null");

        var exp = sequenceFlow.Attribute(_workflow + "Exp");
        param.Exp.Push(exp?.Value ?? "null");

        var method = sequenceFlow.Attribute(_workflow + "Method");
        param.Method.Push(method?.Value ?? "null");

        var evt = sequenceFlow.Attribute(_workflow + "Evt");
        param.Evt.Push(evt?.Value ?? "A");
    }

    private void TraversIntermediateThrowMessage(ModelerParamDto param)
    {
        var throwEvent = param.Myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == param.Target);
        var throwEventId = ((string)throwEvent.Attribute("id")).Split('_')[1];

        //تشخیص برای نوع اوینت

        var localName = throwEvent.Name.LocalName;
        if (localName == "intermediateThrowEvent")
        {
            var messageEvent = throwEvent.Elements(_bpmn2 + "messageEventDefinition").ToList();
            if (messageEvent.Any())
            {
                localName += "Message";
                var workflowEsb = AddWorkflowEsbForThrowMessage(param, throwEvent, localName, throwEventId);
                param.WorkflowEsbsList.Add(workflowEsb);
            }

            var timerEvent = throwEvent.Elements(_bpmn2 + "timerEventDefinition").ToList();
            if (timerEvent.Any())
            {
                localName += "Timer";
                var witingDate = (string)throwEvent.Attribute(_workflow + "WaitingDate");
                var dynamicWaitingDate = (string)throwEvent.Attribute(_workflow + "DynamicWaitingDate");
                var timerType = (string)throwEvent.Attribute(_workflow + "TimerType");
                var obj = new JObject { { "WaitingDate", witingDate }, { "DynamicWaitingDate", dynamicWaitingDate }, { "TimerType", timerType } };
                var workflowEsb = new WorkflowEsb()
                {
                    Info = obj.ToString(),
                    EventId = localName + "_" + throwEventId,
                    WorkflowNextStepId = param.WorkFlowNextStepId
                };
                param.WorkflowEsbsList.Add(workflowEsb);
            }

            var signalEvent = throwEvent.Elements(_bpmn2 + "signalEventDefinition").ToList();
            if (signalEvent.Any())
            {
                localName += "Signal";
                var sendRemoteId = (string)throwEvent.Attribute(_workflow + "SendRemoteId");
                var obj = new JObject { { "SendRemoteId", sendRemoteId } };
                var workflowEsb = new WorkflowEsb
                {
                    Info = obj.ToString(),
                    EventId = localName + "_" + throwEventId,
                    WorkflowNextStepId = param.WorkFlowNextStepId
                };
                param.WorkflowEsbsList.Add(workflowEsb);
            }
        }

        if (localName == "intermediateCatchEvent")
        {
            var messageEvent = throwEvent.Elements(_bpmn2 + "messageEventDefinition").ToList();
            if (messageEvent.Any())
            {
                localName += "Message";
                var remoteId = (string)throwEvent.Attribute(_workflow + "RemoteId");
                var codeId = (string)throwEvent.Attribute(_workflow + "CodeId");
                var obj = new JObject { { "RemoteId", remoteId }, { "CodeId", codeId } };
                var workflowEsb = new WorkflowEsb()
                {
                    Info = obj.ToString(),
                    EventId = localName + "_" + throwEventId,
                    WorkflowNextStepId = param.WorkFlowNextStepId
                };
                param.WorkflowEsbsList.Add(workflowEsb);
            }

            var timerEvent = throwEvent.Elements(_bpmn2 + "timerEventDefinition").ToList();
            if (timerEvent.Any())
            {
                localName += "Timer";
                var witingDate = (string)throwEvent.Attribute(_workflow + "WaitingDate");
                var dynamicWaitingDate = (string)throwEvent.Attribute(_workflow + "DynamicWaitingDate");
                var timerType = (string)throwEvent.Attribute(_workflow + "TimerType");
                var obj = new JObject { { "WaitingDate", witingDate }, { "DynamicWaitingDate", dynamicWaitingDate }, { "TimerType", timerType } };
                var workflowEsb = new WorkflowEsb()
                {
                    Info = obj.ToString(),
                    EventId = localName + "_" + throwEventId,
                    WorkflowNextStepId = param.WorkFlowNextStepId
                };
                param.WorkflowEsbsList.Add(workflowEsb);
            }

            var signalEvent = throwEvent.Elements(_bpmn2 + "signalEventDefinition").ToList();
            if (signalEvent.Any())
            {
                localName += "Signal";
                var remoteId = (string)throwEvent.Attribute(_workflow + "RemoteId");
                var obj = new JObject { { "RemoteId", remoteId } };
                var workflowEsb = new WorkflowEsb()
                {
                    Info = obj.ToString(),
                    EventId = localName + "_" + throwEventId,
                    WorkflowNextStepId = param.WorkFlowNextStepId
                };
                param.WorkflowEsbsList.Add(workflowEsb);
            }
        }

        var EventId = localName + "_" + throwEventId;

        param.Esb.Push(EventId);

        var sequenceFlows = param.Myxml.Descendants(_bpmn2 + "sequenceFlow").Where(d => d.Attribute("sourceRef").Value == param.Target);
        foreach (var sequenceFlow in sequenceFlows)
        {
            var targetThrowMessageId = sequenceFlow.Attribute("targetRef").Value;
            var targetThrowMessage = param.Myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == targetThrowMessageId);
            //Add FlowLine
            var id = (string)sequenceFlow.Attribute("id");
            param.FlowLine.Push("sequence" + id.Split('_')[1]);

            AddFlowInformation(sequenceFlow, param);
            var workflowParam = new ModelerParamDto()
            {
                Target = targetThrowMessageId,
                Myxml = param.Myxml,
                WorkFlowDetailsEnd = param.WorkFlowDetailsEnd,
                WorkFlowDetailsFrom = param.WorkFlowDetailsFrom,
                WorkFlowDetailsTerminate = param.WorkFlowDetailsTerminate,
                WorkFlowDetailsList = param.WorkFlowDetailsList,
                WorkFlowNextStepsList = param.WorkFlowNextStepsList,
                WorkFlowNextStepId = param.WorkFlowNextStepId,
                WorkflowEsbsList = param.WorkflowEsbsList,
                Path = param.Path,
                Esb = param.Esb,
                Evt = param.Evt,
                Exp = param.Exp,
                FlowLine = param.FlowLine,
                Gateway = param.Gateway,
                Method = param.Method
            };
            if (targetThrowMessage.Name.LocalName.ToLower().Contains("endevent"))
            {
                EndEventTarget(workflowParam);
                RemovePreviwesPath(param);
            }
            if (targetThrowMessage.Name.LocalName.ToLower().Contains("task")
                || targetThrowMessage.Name.LocalName.ToLower().Contains("subprocess")
                || targetThrowMessage.Name.LocalName.ToLower().Contains("callactivity"))
            {
                TaskTarget(workflowParam);
                RemovePreviwesPath(param);
            }
            if (targetThrowMessage.Name.LocalName.ToLower().Contains("intermediate"))
            {
                TraversIntermediateThrowMessage(workflowParam);
                RemovePreviwesPath(param);
                param.Esb.Pop();
            }
            if (targetThrowMessage.Name.LocalName.ToLower().Contains("gateway"))
            {
                TraversGatway(workflowParam, EventId);
                RemovePreviwesPath(param);
                param.Gateway.Pop();

            }
        }
    }

    private WorkflowEsb AddWorkflowEsbForThrowMessage(ModelerParamDto param, XElement throwEvent, string localName,
        string throwEventId)
    {
        var emailSubject = (string)throwEvent.Attribute(_workflow + "EmailSubject");
        var emailText = (string)throwEvent.Attribute(_workflow + "EmailText");
        var emailRecieve = (string)throwEvent.Attribute(_workflow + "EmailRecieve");
        var emailReqiester = Convert.ToBoolean((string)throwEvent.Attribute(_workflow + "EmailRequester"));

        var sendRemoteId = (string)throwEvent.Attribute(_workflow + "SendRemoteId");
        var dynamicGetCode = (string)throwEvent.Attribute(_workflow + "DynamicGetCode");
        var formIdForMessage = (string)throwEvent.Attribute(_workflow + "FormIdForMessage");

        var smsText = (string)throwEvent.Attribute(_workflow + "SmsText");
        var smsRecieve = (string)throwEvent.Attribute(_workflow + "SmsRecieve");
        var smsReqiester = Convert.ToBoolean((string)throwEvent.Attribute(_workflow + "SmsRequester"));
        var w = new EsbViewModel()
        {
            EmailBody = emailText,
            EmailSubject = emailSubject,
            EmailRecieve = emailRecieve,
            SmsRecieve = smsRecieve,
            SmsText = smsText,
            SendRemoteId = sendRemoteId,
            EmailRequester = emailReqiester,
            SmsRequester = smsReqiester,
            DynamicGetCode = dynamicGetCode,
            FormIdForMessage = formIdForMessage
        };
        var json = JsonConvert.SerializeObject(w);
        var workflowEsb = new WorkflowEsb()
        {
            Info = json,
            EventId = localName + "_" + throwEventId,
            WorkflowNextStepId = param.WorkFlowNextStepId
        };
        return workflowEsb;
    }

    private void TraversGatway(ModelerParamDto param, string eventId = null)
    {

        var gateway = param.Myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == param.Target);
        var gateway_id = ((string)gateway.Attribute("id")).Split('_')[1];
        param.Gateway.Push(gateway.Name.LocalName + "_" + gateway_id);

        var defaultflow = (string)gateway.Attribute("default");

        var workflowEsb = new WorkflowEsb();
        if (eventId != null)
        {
            var previousEvent = param.WorkflowEsbsList.First(a => a.EventId == eventId);
            workflowEsb.EventId = previousEvent.EventId;
            workflowEsb.FlowEvents = previousEvent.FlowEvents;
            workflowEsb.Info = previousEvent.Info;

            param.WorkflowEsbsList.Remove(previousEvent);
        }

        var sequenceFlowsGateways = param.Myxml.Descendants(_bpmn2 + "sequenceFlow").Where(d => d.Attribute("sourceRef").Value == param.Target);
        foreach (var sequenceFlowsGateway in sequenceFlowsGateways)
        {
            // bug 10224
            //param.WorkFlowNextStepId = GenerateNewIdForWorkflowNextStep(param);
            param.WorkFlowNextStepId = Guid.NewGuid();
            //add FlowLine
            var id = (string)sequenceFlowsGateway.Attribute("id");
            param.FlowLine.Push(id == defaultflow ? "default" + id.Split('_')[1] : "sequence" + id.Split('_')[1]);

            AddFlowInformation(sequenceFlowsGateway, param);

            if (eventId != null)
            {
                workflowEsb = new WorkflowEsb()
                {
                    EventId = workflowEsb.EventId,
                    FlowEvents = workflowEsb.FlowEvents,
                    Info = workflowEsb.Info,
                    WorkflowNextStepId = param.WorkFlowNextStepId
                };
                param.WorkflowEsbsList.Add(workflowEsb);
            }

            var targetGatwayId = sequenceFlowsGateway.Attribute("targetRef").Value;
            var targetGatway = param.Myxml.Descendants().ToList().Single(d => (string)d.Attribute("id") == targetGatwayId);
            var workflowParam = new ModelerParamDto()
            {
                Target = targetGatwayId,
                Myxml = param.Myxml,
                WorkFlowDetailsEnd = param.WorkFlowDetailsEnd,
                WorkFlowDetailsFrom = param.WorkFlowDetailsFrom,
                WorkFlowDetailsTerminate = param.WorkFlowDetailsTerminate,
                WorkFlowDetailsList = param.WorkFlowDetailsList,
                WorkFlowNextStepsList = param.WorkFlowNextStepsList,
                WorkFlowNextStepId = param.WorkFlowNextStepId,
                WorkflowEsbsList = param.WorkflowEsbsList,
                Path = param.Path,
                Esb = param.Esb,
                Evt = param.Evt,
                Exp = param.Exp,
                FlowLine = param.FlowLine,
                Gateway = param.Gateway,
                Method = param.Method,
                BoundryName = param.BoundryName
            };

            if (targetGatway.Name.LocalName.ToLower().Contains("endevent"))
            {
                EndEventTarget(workflowParam);
                RemovePreviwesPath(param);
            }
            if (targetGatway.Name.LocalName.ToLower().Contains("task")
                || targetGatway.Name.LocalName.ToLower().Contains("subprocess")
                || targetGatway.Name.LocalName.ToLower().Contains("callactivity"))
            {
                TaskTarget(workflowParam);
                RemovePreviwesPath(param);
            }
            if (targetGatway.Name.LocalName.ToLower().Contains("gateway"))
            {
                TraversGatway(workflowParam);
                RemovePreviwesPath(param);
                param.Gateway.Pop();

            }
            if (targetGatway.Name.LocalName.ToLower().Contains("intermediate"))
            {
                TraversIntermediateThrowMessage(workflowParam);
                RemovePreviwesPath(param);
                param.Esb.Pop();
            }
        }

    }

    private Guid GenerateNewIdForWorkflowNextStep(ModelerParamDto modelerParam)
    {
        var newId = Guid.NewGuid();
        //var workEsbs =
        //    modelerParam.WorkflowEsbsList.Where(d => d.WorkflowNextStepId == modelerParam.WorkFlowNextStepId);
        //foreach (var workflowEsb in workEsbs)
        //{
        //    workflowEsb.WorkflowNextStepId = newId;
        //}

        return newId;
    }

    private static string ConvertStackToString(Stack<string> path)
    {
        var path2 = new Stack<string>(path.ToArray());
        var p = path2.Aggregate("", (current, item) => current + item + "@");
        return p.Length > 0 ? p.Substring(0, p.Length - 1) : null;
    }

    private WorkFlowDetail AddWorkFlowDetail(XElement node, int? step = null)
    {
        WorkFlowDetail workFlowDetails;

        var staffId = (string)node.Attribute(_workflow + "StaffId");
        if (staffId == "null")
        {
            staffId = null;
        }
        var viewName = (string)node.Attribute(_workflow + "ViewName");
        if (viewName == "null")
        {
            viewName = null;
        }
        var workflowFormId = (string)node.Attribute(_workflow + "WorkFlowFormId");
        if (workflowFormId == "null")
        {
            workflowFormId = null;
        }
        var organizationPostTitleId = (string)node.Attribute(_workflow + "OrganizationPostTitleId");
        if (organizationPostTitleId == "null")
        {
            organizationPostTitleId = null;
        }
        var organizationPostTypeId = (string)node.Attribute(_workflow + "OrganizationPostTypeId");
        if (organizationPostTypeId == "null")
        {
            organizationPostTypeId = null;
        }
        var responseGroupId = (string)node.Attribute(_workflow + "ResponseGroupId");
        if (responseGroupId == "null")
        {
            responseGroupId = null;
        }
        var patternId = (string)node.Attribute(_workflow + "PatternId");
        if (patternId == "null")
        {
            patternId = null;
        }

        var formIdForTask = (string)node.Attribute(_workflow + "FormIdForTask");
        if (formIdForTask == "null")
        {
            formIdForTask = null;
        }

        var dynamicWaitingDate = (string)node.Attribute("DynamicWaitingDate");
        if (dynamicWaitingDate == "null")
        {
            dynamicWaitingDate = null;
        }

        var taskInfo = new TaskInfo() { FormIdForTask = formIdForTask, DynamicWaitingDate = dynamicWaitingDate };

        string nodeName = (string)node.Attribute("name");

        string act = null;
        int? waitingTimeForAct = null;
        if (node.Name.LocalName == "timerTask")
        {
            act = (string)node.Attribute(_workflow + "Act") ?? "A";
            waitingTimeForAct = Convert.ToInt32((string)node.Attribute(_workflow + "WaitingTimeForAct"));
        }

        if (node.Name.LocalName == "adHocSubProcess")
        {
            workFlowDetails = new WorkFlowDetail()
            {
                Id = Guid.Parse((string)node.Attribute(_workflow + "WorkFlowDetailId")),
                Title = nodeName,
                CallProcessId = Guid.Parse((string)node.Attribute(_workflow + "WorkflowId")),
                ExiteMethod = null,
                IsAdHoc = true,
                AdHocWorkflowDetails = (string)node.Attribute(_workflow + "AdHocSelectList")
            };
            return workFlowDetails;
        }
        if (node.Name == _bpmn2 + "callActivity" || node.Name.LocalName == "subProcess")
        {
            ExiteMethod? exiteMethod;
            if (node.Name.LocalName == "subProcess")
                exiteMethod = null;
            else
                exiteMethod = (string)node.Attribute(_workflow + "ExiteMethod") == "2"
                    ? ExiteMethod.Standalone
                    : ExiteMethod.Integrated;

            workFlowDetails = new WorkFlowDetail()
            {
                Id = Guid.Parse((string)node.Attribute(_workflow + "WorkFlowDetailId")),
                Title = nodeName,
                CallProcessId = Guid.Parse((string)node.Attribute(_workflow + "WorkflowId")),
                ExiteMethod = exiteMethod
            };
            return workFlowDetails;
        }
        if (node.Name.LocalName == "businessRuleTask")
        {
            workFlowDetails = new WorkFlowDetail()
            {
                Id = Guid.Parse((string)node.Attribute(_workflow + "WorkFlowDetailId")),
                Title = nodeName,
                Dsr = (string)node.Attribute(_workflow + "Dsr"),
                Step = step,
                BusinessAcceptorMethod = (string)node.Attribute(_workflow + "BusinessAcceptorMethod"),
                EditableFields = (string)node.Attribute(_workflow + "ReadOnlyDynamicForm"),
                HiddenFields = (string)node.Attribute(_workflow + "HiddenFieldDynamicForm"),
                ViewName = viewName,
                IsMultiConfirmReject = Convert.ToBoolean((string)node.Attribute(_workflow + "IsMultiConfirmReject")),
                IsOrLogic = Convert.ToBoolean((string)node.Attribute(_workflow + "IsOrLogic")),
                NoReject = Convert.ToBoolean((string)node.Attribute(_workflow + "NoReject")),
                RequesterAccept = false,
                SelectAcceptor = false,
                BusinessAcceptor = true,
                PrintFileName = (string)node.Attribute(_workflow + "PrintFileText"),
                OrganizationPostTitleId = null,
                OrganizationPostTypeId = null,
                ResponseGroupId = null,
                WorkflowDetailPatternId = null,
                SelectFirstPostPattern = false,
                SelectAllPostPattern = false,
                StaffId = null,
                WorkFlowFormId = workflowFormId == null
                    ? (Guid?)null
                    : Guid.Parse(workflowFormId),
                WaitingTime = Convert.ToInt32((string)node.Attribute(_workflow + "WaitingTime"))
            };
            return workFlowDetails;
        }

        if (node.Name.LocalName == BpmnNodeConstant.ManualTask || node.Name.LocalName == BpmnNodeConstant.ScriptTask)
        {
            var staff = DbContext.Staffs.FirstOrDefault(t => t.PersonalCode == SystemConstant.SystemUser).Id;
            workFlowDetails = new WorkFlowDetail()
            {
                Id = Guid.Parse((string)node.Attribute(_workflow + "WorkFlowDetailId")),
                Title = nodeName,
                Dsr = null,
                Step = step,
                BusinessAcceptorMethod = null,
                EditableFields = null,
                ViewName = viewName,
                IsMultiConfirmReject = false,
                IsOrLogic = false,
                NoReject = false,
                RequesterAccept = false,
                SelectAcceptor = false,
                BusinessAcceptor = false,
                PrintFileName = null,
                OrganizationPostTitleId = null,
                OrganizationPostTypeId = null,
                ResponseGroupId = null,
                WorkflowDetailPatternId = null,
                SelectFirstPostPattern = false,
                SelectAllPostPattern = false,
                StaffId = staff,
                WorkFlowFormId = null,
                WaitingTime = Convert.ToInt32((string)node.Attribute(_workflow + "WaitingTime")),
                WaithingTImeForAct = waitingTimeForAct
            };
            if (node.Name.LocalName == BpmnNodeConstant.ManualTask)
            {
                workFlowDetails.IsManualTask = true;

            }
            else if (node.Name.LocalName == BpmnNodeConstant.ScriptTask)
            {
                workFlowDetails.IsScriptTask = true;
                workFlowDetails.ScriptTaskMethod = (string)node.Attribute(_workflow + "ScriptTaskMethod");
            }


            return workFlowDetails;
        }

        workFlowDetails = new WorkFlowDetail()
        {
            Id = Guid.Parse((string)node.Attribute(_workflow + "WorkFlowDetailId")),
            Title = nodeName,
            Dsr = (string)node.Attribute(_workflow + "Dsr"),
            Step = step,
            BusinessAcceptorMethod = (string)node.Attribute(_workflow + "BusinessAcceptorMethod"),
            EditableFields = (string)node.Attribute(_workflow + "ReadOnlyDynamicForm"),
            HiddenFields = (string)node.Attribute(_workflow + "HiddenFieldDynamicForm"),
            ViewName = viewName,
            IsMultiConfirmReject =
                Convert.ToBoolean((string)node.Attribute(_workflow + "IsMultiConfirmReject")),
            IsOrLogic = Convert.ToBoolean((string)node.Attribute(_workflow + "IsOrLogic")),
            NoReject = Convert.ToBoolean((string)node.Attribute(_workflow + "NoReject")),
            RequesterAccept = Convert.ToBoolean((string)node.Attribute(_workflow + "RequesterAccept")),
            SelectAcceptor = Convert.ToBoolean((string)node.Attribute(_workflow + "SelectAcceptor")),
            BusinessAcceptor = Convert.ToBoolean((string)node.Attribute(_workflow + "BusinessAcceptor")),
            SelectFirstPostPattern = Convert.ToBoolean((string)node.Attribute(_workflow + "SelectPatternFirst")),
            SelectAllPostPattern = Convert.ToBoolean((string)node.Attribute(_workflow + "SelectPatternAll")),
            HasSaveableForm = Convert.ToBoolean((string)node.Attribute("SaveableTask")),

            PrintFileName = (string)node.Attribute(_workflow + "PrintFileText"),
            OrganizationPostTitleId = organizationPostTitleId == null
                ? (Guid?)null
                : Guid.Parse(organizationPostTitleId),
            OrganizationPostTypeId = organizationPostTypeId == null
                ? (Guid?)null
                : Guid.Parse(organizationPostTypeId),
            ResponseGroupId = responseGroupId == null
                ? (Guid?)null
                : Guid.Parse(responseGroupId),
            WorkflowDetailPatternId = patternId == null
                ? (Guid?)null
                : Guid.Parse(patternId)
            ,
            StaffId = staffId == null
                ? (Guid?)null
                : Guid.Parse(staffId),
            WorkFlowFormId = workflowFormId == null
                ? (Guid?)null
                : Guid.Parse(workflowFormId),
            WaitingTime = Convert.ToInt32((string)node.Attribute(_workflow + "WaitingTime")),
            Act = act,
            WaithingTImeForAct = waitingTimeForAct,
            Info = formIdForTask == null
                ? null
                : JsonConvert.SerializeObject(taskInfo)
        };
        return workFlowDetails;
    }

    private void RemovePreviwesPath(ModelerParamDto param)
    {

        param.Exp.Pop();
        param.Evt.Pop();
        param.FlowLine.Pop();
        param.Path.Pop();
        param.Method.Pop();
        //_esb.Pop();
    }

    public Guid CopyDiagram(Guid id, Guid? parrentId, int orginalVersion, int secondaryVersion, string filePath, string username)
    {
        var user = DbContext.Users.Single(c => c.UserName == username);

        var workflow = DbContext.Workflows.Find(id);
        var encoding = new UnicodeEncoding();
        var str = encoding.GetString(workflow?.Content);
        var myxml = XDocument.Parse(str);
        var workflowid = myxml.Descendants().ToList().Attributes(_workflow + "WorkFlowDetailId");

        foreach (var xAttribute in workflowid)
        {
            xAttribute.Value = Guid.NewGuid().ToString();
        }

        var workflowBoundaryid = myxml.Descendants().ToList().Attributes(_workflow + "WorkFlowBoundaryId");

        foreach (var xAttribute in workflowBoundaryid)
        {
            xAttribute.Value = Guid.NewGuid().ToString();
        }

        if (parrentId == null)
        {
            (orginalVersion, secondaryVersion) = GetNewVersion(workflow);
        }

        var newId = Guid.NewGuid();
        var subProcess = myxml.Descendants(_bpmn2 + "subProcess").ToList().Attributes(_workflow + "WorkflowId");
        foreach (var xAttribute in subProcess)
        {
            var newSubId = CopyDiagram(Guid.Parse(xAttribute.Value), newId, orginalVersion, secondaryVersion, "", username);
            xAttribute.Value = newSubId.ToString();
        }

        var adHocSubProcess = myxml.Descendants(_bpmn2 + "adHocSubProcess").ToList().Attributes(_workflow + "WorkflowId");
        foreach (var xAttribute in adHocSubProcess)
        {
            var newSubId = CopyDiagram(Guid.Parse(xAttribute.Value), newId, orginalVersion, secondaryVersion, "", username);
            xAttribute.Value = newSubId.ToString();
        }

        str = myxml.ToString();
        var newWorkflow = new Workflow()
        {
            Id = newId,
            About = workflow.About,
            CodeId = workflow.CodeId,
            Content = encoding.GetBytes(str),
            Dsr = workflow.Dsr,
            FlowTypeId = LookUpRepository.GetByTypeAndCode("FlowType", 1).Id,
            IsActive = false,
            OrginalVersion = orginalVersion,
            SecondaryVersion = secondaryVersion,
            RegisterDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
            RegisterTime = DateTime.Now.ToString("HHmm"),
            Owner = workflow.Owner,
            RemoteId = workflow.RemoteId,
            RequestGroupTypeId = workflow.RequestGroupTypeId,
            RequestTypeId = workflow.RequestTypeId,
            StaffId = user.StaffId,
            SubProcessId = parrentId,
            KeyWords = workflow.KeyWords
        };
        DbContext.Workflows.Add(newWorkflow);

        // Copy the file from the parrent to this work flow 
        string sourcePath = filePath + id + "/";
        string targetPath = filePath + newId + "/";

        // Copy files into new workflow if the directory exists and there is any file in the current workflow 
        if (Directory.Exists(sourcePath))
        {
            var files = Directory.GetFiles(sourcePath);
            if (files.Length > 0)
            {

                if (!Directory.Exists(targetPath))
                {
                    Directory.CreateDirectory(targetPath);
                }

                foreach (var file in files)
                {
                    string sourceFile = Path.Combine(sourcePath, Path.GetFileName(file));
                    string targetFile = Path.Combine(targetPath, Path.GetFileName(file));

                    File.Copy(sourceFile, targetFile, true);
                }
            }
        }

        return newId;
    }

    public List<SelectListItem> GetWorkFlowForms(Guid workFlowId)
    {
        var workflowIds = new List<Guid>();
        workflowIds.Add(workFlowId);

        GetWorkFlowSubIds(workFlowId);
        GetSubProccessTopLevelId(workFlowId);

        workflowIds.AddRange(workFlowSubIds);

        var data = (from wf in DbContext.Workflows
                    join wfd in DbContext.WorkFlowDetails on wf.Id equals wfd.WorkFlowId
                    join wff in DbContext.WorkFlowForms on wfd.WorkFlowFormId equals wff.Id
                    where workflowIds.Contains(wf.Id)
                    select new SelectListItem()
                    {
                        Value = wff.Id.ToString(),
                        Text = wff.PName
                    }).Distinct().ToList();

        return data;
    }

    List<Guid> workFlowSubIds = new List<Guid>();
    public void GetWorkFlowSubIds(Guid workFlowId)
    {
        var workFlowSubs = DbContext.Workflows.Where(c => c.SubProcessId == workFlowId);
        foreach (var workFlowSub in workFlowSubs)
        {
            workFlowSubIds.Add(workFlowSub.Id);
            GetWorkFlowSubIds(workFlowSub.Id);
        }
    }

    public void GetSubProccessTopLevelId(Guid workFlowId)
    {
        var thisWorkFlow = DbContext.Workflows.Single(c => c.Id == workFlowId);
        var workFlowTops = DbContext.Workflows.Where(c => c.Id == thisWorkFlow.SubProcessId).ToList();
        foreach (var workFlowTop in workFlowTops)
        {
            if (!workFlowSubIds.Contains(workFlowTop.Id))
            {
                workFlowSubIds.Add(workFlowTop.Id);
            }
            GetSubProccessTopLevelId(workFlowTop.Id);
        }
    }

    public Guid GetWorkFlowRequestTypeId(Guid workFlowId)
    {
        return DbContext.Workflows.Single(c => c.Id == workFlowId).RequestTypeId;
    }

    public Tuple<int, int> GetNewVersion(Workflow workflow)
    {
        int original, secondary;
        var maxOriginal = DbContext.Workflows.Where(s => s.RequestTypeId == workflow.RequestTypeId).Max(d => d.OrginalVersion);
        var maxSecondary = DbContext.Workflows.Where(s => s.RequestTypeId == workflow.RequestTypeId && s.OrginalVersion == maxOriginal).Max(d => d.SecondaryVersion);
        if (maxSecondary == 9)
        {
            original = maxOriginal + 1;
            secondary = 0;
        }
        else
        {
            original = workflow.OrginalVersion;
            secondary = maxSecondary + 1;
        }

        return new Tuple<int, int>(original, secondary);
    }

    public List<Guid> GetStaffTopNRequests(Guid staffId, int n)
    {
        var topNWorkflowIds = DbContext.Requests
            .Where(c => c.StaffId == staffId)
            .OrderByDescending(c => c.RegisterDate)
            .ThenByDescending(c => c.RegisterTime)
            .Select(c => c.WorkFlowId)
            .Distinct()
            .Take(n)
            .ToList();

        return topNWorkflowIds;
    }

    #endregion
}