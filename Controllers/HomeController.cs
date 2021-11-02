using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;

using Microsoft.AspNetCore.Http;

namespace TimeReportingSystem.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            return View();
        }

        public IActionResult Help()
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
