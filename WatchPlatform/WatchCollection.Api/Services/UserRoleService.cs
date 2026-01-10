using System.Security.Claims;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;

namespace WatchCollection.Api.Services;

public interface IUserRoleService
{
    Task<bool> UserHasRoleAsync(ClaimsPrincipal user, string role);
}

public class UserRoleService : IUserRoleService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<UserRoleService> _logger;

    public UserRoleService(IHttpContextAccessor httpContextAccessor, ILogger<UserRoleService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<bool> UserHasRoleAsync(ClaimsPrincipal user, string role)
    {
        var userId = user.FindFirst("sub")?.Value;
        if (userId is null)
            return false;

        try
        {
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync(
                "https://identityserver-watchcollection.azurewebsites.net");

            if (disco.IsError)
            {
                _logger.LogError("Discovery error: {error}", disco.Error);
                return false;
            }

            if (_httpContextAccessor.HttpContext is null)
                return false;

            var token = await _httpContextAccessor.HttpContext
                .GetTokenAsync("access_token");

            if (token is null)
                return false;

            var request = new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = token,
            };

            var userInfo = await client.GetUserInfoAsync(request);

            if (userInfo.IsError)
            {
                _logger.LogError("UserInfo error: {error}", userInfo.Error);
                return false;
            }

            return userInfo.Claims.Any(c => c.Type == "role" && c.Value == role);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking user role");
            return false;
        }
    }
}
