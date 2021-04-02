using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCClient.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            return View(result);
        }
    }
}