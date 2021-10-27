using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;
using System.Text.Json;
// using System;
// using System.Collections.Generic;
// using System.Text.Json;
using Microsoft.AspNetCore.Http;
// using System.Linq;
using System.IO;


namespace TimeReportingSystem.Controllers
{
    public class ActivitiesController : Controller
    {
        public IActionResult Index(){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                /*
                Console.WriteLine($"Rok: {DateTime.Now.Year.ToString()} Miesiac {DateTime.Now.Month.ToString()}");
                Console.WriteLine(ViewData["User"]);
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                Console.WriteLine(userPath);
                string[] files = Directory.GetFiles(userPath);
                Activities userActivities;
                foreach(string s in files)
                {
                    string json = System.IO.File.ReadAllText(s);
                    Console.WriteLine(json);
                    Activities userMonthActivities = JsonSerializer.Deserialize<Activities>(json);
                    Console.WriteLine(userMonthActivities.activities[0].manager);
                    
                }*/

                return View();
            }
            return View();
            
        }

    }
}