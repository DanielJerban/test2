using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Repositories;

public class EmailLogsRepository : IEmailLogRepository
{
    private readonly IConfiguration _configuration;

    public EmailLogsRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void AddEmailLog(EmailLog model)
    {
        string connectionString = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("There is no connection string!");
        string queryString =
            @"INSERT INTO [dbo].[EmailLogs]
                   ([Id]
                   ,[SenderEmail]
                   ,[RecieverEmail]
                   ,[SentDate]
                   ,[Time]
                   ,[EmailText]
                   ,[SentStatus]
                   ,[ErrorMessage])
                VALUES (@Id ,@SenderEmail, @RecieverEmail, @SentDate, @Time, @EmailText, @SentStatus, @ErrorMessage)";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@SenderEmail", model.SenderEmail);
            command.Parameters.AddWithValue("@RecieverEmail", model.RecieverEmail);
            command.Parameters.AddWithValue("@EmailText", model.EmailText);
            command.Parameters.AddWithValue("@SentDate", model.SentDate);
            command.Parameters.AddWithValue("@Time", model.Time);
            command.Parameters.AddWithValue("@SentStatus", model.SentStatus);
            if (model.ErrorMessage == null)
                command.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);
            else
                command.Parameters.AddWithValue("@ErrorMessage", model.ErrorMessage);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    public IQueryable<EmailLog> GetAllEmailLogs()
    {
        var emaillogs = new List<EmailLog>();
        string conn = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("There is no connection string!");
        using (var connection = new SqlConnection(conn))
        {
            connection.Open();
            var command = @"SELECT top(3000)
	                               [Id]
                                  ,[SenderEmail]
                                  ,[RecieverEmail]
                                  ,[SentDate]
                                  ,[Time]
                                  ,[EmailText]
                                  ,[SentStatus]
                                  ,[ErrorMessage]
                              FROM [dbo].[EmailLogs] order by SentDate desc";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var Log = new EmailLog();
                Log.SenderEmail = reader["SenderEmail"].ToString();
                Log.RecieverEmail = reader["RecieverEmail"].ToString();
                Log.SentDate = (int)reader["SentDate"];
                Log.Time = reader["Time"].ToString();
                Log.SentStatus = (bool)reader["SentStatus"];
                Log.ErrorMessage = reader["ErrorMessage"].ToString();
                Log.EmailText = reader["EmailText"].ToString();

                emaillogs.Add(Log);
            }
        }
        return emaillogs.AsQueryable();
    }
}