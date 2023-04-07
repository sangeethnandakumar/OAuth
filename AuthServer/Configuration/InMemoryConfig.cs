using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthServer.Configuration
{
    public static class InMemoryConfig
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
          new List<IdentityResource>
          {
              new IdentityResources.OpenId(),
              new IdentityResources.Profile()
          };

        public static IEnumerable<ApiScope> GetApiScopes() =>
           new List<ApiScope> {
               new ApiScope("identity", "CompanyEmployee API"),
               new ApiScope("firstname", "CompanyEmployee API"),
               new ApiScope("lastname", "CompanyEmployee API"),
               new ApiScope("email", "CompanyEmployee API"),
               new ApiScope("username", "CompanyEmployee API"),
           };

        public static IEnumerable<IdentityServer4.Models.ApiResource> GetApiResources() =>
            new List<IdentityServer4.Models.ApiResource>
            {
                new IdentityServer4.Models.ApiResource("companyApi", "CompanyEmployee API")
                {
                    Scopes = { "companyApi" }
                }
            };

        public static List<TestUser> GetUsers() =>
          new List<TestUser>
          {
              new TestUser
              {
                  SubjectId = "a9ea0f25-b964-409f-bcce-c923266249b4",
                  Username = "Mick",
                  Password = "MickPassword",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "Mick"),
                      new Claim("family_name", "Mining")
                  }
              },
              new TestUser
              {
                  SubjectId = "c95ddb8c-79ec-488a-a485-fe57a1462340",
                  Username = "Jane",
                  Password = "JanePassword",
                  Claims = new List<Claim>
                  {
                      new Claim("given_name", "Jane"),
                      new Claim("family_name", "Downing")
                  }
              }
          };

        public static IEnumerable<IdentityServer4.Models.Client> GetClients() =>
            new List<IdentityServer4.Models.Client>
            {
                   new IdentityServer4.Models.Client
                   {
                        ClientId = "company-employee",
                        ClientSecrets = new [] { new Secret("codemazesecret".Sha512()) },
                        AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                        AllowedScopes = { IdentityServerConstants.StandardScopes.OpenId, "companyApi" }
                   },
                   new IdentityServer4.Models.Client
                   {
                       ClientName = "MVC Client",
                       ClientId = "mvc-client",
                       AllowedGrantTypes = GrantTypes.Code,
                       RedirectUris = new List<string>{ "https://localhost:44326/signin-oidc" },
                       RequirePkce = false,
                       AllowedScopes = {
                           IdentityServerConstants.StandardScopes.OpenId,
                           IdentityServerConstants.StandardScopes.Profile,
                           "identity",
                           "firstname",
                           "lastname",
                           "email",
                           "username"
                           },
                       ClientSecrets = { new Secret("MVCSecret".Sha512()) },
                       AccessTokenLifetime = 5,
                       IdentityTokenLifetime = 5,
                       UpdateAccessTokenClaimsOnRefresh = true,
                       AlwaysIncludeUserClaimsInIdToken = true,
                       PostLogoutRedirectUris = new List<string> { "https://localhost:44326/signout-callback-oidc" }
                   }
            };
    }
}