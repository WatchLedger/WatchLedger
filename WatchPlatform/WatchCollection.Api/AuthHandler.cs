using System;
using System.Security.Claims;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace WatchCollection.Api;

public class AuthHandler : AuthorizationHandler<ClaimOrRoleRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public AuthHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ClaimOrRoleRequirement requirement)
    {
        var claims = context.User.Claims.ToList();

        // check if the claim is present
        if (!claims.Exists(c => c.Value == requirement.Claim))
            return Task.CompletedTask; // claim not found, do not succeed

        var userId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (userId is null)
        {
            // no user id found, do not succeed
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        // Get roles from idserver
        var client = new HttpClient();
        var disco  = client.GetDiscoveryDocumentAsync(
            "https://identityserver-watchcollection.azurewebsites.net").Result;

        var token = _httpContextAccessor.HttpContext?
            .GetTokenAsync("access_token").Result;

        var request = new UserInfoRequest
        {
            Address = disco.UserInfoEndpoint,
            Token   = token
        };

        var userInfo   = client.GetUserInfoAsync(request).Result;
        var userClaims = userInfo.Claims.ToList();

        var isAdmin = userClaims.Exists(c => c.Type == "role" && c.Value == "Admin");
        var isUser  = userClaims.Exists(c => c.Type == "role" && c.Value == "User");

        //  Check which role is required
        var requiresAdmin        = requirement.Role == "Admin";
        var requiresUser         = requirement.Role == "User";
        var requiresAdminOrUser  = requirement.Role == "AdminOrUser";

        // conditional succeed
        if ((requiresAdmin && isAdmin) ||
            (requiresUser && isUser) ||
            (requiresAdminOrUser && (isAdmin || isUser)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
