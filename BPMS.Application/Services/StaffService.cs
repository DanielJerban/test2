using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Dtos.Staff;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;
using System.Net;
using BPMS.Infrastructure.Services.SMS;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Services;

public class StaffService : IStaffService
{
    private readonly IDistributedCacheHelper _cacheHelper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemSettingService _systemSetting;
    private readonly IPasswordService _passwordService;
    private readonly ISendingSmsService _sendingSmsService;
    private readonly string _defaultImagePath = "/_image/profile_user.jpg";
    private readonly string _tncAutomationBaseUrl;

    public StaffService(IDistributedCacheHelper cacheHelper, IUnitOfWork unitOfWork, ISystemSettingService systemSetting, IPasswordService passwordService, IConfiguration configuration, ISendingSmsService sendingSmsService)
    {
        _unitOfWork = unitOfWork;
        _systemSetting = systemSetting;
        _passwordService = passwordService;
        _sendingSmsService = sendingSmsService;
        _cacheHelper = cacheHelper;
        _tncAutomationBaseUrl = configuration["Common:TorfehnegarAutomationAppBaseUrl"];
    }

    public string GetCompanyName(Guid? chartId)
    {
        if (chartId == null) return null;
        var id = chartId.Value;
        var chart = _unitOfWork.Charts.Get(id);
        if (chart.ChartLevel.Code == 3)
        {
            return chart.Title;
        }
        return GetCompanyName(chart.ParentId);
    }

    public List<Guid> GetRoleIds(Guid userId)
    {
        return _cacheHelper.GetOrSet(CacheKeyHelper.GetStaffCacheKey(userId), () => _unitOfWork.Staffs.GetRoleIds(userId), TimeSpan.FromDays(45));
    }

    public void ResetUserRoleCache(Guid userId)
    {
        _cacheHelper.Remove(CacheKeyHelper.GetStaffCacheKey(userId));
    }

    public UserDefultDto GetDefultUser()
    {
        return _cacheHelper.GetOrSet(CacheKeyHelper.StaffCacheKey(), () => _unitOfWork.Staffs.GetUserDefaultStaff(), TimeSpan.FromDays(30));
    }

    public Result EditStaff(EditStaffDto dto)
    {
        var result = new Result
        {
            IsValid = true,
            Message = "با موفقیت دخیره شد"
        };

        var currentStaff = _unitOfWork.Staffs.Single(c => c.PersonalCode == dto.PersonalCode);

        var ldapSetting = _systemSetting.GetLDAPSetting();
        bool isLdapEnabled = false;
        if (ldapSetting != null)
        {
            isLdapEnabled = ldapSetting.IsSyncerEnable;
        }

        if (isLdapEnabled && (string.IsNullOrEmpty(dto.LdapUsername) || string.IsNullOrEmpty(dto.LdapDomainName)))
        {
            result.IsValid = false;
            result.Message = "در صورت فعال بودن sync کاربران از طریق LDAP تکمیل Ldap User Name و Ldap Domain اجباری می باشد.";
            return result;
        }

        if (!IsEmailUnique(dto.Email, dto.PersonalCode))
        {
            result.IsValid = false;
            result.Message = "آدرس ایمیل وارد شده تکراری است";
            return result;
        }

        if (!IsPhoneNumberUnique(dto.PhoneNumber, dto.PersonalCode))
        {
            result.IsValid = false;
            result.Message = "شماره تماس وارد شده تکراری است";
            return result;
        }

        currentStaff.FName = dto.FName;
        currentStaff.LName = dto.LName;
        currentStaff.PhoneNumber = dto.PhoneNumber;
        currentStaff.StaffTypeId = dto.StaffTypeId;
        currentStaff.ImagePath = dto.ImagePath;
        currentStaff.Email = dto.Email;
        currentStaff.InsuranceNumber = dto.InsuranceNumber;
        currentStaff.IdNumber = dto.IdNumber;

        _unitOfWork.Staffs.AddOrUpdate(currentStaff);

        var user = _unitOfWork.Users.Single(c => c.StaffId == currentStaff.Id);
        user.LDAPDomainName = dto.LdapDomainName;
        user.LDAPUserName = dto.LdapUsername;

        _unitOfWork.Users.AddOrUpdate(user);

        _unitOfWork.Complete();

        return result;
    }

