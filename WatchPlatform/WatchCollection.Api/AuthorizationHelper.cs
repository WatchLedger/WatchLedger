using System.Security.Claims;

namespace WatchCollection.Api;

public static class AuthorizationHelper
{
    private const string IdentityServerIssuer = "https://identityserver-watchcollection.azurewebsites.net";
    
    public static bool HasWriteClaim(ClaimsPrincipal user)
    {
        var scopeClaim = user.Claims.FirstOrDefault(c => c.Type == "scope")?.Value;
        return scopeClaim?.Split(' ').Contains("WatchCollection.Api.Write") ?? false;
    }

    public static bool HasRole(ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }
}
