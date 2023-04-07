using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twileloop.UOW;

namespace AuthServer {
    public class MyResourceStore : IResourceStore {
        private readonly IOptions<DbConfig> dbOptions;

        public MyResourceStore(IOptions<DbConfig> dbOptions) {
            this.dbOptions = dbOptions;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames) {

            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                var resourcesRepo = uow.GetRepository<ApiResource>();
                return resourcesRepo.Find(x => apiResourceNames.Contains(x.Name)).ToList();
            }

        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopesList) {
            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                var resourcesRepo = uow.GetRepository<ApiResource>();

                var allResources = resourcesRepo.GetAll();
                var filteredResources = new List<ApiResource>();

                foreach (var resource in allResources) {
                    if (resource.Scopes != null && resource.Scopes.Any(s => scopesList.Contains(s))) {
                        filteredResources.Add(resource);
                    }
                }

                return filteredResources;
            }
        }


        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopesList) {

            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                var scopeRepo = uow.GetRepository<ApiScope>();
                return scopeRepo.Find(x => scopesList.Contains(x.Name)).ToList();
            }

        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames) {

            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                var identityRepo = uow.GetRepository<IdentityResource>();
                var scopes =  identityRepo.Find(x => scopeNames.Contains(x.Name)).ToList();
                return scopes;
            }

        }

        public async Task<Resources> GetAllResourcesAsync() {

            var allResources = new Resources();

            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                //1. Identity Resources
                var identityRepo = uow.GetRepository<IdentityResource>();
                allResources.IdentityResources = identityRepo.GetAll().ToList();

                //2. API Resources
                var apiRepo = uow.GetRepository<ApiResource>();
                allResources.ApiResources = apiRepo.GetAll().ToList();

                //3. API Scopes
                var apiScope = uow.GetRepository<ApiScope>();
                allResources.ApiScopes = apiScope.GetAll().ToList();
            }

            return allResources;
        }
    }
}