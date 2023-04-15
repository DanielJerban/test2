using BPMS.Domain.Common.ViewModels;
using BPMS.Domain.Entities;
using BPMS.Infrastructure.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Repositories;

public class SmsLogRepository : ISmsLogRepository
{
    private readonly IConfiguration _configuration;

    public SmsLogRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void AddSmsLog(SmsLog model)
    {
        string connectionString = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Connection string not valid.");

        string queryString = @"Insert into SmsLogs (Id, SenderNumber, RecieverNumber, SentDate, SmsText, SentStatus, ErrorMessage, SmsSendType, Time) 
                               values (@Id, @SenderNumber, @RecieverNumber, @SentDate, @SmsText, @SentStatus, @ErrorMessage, @SmsSendType, @Time)";
        using (SqlConnection connection = new SqlConnection(connectionString))
        {

            SqlCommand command = new SqlCommand(queryString, connection);
            command.Parameters.AddWithValue("@Id", model.Id);
            command.Parameters.AddWithValue("@SentDate", model.SentDate);
            command.Parameters.AddWithValue("@SentStatus", model.SentStatus);
            command.Parameters.AddWithValue("@SmsSendType", model.SmsSendType);


            command.Parameters.AddWithValue("@SenderNumber", ((object)model.SenderNumber) ?? DBNull.Value);
            command.Parameters.AddWithValue("@RecieverNumber", ((object)model.RecieverNumber) ?? DBNull.Value);
            command.Parameters.AddWithValue("@SmsText", ((object)model.SmsText) ?? DBNull.Value);
            command.Parameters.AddWithValue("@ErrorMessage", ((object)model.ErrorMessage) ?? DBNull.Value);
            command.Parameters.AddWithValue("@Time", ((object)model.Time) ?? DBNull.Value);

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

    public IEnumerable<SmsLogsViewModel> GetAllSmsLogs()
    {
        var smslogs = new List<SmsLogsViewModel>();
        string Smsconn = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Connection string not valid.");
        using (var connection = new SqlConnection(Smsconn))
        {
            connection.Open();
            var command = @"  SELECT top(3000) Id
                                        ,[SenderNumber]
                                        ,[RecieverNumber]
                                        ,[SentDate]
                                        ,[SmsText]
                                        ,[SentStatus]
                                        ,[ErrorMessage]
                                        ,[SmsSendType]
                                        ,[Time]
                                         FROM SmsLogs";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var sms = new SmsLogsViewModel
                {
                    SenderNumber = reader["SenderNumber"].ToString(),
                    RecieverNumber = reader["RecieverNumber"].ToString(),
                    SmsText = reader["SmsText"].ToString(),
                    SmsSendType = (int)reader["SmsSendType"] == 2 ? "وب سرویس" : "مودم جی اس ام",
                    SentDate = reader["SentDate"].ToString().Insert(4, "/").Insert(7, "/"),
                    Time = reader["Time"].ToString().Insert(2, ":"),
                    SentStatus = (bool)reader["SentStatus"] == true ? "ارسال شده" : "ارسال نشده",
                    ErrorMessage = reader["ErrorMessage"].ToString()
                };
                smslogs.Add(sms);
            }
        }

        return smslogs.OrderByDescending(o => o.SentDate);
    }
}