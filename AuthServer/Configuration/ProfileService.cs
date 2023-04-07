using AuthServer.Services;
using ExpressData;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Configuration
{
    public class ProfileService : IProfileService
    {
        private IUserService _userService;
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public ProfileService(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            this.configuration = configuration;
            this.connectionString = configuration.GetConnectionString("AuthConfigDatabase");
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var authClients = SqlHelper.Query<string>($"SELECT * FROM STRING_SPLIT((SELECT TOP 1 AllowedScopes FROM AuthClients WHERE ClientId='{context.Client.ClientId}'), ',')", connectionString).ToList();
            var user = await _userService.GetUserDetails(context.Subject.Identity.Name);
            var claims = new List<Claim>
            {               
                new Claim("firstname", user.FirstName),
                new Claim("lastname", user.LastName),
                new Claim("username", user.Username)
            };
            foreach(var scope in authClients)
            {
                switch (scope)
                {
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

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}