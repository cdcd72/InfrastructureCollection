using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Infra.Auth.Jwt.Configuration;
using Infra.Core.Auth.Models.Claims;
using Microsoft.Extensions.Options;

namespace Infra.Auth.Jwt.Token;

public class AccessTokenHelper : TokenHelper
{
    private readonly Settings _settings;

    #region Properties

    public int ExpirationMinutes => _settings.AccessTokenExpirationMinutes;

    #endregion

    public AccessTokenHelper(IOptions<Settings> jwtConfig) => _settings = jwtConfig.Value;

    public string GenerateToken(string id, List<Claim> externalClaims = null)
    {
        var claims = new List<Claim>
        {
            new(CustomClaimTypes.Id, id)
        };

        if (externalClaims is not null && externalClaims.Count > 0)
            claims.AddRange(externalClaims);

        return base.GenerateToken(
            _settings.AccessTokenSecret,
            _settings.Issuer,
            _settings.Audience,
            _settings.AccessTokenExpirationMinutes,
            claims);
    }

    public (ClaimsPrincipal, JwtSecurityToken) DecodeToken(string token)
        => base.DecodeToken(
            _settings.AccessTokenSecret,
            _settings.Issuer,
            _settings.Audience,
            token);
}
