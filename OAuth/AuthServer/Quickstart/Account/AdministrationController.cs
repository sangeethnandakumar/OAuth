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
        [Route("GetApi")]
        public async Task<IActionResult> GetApi(Guid apiId)
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var authApiResource = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources WHERE Id='{apiId.ToString()}'", connectionString).FirstOrDefault();
            return Ok(authApiResource);
        }

        [HttpGet]
        [Route("GetScope")]
        public async Task<IActionResult> GetScope(string scopename)
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var scope = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName='{scopename}'", connectionString).FirstOrDefault();
            return Ok(scope);
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
        [Route("GetApiScopes")]
        public async Task<IActionResult> GetApiScopes(Guid apiId)
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var apiScopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName IN(SELECT * FROM dbo.Split((SELECT SupportedScopes FROM AuthApiResources WHERE Id='{apiId.ToString()}'), ','))", connectionString);
            return Ok(apiScopes);
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
        [Route("GetApiNonAssignedScopes")]
        public async Task<IActionResult> GetApiNonAssignedScopes(Guid apiId)
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var apiScopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName NOT IN(SELECT * FROM dbo.Split((SELECT SupportedScopes FROM AuthApiResources WHERE Id='{apiId.ToString()}'), ','))", connectionString);
            return Ok(apiScopes);
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
        [Route("AssignNewScopesToApi")]
        public async Task<IActionResult> AssignNewScopesToApi(Guid apiId, string newScopes)
        {
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var existingScopes = SqlHelper.Query<string>($"SELECT SupportedScopes FROM AuthApiResources WHERE Id='{apiId.ToString()}'", connectionString).FirstOrDefault();
            if (existingScopes != null)
            {
                newScopes = newScopes + "," + existingScopes;
            }
            SqlHelper.Query<int>($"UPDATE AuthApiResources SET SupportedScopes='{newScopes}' WHERE Id='{apiId.ToString()}'", connectionString).FirstOrDefault();
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
            for (var i = 0; i < scopesList.Count; i++)
            {
                if (i != scopesList.Count - 1)
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

        [HttpGet]
        [Route("DeleteApiScope")]
        public async Task<IActionResult> DeleteApiScope(Guid apiId, string scopeName)
        {
            var newScopes = "";
            var connectionString = "Server=DESKTOP-QJ02OLT\\SQLEXPRESS;Database=Inventory;Trusted_Connection=True;";
            var existingScopes = SqlHelper.Query<string>($"SELECT SupportedScopes FROM AuthApiResources WHERE Id='{apiId.ToString()}'", connectionString).FirstOrDefault();
            var scopesList = existingScopes.Split(",").ToList();
            scopesList.Remove(scopeName);
            for (var i = 0; i < scopesList.Count; i++)
            {
                if (i != scopesList.Count - 1)
                {
                    newScopes += scopesList[i] + ",";
                }
                else
                {
                    newScopes += scopesList[i];
                }
            }
            SqlHelper.Query<int>($"UPDATE AuthApiResources SET SupportedScopes='{newScopes}' WHERE Id='{apiId.ToString()}'", connectionString).FirstOrDefault();
            return Ok(true);
        }




        [HttpPost]
        [Route("SaveClient")]
        public async Task<IActionResult> SaveClient(AuthClient client)
        {
            return Ok();
        }

        [HttpPost]
        [Route("SaveApi")]
        public async Task<IActionResult> SaveApi(AuthApiResources api)
        {
            return Ok();
        }

        [HttpPost]
        [Route("SaveScope")]
        public async Task<IActionResult> SaveScope(AuthScope scope)
        {
            return Ok();
        }




        [HttpGet]
        [Route("DeleteClient")]
        public async Task<IActionResult> DeleteClient(string clientId)
        {
            return Ok();
        }

        [HttpGet]
        [Route("DeleteApi")]
        public async Task<IActionResult> DeleteApi(string apiId)
        {
            return Ok();
        }

        [HttpGet]
        [Route("DeleteScope")]
        public async Task<IActionResult> DeleteScope(string scopeName)
        {
            return Ok();
        }


    }
}