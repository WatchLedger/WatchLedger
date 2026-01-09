using System.Security.Claims;

namespace WatchCollection.Api;

public static class AuthorizationHelper
{
    private const string IdentityServerIssuer = "https://identityserver-watchcollection.azurewebsites.net";
    
    public static bool HasWriteClaim(ClaimsPrincipal user)
    {
        return user.Claims.Any(c => 
            c.Type == "WatchCollection.Api.Write" && 
            c.Issuer == IdentityServerIssuer);
    }

    public static bool HasRole(ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }
}
