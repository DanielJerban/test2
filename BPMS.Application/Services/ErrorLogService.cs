using BPMS.Domain.Common.ViewModels;
using BPMS.Infrastructure.Services;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace BPMS.Application.Services;

public class ErrorLogService : IErrorLogService
{
    private readonly IConfiguration _configuration;

    public ErrorLogService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IEnumerable<ElmahErrorLogsViewModel> GetErrorLogsList()
    {
        var errorlogs = new List<ElmahErrorLogsViewModel>();
        string Elmahconn = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string.");
        using (var connection = new SqlConnection(Elmahconn))
        {
            connection.Open();
            var command = @"  SELECT top(3000) [Application]
                                        ,[Host]
                                        ,[Type]
                                        ,[Source]
                                        ,[Message]
                                        ,[User]
                                        ,[StatusCode]
                                        ,[TimeUtc]
                                         ,[Sequence] FROM ELMAH_Error order by Sequence desc";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                var Error = new ElmahErrorLogsViewModel();
                Error.Application = (reader["Application"].ToString());
                Error.Host = reader["Host"].ToString();
                Error.Type = reader["Type"].ToString();
                Error.Source = reader["Source"].ToString();
                Error.Message = reader["Message"].ToString();
                Error.User = reader["User"].ToString();
                Error.StatusCode = reader["StatusCode"].ToString();
                Error.Date = DateTime.Parse(reader["TimeUtc"].ToString()).ToLocalTime().ToString("yyyy/MM/dd");
                Error.Time = DateTime.Parse(reader["TimeUtc"].ToString()).ToLocalTime().ToString("HH:mm:ss");
                Error.Sequence = reader["Sequence"].ToString();

                errorlogs.Add(Error);
            }
        }
        return errorlogs;
    }

    public ElmahErrorLogsViewModel GetErrorBySecuence(int? Sequence)
    {
        var errorlogs = new ElmahErrorLogsViewModel();
        string Elmahconn = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string.");
        using (var connection = new SqlConnection(Elmahconn))
        {
            connection.Open();
            var command = $@"  SELECT [Host]
                                        ,[Message]
                                        ,[User]
                                        ,[TimeUtc]
                                        ,[Sequence]
                                        ,[AllXml]
                                         FROM ELMAH_Error where Sequence={Sequence}";
            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                errorlogs.Host = reader["Host"].ToString();
                errorlogs.Message = reader["Message"].ToString();
                errorlogs.User = reader["User"].ToString();
                errorlogs.Date = DateTime.Parse(reader["TimeUtc"].ToString()).ToLocalTime().ToString();
                errorlogs.Sequence = reader["Sequence"].ToString();
                errorlogs.AllXml = reader["AllXml"].ToString();
            }

        }
        return errorlogs;
    }

    public int GetLastLog()
    {
        int Sequence = 0;
        string Elmahconn = _configuration.GetConnectionString("BPMS.Log-sql") ?? throw new NullReferenceException("Invalid connection string.");
        using (var connection = new SqlConnection(Elmahconn))
        {
            connection.Open();
            var command = $@"  SELECT TOP (1) [Sequence] FROM ELMAH_Error order by Sequence desc";

            var sqlCommand = new SqlCommand(command, connection);
            var reader = sqlCommand.ExecuteReader();
            while (reader.Read())
            {
                Sequence = int.Parse(reader["Sequence"].ToString());
            }

        }
        return Sequence;
    }
}