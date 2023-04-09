using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
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
                var sub = context.Subject.Identity.GetSubjectId();
                user = usersRepo.Find(x => x.Username == sub).FirstOrDefault();
            }
            using (var uow = new UnitOfWork(dbOptions.Value.AuthConfigDatabase)) {
                var clientRepo = uow.GetRepository<IdentityServer4.Models.Client>();
                client = clientRepo.Find(x => x.ClientId == context.Client.ClientId).FirstOrDefault();
            }

            var claims = new List<Claim>
            {
                new Claim("sub", user.Username ?? string.Empty),
                new Claim("name", $"{user.FirstName} {user.LastName}"),
                new Claim("given_name", user.FirstName ?? string.Empty),
                new Claim("family_name", user.LastName ?? string.Empty),
                new Claim("preferred_username", user.Username ?? string.Empty),
                new Claim("email", user.Email ?? string.Empty),
                new Claim("picture", user.Avatar ?? string.Empty)
            };
            context.IssuedClaims.AddRange(claims);

            foreach (var scope in client.AllowedScopes) {
                switch (scope) {
                    case "identity":
                        claims.Add(new Claim("identity", user.Username.ToString()));
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
            foreach (var claim in claims) {
                if (!context.IssuedClaims.Any(c => c.Type == claim.Type)) {
                    context.IssuedClaims.Add(claim);
                }
            }
            return;
        }

        public Task IsActiveAsync(IsActiveContext context) {
            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}