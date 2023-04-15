using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Common.ViewModels.Global;
using BPMS.Domain.Entities;
using BPMS.Infrastructure;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class UserService : IUserService
{
    private readonly IDistributedCacheHelper _cacheHelper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStaffService _staffService;

    public UserService(IUnitOfWork unitOfWork, IDistributedCacheHelper cacheHelper, IStaffService staffService)
    {
        _unitOfWork = unitOfWork;
        _cacheHelper = cacheHelper;
        _staffService = staffService;
    }


    public UserBaseInfoCacheDTO GetUser(string userName)
    {
        return _cacheHelper.GetOrSet
            (CacheKeyHelper.GetUserBaseInfoCacheKey(userName), () => GetUserFromDatabase(userName), TimeSpan.FromDays(45));
    }

    public GetUserFullDataByUsernameDTO GetUserFullDataByUsername(string userName)
    {
        return _cacheHelper.GetOrSet(CacheKeyHelper.GetUserCacheKey(userName), () => GetUserFullDataByUsernameFromDatabase(userName), TimeSpan.FromMinutes(5));
    }

    private GetUserFullDataByUsernameDTO GetUserFullDataByUsernameFromDatabase(string userName)
    {
        GetUserFullDataByUsernameDTO user = _unitOfWork.Users.Where(c => c.UserName == userName).Select(p => new GetUserFullDataByUsernameDTO()
        {
            UserName = p.UserName,
            GoogleAuthKey = p.GoogleAuthKey,
            Id = p.Id,
            IsActive = p.IsActive,
            LDAPDomainName = p.LDAPDomainName,
            LDAPUserName = p.LDAPUserName,
            Password = p.Password,
            SerialNumber = p.SerialNumber,
            StaffId = p.StaffId,
            TwoStepVerification = p.TwoStepVerification,
            TwoStepVerificationByEmail = p.TwoStepVerificationByEmail,
            TwoStepVerificationByGoogleAuthenticator = p.TwoStepVerificationByGoogleAuthenticator,
            Staff = new GetUserFullDataByUsernameStaffDTO()
            {
                AgreementEndDate = p.Staff.AgreementEndDate,
                BuildingId = p.Staff.BuildingId,
                Email = p.Staff.Email,
                EmploymentDate = p.Staff.EmploymentDate,
                EngTypeId = p.Staff.EngTypeId,
                FName = p.Staff.FName,
                Gender = p.Staff.Gender,
                Id = p.Staff.Id,
                IdNumber = p.Staff.IdNumber,
                ImagePath = p.Staff.ImagePath,
                InsuranceNumber = p.Staff.InsuranceNumber,
                LName = p.Staff.LName,
                LocalPhone = p.Staff.LocalPhone,
                PersonalCode = p.Staff.PersonalCode,
                PhoneNumber = p.Staff.PhoneNumber,
                StaffTypeId = p.Staff.StaffTypeId
            }
        }).FirstOrDefault();

        return user;
    }

    public void ResetUserFullDataFromCache(string username)
    {
        _cacheHelper.Remove(CacheKeyHelper.GetUserCacheKey(username));
    }


    public Result UpdateTwoStepVerification(User user, TwoStepVerificationViewModel model)
    {
        var result = new Result() { IsValid = false, Message = "دخیره نشد" };
        if (model.TwoStepVerificationByGoogleAuthenticator && string.IsNullOrEmpty(user.GoogleAuthKey))
        {
            result.IsValid = false;
            result.Message = "برای فعال کردن احراز هویت گوگل ، لطفا ابتدا QR کد بگیرید";
            return result;
        }

        var personToEdit = _unitOfWork.Staffs.Where(i => i.Id == user.StaffId).FirstOrDefault();
        if (personToEdit != null)
        {
            var update = UpdateTwoStepVerificationStaff(model, user, personToEdit);
            if (update)
            {
                result.IsValid = true;
                result.Message = "با موفقیت دخیره شد";
            }
        }

        return result;
    }

    public Result SetGoogleAuthKey(string email, string googleAuthKey)
    {
        var result = new Result() { IsValid = false, Message = "دخیره نشد" };
        var personToEdit = _unitOfWork.Staffs.Where(i => i.Email == email).Single();
        if (personToEdit != null)
        {
            var user = _unitOfWork.Users.Find(i => i.StaffId == personToEdit.Id).Single();
            user.GoogleAuthKey = googleAuthKey;
            _unitOfWork.Users.AddOrUpdate(user);
            _unitOfWork.Complete();
            result.IsValid = true;
            result.Message = "با موفقیت دخیره شد";
        }
        else
        {
            result.IsValid = true;
            result.Message = "پرسنلی با این ایمیل پیدا نشد";
        }

        return result;
    }

    public void ModifyRoleMapUser(List<User> users, Guid roleId)
    {
        var userAdd = from user in users
                      where !_unitOfWork.RoleAccesses.Any(raa => raa.UserId == user.Id && raa.RoleId == roleId)
                      select user.Id;

        foreach (var item in userAdd.ToList())
        {
            var roleAccess = new RoleAccess()
            {
                RoleId = roleId,
                UserId = item
            };

            _unitOfWork.RoleAccesses.AddUserToRoleAccess(roleAccess);
            _staffService.ResetUserRoleCache(item);
        }

        var userRemove = _unitOfWork.RoleAccesses.Where(r => r.RoleId == roleId).ToList()
            .Where(user => users.All(u => u.Id != user.UserId)).ToList();

        if (userRemove.Any())
        {
            foreach (var roleAccess in userRemove.DistinctBy(d => d.UserId))
            {
                roleAccess.User.SerialNumber = Guid.NewGuid();
                _staffService.ResetUserRoleCache(roleAccess.UserId);
            }
            _unitOfWork.RoleAccesses.RemoveUserFromRoleAccess(userRemove);
        }

    }

    private bool UpdateTwoStepVerificationStaff(TwoStepVerificationViewModel model, User user, Staff personToEdit)
    {
        user.TwoStepVerification = model.TwoStepVerification;
        user.TwoStepVerificationByEmail = model.TwoStepVerificationByEmail;
        user.TwoStepVerificationByGoogleAuthenticator = model.TwoStepVerificationByGoogleAuthenticator;

        if (!string.IsNullOrEmpty(model.Email) || !string.IsNullOrEmpty(model.PhoneNumber))
        {
            personToEdit.Email = model.Email;
            personToEdit.PhoneNumber = model.PhoneNumber;
            _unitOfWork.Staffs.AddOrUpdate(personToEdit);
        }

        _unitOfWork.Users.AddOrUpdate(user);
        _unitOfWork.Complete();

        return true;
    }
    public UserProfileInfoViewModel GetUserInfo(string currentUserName)
    {
        var user = _unitOfWork.Users.Where(u => u.UserName == currentUserName).Single();
        UserProfileInfoViewModel model = new UserProfileInfoViewModel();
        var personToEdit = _unitOfWork.Staffs.Where(i => i.Id == user.StaffId).FirstOrDefault();
        if (personToEdit != null)
        {
            var editedPersonUserName = _unitOfWork.Users.Find(t => t.StaffId == personToEdit.Id).FirstOrDefault();

            var mainPersonelType = personToEdit.StaffTypeId;
            var staffTypeTitle = _unitOfWork.LookUps.Find(l => l.Id == mainPersonelType).FirstOrDefault()?.Title;

            var roleNames = _unitOfWork.Staffs.GetRoles(user.Id);

            model.FName = personToEdit.FName;
            model.PersonalCode = personToEdit.PersonalCode;
            model.LName = personToEdit.LName;
            model.PhoneNumber = personToEdit.PhoneNumber;
            model.ImagePath = editedPersonUserName?.UserName + ".jpg";
            model.Email = personToEdit.Email;
            model.StaffTypeId = mainPersonelType;
            model.StaffTypeTitle = staffTypeTitle;
            model.InsuranceNumber = personToEdit.InsuranceNumber;
            model.IdNumber = personToEdit.IdNumber;
            model.UserRoles = roleNames;
            model.TwoStepVerification = user!.TwoStepVerification;
            model.TwoStepVerificationByEmail = user.TwoStepVerificationByEmail;
            model.TwoStepVerificationByGoogleAuthenticator = user.TwoStepVerificationByGoogleAuthenticator;
        }

        return model;
    }
    private UserBaseInfoCacheDTO GetUserFromDatabase(string userName)
    {
        var userInfo = _unitOfWork.Users.Single(c => c.UserName == userName);
        var staff = _unitOfWork.Staffs.Single(c => c.Id == userInfo.StaffId);
        return new UserBaseInfoCacheDTO()
        {
            Id = userInfo.Id,
            UserName = userInfo.UserName,
            StaffId = userInfo.StaffId,
            FullName = staff.FullName
        };
    }
}