using System.Security.Claims;
using Infra.Auth.Jwt.Common;
using Infra.Auth.Jwt.Token;
using Infra.Core.Auth.Abstractions;
using Infra.Core.Auth.Models;
using Infra.Core.Auth.Models.Claims;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Infra.Auth.Jwt;

public class JwtAuthenticator : IAuthenticator
{
    private readonly AccessTokenHelper _accessTokenHelper;
    private readonly RefreshTokenHelper _refreshTokenHelper;
    private readonly IMemoryCache _cache;

    #region Properties

    public int AccessTokenExpirationMinutes { get; set; }

    #endregion

    public JwtAuthenticator(
        AccessTokenHelper accessTokenHelper,
        RefreshTokenHelper refreshTokenHelper,
        IMemoryCache cache)
    {
        _accessTokenHelper = accessTokenHelper;
        _refreshTokenHelper = refreshTokenHelper;
        _cache = cache;

        AccessTokenExpirationMinutes = _accessTokenHelper.ExpirationMinutes;
    }

    public TokenInfo GenerateToken(string id, List<Claim> claims = null)
    {
        var accessToken = _accessTokenHelper.GenerateToken(id, claims);
        var refreshToken = _refreshTokenHelper.GenerateToken();

        // Refresh token can save in database or distributed cache, but use memory cache for demo...
        // This situation represent one user can hold one refresh token...
        _cache.Set(id, refreshToken);

        return new TokenInfo
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public bool ValidateRefreshToken(string refreshToken)
        => _refreshTokenHelper.ValidateToken(refreshToken);

    public TokenInfo RefreshToken(string refreshToken, string accessToken)
    {
        var (principal, token) = _accessTokenHelper.DecodeToken(accessToken);

        if (token?.Header.Alg is not SecurityAlgorithms.HmacSha256)
            throw new SecurityTokenException("無效權杖演算法！");

        var id = principal.FindFirstValue(CustomClaimTypes.Id);

        if (string.IsNullOrWhiteSpace(id))
            throw new SecurityTokenException("無效識別 ID！");

        if (!_cache.TryGetValue(id, out string existingRefreshToken))
            throw new SecurityTokenException("未找到重新刷新權杖！");

        if (refreshToken != existingRefreshToken)
            throw new SecurityTokenException("無效權杖！");

        return GenerateToken(id, principal.Claims.Where(claim => claim.Type != CustomClaimTypes.Id).ToList());
    }

    public void DeleteRefreshToken(string id)
    {
        if (_cache.TryGetValue(id, out string _))
            _cache.Remove(id);
    }

    public void AddTokenToBlackList(string accessToken)
    {
        _cache.TryGetValue(CacheKeys.TokenBlackList, out List<string> tokenBlackList);

        tokenBlackList ??= new List<string>();

        tokenBlackList.Add(accessToken);

        _cache.Remove(CacheKeys.TokenBlackList);

        _cache.Set(CacheKeys.TokenBlackList, tokenBlackList, TimeSpan.FromMinutes(AccessTokenExpirationMinutes));
    }
}
