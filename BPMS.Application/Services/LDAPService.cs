using System.Reflection.PortableExecutable;
using BPMS.Infrastructure;
using BPMS.Infrastructure.Services;

namespace BPMS.Application.Services;

public class LDAPService : ILDAPService
{
    private DirectoryEntry _searchRoot;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISystemSettingService _systemSettingService;
    private readonly IStaffService _staffService;

    public LDAPService(IUnitOfWork unitOfWork, ISystemSettingService systemSettingService, IStaffService staffService)
    {
        _unitOfWork = unitOfWork;
        _systemSettingService = systemSettingService;
        _staffService = staffService;
    }
    public bool ValidateUserInLDAP(string userName, string password, string domainName)
    {
        bool validation = false;
        //try
        //{
        //    using LdapConnection ldc = new LdapConnection(new LdapDirectoryIdentifier((string)null, false, false));
        //    var nc = new NetworkCredential(userName, password, domainName);

        //    ldc.Credential = nc;
        //    ldc.AuthType = AuthType.Negotiate;
        //    ldc.Bind(nc); // user has authenticated at this point, as the credentials were used to login to the dc.
        //    validation = true;
        //}
        //catch (LdapException)
        //{
        //    validation = false;
        //}
        return validation;
    }

    public void LDAPUserSyncTimerEnable()
    {
        //try
        //{
        //    var lastSetting = _systemSettingService.GetLDAPSetting();

        //    if (lastSetting.IsSyncerEnable)
        //    {
        //        var ldapUsers = GetLdapUsers(lastSetting);

        //        foreach (var user in ldapUsers)
        //        {
        //            var userinfo = _unitOfWork.Users.Where(t => t.LDAPUserName == user.LdapUserName || t.UserName == user.LdapUserName).FirstOrDefault();

        //            if (userinfo != null)
        //            {
        //                userinfo.LDAPUserName = user.LdapUserName;
        //                var staff = _unitOfWork.Staffs.Where(t => t.Id == userinfo.StaffId).FirstOrDefault();
        //                staff.Email = !string.IsNullOrEmpty(user.Email) ? user.Email : staff.Email;
        //                staff.PhoneNumber = !string.IsNullOrEmpty(user.PhoneNumber) ? user.PhoneNumber : staff.PhoneNumber;
        //                staff.FName = !string.IsNullOrEmpty(user.FName) ? user.FName : staff.FName;
        //                staff.LName = !string.IsNullOrEmpty(user.LName) ? user.LName : staff.LName;
        //                staff.LocalPhone = !string.IsNullOrEmpty(user.LocalPhone) && user.LocalPhone.Length < 10 ? user.LocalPhone : staff.LocalPhone;
        //                staff.InsuranceNumber = !string.IsNullOrEmpty(user.InsuranceNumber) ? user.InsuranceNumber : staff.InsuranceNumber;
        //                staff.IdNumber = !string.IsNullOrEmpty(user.IdNumber) ? user.IdNumber : staff.IdNumber;
        //                staff.ImagePath = !string.IsNullOrEmpty(user.ImagePath) ? user.ImagePath : staff.ImagePath;
        //                _unitOfWork.Staffs.Update(staff);
        //                _unitOfWork.Users.Update(userinfo);
        //                _unitOfWork.Complete();
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    _staffService.CreateStaffFromActiveDirectory(user);
        //                    _unitOfWork.Complete();
        //                }
        //                catch (System.Data.Entity.Validation.DbEntityValidationException e)
        //                {
        //                    StringBuilder sb = new StringBuilder();
        //                    foreach (var eve in e.EntityValidationErrors)
        //                    {
        //                        sb.AppendLine(
        //                            "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:" +
        //                            " " +
        //                            eve.Entry.Entity.GetType().Name + " " + eve.Entry.State);
        //                        foreach (var ve in eve.ValidationErrors)
        //                        {
        //                            sb.AppendLine("- Property: \"{0}\", Error: \"{1}\"" + " " +
        //                                          ve.PropertyName + " " + ve.ErrorMessage);
        //                        }
        //                    }
        //                    var exceptions = new CustomExceptionHandler();
        //                    exceptions.HandleException(e);
        //                }
        //                catch (Exception e)
        //                {
        //                    var exceptions = new CustomExceptionHandler();
        //                    exceptions.HandleException(e);
        //                }
        //            }
        //        }
        //    }
        //}
        //catch (Exception e)
        //{
        //}
    }
    //private IEnumerable<ActiveDirectoryUserDto> GetLdapUsers(LdapSettingViewModel ldap)
    //{
    //    List<ActiveDirectoryUserDto> ldapUser = new List<ActiveDirectoryUserDto>();

    //    string path = "";
    //    string domainName = MakeDomainName(ldap.DomainName);
    //    if (string.IsNullOrEmpty(ldap.Ip))
    //    {
    //        path = String.Format("LDAP://" + "" + domainName);
    //        _searchRoot = new DirectoryEntry(path);
    //    }
    //    else
    //    {
    //        path = String.Format("LDAP://" + "" + ldap.Ip + "/" + domainName);
    //        _searchRoot = new DirectoryEntry(path, ldap.UserName, ldap.Password);
    //    }

    //    using DirectorySearcher directorySearcher = new DirectorySearcher(_searchRoot);
    //    directorySearcher.Filter = "(&(objectCategory=person)(objectClass=user))";

    //    SearchResultCollection resultCol;

    //    try
    //    {
    //        resultCol = directorySearcher.FindAll();
    //    }
    //    catch (Exception e)
    //    {
    //        var exceptions = new CustomExceptionHandler();
    //        exceptions.HandleException(e);
    //        return ldapUser;
    //    }

    //    var ldapProp = ldap.Properties;


    //    foreach (SearchResult searchResult in resultCol)
    //    {
    //        ldapUser.Add(new ActiveDirectoryUserDto()
    //        {
    //            LdapUserName = GetProperty(searchResult, ldapProp.LdapUserName),
    //            Email = GetProperty(searchResult, ldapProp.Email),
    //            FName = GetProperty(searchResult, ldapProp.FName),
    //            LName = GetProperty(searchResult, ldapProp.LName),
    //            PhoneNumber = GetProperty(searchResult, ldapProp.PhoneNumber),
    //            LocalPhone = GetProperty(searchResult, ldapProp.LocalPhone),
    //            InsuranceNumber = GetProperty(searchResult, ldapProp.InsuranceNumber),
    //            IdNumber = GetProperty(searchResult, ldapProp.IdNumber),
    //            ImagePath = GetProperty(searchResult, ldapProp.ImagePath),
    //            Password = GetProperty(searchResult, ldapProp.UserPassword),
    //            PersonalCode = GetProperty(searchResult, ldapProp.PersonalCode),
    //        });
    //    }

    //    return ldapUser;
    //}
    //private static string MakeDomainName(string domain)
    //{
    //    string DC1 = domain.Substring(0, domain.IndexOf("."));
    //    string DC2 = domain.Substring(domain.IndexOf(".") + 1);
    //    string domainName = $"DC={DC1},DC={DC2}";
    //    return domainName;
    //}

    //private static string GetProperty(SearchResult searchResult, string PropertyName)
    //{
    //    if (!string.IsNullOrEmpty(PropertyName) && searchResult.Properties.Contains(PropertyName))
    //    {
    //        return searchResult.Properties[PropertyName][0].ToString();
    //    }
    //    else
    //    {
    //        return string.Empty;
    //    }
    //}
}