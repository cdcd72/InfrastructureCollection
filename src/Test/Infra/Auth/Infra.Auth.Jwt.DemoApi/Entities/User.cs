namespace Infra.Auth.Jwt.DemoApi.Entities;

public class User : BaseEntity
{
    public string Email { get; set; }

    public string Password { get; set; }

    public Role Role { get; set; }
}