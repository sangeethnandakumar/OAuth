using AuthServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Services
{
    public interface IUserService
    {
        Task<bool> ValidateUser(string username, string password);

        Task<AuthUsers> GetUserDetails(string username);
    }
}