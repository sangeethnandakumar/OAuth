using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configuration
{
    [Table("AuthClients")]
    public class AuthClient
    {
        [Key]
        public Guid? Id { get; set; }

        public string ClientName { get; set; }
        public string ClientDescription { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string AllowedGrantTypes { get; set; }
        public string RedirectUris { get; set; }
        public string PostLogoutRedirectUris { get; set; }
        public string AllowedCorsOrigins { get; set; }
        public int AccessTokenLifetime { get; set; }
        public int IdentityTokenLifetime { get; set; }
        public string AllowedScopes { get; set; }
        public bool? IsActive { get; set; }
        public string MaintananceMessage { get; set; }
    }
}