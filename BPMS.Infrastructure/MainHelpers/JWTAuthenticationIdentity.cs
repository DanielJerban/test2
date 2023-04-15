using System.Security.Principal;

namespace BPMS.Infrastructure.MainHelpers;

public class JwtAuthenticationIdentity : GenericIdentity
{

    public string UserName { get; set; }
    public string UserId { get; set; }
    public string Roles { get; set; }
    public string SerialNumber { get; set; }

    public JwtAuthenticationIdentity(string userName)
        : base(userName)
    {
        UserName = userName;
    }
}