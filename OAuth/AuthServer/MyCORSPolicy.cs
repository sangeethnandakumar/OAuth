using AuthServer.Configuration;
using ExpressData;
using IdentityServer4.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer
{
    public class MyCORSPolicy : ICorsPolicyService
    {
        private readonly IConfiguration config;
        private readonly string connectionString;

        public MyCORSPolicy(IConfiguration config)
        {
            this.config = config;
            this.connectionString = config.GetConnectionString("AuthConfigDatabase");
        }

        public async Task<bool> IsOriginAllowedAsync(string origin)
        {
            var apis = SqlHelper.Query<AuthClient>($"SELECT * FROM AuthClients WHERE AllowedCorsOrigins LIKE '%{origin}%'", connectionString).FirstOrDefault();
            if (apis != null)
            {
                return true;
            }
            return false;
        }
    }
}