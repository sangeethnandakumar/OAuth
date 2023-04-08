using Dapper.Contrib.Extensions;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configuration
{
    public class ApiClient
    {
        [BsonId]
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientDescription { get; set; }
        public string ClientSecret { get; set; }
        public string AllowedGrantTypes { get; set; }
        public string RedirectUris { get; set; }
        public string PostLogoutRedirectUris { get; set; }
        public string[] AllowedCorsOrigins { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public string[] AllowedScopes { get; set; }
        public bool IsActive { get; set; }
        public string MaintananceMessage { get; set; }
        public bool IsBeta { get; set; }
        public bool Is3rdParty { get; set; }
        public string Logo { get; set; }
    }
}