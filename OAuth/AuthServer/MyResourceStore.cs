using AuthServer.Configuration;
using ExpressData;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer
{
    public class MyResourceStore : IResourceStore
    {
        public async Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var connectionString = @"Server=DESKTOP-QJ02OLT\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var apis = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources WHERE Name='{apiResourceNames}' AND IsActive=1", connectionString);
            if (apis != null)
            {
                var result = new List<IdentityServer4.Models.ApiResource>();
                foreach (var api in apis)
                {
                    result.Add(new IdentityServer4.Models.ApiResource
                    {
                        Name = api.Name,
                        DisplayName = api.DisplayName,
                        Scopes = api.SupportedScopes.Split(",").ToList()
                    });
                }
                return result;
            }
            return null;
        }

        public async Task<IEnumerable<IdentityServer4.Models.ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var connectionString = @"Server=DESKTOP-QJ02OLT\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var apis = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources WHERE SupportedScopes LIKE '%{scopeNames}%' AND IsActive=1", connectionString);
            if (apis != null)
            {
                var result = new List<IdentityServer4.Models.ApiResource>();
                foreach (var api in apis)
                {
                    result.Add(new IdentityServer4.Models.ApiResource
                    {
                        Name = api.Name,
                        DisplayName = api.DisplayName,
                        Scopes = api.SupportedScopes.Split(",").ToList()
                    });
                }
                return result;
            }
            return null;
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            throw new NotImplementedException();
        }
    }
}