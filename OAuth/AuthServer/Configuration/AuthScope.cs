using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configuration
{
    [Table("AuthScopes")]
    public class AuthScope
    {
        [Key]
        public Guid? Id { get; set; }

        public string ScopeName { get; set; }
        public string ScopeDescription { get; set; }
    }
}