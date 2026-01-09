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
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ClaimOrRoleRequirement requirement)
    {
        var claims = context.User.Claims.ToList();

        if (!claims.Exists(c => c.Value == requirement.Claim))
        {
            context.Fail();
        }
        else
        {
            var userId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            if (userId is not null)
            {
                var client = new HttpClient();
                var disco = client
                    .GetDiscoveryDocumentAsync("https://identityserver-watchcollection.azurewebsites.net").Result;

                var token = _httpContextAccessor?
                    .HttpContext?.GetTokenAsync("access_token").Result ;

                var request = new UserInfoRequest
                {
                    Address = disco.UserInfoEndpoint,
                    Token = token,

                };

                var userInfo = client.GetUserInfoAsync(request).Result;

                if (userInfo.Claims.ToList()
                    .Exists(c => c.Type == "role" && c.Value == requirement.Role))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
