using AuthServer.Configuration;
using AuthServer.Models;
using ExpressData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    [AllowAnonymous]
    [Route("[controller]")]
    public class AdministrationController : Controller
    {
        public async Task<IActionResult> Index()
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var authClients = SqlHelper.Query<AuthClient>($"SELECT * FROM AuthClients ORDER BY ClientId", connectionString).ToList();
            var authApiResources = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources ORDER BY Name", connectionString).ToList();
            var authScopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes ORDER BY ScopeName", connectionString).ToList();
            var vm = new AdministrationVM
            {
                AuthClient = authClients,
                AuthApiResources = authApiResources,
                AuthScopes = authScopes
            };
            return View(vm);
        }

        [HttpGet]
        [Route("GetClient")]
        public async Task<IActionResult> GetClient(Guid clientId)
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var authClient = SqlHelper.Query<AuthClient>($"SELECT * FROM AuthClients WHERE Id='{clientId.ToString()}'", connectionString).FirstOrDefault();
            return Ok(authClient);
        }

        [HttpGet]
        [Route("GetClientScopes")]
        public async Task<IActionResult> GetClientScopes(Guid clientId)
        {
            //NOTE: Requires 'dbo.Split' function in database
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var authClient = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName IN(SELECT * FROM dbo.Split((SELECT AllowedScopes FROM AuthClients WHERE Id='{clientId.ToString()}'), ','))", connectionString);
            return Ok(authClient);
        }

        [HttpGet]
        [Route("GetClientNonAssignedScopes")]
        public async Task<IActionResult> GetClientNonAssignedScopes(Guid clientId)
        {
            //NOTE: Requires 'dbo.Split' function in database
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var authClient = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName NOT IN(SELECT * FROM dbo.Split((SELECT AllowedScopes FROM AuthClients WHERE Id='{clientId.ToString()}'), ','))", connectionString);
            return Ok(authClient);
        }

        [HttpGet]
        [Route("AssignNewScopesToClient")]
        public async Task<IActionResult> AssignNewScopesToClient(Guid clientId, string newScopes)
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var existingScopes = SqlHelper.Query<string>($"SELECT AllowedScopes FROM AuthClients WHERE Id='{clientId.ToString()}'", connectionString).FirstOrDefault();
            if (existingScopes != null)
            {
                newScopes = newScopes + "," + existingScopes;
            }
            SqlHelper.Query<int>($"UPDATE AuthClients SET AllowedScopes='{newScopes}' WHERE Id='{clientId.ToString()}'", connectionString).FirstOrDefault();
            return Ok(true);
        }

        [HttpGet]
        [Route("DeleteClientScope")]
        public async Task<IActionResult> DeleteClientScope(Guid clientId, string scopeName)
        {
            var newScopes = "";
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var existingScopes = SqlHelper.Query<string>($"SELECT AllowedScopes FROM AuthClients WHERE Id='{clientId.ToString()}'", connectionString).FirstOrDefault();
            var scopesList = existingScopes.Split(",").ToList();
            scopesList.Remove(scopeName);
            for(var i=0; i<scopesList.Count; i++)
            {
                if(i!=scopesList.Count-1)
                {
                    newScopes += scopesList[i] + ",";
                }
                else
                {
                    newScopes += scopesList[i];
                }                
            }
            SqlHelper.Query<int>($"UPDATE AuthClients SET AllowedScopes='{newScopes}' WHERE Id='{clientId.ToString()}'", connectionString).FirstOrDefault();
            return Ok(true);
        }
    }
}