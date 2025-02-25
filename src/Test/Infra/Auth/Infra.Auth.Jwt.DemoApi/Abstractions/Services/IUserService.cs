using Infra.Auth.Jwt.DemoApi.DTOs.User;

namespace Infra.Auth.Jwt.DemoApi.Abstractions.Services;

public interface IUserService
{
    bool IsValidUser(string userEmail, string password, out UserDto userDto);
}
