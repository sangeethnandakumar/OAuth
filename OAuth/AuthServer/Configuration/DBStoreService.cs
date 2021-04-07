using ExpressData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configuration
{
    public static class DBStoreService
    {
        public static void GetDetails(string client)
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var clientName = SqlHelper.Query<string>($"SELECT ClientName FROM AuthClients WHERE ClientId='{client}'", connectionString).FirstOrDefault();
            var logo = SqlHelper.Query<string>($"SELECT Logo FROM AuthClients WHERE ClientId='{client}'", connectionString).FirstOrDefault();
            var isBeta = SqlHelper.Query<bool>($"SELECT IsBeta FROM AuthClients WHERE ClientId='{client}'", connectionString).FirstOrDefault();
        }
    }
}