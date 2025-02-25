using System.ComponentModel.DataAnnotations;

namespace Infra.Auth.Jwt.DemoApi.DTOs.Account;

public class RefreshDto
{
    [Required]
    public string RefreshToken { get; set; }
}
