using Infra.Auth.Jwt.Configuration;
using Microsoft.Extensions.Options;

namespace Infra.Auth.Jwt.Token;

public class RefreshTokenHelper : TokenHelper
{
    private readonly Settings _settings;

    public RefreshTokenHelper(IOptions<Settings> jwtConfig) => _settings = jwtConfig.Value;

    public string GenerateToken()
        => base.GenerateToken(
            _settings.RefreshTokenSecret,
            _settings.Issuer,
            _settings.Audience,
            _settings.RefreshTokenExpirationMinutes);

    public bool ValidateToken(string token)
        => base.ValidateToken(
            _settings.RefreshTokenSecret,
            _settings.Issuer,
            _settings.Audience,
            token);
}
