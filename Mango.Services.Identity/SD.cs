using Duende.IdentityServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Mango.Services.Identity
{
    public static class SD
    {
        public static string Admin = "Admin";
        public static string Customer = "Customer";

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource> { 
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope> {
                new ApiScope("mango","Mango Server"),
                new ApiScope(name:"read", displayName:"Read your data"),
                new ApiScope(name:"write", displayName:"Write your data"),
                new ApiScope(name:"delete", displayName:"Delete your data")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                { 
                  ClientId = "client",
                  ClientSecrets = { new Secret("secret.client".Sha256())},
                  AllowedScopes = { "read", "write", "profile"},
                  AllowedGrantTypes = GrantTypes.ClientCredentials
                },
                new Client
                { 
                    ClientId = "mango", 
                    ClientSecrets = new Secret[]{ new Secret("secret.mango".Sha256())},
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:44333/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:44333/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    { 
                        StandardScopes.OpenId,
                        StandardScopes.Profile,
                        StandardScopes.Email,
                        "mango"
                    }
                }
            };
    }
}
