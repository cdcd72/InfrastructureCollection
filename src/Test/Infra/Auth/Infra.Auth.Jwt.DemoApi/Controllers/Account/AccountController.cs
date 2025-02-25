using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Infra.Auth.Jwt.DemoApi.Abstractions.Services;
using Infra.Auth.Jwt.DemoApi.DTOs.Account;
using Infra.Auth.Jwt.DemoApi.DTOs.Role;
using Infra.Core.Auth.Abstractions;
using Infra.Core.Auth.Models.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infra.Auth.Jwt.DemoApi.Controllers.Account;

public class AccountController : BaseApiController
{
    private readonly IUserService _userService;
    private readonly IAuthenticator _authenticator;

    #region Constructor

    public AccountController(
        IUserService userService,
        IAuthenticator authenticator)
    {
        _userService = userService;
        _authenticator = authenticator;
    }

    #endregion

    [AllowAnonymous]
    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginDto loginInfo)
    {
        if (!_userService.IsValidUser(loginInfo.Email, loginInfo.Password, out var user))
            return BadRequest("Invalid password!");

        if (user is null)
            return BadRequest("User not found!");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.Name)
        };

        var tokenDto = _authenticator.GenerateToken(user.Id, claims);

        return Ok(new AccessDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = new RoleDto
            {
                Id = user.Role.Id,
                Name = user.Role.Name
            },
            AccessToken = tokenDto.AccessToken,
            ExpiresIn = _authenticator.AccessTokenExpirationMinutes * 60 * 1000,
            RefreshToken = tokenDto.RefreshToken,
        });
    }

    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshDto refreshInfo)
    {
        if (!_authenticator.ValidateRefreshToken(refreshInfo.RefreshToken))
            return BadRequest("Invalid refresh token!");

        var accessToken =
            await HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");

        var tokenDto = _authenticator.RefreshToken(refreshInfo.RefreshToken, accessToken);

        _authenticator.AddTokenToBlackList(accessToken);

        return Ok(tokenDto);
    }

    [Authorize]
    [HttpDelete("logout")]
    public async Task<ActionResult> Logout()
    {
        var userId = HttpContext.User.FindFirstValue(CustomClaimTypes.Id);
        
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest("Invalid user id!");

        _authenticator.DeleteRefreshToken(userId);

        var accessToken =
            await HttpContext.GetTokenAsync(JwtBearerDefaults.AuthenticationScheme, "access_token");

        _authenticator.AddTokenToBlackList(accessToken);

        return Ok();
    }
}
