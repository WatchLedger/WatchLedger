using Duende.IdentityServer;
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
            new ApiScope("WatchCollection.Api.Read"),
            new ApiScope("WatchCollection.Api.Write"),
            new ApiScope("WatchValuation.Api.Read"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // postman => [watchcollection api, watchvaluation api]
            new Client
            {
                ClientId = "m2m.postman",
                ClientName = "Postman-Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("PostmanSecretWoohoo".Sha256()) },

                AllowedScopes = { "WatchCollection.Api.Read", "WatchCollection.Api.Write", "WatchValuation.Api.Read"}
            },
            // watchcollection api => watchvaluation api
            new Client{
                ClientId = "m2m.WatchCollection-WatchValuation",
                ClientName = "WatchCollection to WatchValuation Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("WatchCollectionSecretWoohoo".Sha256()) },

                AllowedScopes = { "WatchValuation.Api.Read"}
            },
            // Frontend application => watchcollection api
            new Client{
                ClientId = "m2m.webappp-watchcollection",
                ClientName = "Frontend to WatchCollection and WatchValuation Client",

                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = { new Secret("FrontendSecretWoohoo".Sha256()) },
                AllowedScopes = { 
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "WatchCollection.Api.Read",
                        "WatchCollection.Api.Write"
                    },
                RedirectUris = { "http://localhost:5174/", "https://watchplatform.nathangeleyn.com/" },
                PostLogoutRedirectUris = { "http://localhost:5174/", "https://watchplatform.nathangeleyn.com/" },
                AllowedCorsOrigins = { "http://localhost:5174", "https://watchplatform.nathangeleyn.com" }
            }
        };
}