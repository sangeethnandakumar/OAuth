using AuthServer.Services;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Twileloop.UOW;

namespace AuthServer.Configuration {
    public class ProfileService : IProfileService {
        private readonly IOptions<DbConfig> dbOptions;

        public ProfileService(IOptions<DbConfig> dbOptions) {
            this.dbOptions = dbOptions;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context) {

            IdentityServer4.Models.Client client;
            AuthUsers user;
            using (var uow = new UnitOfWork(dbOptions.Value.IdentityDatatabase)) {
                var usersRepo = uow.GetRepository<AuthUsers>();
                user = usersRepo.Find(x => x.Username == context.Subject.Identity.Name).FirstOrDefault();
            }
            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                var clientRepo = uow.GetRepository<IdentityServer4.Models.Client>();
                client = clientRepo.Find(x => x.ClientId == context.Client.ClientId).FirstOrDefault();
            }

            var claims = new List<Claim>
            {
                new Claim("firstname", user.FirstName),
                new Claim("lastname", user.LastName),
                new Claim("username", user.Username)
            };
            foreach (var scope in client.AllowedScopes) {
                switch (scope) {
                    case "identity":
                        claims.Add(new Claim("identity", user.Id.ToString()));
                        break;
                    case "email":
                        claims.Add(new Claim("email", user.Email ?? String.Empty));
                        break;
                    case "mobile":
                        claims.Add(new Claim("mobile", user.Mobile ?? String.Empty));
                        break;
                    case "avatar":
                        claims.Add(new Claim("avatar", user.Avatar ?? String.Empty));
                        break;
                }
            }
            context.IssuedClaims.AddRange(claims);
            return;
        }

        public Task IsActiveAsync(IsActiveContext context) {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}