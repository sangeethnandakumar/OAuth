using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;
using Twileloop.UOW;

namespace AuthServer {
    public class MyClientStore : IClientStore {
        private readonly IOptions<DbConfig> dbOptions;

        public MyClientStore(IOptions<DbConfig> dbOptions) {
            this.dbOptions = dbOptions;
        }

        public async Task<IdentityServer4.Models.Client> FindClientByIdAsync(string clientId) {

            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                var clientRepo = uow.GetRepository<IdentityServer4.Models.Client>();
                var client = clientRepo.Find(x => x.ClientId == clientId).FirstOrDefault();
                return client;
            }

        }
    }
}