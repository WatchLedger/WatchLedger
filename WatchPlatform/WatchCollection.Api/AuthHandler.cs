using System;
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

        // Als de app client de juiste _apiscope_ niet heeft...
        if (!claims.Exists(c => c.Value == requirement.Claim))
        {
            // ...is het NOT OK.
            context.Fail();
        }
        // Als de app client de juiste scope wel heeft...
        else
        {
            var userId = claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            // ...en de call gebeurt door een _persoon_...
            if (userId is not null)
            {
                // ... dan vragen we meer info aan identity server...
                var client = new HttpClient();
                var disco = client
                    .GetDiscoveryDocumentAsync("https://localhost:5001").Result;

                var token = _httpContextAccessor?
                    .HttpContext?.GetTokenAsync("access_token").Result ;

                var request = new UserInfoRequest
                {
                    Address = disco.UserInfoEndpoint,
                    Token = token,

                };

                var userInfo = client.GetUserInfoAsync(request).Result;

                // ... en de persoon heeft de juiste _rol_...
                if (userInfo.Claims.ToList()
                    .Exists(c => c.Type == "role" && c.Value == requirement.Role))
                {
                    // ...dan is het OK.
                    context.Succeed(requirement);
                }
                // ... en de persoon heeft de juiste _rol_ niet
                else
                {
                    // ...dan is het NOT OK.
                    context.Fail();
                }
            }
            // ...en de call gebeurt door een _systeem_...
            else
            {
                // ...dan is het OK.
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
