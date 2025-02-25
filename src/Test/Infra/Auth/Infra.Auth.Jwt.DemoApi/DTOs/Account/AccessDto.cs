using System.Text.Json.Serialization;
using Infra.Auth.Jwt.DemoApi.DTOs.Role;

namespace Infra.Auth.Jwt.DemoApi.DTOs.Account;

public class AccessDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("role")]
    public RoleDto Role { get; set; }

    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}
