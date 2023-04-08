using AuthServer.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using Twileloop.UOW;

namespace AuthServer {
    public class Program {
        public static void Main(string[] args) {

            //const string connectionString = "Filename=Identities.db;Mode=Shared;Password=admin;";
            //using (var unitOfWork = new UnitOfWork(connectionString)) {
            //    var repo = unitOfWork.GetRepository<AuthUsers>();
            //    // Add a new person.
            //    unitOfWork.BeginTransaction();
            //    try {

            //        var item = new AuthUsers {
            //            FirstName = "Sangeeth",
            //            LastName = "Nandakumar",
            //            Username = "sangee",
            //            Password = "ammu",
            //            IsActive = true,
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