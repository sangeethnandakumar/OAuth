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
            //    var repo = unitOfWork.GetRepository<AuthUsers>();
            //    // Add a new person.
            //    unitOfWork.BeginTransaction();
            //    try {

            //        repo.DeleteAll();

            //        var newUser = new AuthUsers {
            //            Username = "sangee",
            //            FirstName = "Sangeeth",
            //            LastName = "Nandakummar",
            //            Avatar = "https://media.istockphoto.com/id/1391365592/photo/beautiful-afro-woman.jpg?b=1&s=170667a&w=0&k=20&c=VxathWKg4S9MhZFPhxzRgdq34MrV1PDhRMtIT25LOus=",
            //            IsActive = true,
            //            Email = "twileloop@outlook.com",
            //            Password = "ammu"
            //        };

            //        repo.Add(newUser);
                    
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