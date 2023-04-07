using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVCClient.Services
{
    public interface IAuthClient
    {
    }

    public class AuthClient : IAuthClient
    {
        private readonly IHttpContextAccessor _context;
        private readonly HttpClient _client;

        public AuthClient(IHttpContextAccessor context)
        {
            _context = context;
            _client = new HttpClient();
        }
    }
}