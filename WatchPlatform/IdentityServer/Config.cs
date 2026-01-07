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
                ClientSecrets = { new Secret("PostmanSecretWoohoo".Sha256()) },

                AllowedScopes = { "WatchCollection.Api", "WatchValuation.Api"}
            },
            new Client{
                ClientId = "m2m.WatchCollection-WatchValuation",
                ClientName = "WatchCollection to WatchValuation Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("WatchCollectionSecretWoohoo".Sha256()) },

                AllowedScopes = { "WatchValuation.Api"}
            }
        };
}