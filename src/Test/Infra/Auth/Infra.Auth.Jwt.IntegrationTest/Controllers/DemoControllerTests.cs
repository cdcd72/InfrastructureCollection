using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Infra.Auth.Jwt.DemoApi.DTOs.Account;
using Infra.Core.Auth.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infra.Auth.Jwt.IntegrationTest.Controllers;

[TestClass]
public class DemoControllerTests
{
    private readonly HttpClient _client;

    public DemoControllerTests()
    {
        _client = new Api().CreateClient();
    }

    #region 情境 1：Action 沒有掛 [Authorize] 屬性，故有無驗證授權都可以打

    [TestMethod]
    public async Task GetNotLimitedSuccess()
    {
        var response = await _client.GetAsync("v1/api/Demo/situation/1");

        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.AreEqual("Not Authorize", responseString);
    }

    #endregion

    #region 情境 2：Action 有掛 [Authorize] 屬性，有驗證授權才可以打

    [TestMethod]
    public async Task GetLimitedFail()
    {
        var response = await _client.GetAsync("/v1/api/Demo/situation/2");

        // 401
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetLimitedSuccess()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "v1/api/Demo/situation/2");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);

        var response = await _client.SendAsync(request);

        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.AreEqual("Authorized", responseString);
    }

    #endregion

    #region 情境 3：Action 有掛 [Authorize(Roles = AuthConstant.Admin)] 屬性，有驗證授權才可以打

    [TestMethod]
    public async Task GetLimitedWithAdminRoleFail()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "/v1/api/Demo/situation/3");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);

        var response = await _client.SendAsync(request);

        // 403
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [TestMethod]
    public async Task GetLimitedWithAdminRoleSuccess()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "admin@example.com",
            Password = "securePa55"
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "/v1/api/Demo/situation/3");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);

        var response = await _client.SendAsync(request);

        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.AreEqual("Authorized With Role = Admin", responseString);
    }

    #endregion

    #region 情境 4：Action 有掛 [Authorize(Roles = $"{AuthConstant.Admin},{AuthConstant.User}")] 屬性，有驗證授權才可以打

    [TestMethod]
    public async Task GetLimitedWithAdminOrUserRoleWithAdminRoleSuccess()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "admin@example.com",
            Password = "securePa55"
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "/v1/api/Demo/situation/4");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);

        var response = await _client.SendAsync(request);

        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.AreEqual("Authorized With Role = Admin or User", responseString);
    }

    [TestMethod]
    public async Task GetLimitedWithAdminOrUserRoleWithUserRoleSuccess()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });

        var request = new HttpRequestMessage(HttpMethod.Get, "/v1/api/Demo/situation/4");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto?.AccessToken);

        var response = await _client.SendAsync(request);

        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.AreEqual("Authorized With Role = Admin or User", responseString);
    }

    #endregion

    #region 其它應用情境

    [TestMethod]
    public async Task LoginFail()
    {
        await Assert.ThrowsExceptionAsync<HttpRequestException>(() => Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "???"
        }));
    }

    [TestMethod]
    public async Task LoginSuccess()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });

        Assert.IsNotNull(accessDto.AccessToken);
        Assert.IsNotNull(accessDto.RefreshToken);
    }

    [TestMethod]
    public async Task OldAccessTokenForbiddenToAccessApiSuccess()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });

        // 已刷新權杖，故舊有存取權杖不能存取！
        await Refresh(new RefreshDto { RefreshToken = accessDto.RefreshToken }, accessDto.AccessToken);

        var request = new HttpRequestMessage(HttpMethod.Get, "v1/api/Demo/situation/2");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto.AccessToken);

        var response = await _client.SendAsync(request);

        // 401
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task NewAccessTokenCanAccessApiSuccess()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });

        // 已刷新權杖，故舊有存取權杖不能存取！
        var tokenInfo = await Refresh(new RefreshDto { RefreshToken = accessDto.RefreshToken }, accessDto.AccessToken);

        var request = new HttpRequestMessage(HttpMethod.Get, "v1/api/Demo/situation/2");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenInfo.AccessToken);

        var response = await _client.SendAsync(request);

        // 200
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();

        Assert.AreEqual("Authorized", responseString);
    }

    [TestMethod]
    public async Task LogoutSuccess()
    {
        var accessDto = await Login(new LoginDto
        {
            Email = "test@example.com",
            Password = "test"
        });

        // 已登出，故舊有存取權杖不能存取！
        await Logout(accessDto.AccessToken);

        var request = new HttpRequestMessage(HttpMethod.Get, "v1/api/Demo/situation/2");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessDto.AccessToken);

        var response = await _client.SendAsync(request);

        // 401
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Private Method

    private async Task<AccessDto> Login(LoginDto loginDto)
    {
        var response =
            await _client.PostAsync("/v1/api/Account/login",
                new StringContent(JsonSerializer.Serialize(loginDto), Encoding.UTF8, "application/json"));

        response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<AccessDto>(await response.Content.ReadAsStringAsync());
    }

    private async Task<TokenInfo> Refresh(RefreshDto refreshDto, string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "v1/api/Account/Refresh");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        request.Content = new StringContent(JsonSerializer.Serialize(refreshDto), Encoding.UTF8, "application/json");

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<TokenInfo>(await response.Content.ReadAsStringAsync());
    }

    private async Task Logout(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "v1/api/Account/Logout");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();
    }

    #endregion
}
