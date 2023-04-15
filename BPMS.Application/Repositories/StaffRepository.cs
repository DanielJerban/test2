using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Constants;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Application.Repositories;

public class StaffRepository : Repository<Staff>, IStaffsRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    public StaffRepository(BpmsDbContext context, IServiceProvider serviceProvider, IConfiguration configuration) : base(context)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public BpmsDbContext DbContext => Context;

    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();

    public IChartRepository ChartRepository => _serviceProvider.GetRequiredService<IChartRepository>();


    public IEnumerable<StaffViewModel> GetAllStaffForstaffViewMOdel()
    {
        var StaffInStaffViewModel = DbContext.Staffs.Select(p => new StaffViewModel()
        {
            Id = p.Id,
            FullName = p.FName + " " + p.LName,
            PhoneNumber = p.PhoneNumber,
            PersonalCode = p.PersonalCode,
            FName = p.FName,
            LName = p.LName
        });
        return StaffInStaffViewModel;
    }
    public List<string> GetRoles(Guid userId)
    {

        var roles = DbContext.RoleAccesses.Where(i => i.UserId == userId).Select(i => i.RoleId);
        return DbContext.Roles.Where(i => roles.Contains(i.Id)).Select(n => n.Name).ToList();

    }
    public List<Guid> GetRoleIds(Guid userId)
    {
        var roles = DbContext.RoleAccesses.Where(i => i.UserId == userId).Select(i => i.RoleId);
        return DbContext.Roles.Where(i => roles.Contains(i.Id)).Select(n => n.Id).ToList();
    }
    public IEnumerable<StaffDto> GetAllActiveStaff()
    {

        var result = DbContext.Staffs
            .Where(d => d.EngType.Code == 1 && d.Users.FirstOrDefault().IsActive)
            .Select(s => new StaffDto()
            {
                Id = s.Id,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                FName = s.FName,
                LName = s.LName,
                FullName = s.FName + " " + s.LName,
                Title = s.FName + " " + s.LName,
                PersonalCode = s.PersonalCode,
                ImagePath = s.ImagePath,
                StaffTypeId = s.StaffTypeId
            }).OrderBy(d => d.FName);
        return result;

    }

    /// <summary>
    /// Get Subusers id of the current users
    /// </summary>
    /// <returns></returns>
    public List<StaffDto> GetAdminSubStaffs(Guid staffId)
    {
        var Staffs = new List<StaffDto>();
        var query = Context.OrganiztionInfos.FirstOrDefault(w => w.StaffId == staffId);
        if (query != null)
        {

            var checkIf = Context.LookUps.FirstOrDefault(r => r.Id.ToString() == query.OrganiztionPost.Aux);
            if (checkIf.Aux2 != 1.ToString())
            {
                return new List<StaffDto>();
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
                foreach (var thisStaffId in chartUsers)
                {
                    var staffDto = DbContext.Staffs.Include("Users").Where(c => c.Id == thisStaffId && c.Users.FirstOrDefault().IsActive).Select(s => new StaffDto()
                    {
                        Id = s.Id,
                        PhoneNumber = s.PhoneNumber,
                        Email = s.Email,
                        FName = s.FName,
                        LName = s.LName,
                        FullName = s.FName + " " + s.LName,
                        Title = s.FName + " " + s.LName,
                        PersonalCode = s.PersonalCode,
                        ImagePath = s.ImagePath,
                        StaffTypeId = s.StaffTypeId
                    }).SingleOrDefault();

                    if (staffDto != null)
                    {
                        Staffs.Add(staffDto);
                    }
                }
                subUsersId.AddRange(chartUsers);
            }
        }

        return Staffs;
    }

    public void CheckStaffFields(string email, string personalCode, string phone)
    {
        if (DbContext.Staffs.Any(s => s.PersonalCode == personalCode))
        {
            throw new ArgumentException("نام کاربری وارد شده تکراری است");
        }
        if (DbContext.Staffs.Any(s => s.PhoneNumber == phone))
        {
            throw new ArgumentException("شماره تماس وارد شده تکراری است");
        }
        if (DbContext.Staffs.Any(s => s.Email == email))
        {
            throw new ArgumentException("آدرس ایمیل وارد شده تکراری است");
        }

    }

    public Staff? GetCurrentEditedStaff(string personalCode)
    {
        return DbContext.Staffs.FirstOrDefault(staff => staff.PersonalCode == personalCode);

    }

    public Staff GetStaffByPersonelCode(string personelCode)
    {
        var eng = Guid.Parse("016bd9b3-2ead-4e51-81a7-243b65110381");
        var staff = DbContext.Staffs.FirstOrDefault(u => u.PersonalCode == personelCode && u.EngTypeId == eng);
        return staff;
    }

    public IEnumerable<UserViewModel> GetStaffsHaveEmail()
    {
        return DbContext.Users.Where(u => u.IsActive && u.Staff.Email != null).ToList().Select(user => new UserViewModel
        {
            StaffId = user.StaffId,
            PersonelCode = user.Staff.PersonalCode,
            FullName = user.Staff.FName + " " + user.Staff.LName,
            Email = user.Staff.Email

        });
    }

    public bool EmailExistance(string email, Guid staffId)
    {
        if (string.IsNullOrEmpty(email)) return false;
        return DbContext.Staffs.Any(i => i.Email == email && i.Id != staffId);
    }
    public IEnumerable<UserViewModel> GetStaffsHavePhoneNumber()
    {
        return DbContext.Users.Where(u => u.IsActive && u.Staff.PhoneNumber != null && u.Staff.PhoneNumber.Trim() != "").ToList().Select(user => new UserViewModel
        {
            StaffId = user.StaffId,
            PersonelCode = user.Staff.PersonalCode,
            FullName = user.Staff.FName + " " + user.Staff.LName,
            PhoneNumber = user.Staff.PhoneNumber

        });
    }

    public StaffViewModel GetStaffDetail(Guid id)
    {
        var staff = DbContext.Staffs.Find(id);
        if (staff == null)
        {
            throw new ArgumentException("این شخص موجود نمی باشد.");
        }

        var org = staff.OrganiztionInfos.FirstOrDefault(o => o.IsActive && o.Priority);
        if (org == null)
            throw new ArgumentException("این شخص پست اصلی فعالی ندارد.");
        var postId = Guid.Parse(org.OrganiztionPost.Aux);
        var posttype = DbContext.LookUps.Find(postId);
        return new StaffViewModel()
        {
            Id = staff.Id,
            PhoneNumber = staff.PhoneNumber,
            FullName = staff.FullName,
            FName = staff.FName,
            InsuranceNumber = staff.InsuranceNumber,
            StaffType = staff.StaffType.Title,
            IdNumber = staff.IdNumber,
            EmploymentDate = staff.EmploymentDate,
            PersonalCode = staff.PersonalCode,
            EmailAddress = staff.Email,
            LName = staff.LName,
            ImagePath = staff.ImagePath,
            LocalPhone = staff.BuildingId != null ? staff.Building.Aux2 + "-" + staff.LocalPhone : staff.LocalPhone,
            Building = staff.BuildingId != null ? staff.Building.Title : null,
            PostTitle = org.OrganiztionPost.Title,
            PostType = posttype.Title,
            ChartTitle = org.Chart.Title,
            CompanyName = GetCompanyName(org.ChartId)
        };
    }

    public IEnumerable<StaffDto> GetAllStaff()
    {
        var result = DbContext.Staffs
            .Select(s => new StaffDto()
            {
                Id = s.Id,
                PhoneNumber = s.PhoneNumber,
                Email = s.Email,
                FName = s.FName,
                LName = s.LName,
                FullName = s.FName + " " + s.LName,
                Title = s.FName + " " + s.LName,
                PersonalCode = s.PersonalCode,
                ImagePath = s.ImagePath,
                StaffTypeId = s.StaffTypeId
            }).OrderBy(d => d.FName);
        return result;
    }

    public UserDefultDto GetUserDefaultStaff()
    {
        return DbContext.Staffs.Where(t => t.PersonalCode == SystemConstant.SystemUser)
            .Select(s => new UserDefultDto()
            {
                StaffId = s.Id,
                PersonalCode = s.PersonalCode
            }).FirstOrDefault();
    }

    public IEnumerable<DynamicViewModel> GetAllFileUpload(Guid? id, string webRootPath)
    {
        var path = Path.Combine(webRootPath, "Images/upload/file/" + id + "/");
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        var files = Directory.GetFiles(path);
        var list = new List<DynamicViewModel>();
        foreach (var file in files)
        {
            list.Add(new DynamicViewModel() { Title = Path.GetFileName(file) });
        }

        return list;
    }

    public string Download(Guid id, string name, string webRootPath)
    {
        var list = new Dictionary<string, string>();
        var path = "Images/upload/file/" + id + "/";
        var mapPath = Path.Combine(webRootPath, path);
        var files = Directory.GetFiles(mapPath);
        foreach (var file in files)
        {
            list.Add(Path.GetFileName(file), file);
        }
        if (list.Any(a => a.Key == name))
        {
            return path + name;
        }
        return null;
    }

    public void FilePerPerson(Guid id, IFormFile file, string webRootPath)
    {
            string filePath = _configuration["Common:FilePath:PersonFilePath"];
            if (file.Length > 0)
            {
                if (file.Length > 5 * 1024 * 1024)
                    throw new ArgumentException("حجم فایل آپلود شده نمیتوان بیش از 50 مگابایت باشد.");

                var fileName = file.FileName;

                var targetFolder = Path.Combine(webRootPath, filePath + id + "/");

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                var path = Path.Combine(targetFolder, fileName);
                using Stream fileStream = new FileStream(path, FileMode.Create);
                file.CopyTo(fileStream);
            }
            else
                throw new ArgumentException("فایل مورد نظر آپلود نشد");
    }

    public IEnumerable<StaffDto> GetStaffsByParentId(bool isActive)
    {
        Guid staffId = Guid.Parse("b339cc48-d051-4e54-9916-aae142e2ca0c");

        //var staffId = GlobalVariables.User.StaffId;
        var query = Context.OrganiztionInfos.FirstOrDefault(w => w.StaffId == staffId);
        var checkIf = Context.LookUps.FirstOrDefault(r => r.Id.ToString() == query.OrganiztionPost.Aux);
        if (checkIf.Aux2 != 1.ToString())
            return new List<StaffDto>();
        var chartIds = new List<Guid>();
        var Staffs = new List<StaffDto>();
        chartIds.Add(query.Chart.Id);
        var loop = true;
        do
        {
            int count = 0;
            foreach (var item in chartIds)
            {
                foreach (var chart in Context.Charts)
                {
                    if (chart.ParentId == item)
                    {
                        if (!chartIds.Contains(chart.Id))
                        {
                            chartIds.Add(chart.Id);
                            count++;
                        }
                    }
                }
                if (count > 0)
                    break;
            }
            if (count == 0)
                loop = false;
        } while (loop);

        if (isActive)
        {
            foreach (var item in chartIds)
            {

                foreach (var staff in Context.OrganiztionInfos
                             .Where(w => w.ChartId == item))
                {
                    Staffs.Add(new StaffDto()
                    {
                        Id = staff.Staff.Id,
                        FullName = staff.Staff.FullName
                    });
                }
            }
        }

        else
        {
            foreach (var item in chartIds)
            {

                foreach (var staff in Context.OrganiztionInfos
                             .Where(d => d.ChartId == item && d.Staff.EngType.Code == 1 && d.Staff.Users.FirstOrDefault().IsActive == true))
                {
                    Staffs.Add(new StaffDto()
                    {
                        Id = staff.Staff.Id,
                        FullName = staff.Staff.FullName
                    });
                }
            }
        }
        return Staffs.ToList();
    }

    /// <summary>
    /// دریافت شناسه 
    /// </summary>
    /// <param name="staffId"></param>
    /// <returns></returns>
    public List<HeadStaffDto> GetHeadersOfThisStaff(Guid staffId)
    {
        // code = 3 => --کارشناس مسئول
        // code = 1 => --مدیر
        // code = 5 => --مدیر عامل
        var lookUps = DbContext.LookUps
            .Where(c => c.Type == "OrganizationPostType" && (c.Code == 3 || c.Code == 1 || c.Code == 5)).ToList();

        var organizationInfo = DbContext.OrganiztionInfos.SingleOrDefault(c => c.StaffId == staffId);

        var charts = GetChartsParentNode(organizationInfo.ChartId);

        List<HeadStaffDto> headersOfThisStaff = new List<HeadStaffDto>();

        foreach (var chart in charts)
        {
            foreach (var lookUp in lookUps)
            {
                var query = (from orgInfo in DbContext.OrganiztionInfos
                             join look in DbContext.LookUps on orgInfo.OrganiztionPostId equals look.Id
                             join stf in DbContext.Staffs on orgInfo.StaffId equals stf.Id
                             join user in DbContext.Users on stf.Id equals user.StaffId
                             where orgInfo.ChartId == chart.Id && look.Aux == lookUp.Id.ToString() && user.IsActive == true
                             select orgInfo);

                var data = query.ToList().FirstOrDefault();
                if (data != null)
                {
                    headersOfThisStaff.Add(new HeadStaffDto()
                    {
                        Id = data.StaffId,
                        Code = lookUp.Code
                    });
                }
            }
        }

        return headersOfThisStaff;
    }

    public Guid? GetStaffMainOrganizationInfoId(Guid staffId)
    {
        var query = (from staff in Context.Staffs
                     join orgInfo in Context.OrganiztionInfos on staff.Id equals orgInfo.StaffId
                     join lookUp in Context.LookUps on orgInfo.OrganiztionPostId equals lookUp.Id
                     where orgInfo.Priority && orgInfo.IsActive && staff.Id == staffId
                     select lookUp.Id).FirstOrDefault();

        return query;
    }

    public string GetCompanyName(Guid? chartId)
    {
        if (chartId == null) return null;
        var id = chartId.Value;
        var chart = Context.Charts.Single(c => c.Id == id);
        if (chart.ChartLevel.Code == 3)
        {
            return chart.Title;
        }
        return GetCompanyName(chart.ParentId);
    }

    public Staff GetStaffById(Guid staffId)
    {
        return DbContext.Staffs.FirstOrDefault(d => d.Id == staffId);
    }
    public IQueryable<Guid> GetStaffIdsByWorkFlowDetail(WorkFlowDetail nextWorkFlowDetail)
    {
        return from staff in Context.Staffs
               join assingnment in Context.Assingnments on staff.Id equals assingnment.StaffId
               join groupResponse in Context.LookUps on assingnment.ResponseTypeGroupId equals groupResponse.Id
               where groupResponse.Id == nextWorkFlowDetail.ResponseGroupId
               select staff.Id;
    }

    public IEnumerable<StaffRequestsDto> GetRequestCountByStaff(Guid staffId, int startDate, int endDate)
    {

        var requestsList = from request in Context.Requests
                           join workflow in Context.Workflows on request.WorkFlowId equals workflow.Id
                           where request.StaffId == staffId && request.RegisterDate >= startDate && request.RegisterDate <= endDate
                           group new { workflow, request } by new { workflow.RequestTypeId, request.RequestStatusId } into grp

                           select new StaffRequestsDto
                           {
                               WorkFlowTitle = Context.LookUps.Where(l => l.Id == grp.Key.RequestTypeId).FirstOrDefault().Title,
                               RequestStatusTitle = Context.LookUps.Where(l => l.Id == grp.Key.RequestStatusId).FirstOrDefault().Title,
                               RequestCount = grp.Count()
                           };

        return requestsList.ToList();
    }

    #region Helpers

    public List<Chart> GetChartsParentNode(Guid chartId)
    {
        var ch = new List<Chart>();
        FindParentFromChartId(chartId, ch);
        return ch;
    }

    public void FindParentFromChartId(Guid? chartId, List<Chart> chartList)
    {
        var chart = DbContext.Charts.FirstOrDefault(c => c.Id == chartId && c.IsActive);
        if (chart != null)
        {
            chartList.Add(chart);
            FindParentFromChartId(chart.ParentId, chartList);
        }
    }

    public List<StaffViewModel> GetManagers()
    {
        var managerType = DbContext.LookUps.SingleOrDefault(c => c.Type == "OrganizationPostType");

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