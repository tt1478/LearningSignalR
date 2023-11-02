using IdentityServer4.Models;

namespace AuthProvider
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> { "role" }
                }
            };
        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("SignalR.read"),
                new ApiScope("SignalR.write"),
                new ApiScope("UnitTesting")

            };
        public static IEnumerable<ApiResource> ApiResources => 
            new[] 
            {
                new ApiResource() 
                {
                    Name = "SignalR",
                    Scopes = new List<string>() { "SignalR.read", "SignalR.write" },
                    ApiSecrets = new List<Secret>() { new Secret("ScopedSecret".Sha256())  },
                    UserClaims = new List<string>() { "role" }
                }
            };
        public static IEnumerable<Client> Clients =>
            new[]
            {
                 new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedScopes = { "SignalR.read", "SignalR.write" }
                },
                 new Client
                {
                    ClientId = "unitTesting",
                    ClientName = "unitTesting Client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedScopes = { "UnitTesting" }
                },
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "SignalR1",
                    ClientSecrets = { new Secret("ClientSecret1".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:7142/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:7142/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:7142/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "SignalR.read" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false
                },
                new Client
                {
                    ClientId = "SignalR2",
                    ClientSecrets = { new Secret("ClientSecret2".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:7186/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:7186/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:7186/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "SignalR.read" },
                    RequirePkce = true,
                    RequireConsent = true,
                    AllowPlainTextPkce = false
                },
            };
    }
}
