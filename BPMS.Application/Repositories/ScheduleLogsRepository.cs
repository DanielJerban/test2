using BPMS.Application.Repositories.Base;
using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Data;
using BPMS.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BPMS.Application.Repositories;

public class ScheduleLogsRepository : Repository<ScheduleLog>, IScheduleLogRepository
{
    private readonly string _logConnectionString;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    public ScheduleLogsRepository(BpmsDbContext context, IConfiguration configuration, IServiceProvider serviceProvider) : base(context)
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;
        _logConnectionString = configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Connection string not valid.");
    }

    public BpmsDbContext DbContext => Context;
    public ILookUpRepository LookUpRepository => _serviceProvider.GetRequiredService<ILookUpRepository>();


    public string GetLastRunTimeDate(int code)
    {
        var look = LookUpRepository.GetByTypeAndCode("TaskType", 8);
        var schedule = DbContext.Schedules.FirstOrDefault(d => d.TaskTypeId == look.Id);

        var registerDate = String.Empty;
        using (var connection = new SqlConnection(_logConnectionString))
        {
            connection.Open();

            var command = @" SELECT TOP (1) 
                                     [Id]
                                    ,[ScheduleId]
                                    ,[RunDate]
                                    ,[RunTime]
                                    ,[RegisterDate]
                                    ,[RegisterTime]
                                FROM [ScheduleLogs]
                                order by RegisterDate desc,RegisterTime desc";

            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {
                registerDate = reader["RegisterDate"].ToString();

            }
        }
        if (!string.IsNullOrEmpty(registerDate))
            return registerDate.Insert(4, "/").Insert(7, "/");

        return registerDate;
    }


    public void AddScheduleLog(ScheduleLogViewModel model)
    {
        string queryString = @"Insert into ScheduleLogs (Id, ScheduleId, RunDate, RunTime, RegisterDate, RegisterTime) 
                                   values (@Id, @ScheduleId, @RunDate, @RunTime, @RegisterDate, @RegisterTime)";
        using (SqlConnection connection = new SqlConnection(_logConnectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@ScheduleId", model.ScheduleId);
            command.Parameters.AddWithValue("@RunDate", model.RunDate);
            command.Parameters.AddWithValue("@RunTime", model.RunTime);
            command.Parameters.AddWithValue("@RegisterDate", model.RegisterDate);
            command.Parameters.AddWithValue("@RegisterTime", model.RegisterTime);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                connection.Close();
            }

            connection.Close();
        }
    }

    public IEnumerable<GetScheduleLogViewModel> GetScheduleLogsByScheduleId()
    {
        var scheduleLog = new List<GetScheduleLogViewModel>();
        string bpmsConnection = _configuration.GetConnectionString("BPMS-sql") ?? throw new NullReferenceException("Connection string not valid.");
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(bpmsConnection);
        string database = builder.InitialCatalog;
        using (var connection = new SqlConnection(_logConnectionString))
        {
            connection.Open();
            var command = $@"  SELECT s.Title,sl.ScheduleId,sl.RegisterDate,sl.RegisterTime,sl.RunDate,sl.RunTime
                                   FROM [dbo].[ScheduleLogs] sl
                                   INNER JOIN [{database}].[dbo].[Schedules] s 
                                   ON s.Id=sl.ScheduleId";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var Schedule = new GetScheduleLogViewModel();
                Schedule.ScheduleId = Guid.Parse(reader["ScheduleId"].ToString());
                Schedule.Schedule = reader["Title"].ToString();
                Schedule.RegisterDate = (int)reader["RegisterDate"];
                Schedule.RegisterTime = reader["RegisterTime"].ToString();
                Schedule.RunDate = (int)reader["RunDate"];
                Schedule.RunTime = reader["RunTime"].ToString();
                scheduleLog.Add(Schedule);
            }
        }
        return scheduleLog;
    }

    public ScheduleLastLogDateViewModel GetScheduleLastLog(Guid id)
    {
        var schedule = new ScheduleLastLogDateViewModel();

        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_logConnectionString);
        string database = builder.InitialCatalog;

        using var connection = new SqlConnection(_logConnectionString);
        connection.Open();
        string query = @$" SELECT TOP (1) 
                                  [RegisterDate]
                                 ,[RegisterTime]
                                  FROM [{database}].[dbo].[ScheduleLogs]
                                  Where ScheduleId = @id  
                                  order by RegisterDate desc,[RegisterTime] desc";
        var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@id", id);

        var reader = command.ExecuteReader();
        while (reader.Read())
        {
            schedule.RegisterDate = (int)reader["RegisterDate"];
            schedule.RegisterTime = reader["RegisterTime"].ToString();
        }

        return schedule;
    }

    public IEnumerable<GetScheduleLogViewModel> GetAllScheduleLogs()
    {
        var ScheduleLog = new List<GetScheduleLogViewModel>();
        string bpmsConnection = _configuration.GetConnectionString("BPMS-sql") ?? throw new NullReferenceException("Connection string not valid.");
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(bpmsConnection);
        string database = builder.InitialCatalog;
        using (var connection = new SqlConnection(_logConnectionString))
        {
            connection.Open();
            var command = $@"  SELECT s.Title,l.Title as TaskType,sl.ScheduleId,sl.RegisterDate,sl.RegisterTime,sl.RunDate,sl.RunTime
                                   FROM [dbo].[ScheduleLogs] sl
                                   INNER JOIN [{database}].[dbo].[Schedules] s 
                                   ON s.Id=sl.ScheduleId
                                   INNER JOIN [{database}].[dbo].[LookUps] l
                                   ON s.TaskTypeId=l.id";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var Schedule = new GetScheduleLogViewModel();
                Schedule.ScheduleId = Guid.Parse(reader["ScheduleId"].ToString());
                Schedule.Schedule = reader["Title"].ToString();
                Schedule.TaskType = reader["TaskType"].ToString();
                Schedule.RegisterDate = (int)reader["RegisterDate"];
                Schedule.RegisterTime = reader["RegisterTime"].ToString();
                Schedule.RunDate = (int)reader["RunDate"];
                Schedule.RunTime = reader["RunTime"].ToString();
                ScheduleLog.Add(Schedule);
            }
        }
        return ScheduleLog;
    }
}