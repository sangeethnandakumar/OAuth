using AuthServer.Configuration;
using ExpressData;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public UserService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = configuration.GetConnectionString("AuthConfigDatabase");
        }

        public async Task<AuthUsers> GetUserDetails(string username)
        {
            var authUser = SqlHelper.Query<AuthUsers>($"SELECT TOP 1 * FROM AuthUsers WHERE Username='{username}'", connectionString).FirstOrDefault();
            return authUser;
        }

        public async Task<bool> ValidateUser(string username, string password)
        {
            var authUser = SqlHelper.Query<AuthUsers>($"SELECT TOP 1 * FROM AuthUsers WHERE Username='{username}' AND Password='{password}' AND IsActive=1", connectionString).FirstOrDefault();
            if (authUser != null)
            {
                return true;
            }
            return false;
        }
    }
}