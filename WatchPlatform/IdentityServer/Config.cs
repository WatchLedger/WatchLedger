using Duende.IdentityServer.Models;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("WatchCollection.Api"),
            new ApiScope("WatchValuation.Api"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // postman => watchcollection api
            new Client
            {
                ClientId = "m2m.postman",
                ClientName = "Postman-Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                AllowedScopes = { "WatchCollection.Api", "WatchValuation.Api"}
            }
        };
}