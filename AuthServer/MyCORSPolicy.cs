using AuthServer.Configuration;
using IdentityServer4.Services;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Twileloop.UOW;

namespace AuthServer {
    public class MyCORSPolicy : ICorsPolicyService {
        private readonly IOptions<DbConfig> dbOptions;

        public MyCORSPolicy(IOptions<DbConfig> dbOptions)
        {
            this.dbOptions = dbOptions;
        }

        public async Task<bool> IsOriginAllowedAsync(string origin) {
            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                var clientRepo = uow.GetRepository<ApiClient>();
                return clientRepo.GetAll().Any(x => x.AllowedCorsOrigins.Contains(origin));
            }
        }
    }
}