    public void CreateStaff(CreateStaffDto dto)
    {
        var newstaff = new Staff()
        {
            FName = dto.FName,
            LName = dto.LName,
            PhoneNumber = dto.PhoneNumber,
            PersonalCode = dto.PersonalCode,
            StaffTypeId = dto.StaffTypeId,
            ImagePath = dto.ImagePath,
            Email = dto.Email,
            InsuranceNumber = dto.InsuranceNumber,
            IdNumber = dto.IdNumber,
            EngTypeId = _unitOfWork.LookUps.Single(c => c.Code == 1 && c.Type == "EngType").Id

        };

        _unitOfWork.Staffs.Add(newstaff);


        var newUser = new User()
        {
            IsActive = true,
            Password = _passwordService.EncryptPassword(dto.PersonalCode, dto.Password),
            StaffId = newstaff.Id,
            UserName = dto.PersonalCode,
            LDAPDomainName = dto.LdapDomainName,
            LDAPUserName = dto.LdapUserName
        };

        var roleAccess = new RoleAccess()
        {
            RoleId = Guid.Parse("343D9133-3BAC-40F0-88FB-F1A3806BE6CC"),
            UserId = newUser.Id

        };

        _unitOfWork.RoleAccesses.Add(roleAccess);
        _unitOfWork.Users.Add(newUser);

        _unitOfWork.Complete();

        _sendingSmsService.SendSms(dto.PhoneNumber, $"به سامانه BPMS خوش آمدید. \n رمز ورود شما: {dto.Password}");
    }

    public void CreateStaffFromActiveDirectory(ActiveDirectoryUserDto dto)
    {
        if (dto.LdapUserName != null)
        {
            var newstaff = new Staff()
            {
                EngTypeId = _unitOfWork.LookUps.GetLookUpByTypeAndCode(1, "EngType"),
                FName = !string.IsNullOrEmpty(dto.FName) ? dto.FName : dto.LdapUserName,
                LName = !string.IsNullOrEmpty(dto.LName) ? dto.LName : dto.LdapUserName,
                Email = !string.IsNullOrEmpty(dto.Email) ? dto.Email : null,
                ImagePath = !string.IsNullOrEmpty(dto.ImagePath) ? dto.ImagePath : _defaultImagePath,
                StaffTypeId = _unitOfWork.LookUps.GetLookUpByTypeAndCode(3, "StaffType"),
                PersonalCode = !string.IsNullOrEmpty(dto.PersonalCode) ? dto.PersonalCode : dto.LdapUserName,
                LocalPhone = !string.IsNullOrEmpty(dto.LocalPhone) && dto.LocalPhone.Length < 10 ? dto.LocalPhone : null,
                PhoneNumber = !string.IsNullOrEmpty(dto.PhoneNumber) ? dto.PhoneNumber : null,
                InsuranceNumber = !string.IsNullOrEmpty(dto.InsuranceNumber) ? dto.InsuranceNumber : null,
                IdNumber = !string.IsNullOrEmpty(dto.IdNumber) ? dto.IdNumber : null,
            };

            _unitOfWork.Staffs.Add(newstaff);

            var user = new User()
            {
                IsActive = true,
                Password = _passwordService.EncryptPassword(dto.LdapUserName, dto.Password != "" ? dto.Password : dto.LdapUserName),
                StaffId = newstaff.Id,
                UserName = dto.LdapUserName,
                LDAPUserName = dto.LdapUserName,
            };

            var roleAccess = new RoleAccess()
            {
                UserId = user.Id,
                RoleId = Guid.Parse("343D9133-3BAC-40F0-88FB-F1A3806BE6CC")
            };
            _unitOfWork.RoleAccesses.Add(roleAccess);
            _unitOfWork.Users.Add(user);

            _unitOfWork.Complete();
        }
    }


    public void AddOrUpdateStaffFromAutomation(StaffPhpDto dto, string webRootPath)
    {
        var correctPersonalCode = dto.PersonalCode[0];
        switch (dto.Action)
        {
            case "Insert":
            case "Update":
            {
                AddOrUpdateAutomationStaff(dto, correctPersonalCode, webRootPath);
                break;
            }
            case "Delete":
            {
                var staff = _unitOfWork.Staffs.SingleOrDefault(s => s.PersonalCode == correctPersonalCode);
                DeleteInAutomation(staff);
                break;
            }

            default:
                throw new ArgumentException("خطا در ارسال نوع درخواست");
        }
    }

    private void AddOrUpdateAutomationStaff(StaffPhpDto dto, string personalCode, string webRootPath)
    {
        var staff = _unitOfWork.Staffs.SingleOrDefault(c => c.PersonalCode == personalCode);
        if (staff == null)
        {
            // Create New
            CreateInAutomation(dto, personalCode, webRootPath);
        }
        else
        {
            // Update
            UpdateInAutomation(dto, staff, webRootPath);
        }
    }

