using BPMS.Domain.Common.Constants.PermissionStructure;
using BPMS.Domain.Common.Enums;
using BPMS.Domain.Common.Helpers;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Repositories;

public class PermissionLogsRepository : IPermissionLogsRepository
{
    private readonly IConfiguration _configuration;

    public PermissionLogsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void AddPermissionLog(List<PermissionLog> models)
    {
        string connectionString = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string");
        string queryString = @"INSERT INTO [dbo].[PermissionLogs]
                                           ([Id]
                                           ,[ClaimType]
                                           ,[ClaimValue]
                                           ,[ClaimDesc]
                                           ,[RoleId]
                                           ,[RoleName]
                                           ,[CreatorUserId]
                                           ,[ActionType]
                                           ,[CreateDateTime]
                                           ,[CreateDate]
                                           ,[CreateTime])
                                     VALUES
                                           (@Id
                                           ,@ClaimType
                                           ,@ClaimValue
                                           ,@ClaimDesc
                                           ,@RoleId
                                           ,@RoleName
                                           ,@CreatorUserId
                                           ,@ActionType
                                           ,@CreateDateTime
                                           ,@CreateDate
                                           ,@CreateTime)";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();
            SqlCommand command = new SqlCommand(queryString, connection);

            foreach (var model in models)
            {
                command.Parameters.AddWithValue("@Id", Guid.NewGuid());
                command.Parameters.AddWithValue("@ClaimType", model.ClaimType);
                command.Parameters.AddWithValue("@ClaimValue", model.ClaimValue);
                command.Parameters.AddWithValue("@RoleId", model.RoleId);
                command.Parameters.AddWithValue("@RoleName", model.RoleName);
                command.Parameters.AddWithValue("@CreatorUserId", model.CreatorUserId);
                command.Parameters.AddWithValue("@ActionType", model.ActionType);
                command.Parameters.AddWithValue("@CreateDateTime", model.CreateDateTime);
                command.Parameters.AddWithValue("@CreateDate", model.CreateDate);
                command.Parameters.AddWithValue("@CreateTime", model.CreateTime);

                if (model.ClaimDesc == null)
                    command.Parameters.AddWithValue("@ClaimDesc", DBNull.Value);
                else
                    command.Parameters.AddWithValue("@ClaimDesc", model.ClaimDesc);

                command.ExecuteNonQuery();
                command.Parameters.Clear();
            }
        }
    }

    public List<PermissionLog> GetPermissionLogs()
    {
        var logs = new List<PermissionLog>();
        string logsConn = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string");

        string bpmsConn = _configuration.GetConnectionString("BPMS-sql") ?? throw new NullReferenceException("Invalid connection string");

        var logsDb = new SqlConnectionStringBuilder(logsConn).InitialCatalog;
        var bpmsDb = new SqlConnectionStringBuilder(bpmsConn).InitialCatalog;

        using (var connection = new SqlConnection(bpmsConn))
        {
            connection.Open();
            var command = $@"SELECT TOP (3000)
                                 A.Id
		                        ,A.ClaimType
		                        ,A.ClaimValue
		                        ,A.ClaimDesc
		                        ,A.RoleId
		                        ,A.RoleName
		                        ,A.CreatorUserId
		                        ,A.ActionType
		                        ,A.CreateDateTime
		                        ,A.CreateDate
		                        ,A.CreateTime
		                        ,A.UserName
		                        ,A.StaffId
		                        ,S.FName
		                        ,S.LName
                               From ( SELECT TOP (3000)
                                 PL.Id
		                        ,PL.ClaimType
		                        ,PL.ClaimValue
		                        ,PL.ClaimDesc
		                        ,PL.RoleId
		                        ,PL.RoleName
		                        ,PL.CreatorUserId
		                        ,PL.ActionType
		                        ,PL.CreateDateTime
		                        ,PL.CreateDate
		                        ,PL.CreateTime
		                        ,U.UserName
		                        ,U.StaffId
	                        FROM [{logsDb}].[dbo].[PermissionLogs] as PL
	                        LEFT JOIN [{bpmsDb}].[dbo].[Users] as U 
							ON U.Id = PL.CreatorUserId ) as A
                            LEFT JOIN [{bpmsDb}].[dbo].[Staffs] as S
                            ON S.Id = A.StaffId
	                        ORDER BY A.CreateDateTime DESC";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var LName = reader["LName"].ToString();
                var FName = reader["FName"].ToString();
                var log = new PermissionLog();

                log.Id = Guid.Parse(reader["Id"].ToString());
                log.ClaimType = reader["ClaimType"].ToString();
                log.ClaimValue = reader["ClaimValue"].ToString();
                log.ClaimDesc = reader["ClaimDesc"].ToString();
                log.RoleId = Guid.Parse(reader["RoleId"].ToString());
                log.RoleName = reader["RoleName"].ToString();
                log.CreatorUserId = Guid.Parse(reader["CreatorUserId"].ToString());
                log.ActionType = (PermissionActionType)int.Parse(reader["ActionType"].ToString());
                log.CreateDate = int.Parse(reader["CreateDate"].ToString());
                log.CreateTime = reader["CreateTime"].ToString();
                log.CreateDateTime = HelperBs.ConvertMiladiToShamsi(DateTime.Parse(reader["CreateDateTime"].ToString()));
                log.CreatorUsername = reader["UserName"].ToString();
                if (!string.IsNullOrEmpty(reader["StaffId"].ToString()))
                {
                    log.StaffId = Guid.Parse(reader["StaffId"].ToString());
                }
                log.FullName = FName + " " + LName;
                logs.Add(log);
            }
        }
        return logs;
    }

    public List<PermissionLogsViewModel> GetPermissionLogsModels()
    {
        var permissionLogs = GetPermissionLogs();
        var permissionLogsVM = new List<PermissionLogsViewModel>();
        foreach (var item in permissionLogs)
        {
            var permissionLogVM = new PermissionLogsViewModel
            {
                Id = item.Id,
                ClaimType = GetClaimType(item.ClaimType),
                ClaimValue = item.ClaimValue,
                ClaimDesc = item.ClaimDesc,
                RoleName = item.RoleName,
                FullName = item.FullName,
                CreatorUser = item.CreatorUsername,
                ActionType = item.ActionType,
                CreateDateTime = item.CreateDate.ToString().Insert(4, "/").Insert(7, "/")
                                 + " " + item.CreateTime.Insert(2, ":")
            };
            permissionLogsVM.Add(permissionLogVM);
        }
        return permissionLogsVM;
    }

    private string GetClaimType(string id)
    {
        switch (id)
        {
            case PermissionPolicyType.RoutePermission:
                return "دسترسی صفحات";
            case PermissionPolicyType.WidgetPermission:
                return "ابزارها";
            case PermissionPolicyType.WorkFlowPermission:
                return "فرآیندها";
            case PermissionPolicyType.WorkFlowFormPreviewPermission:
                return "تغییرات فرم ها";
            case PermissionPolicyType.WorkFlowIndexPermission:
                return "ایحاد شاخص برای  فرایندها";
            case PermissionPolicyType.DynamicChartReportPermission:
                return "تغییرات نمودارهای گزارشات";
            case PermissionPolicyType.WorkFlowPreviewPermission:
                return "نمایش مدل BPMN";
            case PermissionPolicyType.WorkFlowFormListPermission:
                return "نمایش لیست ها";
            case PermissionPolicyType.WorkFlowStatusPermission:
                return "وضعیت درخواست ها";
            case PermissionPolicyType.ReportPermission:
                return "گزارشهای ایجاد شده";
            default: return "";
        }
    }
}