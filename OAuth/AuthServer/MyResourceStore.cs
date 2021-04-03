using AuthServer.Configuration;
using ExpressData;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer
{
    public class MyResourceStore : IResourceStore
    {
        private readonly IConfiguration config;
        private readonly string connectionString;

        public MyResourceStore(IConfiguration config)
        {
            this.config = config;
            this.connectionString = config.GetConnectionString("AuthConfigDatabase");
        }

        public async Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var apis = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources WHERE Name='{apiResourceNames}' AND IsActive=1", connectionString);
            if (apis != null)
            {
                var result = new List<IdentityServer4.Models.ApiResource>();
                foreach (var api in apis)
                {
                    var availableScopes = new List<string>() { "openid", "profile" };
                    availableScopes.AddRange(api.SupportedScopes.Split(",").ToList());
                    result.Add(new IdentityServer4.Models.ApiResource
                    {
                        Name = api.Name,
                        DisplayName = api.DisplayName,
                        Scopes = availableScopes
                    });
                }
                return result;
            }
            return null;
        }

        public async Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopesList)
        {
            var scopeNames = scopesList.ToList();
            var likeStatements = "";
            for (var i = 0; i < scopeNames.Count(); i++)
            {
                if (i == scopeNames.Count() - 1)
                {
                    likeStatements += $"SupportedScopes LIKE '%{scopeNames[i]}%'";
                }
                else
                {
                    likeStatements += $"SupportedScopes LIKE '%{scopeNames[i]}%' OR ";
                }
            }
            var apis = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources WHERE ({likeStatements}) AND IsActive=1", connectionString);
            if (apis != null)
            {
                var result = new List<IdentityServer4.Models.ApiResource>();
                foreach (var api in apis)
                {
                    var availableScopes = new List<string>() { "openid", "profile" };
                    availableScopes.AddRange(api.SupportedScopes.Split(",").ToList());
                    result.Add(new IdentityServer4.Models.ApiResource
                    {
                        Name = api.Name,
                        DisplayName = api.DisplayName,
                        Scopes = availableScopes
                    });
                }
                return result;
            }
            return null;
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopesList)
        {
            var scopeNames = scopesList.ToList();
            var likeStatements = "";
            for (var i = 0; i < scopeNames.Count(); i++)
            {
                if (i == scopeNames.Count() - 1)
                {
                    likeStatements += $"ScopeName='{scopeNames[i]}'";
                }
                else
                {
                    likeStatements += $"ScopeName='{scopeNames[i]}' OR ";
                }
            }
            var scopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ({likeStatements})", connectionString);
            if (scopes != null)
            {
                var result = new List<IdentityServer4.Models.ApiScope>();
                foreach (var scope in scopes)
                {
                    result.Add(new IdentityServer4.Models.ApiScope
                    {
                        Name = scope.ScopeName,
                        DisplayName = scope.ScopeDescription
                    });
                }
                return result;
            }
            return null;
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return new List<IdentityResource>
             {
                  new IdentityResources.OpenId(),
                  new IdentityResources.Profile()
             };
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var allResources = new Resources();
            allResources.IdentityResources =
             new List<IdentityResource>
             {
                  new IdentityResources.OpenId(),
                  new IdentityResources.Profile()
             };
            var apis = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources WHERE IsActive=1", connectionString);
            if (apis != null)
            {
                var result = new List<IdentityServer4.Models.ApiResource>();
                foreach (var api in apis)
                {
                    var availableScopes = new List<string>() { "openid", "profile" };
                    availableScopes.AddRange(api.SupportedScopes.Split(",").ToList());
                    result.Add(new IdentityServer4.Models.ApiResource
                    {
                        Name = api.Name,
                        DisplayName = api.DisplayName,
                        Scopes = availableScopes
                    });
                }
                allResources.ApiResources = result;
            }

            var scopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes", connectionString);
            if (scopes != null)
            {
                var result = new List<IdentityServer4.Models.ApiScope>();
                foreach (var scope in scopes)
                {
                    result.Add(new IdentityServer4.Models.ApiScope
                    {
                        Name = scope.ScopeName,
                        DisplayName = scope.ScopeDescription
                    });
                }
                allResources.ApiScopes = result;
            }

            return allResources;
        }
    }
}