    private void CreateInAutomation(StaffPhpDto dto, string correctPersonalCode, string webRootPath)
    {
        int? employmentDate = null;
        if (!string.IsNullOrWhiteSpace(dto.EmploymentDate))
        {
            employmentDate = int.Parse(dto.EmploymentDate.Replace("-", ""));
        }
        int? agreementEndDate = null;
        if (!string.IsNullOrWhiteSpace(dto.AgreementEndDate))
        {
            agreementEndDate = int.Parse(dto.AgreementEndDate.Replace("-", ""));
        }
        var eng = _unitOfWork.LookUps.Single(a => a.Type == "EngType" && a.Code == 1).Id;
        if (dto.EngType == "delete")
        {
            eng = _unitOfWork.LookUps.Single(a => a.Type == "EngType" && a.Code == 2).Id;
        }

        Guid? building = null;
        if (!string.IsNullOrWhiteSpace(dto.Building) && dto.Building != "-1")
        {
            int code;
            switch (dto.Building)
            {
                case "ظفر": code = 1; break;
                case "البرز": code = 2; break;
                case "مطهری": code = 3; break;
                case "اطلسی": code = 4; break;
                default: throw new ArgumentException("فرمت ساختمان وارد شده صحیح نمی باشد.");
            }

            var build = _unitOfWork.LookUps.Single(l => l.Type == "Building" && l.Code == code);
            if (build == null)
            {
                throw new ArgumentException("ساختمان وارد شده صحیح نمی باشد.");
            }

            building = build.Id;
        }

        var gender = dto.Gender == "مذکر" ? (byte)1 : (byte)2;
        dto.UserName ??= correctPersonalCode;

        if (string.IsNullOrEmpty(dto.ImagePath))
        {
            dto.ImagePath = _defaultImagePath;
        }

        var staff = new Staff()
        {
            EngTypeId = eng,
            FName = dto.FName,
            LName = dto.LName,
            ImagePath = dto.ImagePath,
            StaffTypeId = _unitOfWork.LookUps.Single(c => c.Type == "StaffType" && c.Code == 3).Id,
            PhoneNumber = dto.PhoneNumber,
            PersonalCode = correctPersonalCode,
            IdNumber = dto.IdNumber,
            InsuranceNumber = dto.InsuranceNumber,
            EmploymentDate = employmentDate,
            AgreementEndDate = agreementEndDate,
            BuildingId = building,
            LocalPhone = dto.LocalPhone,
            Gender = gender
        };
        _unitOfWork.Staffs.Add(staff);

        var user = new User
        {
            IsActive = true,
            Password = _passwordService.EncryptPassword(dto.UserName, dto.UserName),
            UserName = dto.UserName,
            StaffId = staff.Id
        };

        _unitOfWork.Users.Add(user);

        var roleAccess = new RoleAccess()
        {
            UserId = user.Id,
            RoleId = Guid.Parse("343D9133-3BAC-40F0-88FB-F1A3806BE6CC")
        };

        _unitOfWork.RoleAccesses.Add(roleAccess);

        if (!dto.ImagePath.Contains("_image"))
        {
            DownloadImageProfileFromTorfehnegarAutomationApp(correctPersonalCode, dto.ImagePath,webRootPath);
        }

        _unitOfWork.Complete();
    }

