using Infra.Auth.Jwt.DemoApi.DTOs.Role;

namespace Infra.Auth.Jwt.DemoApi.DTOs.User;

public class UserDto
{
    public string Id { get; set; }

    public string Email { get; set; }

    public RoleDto Role { get; set; }
}
