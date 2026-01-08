using System;
using Microsoft.AspNetCore.Authorization;

namespace WatchCollection.Api;

public class ClaimOrRoleRequirement : IAuthorizationRequirement
{
    public string Claim { get; }
    public string Role { get; }

    public ClaimOrRoleRequirement(string claim, string role)
    {
        Claim = claim;
        Role = role;
    }
}
