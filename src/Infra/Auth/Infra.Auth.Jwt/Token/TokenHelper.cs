using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

#pragma warning disable CA1822

namespace Infra.Auth.Jwt.Token;

public class TokenHelper
{
    protected string GenerateToken(
        string secret, string issuer, string audience, int expireMinutes, IEnumerable<Claim> claims = null)
    {
        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMinutes(expireMinutes),
            credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected (ClaimsPrincipal, JwtSecurityToken) DecodeToken(
        string secret, string issuer, string audience, string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new SecurityTokenException("無效權杖！");

        var principal = new JwtSecurityTokenHandler()
            .ValidateToken(token,
                new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false
                },
                out var validatedToken);

        return (principal, validatedToken as JwtSecurityToken);
    }

    protected bool ValidateToken(string secret, string issuer, string audience, string token)
    {
        try
        {
            DecodeToken(secret, issuer, audience, token);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
