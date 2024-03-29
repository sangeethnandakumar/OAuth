using AuthServer.Configuration;
using AuthServer.Models;
using ExpressData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IdentityServerHost.Quickstart.UI
{
    [Route("[controller]")]
    public class AdministrationController : Controller
    {
        private readonly IConfiguration config;
        private readonly string connectionString;

        public AdministrationController(IConfiguration config)
        {
            this.config = config;
            this.connectionString = config.GetConnectionString("AuthConfigDatabase");
        }

        public async Task<IActionResult> Index()
        {
            var authClients = SqlHelper.Query<ApiClient>($"SELECT * FROM AuthClients ORDER BY IsActive DESC, ClientName", connectionString).ToList();
            var authApiResources = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources ORDER BY IsActive DESC, Name", connectionString).ToList();
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
            var authClient = SqlHelper.Query<ApiClient>($"SELECT * FROM AuthClients WHERE Id='{clientId.ToString()}'", connectionString).FirstOrDefault();
            return Ok(authClient);
        }

        [HttpGet]
        [Route("GetApi")]
        public async Task<IActionResult> GetApi(Guid apiId)
        {
            var authApiResource = SqlHelper.Query<AuthApiResources>($"SELECT * FROM AuthApiResources WHERE Id='{apiId.ToString()}'", connectionString).FirstOrDefault();
            return Ok(authApiResource);
        }

        [HttpGet]
        [Route("GetScope")]
        public async Task<IActionResult> GetScope(Guid scopeId)
        {
            var scope = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE Id='{scopeId.ToString()}'", connectionString).FirstOrDefault();
            return Ok(scope);
        }

        [HttpGet]
        [Route("GetClientScopes")]
        public async Task<IActionResult> GetClientScopes(Guid clientId)
        {
            //NOTE: Requires 'STRING_SPLIT' function in database
            var authClient = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName IN(SELECT * FROM STRING_SPLIT((SELECT AllowedScopes FROM AuthClients WHERE Id='{clientId.ToString()}'), ','))", connectionString);
            return Ok(authClient);
        }

        [HttpGet]
        [Route("GetApiScopes")]
        public async Task<IActionResult> GetApiScopes(Guid apiId)
        {
            var apiScopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName IN(SELECT * FROM STRING_SPLIT((SELECT SupportedScopes FROM AuthApiResources WHERE Id='{apiId.ToString()}'), ','))", connectionString);
            return Ok(apiScopes);
        }

        [HttpGet]
        [Route("GetClientNonAssignedScopes")]
        public async Task<IActionResult> GetClientNonAssignedScopes(Guid clientId)
        {
            //NOTE: Requires 'STRING_SPLIT' function in database
            if (clientId != Guid.Empty)
            {
                var scopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName NOT IN(SELECT * FROM STRING_SPLIT((SELECT AllowedScopes FROM AuthClients WHERE Id='{clientId.ToString()}'), ','))", connectionString);
                return Ok(scopes);
            }
            else
            {
                var scopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes", connectionString);
                return Ok(scopes);
            }
        }

        [HttpGet]
        [Route("GetApiNonAssignedScopes")]
        public async Task<IActionResult> GetApiNonAssignedScopes(Guid apiId)
        {
            var apiScopes = SqlHelper.Query<AuthScope>($"SELECT * FROM AuthScopes WHERE ScopeName NOT IN(SELECT * FROM STRING_SPLIT((SELECT SupportedScopes FROM AuthApiResources WHERE Id='{apiId.ToString()}'), ','))", connectionString);
            return Ok(apiScopes);
        }

        [HttpGet]
        [Route("AssignNewScopesToClient")]
        public async Task<IActionResult> AssignNewScopesToClient(Guid clientId, string newScopes)
        {
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
        public async Task<IActionResult> SaveClient(ApiClient client)
        {
            try
            {
                //if (client.Id == null)
                //{
                //    client.Id = Guid.NewGuid();
                //    SqlHelper.Insert<ApiClient>(client, connectionString);
                //}
                //else
                //{
                //    SqlHelper.Update<ApiClient>(client, connectionString);
                //}
            }
            catch (Exception ex)
            {
            }
            return Ok();
        }

        [HttpPost]
        [Route("SaveApi")]
        public async Task<IActionResult> SaveApi(AuthApiResources api)
        {
            if (api.Id == null)
            {
                api.Id = Guid.NewGuid();
                SqlHelper.Insert<AuthApiResources>(api, connectionString);
            }
            else
            {
                SqlHelper.Update<AuthApiResources>(api, connectionString);
            }
            return Ok();
        }

        [HttpPost]
        [Route("SaveScope")]
        public async Task<IActionResult> SaveScope(AuthScope scope)
        {
            if (scope.Id == null)
            {
                scope.Id = Guid.NewGuid();
                SqlHelper.Insert<AuthScope>(scope, connectionString);
            }
            else
            {
                SqlHelper.Update<AuthScope>(scope, connectionString);
            }
            return Ok();
        }

        [HttpGet]
        [Route("EnableDisableClient")]
        public async Task<IActionResult> EnableDisableClient(Guid clientId)
        {
            SqlHelper.Query<string>($"UPDATE AuthClients SET IsActive=IsActive^1 WHERE Id='{clientId}'", connectionString).FirstOrDefault();
            return Ok(true);
        }

        [HttpGet]
        [Route("EnableDisableApi")]
        public async Task<IActionResult> EnableDisableApi(Guid apiId)
        {
            SqlHelper.Query<string>($"UPDATE AuthApiResources SET IsActive=IsActive^1 WHERE Id='{apiId}'", connectionString).FirstOrDefault();
            return Ok(true);
        }

        [HttpGet]
        [Route("DeleteClient")]
        public async Task<IActionResult> DeleteClient(Guid clientId)
        {
            SqlHelper.Query<string>($"DELETE FROM AuthClients WHERE Id='{clientId}'", connectionString).FirstOrDefault();
            return Ok(true);
        }

        [HttpGet]
        [Route("DeleteApi")]
        public async Task<IActionResult> DeleteApi(Guid apiId)
        {
            SqlHelper.Query<string>($"DELETE FROM AuthApiResources WHERE Id='{apiId}'", connectionString).FirstOrDefault();
            return Ok(true);
        }

        [HttpGet]
        [Route("DeleteScope")]
        public async Task<IActionResult> DeleteScope(Guid scopeId)
        {
            SqlHelper.Query<string>($"DELETE FROM AuthScopes WHERE Id='{scopeId}'", connectionString).FirstOrDefault();
            return Ok(true);
        }
    }
}