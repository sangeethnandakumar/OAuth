using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCClient.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVCClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var result = await HttpContext.AuthenticateAsync();
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var result = await HttpContext.AuthenticateAsync();
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            //Try to hit API
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5005");
            if (!disco.IsError)
            {
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint,
                    ClientId = "mvc-client-requester",
                    ClientSecret = "MVCSecret",
                    Scope = "Api1"
                });

                if (tokenResponse.IsError)
                {
                    Console.WriteLine(tokenResponse.Error);
                }

                var token = tokenResponse.AccessToken;

                var clientA = new RestClient("https://localhost:44365/WeatherForecast");
                clientA.Timeout = -1;
                var request = new RestRequest(Method.GET);
                request.AddHeader("Authorization", $"Bearer {token}");
                IRestResponse response = clientA.Execute(request);
                Console.WriteLine(response.Content);
            }

            return View(result);
        }
    }
}