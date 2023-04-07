using AuthServer.Configuration;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using Twileloop.UOW;

namespace AuthServer {
    public class Program {
        public static void Main(string[] args) {
            //const string connectionString = "Filename=AuthConfig.db;Mode=Shared;Password=admin;";
            //using (var unitOfWork = new UnitOfWork(connectionString)) {
            //    var repo = unitOfWork.GetRepository<IdentityServer4.Models.Client>();
            //    // Add a new person.
            //    unitOfWork.BeginTransaction();
            //    try {

            //        var item = new IdentityServer4.Models.Client {
            //            ClientName = "Twileloop Surveys Web",
            //            ClientId = "twileloop-surveys-web",
            //            ClientSecrets = new List<Secret> { new Secret("admin".Sha256()) },
            //            AllowedScopes = new string[] { "read", "write" },
            //            AccessTokenLifetime = 5,
            //            IdentityTokenLifetime = 5,
            //            AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
            //            RedirectUris = new string[] { "https://localhost:44326/signin-oidc" },
            //            PostLogoutRedirectUris = new string[] { "https://localhost:44326/signout-callback-oidc" },
            //            AllowedCorsOrigins = new string[] { "https://google.com", "https://youtube.com" }
            //        };
            //        repo.Add(item);

            //        unitOfWork.Commit();
            //    }
            //    catch (Exception ex) {
            //        unitOfWork.Rollback();
            //    }
            //}


            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
    }
}