using System;
using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsFactory;

    public ProfileService(UserManager<ApplicationUser> userManager,
                          IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory)
    {
        _userManager = userManager;
        _claimsFactory = claimsFactory;
    }

    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var userId = context.Subject.GetSubjectId();
        var user = await _userManager.FindByIdAsync(userId);

        var principal = await _claimsFactory.CreateAsync(user ?? throw new InvalidOperationException("User not found"));
        var claims = principal.Claims.ToList();


        var roles = await _userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(JwtClaimTypes.Role, r)));


        claims = claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
        context.IssuedClaims = claims;
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        var userId = context.Subject.GetSubjectId();
        context.IsActive = await _userManager.FindByIdAsync(userId) is not null;
    }

}
