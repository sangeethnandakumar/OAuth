using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configuration
{
    [Table("AuthApiResources")]
    public class AuthApiResources
    {
        [ExplicitKey]
        public Guid? Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string SupportedScopes { get; set; }
    }
}