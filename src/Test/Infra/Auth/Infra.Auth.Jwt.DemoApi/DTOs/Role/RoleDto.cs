using System.Text.Json.Serialization;

namespace Infra.Auth.Jwt.DemoApi.DTOs.Role;

public class RoleDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
