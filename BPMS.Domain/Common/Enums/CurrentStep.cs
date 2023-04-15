namespace BPMS.Domain.Common.Enums;

public enum VerificationSteps
{
    VerificationByGoogleAuthenticator = 5,
    VerificationBySms = 10,
    VerificationByEmail = 15,
    VerificationByUsernameAndPassword = 20

}