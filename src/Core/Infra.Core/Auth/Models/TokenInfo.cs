using System.Text.Json.Serialization;

namespace Infra.Core.Auth.Models;

public class TokenInfo
{
    [JsonPropertyName("AccessToken")]
    public string AccessToken { get; set; }

    [JsonPropertyName("RefreshToken")]
    public string RefreshToken { get; set; }
}
