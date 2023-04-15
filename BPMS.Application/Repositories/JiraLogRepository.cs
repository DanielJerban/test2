using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Repositories;

public class JiraLogRepository : IJiraLogRepository
{
    private readonly string _connectionString;

    public JiraLogRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Connection string not valid");
    }

    public void AddJiraLog(JiraLogViewModel model)
    {
        string queryString = @"Insert into JiraLog (Id, UserId, UserName, RegisterDate, RequestUrl, RequestData, ResponseData) 
                               values (@Id, @UserId, @UserName, @RegisterDate, @RequestUrl, @RequestData, @ResponseData)";
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@UserId", model.UserId);
            command.Parameters.AddWithValue("@UserName", model.UserName);
            command.Parameters.AddWithValue("@RegisterDate", model.RegisterDate);
            command.Parameters.AddWithValue("@RequestUrl", model.RequestUrl);
            command.Parameters.AddWithValue("@ResponseData", model.ResponseData);

            SqlParameter requestDataParam = command.Parameters.AddWithValue("@RequestData", model.RequestData);
            if (model.RequestData == null)
            {
                requestDataParam.Value = DBNull.Value;
            }

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

    public IEnumerable<JiraLogsViewModel> GetJiraLogsList()
    {
        var jiralogs = new List<JiraLogsViewModel>();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = @"  SELECT top(3000) Id
                                        ,[UserName]
                                        ,[RegisterDate]
                                         FROM JiraLog";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var Jira = new JiraLogsViewModel();

                Jira.Id = Guid.Parse(reader["Id"].ToString());
                Jira.UserName = reader["UserName"].ToString();
                Jira.Date = DateTime.Parse(reader["RegisterDate"].ToString()).ToLocalTime().ToString("yyyy/MM/dd");
                Jira.Time = DateTime.Parse(reader["RegisterDate"].ToString()).ToLocalTime().ToString("HH:mm:ss");
                jiralogs.Add(Jira);
            }
        }

        return jiralogs;
    }

    public JiraLogDetailViewModel GetJiraLogById(Guid Id)
    {
        var jiralog = new JiraLogDetailViewModel();
        using (var connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            var command = $@"  SELECT RequestUrl
                                        ,[RequestData]
                                        ,[ResponseData]
                                         FROM JiraLog where Id='{Id}'";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();

            while (reader.Read())
            {

                jiralog.RequestUrl = reader["RequestUrl"].ToString();
                jiralog.RequestData = reader["RequestData"].ToString();
                jiralog.ResponseData = reader["ResponseData"].ToString();

            }
        }

        return jiralog;
    }
}