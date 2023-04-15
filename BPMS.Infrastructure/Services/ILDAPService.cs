namespace BPMS.Infrastructure.Services;

public interface ILDAPService
{
    bool ValidateUserInLDAP(string userName, string password, string domainName);
    void LDAPUserSyncTimerEnable();
}