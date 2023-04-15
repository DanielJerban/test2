using BPMS.Domain.Common.Dtos;
using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure.MainHelpers;
using BPMS.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Repositories;

public class UserLoginOutRepository : IUserLoginOutRepository
{
    private readonly CustomExceptionHandler _elmahExceptions = new();
    private readonly IConfiguration _configuration;

    public UserLoginOutRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void AddUserLoginOut(UserLoginOutDto model)
    {
        string connectionString = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string.");
        string command = @" INSERT INTO [dbo].[UserLoginOuts] 
                                (
                                    Id,
                                    UserId,
                                    LoginOutTypeId,
                                    Ip,
                                    BrowserName,
                                    MachineName,
                                    Date,
                                    Time,
                                    UserName
                                ) VALUES 
                                (
                                    @Id,
                                    @UserId,
                                    @LoginOutTypeId,
                                    @Ip,
                                    @BrowserName,
                                    @MachineName,
                                    @Date,
                                    @Time,
                                    @UserName
                                )";
        using SqlConnection connection = new SqlConnection(connectionString);
        SqlCommand sqlCommand = new SqlCommand(command, connection);
        sqlCommand.Parameters.AddWithValue("@Id", model.Id);
        sqlCommand.Parameters.AddWithValue("@UserId", model.UserId);
        sqlCommand.Parameters.AddWithValue("@LoginOutTypeId", model.LoginOutTypeId);
        sqlCommand.Parameters.AddWithValue("@Ip", model.Ip);
        sqlCommand.Parameters.AddWithValue("@BrowserName", model.BrowserName);
        sqlCommand.Parameters.AddWithValue("@MachineName", model.MachineName);
        sqlCommand.Parameters.AddWithValue("@Date", model.Date);
        sqlCommand.Parameters.AddWithValue("@Time", model.Time);
        sqlCommand.Parameters.AddWithValue("@UserName", model.UserName);

        try
        {
            connection.Open();
            sqlCommand.ExecuteNonQuery();
            connection.Close();
        }
        catch (Exception ex)
        {
            connection.Close();
            _elmahExceptions.HandleException(ex);
        }
    }

    public void DeleteUserLoginOutLog(Guid userId)
    {
        string connectionString = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string.");
        string command = $@" delete from [dbo].[UserLoginOuts] where UserId='{userId}'";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand sqlCommand = new SqlCommand(command, connection);

            try
            {
                connection.Open();
                sqlCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
                _elmahExceptions.HandleException(ex);
            }

            connection.Close();
        }
    }

    public IEnumerable<UserLoginOutViewModel> GetRecordsForLoginOut(string username)
    {
        var loginOutsLogs = new List<UserLoginOutViewModel>();

        string logsConnection = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string.");
        var logBuilder = new SqlConnectionStringBuilder(logsConnection);
        string logDatabaseName = logBuilder.InitialCatalog;

        string bpmsConnection = _configuration.GetConnectionString("BPMS-Sql") ?? throw new NullReferenceException("Invalid connection string.");
        var bpmsBuilder = new SqlConnectionStringBuilder(bpmsConnection);
        string bpmsDatabaseName = bpmsBuilder.InitialCatalog;

        using (var connection = new SqlConnection(logsConnection))
        {
            connection.Open();
            var command = @$"select 
                                	case
                                		when staffs.Id is not null 
                                		then
                                			staffs.FName + ' ' + staffs.LName
                                		else
                                			' --- '
                                	end as FullName,
                                	loginout.UserName as UserName,
                                	lookups.Title as LoginType,
                                	SUBSTRING(Cast(loginout.Date as nvarchar(50)),0,5)+'/'+SUBSTRING(Cast(loginout.Date as nvarchar(50)),5,2) + '/' + SUBSTRING(Cast(loginout.Date as nvarchar(50)),7,2) as Date,
                                	SUBSTRING(loginout.Time ,0,3)+':'+SUBSTRING(loginout.Time,3,4) as Time,
                                	loginout.Ip as IP,
                                	loginout.MachineName as MachineName, 
                                	loginout.BrowserName
                                from [{logDatabaseName}].dbo.UserLoginOuts as loginout
                                left join [{bpmsDatabaseName}].dbo.Users as users on loginout.UserId = users.Id
                                left join [{bpmsDatabaseName}].dbo.Staffs as staffs on users.StaffId = staffs.Id
                                left join [{bpmsDatabaseName}].dbo.LookUps as lookups on loginout.LoginOutTypeId = lookups.Id";

            if (!string.IsNullOrEmpty(username))
            {
                command += $@" where loginout.UserName = '{username}' or users.UserName = '{username}'";
            }

            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var usersLogs = new UserLoginOutViewModel();
                usersLogs.UserName = reader["UserName"].ToString();
                usersLogs.Ip = reader["IP"].ToString();
                usersLogs.FullName = reader["FullName"].ToString();
                usersLogs.LoginOut = reader["LoginType"].ToString();
                usersLogs.MachineName = reader["MachineName"].ToString();
                usersLogs.Date = reader["Date"].ToString();
                usersLogs.Time = reader["Time"].ToString();
                usersLogs.BrowserName = reader["BrowserName"].ToString();

                loginOutsLogs.Add(usersLogs);
            }
        }
        return loginOutsLogs;
    }
}