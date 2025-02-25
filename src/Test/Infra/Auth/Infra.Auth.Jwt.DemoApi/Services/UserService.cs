using System.Collections.Generic;
using Infra.Auth.Jwt.Constants;
using Infra.Auth.Jwt.DemoApi.Abstractions.Services;
using Infra.Auth.Jwt.DemoApi.DTOs.Role;
using Infra.Auth.Jwt.DemoApi.DTOs.User;
using Infra.Auth.Jwt.DemoApi.Entities;

namespace Infra.Auth.Jwt.DemoApi.Services;

public class UserService : IUserService
{
    private readonly Dictionary<string, User> users = new()
    {
        {
            "test@example.com",
            new User
            {
                Id = "ab2bd817-98cd-4cf3-a80a-53ea0cd9c200",
                Email = "test@gmail.com",
                Password = "test",
                Role = new Role
                {
                    Id = "fb2bd817-98cd-4cf3-a80a-53ea0cd9c20f",
                    Name = AuthConstant.User
                }
            }
        },
        {
            "admin@example.com",
            new User
            {
                Id = "cb2bd887-98cd-4cf3-a86a-53ea0cd9c200",
                Email = "admin@gmail.com",
                Password = "securePa55",
                Role = new Role
                {
                    Id = "eb2bd817-98cd-4cf3-a80a-53ea0cd9c20e",
                    Name = AuthConstant.Admin
                }
            }
        }
    };

    public bool IsValidUser(string userEmail, string password, out UserDto userDto)
    {
        userDto = null;

        if (!users.TryGetValue(userEmail, out var user))
            return false;

        var isValidUser = user.Password == password;

        if(isValidUser)
            userDto = GetUser(userEmail);

        return isValidUser;

    }

    private UserDto GetUser(string userEmail)
    {
        if (users.TryGetValue(userEmail, out var user))
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = new RoleDto
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name
                }
            };
        }

        return null;
    }
}
