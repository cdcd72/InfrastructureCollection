using System.Security.Claims;
using Infra.Core.Auth.Models;

namespace Infra.Core.Auth.Abstractions;

public interface IAuthenticator
{
    /// <summary>
    /// 存取權杖過期分鐘
    /// </summary>
    int AccessTokenExpirationMinutes { get; }

    /// <summary>
    /// 產生權杖
    /// </summary>
    /// <param name="id">識別 ID</param>
    /// <param name="claims">宣告（其它補充資訊，不可放敏感資料）</param>
    /// <returns>存取、重新刷新權杖...等資訊</returns>
    TokenInfo GenerateToken(string id, List<Claim> claims = null);

    /// <summary>
    /// 刷新權杖
    /// </summary>
    /// <param name="refreshToken">重新刷新權杖</param>
    /// <param name="accessToken">存取權杖</param>
    /// <returns>存取、重新刷新權杖...等資訊</returns>
    TokenInfo RefreshToken(string refreshToken, string accessToken);

    /// <summary>
    /// 驗證重新刷新權杖
    /// </summary>
    /// <param name="refreshToken">重新刷新權杖</param>
    /// <returns></returns>
    bool ValidateRefreshToken(string refreshToken);

    /// <summary>
    /// 刪除重新刷新權杖
    /// </summary>
    /// <param name="id">識別 ID</param>
    void DeleteRefreshToken(string id);

    /// <summary>
    /// 將存取權杖加入至黑名單
    /// </summary>
    /// <param name="accessToken">存取權杖</param>
    void AddTokenToBlackList(string accessToken);
}
