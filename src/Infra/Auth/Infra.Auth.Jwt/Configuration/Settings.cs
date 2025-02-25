namespace Infra.Auth.Jwt.Configuration;

public class Settings
{
    public const string SectionName = "Auth:Jwt";

    /// <summary>
    /// Access token secret
    /// </summary>
    public string AccessTokenSecret { get; set; }

    /// <summary>
    /// Access token expiration minutes
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; }

    /// <summary>
    /// Refresh token secret
    /// </summary>
    public string RefreshTokenSecret { get; set; }

    /// <summary>
    /// Refresh token expiration minutes
    /// </summary>
    public int RefreshTokenExpirationMinutes { get; set; }

    /// <summary>
    /// Issuer
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// Audience
    /// </summary>
    public string Audience { get; set; }
}
