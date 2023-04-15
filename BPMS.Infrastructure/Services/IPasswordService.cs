namespace BPMS.Infrastructure.Services;

public interface IPasswordService
{
    string EncryptPassword(string username, string password);
}