using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.Dtos.User;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using BPMS.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    public UserRepository(BpmsDbContext context, IServiceProvider serviceProvider, IConfiguration configuration) : base(context)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public BpmsDbContext DbContext => Context;


    public IPasswordService PasswordService => _serviceProvider.GetRequiredService<IPasswordService>();
    public ILDAPService LDAPService => _serviceProvider.GetRequiredService<ILDAPService>();
    public ILoginService LoginService => _serviceProvider.GetRequiredService<ILoginService>();

    public IEnumerable<StaffViewModel> GetStaffs()
    {
        var query = from staff in DbContext.Staffs
                    where !DbContext.Users.Any(user => user.StaffId == staff.Id)
                    select staff;
        return query.Select(s => new StaffViewModel()
        {
            Id = s.Id,
            PhoneNumber = s.PhoneNumber,
            FullName = s.FName + " " + s.LName,
            PersonalCode = s.PersonalCode

        });


    }

    public IEnumerable GetStaffAndUsers()
    {
        return (
            from user in DbContext.Users
            join staff in DbContext.Staffs on user.StaffId equals staff.Id into leftjoin
            from staff in leftjoin.DefaultIfEmpty()
            select new UserViewModel()
            {
                Id = user.Id,
                StaffId = staff.Id,
                UserName = user.UserName,
                FullName = staff.FName + " " + staff.LName,
                IsActive = user.IsActive,
                PersonelCode = staff.PersonalCode,
                PhoneNumber = staff.PhoneNumber,
                InsuranceNumber = staff.InsuranceNumber,
                IdNumber = staff.IdNumber,
                EmploymentDate = staff.EmploymentDate
            }
        ).ToList();
    }

    public StaffDto ExternalLogin(UserDto model)
    {
        if (string.IsNullOrWhiteSpace(model.Ip) || string.IsNullOrWhiteSpace(model.MachineName))
        {
            throw new ArgumentException(@"Ip و MachineName وارد نشده است");
        }

        var sys = DbContext.LookUps.Single(l => l.Code == 1 && l.Type == "System");
        if (sys.IsActive)
        {
            throw new ArgumentException(sys.Title);
        }

        var user = DbContext.Users.FirstOrDefault(
            u => u.UserName == model.UserName && u.Password == model.Password);

        if (user == null)
        {
            throw new ArgumentException(@"نام کاربری یا کلمه عبور صحیح نیست.");
        }

        if (!user.IsActive)
        {
            throw new ArgumentException(@"حساب کاربری شما فعال نشده است.");
        }

        if (user.Staff.EngType.Code == 2)
        {
            throw new ArgumentException(@"شما اجازه ورود به سایت را ندارید.");
        }

        Task.Factory.StartNew(() =>
            LoginService.LogSuccessLogin(user.Id, user.UserName, new UserSystemDataDto()
            {
                IP = model.Ip,
                BrowserName = "",
                HostName = model.MachineName,
                BrowserVersion = ""
            })
        );

        var staff = user.Staff;
        var staffdto = new StaffDto()
        {
            Id = staff.Id,
            FName = staff.FName,
            LName = staff.LName,
            Email = staff.FullName,
            EmploymentDate = staff.EmploymentDate,
            EngTypeId = staff.EngTypeId,
            IdNumber = staff.IdNumber,
            ImagePath = "/images" + staff.ImagePath,
            InsuranceNumber = staff.InsuranceNumber,
            PersonalCode = staff.PersonalCode,
            PhoneNumber = staff.PhoneNumber,
            StaffTypeId = staff.StaffTypeId,
            UserName = user.UserName,
            UserId = user.Id,
            SerialNumber = user.SerialNumber
        };
        return staffdto;
    }

    // todo: Uncomment later 
    public void Logout(UserSystemDataDto model)
    {
        //var userName = HttpContext.Current.User.Identity.Name;
        //var user = DbContext.Users.FirstOrDefault(u => u.UserName == userName);
        //if (user == null) return;

        //FormsAuthentication.SignOut();

        //Task.Factory.StartNew(() =>
        //    LoginService.LogSuccessLogout(user.Id, user.UserName, model)
        //);

        //var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, "")
        //{ Expires = DateTime.Now.AddYears(-1) };
        //HttpContext.Current.Response.Cookies.Add(cookie);
        //MainHub.LogoutUser(user.UserName);
    }

    public User FindUser(string username, string password)
    {
        var passwordHash = HashPassword.EncodePasswordMd5(password);
        return DbContext.Users.FirstOrDefault(x => x.UserName == username && x.Password == passwordHash);
    }

    public UserLoginOutSimpleDto GetUserLastSuccessLogin(string username)
    {
        var lookupId = DbContext.LookUps.Single(c => c.Type == "UserLoginOutType" && c.Code == 1).Id;
        var userId = DbContext.Users.Single(c => c.UserName == username).Id;

        string connectionString = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string.");
        string query = $@"SELECT TOP 2 
                                Ip,
	                            SUBSTRING(Cast([Date] as nvarchar(50)),0,5)+'/'+SUBSTRING(Cast([Date] as nvarchar(50)),5,2) + '/' + SUBSTRING(Cast([Date] as nvarchar(50)),7,2) as [Date],
	                            SUBSTRING([Time] ,0,3)+':'+SUBSTRING([Time],3,4) as [Time]
                              FROM UserLoginOuts 
                              WHERE LoginOutTypeId = '{lookupId}' 
                              AND UserId = '{userId}'
                              ORDER BY [Date] DESC , [Time] DESC";

        using var connection = new SqlConnection(connectionString);
        SqlCommand command = new SqlCommand(query, connection);

        var logs = new List<UserLoginOutSimpleDto>();

        connection.Open();
        SqlDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            logs.Add(new UserLoginOutSimpleDto()
            {
                IP = reader[0].ToString(),
                Date = reader[1].ToString(),
                Time = reader[2].ToString()
            });
        }
        reader.Close();

        return logs.Count == 2 ? logs[1] : null;
    }
}