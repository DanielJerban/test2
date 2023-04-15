using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Globalization;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Exceptions = BPMS.Infrastructure.MainHelpers.CustomExceptionHandler;

namespace BPMS.Application.Repositories;

public class ReportRepository : Repository<Report>, IReportRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public ReportRepository(BpmsDbContext context, IServiceProvider serviceProvider, IConfiguration configuration) : base(context)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public BpmsDbContext DbContext => Context;

    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();
    public IFlowRepository FlowRepository => _serviceProvider.GetRequiredService<IFlowRepository>();
    public IChartRepository ChartRepository => _serviceProvider.GetRequiredService<IChartRepository>();


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
    public IEnumerable<ReportViewModel> GetReportsForCurrentUser(Guid staffId)
    {
        var userId = DbContext.Users.Single(d => d.StaffId == staffId).Id;

        var access = from user in DbContext.Users
                     join roleAccess in DbContext.RoleAccesses on user.Id equals roleAccess.UserId
                     join role in DbContext.Roles on roleAccess.RoleId equals role.Id
                     where role.Name == "sysadmin" && user.Id == userId
                     select role;
        if (access.Any())
        {
            return DbContext.Reports.Select(d => new ReportViewModel()
            {
                Id = d.Id,
                Title = d.Title,
                CreatorId = d.CreatorId,
                IsActive = d.IsActive,
                RegisterDate = d.RegisterDate,
                Creator = d.Creator.FName + " " + d.Creator.LName,
                WorkflowName = d.WorkflowId == null ? "" : d.Workflow.RequestType.Title + " " + d.Workflow.OrginalVersion + "." + d.Workflow.SecondaryVersion
            });
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

        var checkReportInAccess = from reports in DbContext.Reports.ToList()
                                  join roleClaim in DbContext.RoleClaims on reports.Id.ToString() equals roleClaim.ClaimValue
                                  join roleId in roleIds on roleClaim.RoleId equals roleId
                                  where roleClaim.ClaimType == PermissionPolicyType.ReportPermission
                                  select new ReportViewModel()
                                  {
                                      Id = reports.Id,
                                      Title = reports.Title,
                                      CreatorId = reports.CreatorId,
                                      IsActive = reports.IsActive,
                                      RegisterDate = reports.RegisterDate,
                                      Creator = reports.Creator.FName + " " + reports.Creator.LName
                                  };

        var checkeportForPerson = from reports in DbContext.Reports.ToList()
                                  where reports.CreatorId == staffId
                                  select new ReportViewModel()
                                  {
                                      Id = reports.Id,
                                      Title = reports.Title,
                                      CreatorId = reports.CreatorId,
                                      IsActive = reports.IsActive,
                                      RegisterDate = reports.RegisterDate,
                                      Creator = reports.Creator.FName + " " + reports.Creator.LName
                                  };
        var all = checkeportForPerson.Union(checkReportInAccess);
        return all.DistinctBy(d => d.Id).ToList();

    }

    public ReportViewModel LoadReport(Guid id)
    {
        var encoding = new UnicodeEncoding();
        var report = DbContext.Reports.Find(id);
        var expersion = encoding.GetString(report.Expersion);
        return new ReportViewModel()
        {
            Id = report.Id,
            Title = report.Title,
            RegisterDate = report.RegisterDate,
            Content = HelperBs.EncodeUri(expersion)
        };
    }

    public ReportViewModel GetReportById(Guid id)
    {
        var report = DbContext.Reports.Find(id);

        if (report != null)
            return new ReportViewModel()
            {
                Id = report.Id,
                Title = report.Title,
                IsActive = report.IsActive,
                PrintFileName = report.PrintFileName,
                WorkflowId = report.WorkflowId
            };
        return new ReportViewModel();
    }

    public Guid SaveReport(ReportViewModel model, IFormFile file, string username, string webRootPath)
    {
        var currentUser = DbContext.Users.Single(c => c.UserName == username);
        string printFileName = null;
        if (model.Id == Guid.Empty)
        {
            model.Id = Guid.NewGuid();
        }
        else if (!string.IsNullOrWhiteSpace(model.PrintFileName))
        {
            var rep = DbContext.Reports.Find(model.Id);
            printFileName = rep.PrintFileName;
        }

        //upload file
        string filePath = _configuration["Common:FilePath:ReportFilePath"];
        if (file.Length > 0)
        {
            if (file.Length > 50 * 1024 * 1024)
                throw new ArgumentException("حجم فایل آپلود شده نمیتوان بیش از 50 مگابایت باشد.");

            var fileName = file.FileName;
            var extension = Path.GetExtension(fileName)?.ToLower();
            if (extension != ".mrt")
                throw new ArgumentException("فرمت فایل غیر مجاز است.");
            var targetFolder = Path.Combine(webRootPath, filePath + model.Id);

            if (Directory.Exists(targetFolder))
            {
                Directory.Delete(targetFolder, true);
            }
            Directory.CreateDirectory(targetFolder);
            var name = Path.GetFileNameWithoutExtension(fileName) + Guid.NewGuid().ToString().Replace("-", "") +
                       Path.GetExtension(fileName);
            var path = Path.Combine(targetFolder, name);
            using Stream fileStream = new FileStream(path, FileMode.Create);
            file.CopyTo(fileStream);
            printFileName = name;
        }
        else
            throw new ArgumentException("فایل مورد نظر آپلود نشد");
        //
        var encoding = new UnicodeEncoding();
        var json = HttpUtility.UrlDecode(model.Content, System.Text.Encoding.UTF8);
        var bytes = encoding.GetBytes(json ?? throw new InvalidOperationException());

        var report = new Report()
        {
            Id = model.Id,
            Title = model.Title,
            IsActive = model.IsActive,
            CreatorId = currentUser.StaffId,
            RegisterDate = int.Parse(DateTime.Now.ToString("yyyyMMdd")),
            Expersion = bytes,
            WorkflowId = model.WorkflowId
        };
        if (!string.IsNullOrWhiteSpace(printFileName))
        {
            report.PrintFileName = printFileName;
        }
        DbContext.Reports.Update(report);
        return report.Id;
    }

    public void DeleteReport(Guid id, string webRootPath)
    {
        var report = DbContext.Reports.Find(id);
        if (report == null)
            throw new ArgumentException("رکورد مورد نظر پیدا نشد.");

        var diagram = DbContext.DynamicCharts.Where(d => d.ReportId == id);
        if (diagram.Any())
            throw new ArgumentException("به دلیل استفاده در نمودار گزارش امکان حذف ندارد.");

        DbContext.Reports.Remove(report);
        var path = Path.Combine(webRootPath, "Images/upload/reports/" + id);
        if (Directory.Exists(path))
        {
            Directory.Delete(Path.Combine(webRootPath, "Images/upload/reports/" + id), true);
        }
    }

    public DynamicViewModel GenerateTreeForReportGenerator()
    {
        var report = new List<ReportTreeViewModel>
        {
            // کاربران 
            new ReportTreeViewModel()
            {
                Text = "کاربران",
                Id = "Staffs",
                ImageUrl = "../Content/images/icon/Table.png",
                Expanded = false,
                Type = "Table",
                Items = new List<ReportTreeViewModel>()
                {
                    new ReportTreeViewModel()
                    {
                        Id = "Id",
                        Text = "شناسه",
                        ImageUrl = "../Content/images/icon/PrimaryKeyField.png",
                        Type = "Guid",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "PersonalCode",
                        Text = "نام کاربری",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "FName",
                        Text = "نام",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "LName",
                        Text = "نام خانوادگی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "PhoneNumber",
                        Text = "تلفن همراه",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Email",
                        Text = "ایمیل",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Staffs"
                    },

                    new ReportTreeViewModel()
                    {
                        Id = "StaffTypeId",
                        Text = "شناسه نوع کاربر",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-FK-LookUps",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id =  "EngTypeId",
                        Text = "شناسه وضعیت همکاری",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-LookUps",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "InsuranceNumber",
                        Text = "شماره بیمه",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "IdNumber",
                        Text = "شماره شناسنامه",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "EmploymentDate",
                        Text = "تاریخ استخدام",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "LocalPhone",
                        Text = "تلفن داخلی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "String",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "BuildingId",
                        Text = "شناسه ساختمان",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-LookUps",
                        Table = "Staffs"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Gender",
                        Text = "جنسیت",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "byte",
                        Table = "Staffs"
                    }
                }
            },
            // گردش کار
            new ReportTreeViewModel()
            {
                Text = "گردش کار",
                Id = "Workflows",
                ImageUrl = "../Content/images/icon/Table.png",
                Type = "Table",
                Items = new List<ReportTreeViewModel>()
                {
                    new ReportTreeViewModel()
                    {
                        Id = "Id",
                        Text = "شناسه",
                        ImageUrl = "../Content/images/icon/PrimaryKeyField.png",
                        Type = "Guid",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RequestTypeId",
                        Text = "شناسه نوع درخواست",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-FK-LookUps",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "StaffId",
                        Text = "شناسه کاربر",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-FK-Staffs",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RegisterDate",
                        Text = "تاریخ ثبت",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "IsActive",
                        Text = "فعال",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "bool",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Dsr",
                        Text = "توضیحات",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "OrginalVersion",
                        Text = "نسخه اصلی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "SecondaryVersion",
                        Text = "نسخه فرعی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "FlowTypeId",
                        Text = "شناسه نوع گردش",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-FK-LookUps",
                        Table = "Workflows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RemoteId",
                        Text = "شناسه کنترل از راه دور",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid",
                        Table = "Workflows"
                    }
                },
                Expanded = false
            },
            // جزئیات گردش کار
            new ReportTreeViewModel()
            {
                Text = "جزئیات گردش کار",
                Id = "WorkFlowDetails",
                ImageUrl = "../Content/images/icon/Table.png",
                Type = "Table",
                Items = new List<ReportTreeViewModel>()
                {
                    new ReportTreeViewModel()
                    {
                        Id = "Id",
                        Text = "شناسه",
                        ImageUrl = "../Content/images/icon/PrimaryKeyField.png",
                        Type = "Guid",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "WorkFlowId",
                        Text = "شناسه گردش کار",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-FK-Workflows",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "StaffId",
                        Text = "شناسه کاربر",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-FK-Staffs",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "OrganizationPostTypeId",
                        Text = "شناسه نوع پست سازمانی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-FK-LookUps",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "OrganizationPostTitleId ",
                        Text = "شناسه عنوان پست سازمانی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-FK-LookUps",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "IsMultiConfirmReject",
                        Text = "تایید و رد گروهی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "bool",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RequesterAccept",
                        Text = "تایید توسط درخواست دهنده",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "bool",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "IsOrLogic",
                        Text = "تایید با منطق or",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "bool",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "BusinessAcceptor",
                        Text = "پاسخ دهنده توسط کد",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "bool",
                        Table = "WorkFlowDetails"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Title",
                        Text = "عنوان مرحله",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "WorkFlowDetails"
                    }
                },
                Expanded = false
            },
            // درخواست ها
            new ReportTreeViewModel()
            {
                Text = "درخواست",
                Id = "Requests",
                ImageUrl = "../Content/images/icon/Table.png",
                Expanded = false,
                Type = "Table",
                Items = new List<ReportTreeViewModel>()
                {
                    new ReportTreeViewModel()
                    {
                        Id = "Id",
                        Text = "شناسه",
                        ImageUrl = "../Content/images/icon/PrimaryKeyField.png",
                        Type = "Guid",
                        Table = "Requests"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RegisterDate",
                        Text = "تاریخ درخواست",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "Requests"

                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RegisterTime",
                        Text = "زمان درخواست",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Requests"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RequestNo",
                        Text = "شماره درخواست",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "long",
                        Table = "Requests"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "WorkFlowId",
                        Text = "شناسه گردش کار",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-Workflows",
                        Table = "Requests"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RequestStatusId",
                        Text = "شناسه وضعیت درخواست",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-LookUps",
                        Table = "Requests"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "StaffId",
                        Text = "شناسه کاربران",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-Staffs",
                        Table = "Requests"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "OrganizationPostTitleId",
                        Text = "شناسه پست سازمانی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-LookUps",
                        Table = "Requests"
                    }
                }
            },
            // گردش
            new ReportTreeViewModel()
            {
                Text = "گردش",
                Id = "Flows",
                ImageUrl = "../Content/images/icon/Table.png",
                Type = "Table",
                Items = new List<ReportTreeViewModel>()
                {
                    new ReportTreeViewModel()
                    {
                        Id = "Id",
                        Text = "شناسه",
                        ImageUrl = "../Content/images/icon/PrimaryKeyField.png",
                        Type = "Guid",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "StaffId",
                        Text = "شناسه کاربران",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-Staffs",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RequestId",
                        Text = "شناسه درخواست ها",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-Requests",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "FlowStatusId",
                        Text = "شناسه وضعیت پاسخ",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-LookUps",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "ResponseDate",
                        Text = "تاریخ پاسخ",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "ResponseTime",
                        Text = "زمان پاسخ",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "DelayDate",
                        Text = "تاریخ دریافت",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "DelayTime",
                        Text = "زمان دریافت",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Dsr",
                        Text = "توضیحات",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "PreviousFlowId",
                        Text = "شناسه گردش کار قبلی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-Flows",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "WorkFlowDetailId",
                        Text = "شناسه جزئیات گردش کار",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "Guid-Fk-WorkFlowDetails",
                        Table = "Flows"
                    }
                },
                Expanded = false
            }, 
            // شرکت ها
            new ReportTreeViewModel()
            {
                Text = "شرکت",
                Id = "Companies",
                ImageUrl = "../Content/images/icon/Table.png",
                Type = "Table",
                Items = new List<ReportTreeViewModel>()
                {
                    new ReportTreeViewModel()
                    {
                        Id = "Id",
                        Text = "شناسه",
                        ImageUrl = "../Content/images/icon/PrimaryKeyField.png",
                        Type = "Guid",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Name",
                        Text = "نام شرکت",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "EconomicCode",
                        Text = "کد اقتصادی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "ShortName",
                        Text = "نام اختصاری",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Telephone",
                        Text = "تلفن",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Fax",
                        Text = "فکس",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Email",
                        Text = "پست الکترونیک",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "WebSite",
                        Text = "وب سایت",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "PostalCode",
                        Text = "کد پستی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "FullAddress",
                        Text = "آدرس",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Dsr",
                        Text = "توضیحات",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RegisterDate",
                        Text = "تاریخ ثبت",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "NationalCode",
                        Text = "شناسه ملی",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "RegistrationNo",
                        Text = "شماره ثبت",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Companies"
                    },
                },
                Expanded = false
            },
            // جدول مراجعه
            new ReportTreeViewModel()
            {
                Text = "جدول مراجعه",
                Id = "LookUps",
                ImageUrl = "../Content/images/icon/Table.png",
                Type = "Table",
                Items = new List<ReportTreeViewModel>()
                {
                    new ReportTreeViewModel()
                    {
                        Id = "Id",
                        Text = "شناسه",
                        ImageUrl = "../Content/images/icon/PrimaryKeyField.png",
                        Type = "Guid",
                        Table = "LookUps"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Code",
                        Text = "کد",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "int",
                        Table = "LookUps"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Type",
                        Text = "نوع",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "LookUps"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Title",
                        Text = "عنوان",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "LookUps"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Aux",
                        Text = "مقدار",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "string",
                        Table = "Flows"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "Aux2",
                        Text = "مقدار2",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "strings",
                        Table = "LookUps"
                    },
                    new ReportTreeViewModel()
                    {
                        Id = "IsActive",
                        Text = "فعال",
                        ImageUrl = "../Content/images/icon/FieldNode.png",
                        Type = "bool",
                        Table = "LookUps"
                    }
                },
                Expanded = false
            }
        };

        // فرآیندها
        var childWorkFlows = new List<ReportTreeViewModel>();

        var workflows = DbContext.Workflows.Include(d => d.RequestType)
            .Where(d => d.RequestType.Type == "RequestType" && d.RequestType.IsActive && d.SubProcessId == null)
            .OrderBy(d => d.RequestType.Title);
        foreach (var workflow in workflows)
        {
            childWorkFlows.Add(new ReportTreeViewModel()
            {
                Id = workflow.RequestTypeId.ToString(),
                Text = workflow.RequestType.Title,
                ImageUrl = "../Content/images/icon/Table.png",
                Type = "RequestTable",
            });
        }

        report.Add(new ReportTreeViewModel()
        {
            Text = "فرآیندها",
            Id = "Workflows",
            Type = "Workflows",
            SpriteCssClass = "fa fa-gear",
            Items = childWorkFlows.DistinctBy(d => d.Id).ToList(),
            Expanded = false
        });
        var model = new DynamicViewModel()
        {
            Tree = HelperBs.EncodeUri(JsonConvert.SerializeObject(report))
        };
        return model;
    }

    public string ExecuteQueryInDb(string code, List<string> jsonTables, List<List<string>> tableFields, ref int total, int pageNo = 0)
    {
        var encoding = new UnicodeEncoding();
        var finalQuery = string.Empty;
        var count = 0;
        for (var i = 0; i < jsonTables.Count; i++)
        {
            if (!Guid.TryParse(jsonTables[i], out var reqTypeId)) continue;
            finalQuery += $" if (OBJECT_ID ('tempdb.dbo.[#{jsonTables[i]}]') is not null) ";
            finalQuery += $" drop table [#{jsonTables[i]}];" + Environment.NewLine + " " + Environment.NewLine;

            var query = DbContext.Requests.Where(d => d.Workflow.RequestTypeId == reqTypeId && d.RegisterDate >= 13990101);
            total = query.Count();
            var req = pageNo == 0 ? query.ToList() :
                query.OrderByDescending(a => a.RegisterDate).ThenByDescending(a => a.RegisterTime)
                    .Skip((pageNo - 1) * 15).Take(15).ToList();
            var array = new JArray();
            foreach (var request in req)
            {
                var obj = JObject.Parse(encoding.GetString(request.Value));
                obj.Add("RequestId", request.Id);
                array.Add(obj);
            }

            finalQuery += @"DECLARE @json" + count + " NVARCHAR(MAX);" + Environment.NewLine;
            finalQuery += "SET @json" + count + " = '" + array.ToString().Replace("'", "''") + "';" + Environment.NewLine;
            finalQuery += "select * into [#" + reqTypeId + "] from OPENJSON(@json" + count + ") with (" + Environment.NewLine;

            var namesList = new List<string>();

            const string type = "nvarchar(max)";
            foreach (var name in tableFields[i])
            {
                var checkName = name.ToLower();
                if (namesList.Contains(checkName))
                {
                    continue;
                }

                finalQuery += "[" + name + "] " + type + " '$." + name + "'," + Environment.NewLine;
                namesList.Add(checkName);
            }

            finalQuery = finalQuery.Substring(0, finalQuery.Length - 3) + ");" + Environment.NewLine;

            count++;
            code = code.Replace(jsonTables[i], $"#{jsonTables[i]}");
        }

        finalQuery += code + Environment.NewLine;
        if (code.ToLower().Contains("delete "))
        {
            throw new ArgumentException("نمی توانید از کلمه Delete در کد استفاده کنید.");
        }
        if (code.ToLower().Contains("drop "))
        {
            throw new ArgumentException("نمی توانید از کلمه Drop در متد استفاده کنید.");
        }
        if (code.ToLower().Contains("update "))
        {
            throw new ArgumentException("نمی توانید از کلمه Update در متد استفاده کنید.");
        }
        if (code.ToLower().Contains("insert "))
        {
            throw new ArgumentException("نمی توانید از کلمه Insert در متد استفاده کنید.");
        }
        if (code.ToLower().Contains("alter "))
        {
            throw new ArgumentException("نمی توانید از کلمه Alter در متد استفاده کنید.");
        }
        if (code.ToLower().Contains("create "))
        {
            throw new ArgumentException("نمی توانید از کلمه create در متد استفاده کنید.");
        }

        // TODO: Read data using dapper later 
        //var result = DbContext.Database.DynamicSqlQuery(finalQuery);
        // TODO: and remove this line 
        var result = new  List<string>();
        var data = JsonConvert.SerializeObject(result);

        var dataArr = JArray.Parse(data);
        if (!dataArr.Any())
        {
            throw new ArgumentException("داده ای وجود ندارد");
        }
        var column = new JArray();
        foreach (var item in dataArr.First)
        {
            var prop = item.GetType().GetProperty("Name");
            var name = prop.GetValue(item, null).ToString();
            column.Add(new JObject() { { "field", name }, { "title", name.Replace("_", " ") } });
        }

        var finalResult = new JObject { { "columns", column }, { "data", dataArr } };
        return finalResult.ToString();
    }

    public List<ReportTreeViewModel> GetPropertiesFromRequestType(Guid requestTypeId)
    {
        var list = new List<ReportTreeViewModel>();
        var workflows = DbContext.Workflows.Where(w => w.RequestTypeId == requestTypeId);
        foreach (var workflow in workflows)
        {
            list.AddRange(GetPropertiesFromProcess(workflow.Id));
        }

        return list.DistinctBy(d => d.Id).ToList();
    }

    public List<ReportTreeViewModel> GetPropertiesFromProcess(Guid id)
    {
        var list = new List<ReportTreeViewModel>();
        var workflow = DbContext.Workflows.Single(w => w.Id == id);
        foreach (var workflowdetail in workflow.WorkflowDetails)
        {
            if (workflowdetail.CallProcessId != null)
            {
                list.AddRange(GetPropertiesFromProcess(workflowdetail.CallProcessId.Value));
            }
            var form = DbContext.WorkFlowForms.FirstOrDefault(w => w.Id == workflowdetail.WorkFlowFormId);
            if (form == null) continue;
            list.AddRange(GetPropertiesFormFOrPerocess(form));

        }
        return list.DistinctBy(d => d.Id).ToList();
    }

    private IEnumerable<ReportTreeViewModel> GetPropertiesFormFOrPerocess(WorkFlowForm form)
    {
        var list = new List<ReportTreeViewModel>();
        var encoding = new UnicodeEncoding();
        var jsonFile = encoding.GetString(form.Content);
        var json = HttpUtility.UrlDecode(jsonFile, System.Text.Encoding.UTF8);

        var jobj = JObject.Parse(json);

        var fileds = jobj["fields"];

        foreach (var filed in fileds)
        {
            var fli = filed.First.ToObject<JObject>();
            var option = "";
            if (fli["options"] != null)
                option = fli["options"].ToString();
            var type = (string)fli["meta"]["id"];
            string label = null;
            switch (type)
            {
                case "paragraph": continue;
                case "hidden": continue;
                case "upload": continue;
                case "button": continue;
                case "header": continue;
                case "divider": continue;
                case "loadform":
                    var value = Guid.Parse((string)fli["attrs"]["value"]);
                    var subform = DbContext.WorkFlowForms.Find(value);
                    list.AddRange(GetPropertiesFormFOrPerocess(subform));
                    continue;
                case "company":
                    type = "Select";
                    break;
                case "staff":
                    type = "Select";
                    break;
                case "client":
                    type = "Select";
                    break;
                case "lookup":
                    type = "Select";
                    break;
                case "lookup2N":
                    type = "Select";
                    list.Add(Level2LookupReport(fli, type));
                    break;
                case "number":
                    type = "Single";
                    break;
                case "checkbox":
                    type = "Boolean";
                    break;
                case "newCheckbox":
                    type = "Boolean";
                    label = (string)fli["attrs"]["label"];
                    break;
                case "select":
                    type = "Select";
                    break;
                case "radio":
                    type = "Select";
                    break;
                case "textarea":
                    type = "Textarea";
                    break;

                default:
                    type = "String";

                    break;
            }

            var attrid = (string)fli["attrs"]["id"];
            var reportName = (string)fli["attrs"]["data-reportName"];

            if (!string.IsNullOrWhiteSpace(reportName))
                label = reportName;
            else if ((string)fli["meta"]["id"] != "newCheckbox")
                label = (string)fli["config"]["label"];

            list.Add(new ReportTreeViewModel()
            {
                Id = attrid,
                Text = label,
                Type = type,
                Options = option
            });

        }
        return list;
    }

    private static ReportTreeViewModel Level2LookupReport(JObject fli, string type)
    {
        var reportName2 = (string)fli["attrs"]["data-reportNameTitle2"];
        var attrid2 = (string)fli["attrs"]["id"] + "2";
        var label2 = (string)fli["attrs"]["data-title2"];
        if (!string.IsNullOrWhiteSpace(reportName2))
            label2 = reportName2;
        return new ReportTreeViewModel()
        {
            Id = attrid2,
            Text = label2,
            Type = type
        };
    }

    public string GetResultReportById(Guid id, ref int count, int pageNo = 0)
    {
        var result = "";
        var encoding = new UnicodeEncoding();
        var report = DbContext.Reports.Find(id);
        var expersion = encoding.GetString(report.Expersion);
        var obj = JObject.Parse(expersion);
        var diagram = JObject.Parse(obj["diagram"].ToString());
        var nodeDataArray = diagram["nodeDataArray"];
        var code = obj["code"].ToString();
        var tables = new List<string>();
        var tableFields = new List<List<string>>();
        if (nodeDataArray == null)
        {
            throw new ArgumentException("داده ای وجود ندارد");
        }

        foreach (var node in nodeDataArray)
        {
            var thisshape = node["key"].ToString();
            tables.Add(thisshape.Split('/')[0]);
            var props = JArray.Parse(node["fields"].ToString());
            var fields = new List<string>();
            foreach (var prop in props)
            {
                var name = prop["info"].ToString();
                fields.Add(name);
            }
            tableFields.Add(fields);
        }

        result = ExecuteQueryInDb(code, tables, tableFields, ref count, pageNo);

        return result;
    }

    private IQueryable<Report> GetMyReport(string username)
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

        var roleId = new List<Guid>();
        roleId.AddRange(roleMapPostTitleAccessId);
        roleId.AddRange(roleMapChartAccessIds);
        roleId.AddRange(roleAccessesId);
        roleId.AddRange(roleMapPostTypeAccessId);
        roleId = roleId.Distinct().ToList();

        var reportQueryRoleClaim = from report in DbContext.Reports
                                   join roleClaim in DbContext.RoleClaims on report.Id.ToString() equals roleClaim.ClaimValue
                                   join rol in roleId on roleClaim.RoleId equals rol
                                   where report.IsActive && roleClaim.ClaimType == PermissionPolicyType.ReportPermission
                                   select report;

        var reportQueryUserClaim = from report in DbContext.Reports
                                   join userClaim in DbContext.UserClaims on report.Id.ToString() equals userClaim.ClaimValue
                                   where report.IsActive && userClaim.ClaimType == PermissionPolicyType.ReportPermission && userClaim.UserId == userId
                                   select report;
        var reportQuery = reportQueryRoleClaim.Union(reportQueryUserClaim);

        return reportQuery;
    }

    public DynamicReportViewModel GetReportByAccess(string username)
    {
        var reportQuery = GetMyReport(username);


        var reportList = reportQuery.Select(d => new SelectListItem
        {
            Text = d.Title,
            Value = d.Id.ToString()
        }).Distinct().OrderBy(f => f.Text);

        var workflowQuery = from workflow in DbContext.Workflows
                            join report in reportQuery on workflow.Id equals report.WorkflowId
                            select workflow;
        var workflowList = workflowQuery.Select(d => new SelectListItem
        {
            Text = d.RequestType.Title + " " + d.OrginalVersion + "." + d.SecondaryVersion,
            Value = d.Id.ToString()
        }).Distinct().OrderBy(f => f.Text);

        var requestTypeList = workflowQuery.Select(d => new SelectListItem
        {
            Text = d.RequestType.Title,
            Value = d.RequestTypeId.ToString()
        }).Distinct().OrderBy(f => f.Text);

        return new DynamicReportViewModel
        {
            ReportListItem = reportList,
            WorkflowItems = workflowList,
            RequestTypeItems = requestTypeList
        };
    }

    public IEnumerable<ReportViewModel> GetAllReport()
    {
        return DbContext.Reports.Select(s => new ReportViewModel()
        {
            Id = s.Id,
            Creator = s.Creator.FName + " " + s.Creator.LName,
            Title = s.Title
        });
    }

    #region Delayed Request Reports 

    public List<RequestDelayGridViewModel> GetDelayedReports(Guid? chartId)
    {
        var data = chartId == null ? GetIncompletedRequests() : GetSpecificChartIncompletedRequests(chartId);

        var vmList = new List<RequestDelayGridViewModel>();

        var s2w = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);
        var holyday = DbContext.Holydays.ToList();
        foreach (var item in data)
        {
            // Calculating the total delay hours 
            var reqDate = DateTime.ParseExact(item.DelayDate + item.DelayTime, "yyyyMMddHHmm", null,
                DateTimeStyles.None);

            var diff = Math.Round(FlowRepository.CalculateDelay(reqDate, DateTime.Now, s2w, thr, holyday));
            var sediment = (diff > item.WaitingTime) ? diff - item.WaitingTime : 0;
            var timeToDo = FlowRepository.CalculateTimeToDo(reqDate, item.WaitingTime, s2w, thr, holyday);

            var vm = new RequestDelayGridViewModel
            {
                ApplicantName = item.FullName,
                PersonalName = item.PersonalName,
                FlowLevelName = item.StepTitle,
                RequestTypeTitle = item.RequestTypeTitle,
                FlowNameAndVersion = item.WorkflowNameAndVersion,
                SubprocessName = item.SubprocessName,
                FlowName = item.WorkflowName,
                RequestNumber = item.RequestNo.ToString(),
                DelayHour = sediment,
                RequestId = item.RequestId,
                RequestType = item.RequestTypeTitle,
                RequestDate = item.RequestDate,
                RequestTime = item.RequestTime,
                RequestDateTime = item.RequestDate + "  " + item.RequestTime,
                TimeToDo = timeToDo?.ToString("yyyy/MM/dd HH:mm")
            };

            // Filtering the delayed requests
            if (sediment > 0)
            {
                vmList.Add(vm);
            }
        }

        return vmList;
    }

    public List<RequestDelayGridViewModel> GetDelayedReport_AdminSubUsers(Guid staffId)
    {
        #region Get sub staffs of the current staff

        var adminSubStaffs = GetAdminSubStaffs(staffId);

        #endregion


        #region Delayed requests 

        var data = GetIncompletedRequests();

        List<RequestDelayGridViewModel> vmList = new List<RequestDelayGridViewModel>();

        var s2w = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);
        var holyday = DbContext.Holydays.ToList();

        if (!adminSubStaffs.Any())
        {
            return new List<RequestDelayGridViewModel>();
        }

        foreach (var item in data)
        {
            if (adminSubStaffs.Contains(item.StaffId))
            {
                // Calculating the total delay hours 
                var reqDate = DateTime.ParseExact(item.DelayDate + item.DelayTime, "yyyyMMddHHmm", null,
                    DateTimeStyles.None);
                var diff = Math.Round(FlowRepository.CalculateDelay(reqDate, DateTime.Now, s2w, thr, holyday));
                var sediment = (diff > item.WaitingTime) ? diff - item.WaitingTime : 0;
                var timeToDo = FlowRepository.CalculateTimeToDo(reqDate, item.WaitingTime, s2w, thr, holyday);

                var vm = new RequestDelayGridViewModel
                {
                    ApplicantName = item.FullName,
                    PersonalName = item.PersonalName,
                    FlowLevelName = item.StepTitle,
                    RequestTypeTitle = item.RequestTypeTitle,
                    FlowNameAndVersion = item.WorkflowNameAndVersion,
                    SubprocessName = item.SubprocessName,
                    FlowName = item.WorkflowName,
                    RequestNumber = item.RequestNo.ToString(),
                    DelayHour = sediment,
                    RequestId = item.RequestId,
                    RequestType = item.RequestTypeTitle,
                    RequestDate = item.RequestDate,
                    RequestTime = item.RequestTime,
                    RequestDateTime = item.RequestDate + "  " + item.RequestTime,
                    TimeToDo = timeToDo?.ToString("yyyy/MM/dd HH:mm")
                };


                // Filtering the delayed requests
                if (sediment > 0)
                {
                    vmList.Add(vm);
                }
            }
        }
        #endregion

        return vmList;
    }

    #endregion

    #region All No Action Request Reprot 

    public List<RequestDelayGridViewModel> GetNoActionRequestReport()
    {
        var data = GetIncompletedRequests();

        var vmList = new List<RequestDelayGridViewModel>();

        var s2w = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);
        var holyday = DbContext.Holydays.ToList();
        foreach (var item in data)
        {
            // Calculating the total delay hours 
            var reqDate = DateTime.ParseExact(item.DelayDate + item.DelayTime, "yyyyMMddHHmm", null,
                DateTimeStyles.None);

            var diff = Math.Round(FlowRepository.CalculateDelay(reqDate, DateTime.Now, s2w, thr, holyday));
            var sediment = (diff > item.WaitingTime) ? (diff - item.WaitingTime) : 0;
            var timeToDo = FlowRepository.CalculateTimeToDo(reqDate, item.WaitingTime, s2w, thr, holyday);

            var vm = new RequestDelayGridViewModel
            {
                ApplicantName = item.FullName,
                PersonalName = item.PersonalName,
                FlowLevelName = item.StepTitle,
                RequestTypeTitle = item.RequestTypeTitle,
                FlowNameAndVersion = item.WorkflowNameAndVersion,
                SubprocessName = item.SubprocessName,
                FlowName = item.WorkflowName,
                RequestNumber = item.RequestNo.ToString(),
                DelayHour = sediment,
                RequestId = item.RequestId,
                RequestType = item.RequestTypeTitle,
                RequestDate = item.RequestDate,
                RequestTime = item.RequestTime,
                RequestDateTime = item.RequestDate + "  " + item.RequestTime,
                TimeToDo = timeToDo?.ToString("yyyy/MM/dd HH:mm"),
                FlowId = item.FlowId,
                WorkFlowDetailId = item.WorkflowDetailId
            };

            vmList.Add(vm);
        }

        return vmList;
    }

    public List<RequestDelayGridViewModel> GetAllRequestByAdminSubUsers(Guid staffId)
    {
        var adminSubStaffs = GetAdminSubStaffs(staffId);

        var data = GetIncompletedRequests();

        List<RequestDelayGridViewModel> vmList = new List<RequestDelayGridViewModel>();

        var s2w = LookUpRepository.GetByTypeAndCode("WorkTime", 1);
        var thr = LookUpRepository.GetByTypeAndCode("WorkTime", 2);
        var holyday = DbContext.Holydays.ToList();

        if (!adminSubStaffs.Any())
        {
            return new List<RequestDelayGridViewModel>();
        }

        foreach (var item in data)
        {
            if (adminSubStaffs.Contains(item.StaffId))
            {
                // Calculating the total delay hours 
                var reqDate = DateTime.ParseExact(item.DelayDate + item.DelayTime, "yyyyMMddHHmm", null,
                    DateTimeStyles.None);
                var diff = Math.Round(FlowRepository.CalculateDelay(reqDate, DateTime.Now, s2w, thr, holyday));
                var sediment = (diff > item.WaitingTime) ? diff - item.WaitingTime : 0;
                var timeToDo = FlowRepository.CalculateTimeToDo(reqDate, item.WaitingTime, s2w, thr, holyday);

                var vm = new RequestDelayGridViewModel
                {
                    ApplicantName = item.FullName,
                    PersonalName = item.PersonalName,
                    FlowLevelName = item.StepTitle,
                    RequestTypeTitle = item.RequestTypeTitle,
                    FlowNameAndVersion = item.WorkflowNameAndVersion,
                    SubprocessName = item.SubprocessName,
                    FlowName = item.WorkflowName,
                    RequestNumber = item.RequestNo.ToString(),
                    DelayHour = sediment,
                    RequestId = item.RequestId,
                    RequestType = item.RequestTypeTitle,
                    RequestDate = item.RequestDate,
                    RequestTime = item.RequestTime,
                    RequestDateTime = item.RequestDate + "  " + item.RequestTime,
                    TimeToDo = timeToDo?.ToString("yyyy/MM/dd HH:mm")
                };

                vmList.Add(vm);
            }
        }

        return vmList;
    }

    public IEnumerable<SelectListItem> GetReportByWorkflowId(Guid? workflowId, string username)
    {
        var reportQuery = GetMyReport(username);

        var reportList = reportQuery.Where(d => workflowId == null || d.WorkflowId == workflowId).Select(d => new SelectListItem
        {
            Text = d.Title,
            Value = d.Id.ToString()
        }).Distinct().OrderBy(f => f.Text);
        return reportList;
    }

    public IEnumerable<SelectListItem> GetWorkflowByRequestTypeId(Guid? requestTypeId, string username)
    {
        var reportQuery = GetMyReport(username);

        var workflowQuery = from workflow in DbContext.Workflows
                            join report in reportQuery on workflow.Id equals report.WorkflowId
                            where requestTypeId == null || workflow.RequestType.Id == requestTypeId
                            select workflow;
        var workflowList = workflowQuery.Select(d => new SelectListItem
        {
            Text = d.RequestType.Title + " " + d.OrginalVersion + "." + d.SecondaryVersion,
            Value = d.Id.ToString()
        }).Distinct().OrderBy(f => f.Text);

        return workflowList;
    }

    #endregion

    #region Automation Report

    public void RemindUserTasksThrowAutomation()
    {
        try
        {
            var torfenegarAutomationBaseUrl = _configuration["Common:TorfehnegarAutomationAppBaseUrl"];
            var bpmsBaseUrl = _configuration["Common:BpmsBaseUrl"];

            Guid statusId = Guid.Empty;
            //ایدی مربوط به وضعیت اقدام نشده
            var status = DbContext.LookUps.SingleOrDefault(d => d.Type == "FlowStatus" && d.Code == 1);
            if (status != null)
                statusId = status.Id;
            var result = (from flow in DbContext.Flows
                          join staff in DbContext.Staffs on flow.StaffId equals staff.Id
                          where flow.FlowStatusId == statusId && flow.IsActive
                          select new
                          {
                              flow.Id,
                              staff.FName,
                              staff.LName,
                              staff.PersonalCode,
                              staff.Gender
                          }).Distinct();

            var personalCodesToSend = (from en in result
                                       group new { en.Id } by new
                                       {
                                           en.FName,
                                           en.LName,
                                           en.PersonalCode,
                                           en.Gender
                                       }
                into grp
                                       orderby grp.Count() descending
                                       select new
                                       {
                                           Count = grp.Count(),
                                           FullName = grp.Key.FName + " " + grp.Key.LName,
                                           PersonelCode = grp.Key.PersonalCode,
                                           Gender = grp.Key.Gender
                                       }).ToList();


            foreach (var personal in personalCodesToSend)
            {

                string gender;
                if (personal.Gender == 1)
                {
                    gender = " جناب آقای ";
                }
                else if (personal.Gender == 2)
                {
                    gender = " سرکار خانم ";
                }
                else
                {
                    gender = " همکار گرامی ";
                }

                var content = $@"<p>{gender}{personal.FullName} </br> شما {personal.Count} 
                                      درخواست اقدام نشده در سامانه مدیریت فرآیند های کسب و کار دارید 
                                      لطفا برای پاسخ به درخواست های خود از لینک زیر استفاده کنید.
                                      </p>
                                      <a target='_blank' href='/fa/site/user/bpmslogin'>{bpmsBaseUrl}</a>";

                var uri = new Uri($"{torfenegarAutomationBaseUrl}/api/sendletter");
                var clients = new RestClient(uri);
                var requests = new RestRequest(uri, Method.Post) { Timeout = 10000 };
                requests.AddParameter("sender_pc", SystemConstant.SystemUser);
                requests.AddParameter("receivers_pc", personal.PersonelCode);
                requests.AddParameter("content", content);
                requests.AddParameter("subject", "گزارش وضعیت درخواست ها");
                requests.AddParameter("password",
                    HashPassword.EncodePasswordMd5("anx7gr").Replace("-", "").ToLower());
                requests.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                clients.Execute(requests);
            }
        }
        catch (Exception e)
        {
            var exceptions = new Exceptions();
            exceptions.HandleException(e);
        }
    }

    public void SendFlowsReportThrowAutomation()
    {
        try
        {
            var torfenegarAutomationBaseUrl = _configuration["Common:TorfehnegarAutomationAppBaseUrl"];

            var managers = GetManagers();

            foreach (var manager in managers)
            {
                var data = GetDelayedReport_AdminSubUsers(manager.Id);

                List<RequestDelayExcelViewModel> excelData = new List<RequestDelayExcelViewModel>();
                foreach (var item in data)
                {
                    excelData.Add(new RequestDelayExcelViewModel()
                    {
                        ApplicantName = item.ApplicantName,
                        SubprocessName = item.SubprocessName,
                        TimeToDo = item.TimeToDo,
                        FlowNameAndVersion = item.FlowNameAndVersion,
                        RequestDateTime = item.RequestDateTime,
                        DelayHour = item.DelayHour,
                        FlowLevelName = item.FlowLevelName,
                        PersonalName = item.PersonalName,
                        RequestNumber = item.RequestNumber
                    });
                }

                var fileBytes = HelperBs.ConvertToExcel(excelData.OrderByDescending(c => c.DelayHour));
                var client = new RestClient($"{torfenegarAutomationBaseUrl}/webservice/letter");
                var request = new RestRequest(new Uri($"{torfenegarAutomationBaseUrl}/webservice/letter"), Method.Post);
                request.AddHeader("Authorization", "Bearer tnc_automation_webservice_Letter_apikey");
                request.AddFile("letter_file", fileBytes, "گزارش درخواست های تاخیر دار.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                request.AddParameter("sender_pc", SystemConstant.SystemUser);
                request.AddParameter("receivers_pc", manager.PersonalCode);
                request.AddParameter("subject", "لیست درخواست های دارای تاخیر پرسنل زیرمجموعه من در سامانه مدیریت فرآیند های کسب و کار طرفه نگار ");
                request.AddParameter("content", "لیست درخواست های دارای تاخیر زیرمجموعه در سامانه مدیریت فرآیند های کسب و کار طرفه نگار به پیوست تقدیم می گردد.");
                client.Execute(request);
            }
        }
        catch (Exception e)
        {
            var exceptions = new Exceptions();
            exceptions.HandleException(e);
        }
    }

    #endregion


    #region Helpers


    /// <summary>
    /// Get Subusers id of the current users
    /// </summary>
    /// <returns></returns>
    public List<Guid> GetAdminSubStaffs(Guid staffId)
    {
        // Maddad khani Staff Id 
        // Guid staffId = Guid.Parse("b339cc48-d051-4e54-9916-aae142e2ca0c");
        // Hasan Farahani 
        // Guid staffId = Guid.Parse("D4618A2A-F662-465E-8AB6-14A87F7354BE");
        // Hamidreza Sedighian
        // Guid staffId = Guid.Parse("356D9E59-F728-4DFC-9FE4-FBBA1D9AAC8C");

        var query = Context.OrganiztionInfos.FirstOrDefault(w => w.StaffId == staffId);
        var checkIf = Context.LookUps.FirstOrDefault(r => r.Id.ToString() == query.OrganiztionPost.Aux);
        if (checkIf.Aux2 != 1.ToString())
        {
            return new List<Guid>();
        }
        var chartIds = new List<Guid>();
        chartIds.Add(query.Chart.Id);

        // Get this chart sub charts and charts users 
        // => means this chart and sub charts users 
        var thisChartSubCharts = ChartRepository.GetSubCharts(query.Chart.Id);
        List<Guid> subUsersId = new List<Guid>();
        foreach (var chart in thisChartSubCharts)
        {
            var chartUsers = ChartRepository.GetChartStaffsId(chart.Id);
            subUsersId.AddRange(chartUsers);
        }

        return subUsersId;
    }

    /// <summary>
    /// Get all Incompleted requests
    /// </summary>
    /// <returns></returns>
    public List<RequestDelayViewModel> GetIncompletedRequests()
    {
        // Incompleted preogress code
        int code = 1;

        var query1 = from workflows in DbContext.Workflows
                     join req in DbContext.Requests on workflows.Id equals req.WorkFlowId
                     join flow in DbContext.Flows on req.Id equals flow.RequestId
                     where flow.IsActive && (flow.LookUpFlowStatus.Code == code)
                     select flow;

        var all = query1.Select(s => new RequestDelayViewModel()
        {
            PersonalName = s.Staff.FName + " " + s.Staff.LName,
            RequestNo = s.Request.RequestNo,
            FullName = s.Request.Staff.FName + " " + s.Request.Staff.LName,
            PersonalCode = s.Request.Staff.PersonalCode,
            CurrentStatus = s.LookUpFlowStatus.Title,
            RequestDate = s.Request.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
            RequestTime = s.Request.RegisterTime.Insert(2, ":"),
            StepTitle = s.WorkFlowDetail.Title,
            FlowId = s.Id,
            RequestTypeTitle = s.WorkFlowDetail.WorkFlow.RequestType.Title + " / نسخه " + s.WorkFlowDetail.WorkFlow.OrginalVersion + "." + s.WorkFlowDetail.WorkFlow.SecondaryVersion,
            RequestId = s.RequestId,
            RequestTypeId = s.WorkFlowDetail.WorkFlow.RequestTypeId,
            WorkflowDetailId = s.WorkFlowDetailId,
            WorkflowId = s.WorkFlowDetail.WorkFlowId,
            Workflow = s.WorkFlowDetail.WorkFlow,
            StaffId = s.StaffId,
            ImagePath = s.Request.Staff.ImagePath,
            Name = s.WorkFlowDetail.WorkFlow.RequestType.Title,
            // for delay calculate
            DelayDate = s.DelayDate,
            DelayTime = s.DelayTime,
            WaitingTime = s.WorkFlowDetail.WaitingTime
        });

        var data = all.ToList();
        var newData = SeperateWorkflowAndSubprocess(data);

        return newData;
    }

    public List<RequestDelayViewModel> GetSpecificChartIncompletedRequests(Guid? chartId)
    {
        if (chartId == null)
        {
            return new List<RequestDelayViewModel>();
        }
        Guid fixChartId = (Guid)chartId;

        // Get this chart sub charts and charts users 
        // => means this chart and sub charts users 
        var thisChartSubCharts = ChartRepository.GetSubCharts(fixChartId);
        List<Guid> subUsersId = new List<Guid>();
        foreach (var chart in thisChartSubCharts)
        {
            var chartUsers = ChartRepository.GetChartStaffsId(chart.Id);
            subUsersId.AddRange(chartUsers);
        }

        // Incompleted preogress code
        int code = 1;
        var query1 = from workflows in DbContext.Workflows
                     join req in DbContext.Requests on workflows.Id equals req.WorkFlowId
                     join flow in DbContext.Flows on req.Id equals flow.RequestId
                     where flow.IsActive
                           && (flow.LookUpFlowStatus.Code == code)
                           && subUsersId.Contains(req.StaffId)
                     select flow;

        var all = query1.Select(s => new RequestDelayViewModel()
        {
            PersonalName = s.Staff.FName + " " + s.Staff.LName,
            RequestNo = s.Request.RequestNo,
            FullName = s.Request.Staff.FName + " " + s.Request.Staff.LName,
            PersonalCode = s.Request.Staff.PersonalCode,
            CurrentStatus = s.LookUpFlowStatus.Title,
            RequestDate = s.Request.RegisterDate.ToString().Insert(4, "/").Insert(7, "/"),
            RequestTime = s.Request.RegisterTime.Insert(2, ":"),
            StepTitle = s.WorkFlowDetail.Title,
            FlowId = s.Id,
            RequestTypeTitle = s.WorkFlowDetail.WorkFlow.RequestType.Title + " / نسخه " + s.WorkFlowDetail.WorkFlow.OrginalVersion + "." + s.WorkFlowDetail.WorkFlow.SecondaryVersion,
            RequestId = s.RequestId,
            RequestTypeId = s.WorkFlowDetail.WorkFlow.RequestTypeId,
            WorkflowDetailId = s.WorkFlowDetailId,
            WorkflowId = s.WorkFlowDetail.WorkFlowId,
            Workflow = s.WorkFlowDetail.WorkFlow,
            StaffId = s.StaffId,
            ImagePath = s.Request.Staff.ImagePath,
            Name = s.WorkFlowDetail.WorkFlow.RequestType.Title,
            // for delay calculate
            DelayDate = s.DelayDate,
            DelayTime = s.DelayTime,
            WaitingTime = s.WorkFlowDetail.WaitingTime
        });

        var data = all.ToList();
        //if (data.Count > 0)
        //{
        //    data = data.Take(50).ToList();
        //}
        var newData = SeperateWorkflowAndSubprocess(data);

        return newData;
    }

    public List<RequestDelayViewModel> SeperateWorkflowAndSubprocess(List<RequestDelayViewModel> data)
    {
        var all = new List<RequestDelayViewModel>();

        foreach (var item in data.ToList())
        {
            string workflowName = "";
            string subprocessName = "";
            string workflowVersion = "";

            GetFlowDetail(item.Workflow, item.Name, out workflowName, out subprocessName, out workflowVersion);

            item.WorkflowName = workflowName;
            item.SubprocessName = subprocessName;
            item.WorkFlowVersion = workflowVersion;
            item.WorkflowNameAndVersion = workflowName + " / نسخه " + workflowVersion;

            all.Add(item);
        }

        return all;
    }


    /// <summary>
    /// Get Workflow Name, Subprocess Name, Workflow Version 
    /// </summary>
    /// <param name="workFlow"></param>
    /// <param name="name">workflow name or subprocess name</param>
    /// <param name="flowName"> Workflow name</param>
    /// <param name="subprocessName">Workflow Subprocess Name</param>
    /// <param name="workflowVersion">Workflow Version</param>
    public void GetFlowDetail(Workflow workFlow, string name, out string flowName, out string subprocessName, out string workflowVersion)
    {
        //var workFlowId = _dbContext.WorkFlowDetails.SingleOrDefault(c => c.Id == workflowDetailId).WorkFlowId;
        //var workFlow = _dbContext.Workflows.SingleOrDefault(c => c.Id == workflowId);

        if (workFlow != null && workFlow.SubProcessId == null)
        {
            // get the work flow name only 
            //flowName = _dbContext.LookUps.SingleOrDefault(c => c.Id == workFlow.RequestTypeId).Title;
            flowName = name;
            subprocessName = "";
            workflowVersion = workFlow.OrginalVersion + "." + workFlow.SecondaryVersion;
        }
        else
        {
            // get subprocess name from this workflow 
            //subprocessName = _dbContext.LookUps.SingleOrDefault(c => c.Id == workFlow.RequestTypeId).Title;
            subprocessName = name;

            // now get the work flow name 
            var firstParentWorkflowId = GetParentWorkFlowId(workFlow.Id);
            var flow = DbContext.Workflows.Include(c => c.RequestType)
                .SingleOrDefault(c => c.Id == firstParentWorkflowId);
            flowName = flow.RequestType.Title;
            workflowVersion = flow.OrginalVersion + "." + flow.SecondaryVersion;
        }


    }

    /// <summary>
    /// Gets subprocess first parent id
    /// </summary>
    /// <param name="id">workflowId</param>
    public Guid GetParentWorkFlowId(Guid? id)
    {
        var thisWorkFlow = DbContext.Workflows.SingleOrDefault(c => c.Id == id);
        Guid workflowId = thisWorkFlow.Id;
        if (thisWorkFlow.SubProcessId == null)
        {
            return workflowId;
        }
        Guid? subprocessId = thisWorkFlow.SubProcessId;

        return GetParentWorkFlowId(subprocessId);
    }




    /// <summary>
    /// Staffs with type of OrganizationPostType = مدیر
    /// </summary>
    /// <returns></returns>
    public List<StaffViewModel> GetManagers()
    {
        var managerType = DbContext.LookUps.SingleOrDefault(c => c.Type == "OrganizationPostType" && c.Code == 1);

        var data = (from org in DbContext.OrganiztionInfos
                    join look in DbContext.LookUps on org.OrganiztionPostId equals look.Id
                    join stf in DbContext.Staffs on org.StaffId equals stf.Id
                    join user in DbContext.Users on stf.Id equals user.StaffId
                    where look.Aux == managerType.Id.ToString() && user.IsActive == true
                    select new StaffViewModel()
                    {
                        Id = stf.Id,
                        FName = stf.FName,
                        LName = stf.LName,
                        PersonalCode = stf.PersonalCode
                    }).ToList();

        return data;
    }

    #endregion
}