// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;

namespace IdentityServerHost.Quickstart.UI
{
    public class ConsentViewModel : ConsentInputModel
    {
        public string ClientId { get; set; }
        public string ClientDisplayName { get; set; }
        public string ClientIcon { get; set; }
        public bool IsBeta { get; set; }
        public bool Is3rdParty { get; set; }
        public string SingleSignOnAuthorityName { get; set; }



        public string ClientName { get; set; }
        public string ClientUrl { get; set; }
        public string ClientLogoUrl { get; set; }
        public bool AllowRememberConsent { get; set; }

        public IEnumerable<ScopeViewModel> IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel> ApiScopes { get; set; }
    }
}
