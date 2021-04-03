using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer
{
    public class MyCORSPolicy : ICorsPolicyService
    {
        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            return true;
        }
    }
}