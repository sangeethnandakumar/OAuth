using AuthServer.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
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

            //        var client = repo.GetAll().FirstOrDefault();
            //        client.IdentityTokenLifetime = 60;
            //        client.AccessTokenLifetime = 60;

            //        repo.Update(client);
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