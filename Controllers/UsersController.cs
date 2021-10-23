using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TimeReportingSystem.Controllers
{
    public class UsersController : Controller
    {
        // private readonly TimeReportingSystemContext _context;
        // public async Task<IActionResult> Index(){
        //     return View(await );
        // }
        public IActionResult Index()
        {
            try
            {
                string json = System.IO.File.ReadAllText("./wwwroot/json/Users.json");
                // Console.WriteLine(json);
                Users userList = JsonSerializer.Deserialize<Users>(json);
                // Console.WriteLine($"DATE: {userList.users[1].name}");
                return View(userList);
            }
            catch (System.Exception)
            {
                
                throw;
            }
            
            // return View();
        }
        public IActionResult SignUp(){
            return View();
        }
    }
}
