using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;
using System.Text.Json;
using System.Text.RegularExpressions;
// using System;
using System.Collections.Generic;
// using System.Text.Json;
using Microsoft.AspNetCore.Http;
// using System.Linq;
using System.IO;


namespace TimeReportingSystem.Controllers
{
    public class RaportsController : Controller
    {
        public IActionResult Index(string year, string month){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                Console.WriteLine($"Rok: {DateTime.Now.Year.ToString()} Miesiac {DateTime.Now.Month.ToString()}");
                ViewData["Raport"] = "false";

                if(year == null || month == null){
                    year = DateTime.Now.Year.ToString();
                    month = DateTime.Now.Month.ToString();
                }
                ViewData["Year"] = year;
                ViewData["Month"] = month;
                Console.WriteLine(ViewData["Year"]);
                Console.WriteLine(ViewData["Month"]);
                
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                
                string fileName = ViewData["User"].ToString() + '-' + year + '-' + month + ".json"; 
                //TODO przeniesci to do tworzenia user-a
                if(Directory.Exists(userPath))
                {
                    Console.WriteLine("F1");
                    if(System.IO.File.Exists(userPath + '/' + fileName) && Directory.GetFiles(userPath).Length>0)
                    {
                        // var tempTable = Directory.GetFiles(userPath).Select(i => i = Regex.Match(i, "-[0-9]{4}-[0-9]{1,2}").ToString());
                        // ViewData["Dates"] = tempTable.Select(i => i = i.Substring(1, i.Length - 1));
                        string json = System.IO.File.ReadAllText(userPath + '/' + fileName);
                        Raport activityRaport = JsonSerializer.Deserialize<Raport>(json);
                        ViewData["Raport"] = "true";
                        return View(activityRaport);
                    }
                    else{
                        Console.WriteLine("F3");
                        // System.IO.File.Create(userPath + '/' + fileName);
                        // Raport newRaport = new Raport();
                        // newRaport.frozen = false;
                        // newRaport.entries = new List<Entry>();
                        // newRaport.accepted = new List<Accepted>();
                        // string saveJson = JsonSerializer.Serialize<Raport>(newRaport);
                        // System.IO.File.WriteAllText(userPath + '/' + fileName, saveJson);
                        return View();
                    }
                }
                else{
                    Console.WriteLine("F4");
                    Directory.CreateDirectory(userPath);
                    RedirectToAction("Index", "Raports");
                }
            }
            Console.WriteLine("F5");
            return RedirectToAction("Index", "Home");
            
        }

        public IActionResult CreateEntry(string year, string month){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
                Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
                ViewData["projectsInfo"] = ToDictionary(activityList);
                
                // Console.WriteLine(ViewData["projectCodes"]);
            }
             
            return View();
        }
        [HttpPost]
        public IActionResult CreateEntry(Entry e){
            
            return RedirectToAction("Index", "Raports");
        }

        public static Dictionary<string, List<string>> ToDictionary(Activities a)
        {
            
            var codes = new Dictionary<string, List<string>>();
            foreach (var item in a.activities)
            {
                if(item.active == true){
                    var subCodes = new List<string>();
                    item.subactivities.ForEach(delegate(Subactivity s){subCodes.Add(s.code);});
                    codes.Add(item.code, subCodes);
                }
            }            
            
            return codes;
        }

    }
}