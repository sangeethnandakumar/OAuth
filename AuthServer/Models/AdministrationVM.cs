using AuthServer.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class AdministrationVM
    {
        public List<ApiClient> AuthClient { get; set; }
        public List<AuthApiResources> AuthApiResources { get; set; }
        public List<AuthScope> AuthScopes { get; set; }
    }
}