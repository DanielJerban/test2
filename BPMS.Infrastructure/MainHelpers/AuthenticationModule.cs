using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BPMS.Infrastructure.MainHelpers;

public class AuthenticationModule
{
    private const string CommunicationKey = "GQDsrahianenoortcd21d{df}ewfbbbbbbbbbbbFiwdfDffVvVBrk%$3";
    readonly SymmetricSecurityKey _signingKey = new(Encoding.UTF8.GetBytes(CommunicationKey));

    private const string EncriptionKey = "RzvS#inaFa*tem%i";
    readonly SymmetricSecurityKey _encriptKey = new(Encoding.ASCII.GetBytes(EncriptionKey));

    public string GenerateTokenForUser(string userName, string userId, string roles, Guid serialNumber)
    {
        var now = DateTime.UtcNow;
        var signingCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256Signature);
        var encryptingCredentials = new EncryptingCredentials(_encriptKey, SecurityAlgorithms.Aes128KW,SecurityAlgorithms.Aes128CbcHmacSha256);

        var claimsIdentity = new ClaimsIdentity(new List<Claim>()
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Role, roles),
            new Claim(ClaimTypes.SerialNumber, serialNumber.ToString()),
        }, "Custom");
        var securityTokenDescriptor = new SecurityTokenDescriptor()
        {                
            Audience = "Mobile,Aoutomation",
            Issuer = "self",
            Subject = claimsIdentity,
            SigningCredentials = signingCredentials,
            Expires =  now.AddYears(1),
            EncryptingCredentials = encryptingCredentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
        var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

        return signedAndEncodedToken;

    }

    /// Using the same key used for signing token, user payload is generated back
    public JwtSecurityToken GenerateUserClaimFromJWT(string authToken)
    {

        var validationParameters = new TokenValidationParameters
        {
            ClockSkew = TimeSpan.Zero, // default: 5 min
            RequireSignedTokens = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(CommunicationKey)),

            RequireExpirationTime = true,
            ValidateLifetime = true,

            ValidateAudience = true, //default : false
            ValidAudience = "Mobile,Aoutomation",

            ValidateIssuer = true, //default : false
            ValidIssuer = "self",

            TokenDecryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EncriptionKey))
        };
        var tokenHandler = new JwtSecurityTokenHandler();

        SecurityToken validatedToken;

        try
        {

            tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
        }
        catch (Exception)
        {
            return null;

        }

        return validatedToken as JwtSecurityToken;

    }

    public JwtAuthenticationIdentity PopulateUserIdentity(JwtSecurityToken userPayloadToken)
    {
        string name = ((userPayloadToken)).Claims.FirstOrDefault(m => m.Type == "unique_name").Value;
        string userId = ((userPayloadToken)).Claims.FirstOrDefault(m => m.Type == "nameid").Value;
        string role = ((userPayloadToken)).Claims.FirstOrDefault(m => m.Type == "role").Value;
        string serialnumber = ((userPayloadToken)).Claims.FirstOrDefault(m => m.Type == "certserialnumber").Value;
        return new JwtAuthenticationIdentity(name) { UserId = userId, UserName = name, Roles = role, SerialNumber = serialnumber};

    }
}