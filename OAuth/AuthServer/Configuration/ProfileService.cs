using AuthServer.Services;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = _userService.GetUserDetails(context.Subject.Identity.Name);
            //context.IssuedClaims.AddRange(user.Claims);
            return Task.FromResult(0);
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.FromResult(0);
        }
    }
}