    private void UpdateInAutomation(StaffPhpDto dto, Staff staff, string webRootPath)
    {
        staff.LocalPhone = !string.IsNullOrWhiteSpace(dto.LocalPhone) ? dto.LocalPhone : "";

        if (!string.IsNullOrWhiteSpace(dto.Building) && dto.Building != "-1")
        {
            int code;
            switch (dto.Building)
            {
                case "ظفر": code = 1; break;
                case "البرز": code = 2; break;
                case "مطهری": code = 3; break;
                case "اطلسی": code = 4; break;
                default: throw new ArgumentException("فرمت ساختمان وارد شده صحیح نمی باشد.");
            }

            var build = _unitOfWork.LookUps.SingleOrDefault(l => l.Type == "Building" && l.Code == code);
            if (build == null)
            {
                throw new ArgumentException("ساختمان وارد شده صحیح نمی باشد.");
            }

            staff.BuildingId = build.Id;
        }
        else
        {
            staff.BuildingId = null;
        }

        if (!string.IsNullOrWhiteSpace(dto.IsActive))
        {
            if (dto.IsActive != "active")
            {
                var flow = _unitOfWork.Flows.Where(f => f.StaffId == staff.Id && f.LookUpFlowStatus.Code == 1);
                if (flow.Any())
                {
                    throw new ArgumentException("پرسنل " + staff.PersonalCode + " به دلیل داشتن درخواست دریافت شده اقدام نشده قابلیت غیر فعال سازی ندارد.");
                }
                var wfd = _unitOfWork.WorkflowDetails.Where(w => w.StaffId == staff.Id && w.WorkFlow.IsActive);
                if (wfd.Any())
                {
                    throw new ArgumentException("پرسنل " + staff.PersonalCode + " به دلیل تعریف در مدل bpmn قابلیت غیر فعال سازی ندارد.");
                }
                var req = _unitOfWork.Request.Where(r => r.StaffId == staff.Id && (r.RequestStatus.Code == 1 || r.RequestStatus.Code == 2));
                if (req.Any())
                {
                    throw new ArgumentException("پرسنل " + staff.PersonalCode + " به دلیل داشتن درخواست ارسال شده اقدام نشده یا در حال اقدام قابلیت غیر فعال سازی ندارد.");
                }
            }

            var user = _unitOfWork.Users.Where(c => c.StaffId == staff.Id).FirstOrDefault();
            if (user != null)
            {
                user.IsActive = true;
                _unitOfWork.Users.AddOrUpdate(user);
            }
        }
        if (!string.IsNullOrWhiteSpace(dto.Gender))
        {
            staff.Gender = dto.Gender == "مذکر" ? (byte)1 : (byte)2;
        }
        if (!string.IsNullOrWhiteSpace(dto.FName))
        {
            staff.FName = dto.FName;
        }

        if (!string.IsNullOrWhiteSpace(dto.LName))
        {
            staff.LName = dto.LName;
        }

        if (!string.IsNullOrWhiteSpace(dto.EngType))
        {
            var eng = _unitOfWork.LookUps.Single(c => c.Type == "EngType" && c.Code == 1).Id;
            if (dto.EngType == "delete")
            {
                var flow = _unitOfWork.Flows.Where(f => f.StaffId == staff.Id && f.LookUpFlowStatus.Code == 1);
                if (flow.Any())
                {
                    throw new ArgumentException("پرسنل " + staff.PersonalCode + " به دلیل داشتن درخواست دریافت شده اقدام نشده قابلیت غیر فعال سازی ندارد.");
                }
                var wfd = _unitOfWork.WorkflowDetails.Where(w => w.StaffId == staff.Id && w.WorkFlow.IsActive);
                if (wfd.Any())
                {
                    throw new ArgumentException("پرسنل " + staff.PersonalCode + " به دلیل تعریف در مدل bpmn قابلیت غیر فعال سازی ندارد.");
                }
                var req = _unitOfWork.Request.Where(r => r.StaffId == staff.Id && (r.RequestStatus.Code == 1 || r.RequestStatus.Code == 2));
                if (req.Any())
                {
                    throw new ArgumentException("پرسنل " + staff.PersonalCode + " به دلیل داشتن درخواست ارسال شده اقدام نشده یا در حال اقدام قابلیت غیر فعال سازی ندارد.");
                }
                eng = _unitOfWork.LookUps.Single(c => c.Type == "EngType" && c.Code == 2).Id;
            }

            staff.EngTypeId = eng;
        }

        if (!string.IsNullOrEmpty(dto.ImagePath))
        {
            staff.ImagePath = dto.ImagePath;
            if (!dto.ImagePath.Contains("_image"))
            {
                DeleteProfileImage(staff.PersonalCode, webRootPath);
                DownloadImageProfileFromTorfehnegarAutomationApp(staff.PersonalCode, dto.ImagePath, webRootPath);
            }
        }

        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            staff.PhoneNumber = dto.PhoneNumber;
        }

        if (!string.IsNullOrWhiteSpace(dto.IdNumber))
        {
            staff.IdNumber = dto.IdNumber;
        }

        if (!string.IsNullOrWhiteSpace(dto.InsuranceNumber))
        {
            staff.InsuranceNumber = dto.InsuranceNumber;
        }

        if (!string.IsNullOrWhiteSpace(dto.EmploymentDate))
        {
            var employmentDate = int.Parse(dto.EmploymentDate.Replace("-", ""));
            staff.EmploymentDate = employmentDate;
        }
        if (!string.IsNullOrWhiteSpace(dto.AgreementEndDate))
        {
            var agreementEndDate = int.Parse(dto.AgreementEndDate.Replace("-", ""));
            staff.AgreementEndDate = agreementEndDate;
        }

