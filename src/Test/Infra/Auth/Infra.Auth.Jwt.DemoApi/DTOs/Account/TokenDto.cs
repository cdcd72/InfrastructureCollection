using System.Text.Json.Serialization;

namespace Infra.Auth.Jwt.DemoApi.DTOs.Account;

public class TokenDto
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
}
