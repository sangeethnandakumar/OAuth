using AuthServer.Services;
using IdentityServer4.Models;
using IdentityServer4.Services;
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

        public ProfileService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userService.GetUserDetails(context.Subject.Identity.Name);
            var claims = new List<Claim>
            {
                new Claim("identity", user.Id.ToString()),
                new Claim("firstname", user.FirstName),
                new Claim("lastname", user.LastName),
                new Claim("email", user.Email),
                new Claim("username", user.Username)
            };
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