        _unitOfWork.Staffs.AddOrUpdate(staff);
        _unitOfWork.Complete();
    }

    private void DeleteInAutomation(Staff person)
    {
        if (person == null)
        {
            return;
        }
        var flow = _unitOfWork.Flows.Where(f => f.StaffId == person.Id && f.LookUpFlowStatus.Code == 1);
        if (flow.Any())
        {
            throw new ArgumentException("پرسنل " + person.PersonalCode + " به دلیل داشتن درخواست دریافت شده اقدام نشده قابلیت غیر فعال سازی ندارد.");
        }
        var wfd = _unitOfWork.WorkflowDetails.Where(w => w.StaffId == person.Id && w.WorkFlow.IsActive);
        if (wfd.Any())
        {
            throw new ArgumentException("پرسنل " + person.PersonalCode + " به دلیل تعریف در مدل bpmn قابلیت غیر فعال سازی ندارد.");
        }
        var req = _unitOfWork.Request.Where(r => r.StaffId == person.Id && (r.RequestStatus.Code == 1 || r.RequestStatus.Code == 2));
        if (req.Any())
        {
            throw new ArgumentException("پرسنل " + person.PersonalCode + " به دلیل داشتن درخواست ارسال شده اقدام نشده یا در حال اقدام قابلیت غیر فعال سازی ندارد.");
        }
        person.Users.FirstOrDefault().IsActive = false;
        person.EngTypeId = Guid.Parse("B74ADD65-BF94-4919-A5A6-1E511936C8CD");
        person.Users.First().SerialNumber = Guid.NewGuid();
    }

    private bool IsEmailUnique(string email, string currentStaffPersonalCode)
    {
        return !_unitOfWork.Staffs.Any(c => c.Email == email.Replace(" ", "") && c.PersonalCode != currentStaffPersonalCode);
    }

    private bool IsPhoneNumberUnique(string phoneNumber, string currentStaffPersonalCode)
    {
        return !_unitOfWork.Staffs.Any(c => c.PhoneNumber == phoneNumber && c.PersonalCode != currentStaffPersonalCode);
    }

    private void DownloadImageProfileFromTorfehnegarAutomationApp(string personalCode, string sentImagePath, string webRootPath)
    {
        string profileImagePhysicalSavePath = GetImageProfilePhysicalPath(personalCode, webRootPath);

        using (var client = new WebClient())
        {
            var uri = new Uri(_tncAutomationBaseUrl + sentImagePath);

            client.DownloadFile(uri, profileImagePhysicalSavePath);
        }
    }

    private void DeleteProfileImage(string personalCode, string wenRootPath)
    {
        string profileImagePhysicalSavePath = GetImageProfilePhysicalPath(personalCode, wenRootPath);

        if (File.Exists(profileImagePhysicalSavePath))
        {
            File.Delete(profileImagePhysicalSavePath);
        }
    }

    private string GetImageProfilePhysicalPath(string personalCode, string wenRootPath)
    {
        string basePath = "Images/upload/personnelImages/";
        string basePhysicalPath = Path.Combine(wenRootPath, basePath);

        if (!Directory.Exists(basePhysicalPath))
        {
            Directory.CreateDirectory(basePhysicalPath);
        }

        string profileImageRelationalSavePath = $"{basePath}{personalCode}.jpg";
        string profileImagePhysicalSavePath = Path.Combine(wenRootPath, profileImageRelationalSavePath);
        return profileImagePhysicalSavePath;
    }

    public IEnumerable<StaffRequestsDto> GetRequestCountByStaff(string userName, string startDate, string endDate)
    {
        try
        {
            if (string.IsNullOrEmpty(startDate))
                startDate = "1398/01/01";

            if (string.IsNullOrEmpty(endDate))
                endDate = Util.ConvertMiladiToShamsi(DateTime.Now).ToString().Substring(0, 10);


            var stDate = int.Parse(startDate.Replace("/", ""));
            var enDate = int.Parse(endDate.Replace("/", ""));

            var staffId = _unitOfWork.Users.Where(u => u.UserName == userName).Single().StaffId;
            return _unitOfWork.Staffs.GetRequestCountByStaff(staffId, stDate, enDate);
        }

        catch (InvalidOperationException)
        {

            throw new InvalidOperationException("کاربر مورد نظر یافت نشد");
        }

        catch (FormatException e)
        {
            throw new FormatException("فرمت اطلاعات ورودی نادرست است");
        }

    }
}