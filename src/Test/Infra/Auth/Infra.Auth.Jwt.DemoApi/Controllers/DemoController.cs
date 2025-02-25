using Infra.Auth.Jwt.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Infra.Auth.Jwt.DemoApi.Controllers;

public class DemoController : BaseApiController
{
    [HttpGet("situation/1")]
    public string GetNotLimited()
        => "Not Authorize";

    [Authorize]
    [HttpGet("situation/2")]
    public string GetLimited()
        => "Authorized";

    [Authorize(Roles = AuthConstant.Admin)]
    [HttpGet("situation/3")]
    public string GetLimitedWithAdminRole()
        => "Authorized With Role = Admin";

    [Authorize(Roles = $"{AuthConstant.Admin},{AuthConstant.User}")]
    [HttpGet("situation/4")]
    public string GetLimitedWithAdminOrUserRole()
        => "Authorized With Role = Admin or User";
}
