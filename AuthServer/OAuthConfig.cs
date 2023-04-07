using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace AuthServer
{
    public class ApiResource2
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<string> Scopes { get; set; }
    }

    public class Client
    {
        public string ClientName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string GrandType { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public string RedirectUri { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public List<string> AllowedScopes { get; set; }
        public List<string> AllowedCrossOrgins { get; set; }
    }

    public class OAuthConfig
    {
        public int IdentityServerCookieLifetime { get; set; }
        public List<ApiResource2> ApiResources { get; set; }
        public List<string> ApiScopes { get; set; }
        public List<Client> Clients { get; set; }

        public IEnumerable<IdentityServer4.Models.ApiResource> GetApiResources()
        {
            var result = new List<IdentityServer4.Models.ApiResource>();
            foreach (var res in ApiResources)
            {
                result.Add(new IdentityServer4.Models.ApiResource
                {
                    Name = res.Name,
                    DisplayName = res.DisplayName,
                    Scopes = res.Scopes
                });
            }
            return result;
        }

        public IEnumerable<IdentityResource> GetIdentityResources() =>
        new List<IdentityResource>
        {
              new IdentityResources.OpenId(),
              new IdentityResources.Profile()
        };

        public IEnumerable<IdentityServer4.Models.ApiScope> GetApiScopes()
        {
            var result = new List<IdentityServer4.Models.ApiScope>();
            foreach (var scope in ApiScopes)
            {
                result.Add(new IdentityServer4.Models.ApiScope(scope, Guid.NewGuid().ToString()));
            }
            return result;
        }

        public IEnumerable<IdentityServer4.Models.Client> GetClients()
        {
            var result = new List<IdentityServer4.Models.Client>();
            foreach (var client in Clients)
            {
                var allowedScopes = new List<string>() { "openid", "profile" };
                allowedScopes.AddRange(client.AllowedScopes);
                switch (client.GrandType)
                {
                    case "code":
                        result.Add(new IdentityServer4.Models.Client
                        {
                            ClientName = client.ClientName,
                            ClientId = client.ClientId,
                            AllowedGrantTypes = GrantTypes.Code,
                            RedirectUris = new List<string> { client.RedirectUri },
                            AllowedScopes = allowedScopes,
                            ClientSecrets = { new Secret(client.ClientSecret.Sha512()) },
                            AccessTokenLifetime = client.AccessTokenLifetime,
                            IdentityTokenLifetime = client.IdentityTokenLifetime,
                            RequirePkce = false,
                            UpdateAccessTokenClaimsOnRefresh = true,
                            AlwaysIncludeUserClaimsInIdToken = true,
                            PostLogoutRedirectUris = new List<string> { client.PostLogoutRedirectUri },
                        });
                        break;

                    case "client_credentials":
                        result.Add(new IdentityServer4.Models.Client
                        {
                            ClientId = client.ClientId,
                            AllowedGrantTypes = GrantTypes.ClientCredentials,
                            ClientSecrets =
                            {
                                new Secret(client.ClientSecret.Sha256())
                            },
                            AllowedScopes = allowedScopes,
                            AccessTokenLifetime = client.AccessTokenLifetime,
                            IdentityTokenLifetime = client.IdentityTokenLifetime
                        });
                        break;

                    case "implicit":
                        result.Add(new IdentityServer4.Models.Client
                        {
                            ClientId = client.ClientId,
                            ClientName = client.ClientName,
                            AllowedGrantTypes = GrantTypes.Implicit,
                            AllowAccessTokensViaBrowser = true,
                            AllowedCorsOrigins = client.AllowedCrossOrgins,
                            AllowRememberConsent = true,
                            AllowedScopes = allowedScopes,
                            RedirectUris = { client.RedirectUri },
                            PostLogoutRedirectUris = { client.PostLogoutRedirectUri }
                        });
                        break;

                    default:
                        break;
                }
            }
            return result;
        }
    }
}