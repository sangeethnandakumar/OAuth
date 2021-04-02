﻿using AuthServer.Configuration;
using ExpressData;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;
using GrantTypes = IdentityServer4.Models.GrantTypes;

namespace AuthServer
{
    public class MyClientStore : IClientStore
    {
        public async Task<IdentityServer4.Models.Client> FindClientByIdAsync(string clientId)
        {
            var connectionString = @"Server=DESKTOP-QJ02OLT\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var client = SqlHelper.Query<AuthClient>($"SELECT * FROM AuthClients WHERE ClientId='{clientId}' AND IsActive=1", connectionString).FirstOrDefault();
            if (client != null)
            {
                var allowedScopes = new List<string>() { "openid", "profile" };
                allowedScopes.AddRange(client.AllowedScopes.Split(","));
                return new IdentityServer4.Models.Client
                {
                    ClientName = client.ClientName,
                    ClientId = client.ClientId,
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> { client.RedirectUris },
                    ClientSecrets = { new Secret(client.ClientSecret.Sha512()) },
                    AccessTokenLifetime = client.AccessTokenLifetime,
                    IdentityTokenLifetime = client.IdentityTokenLifetime,
                    RequirePkce = false,
                    UpdateAccessTokenClaimsOnRefresh = true,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    PostLogoutRedirectUris = new List<string> { client.PostLogoutRedirectUris },
                    AllowedScopes = allowedScopes
                };
            }
            return null;
        }
    }
}