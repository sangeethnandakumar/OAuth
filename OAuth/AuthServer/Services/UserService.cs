using AuthServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Services
{
    public class UserService : IUserService
    {
        public async Task<User> GetUserDetails(string username)
        {
            return new User
            {
                FirstName = "Sangeeth",
                LastName = "Nandakumar",
                Id = Guid.NewGuid(),
                Username = "sangee"
            };
        }

        public async Task<bool> ValidateUser(string username, string password)
        {
            return true;
        }
    }
}