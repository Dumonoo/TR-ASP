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
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.IO;


namespace TimeReportingSystem.Controllers{
    public class ProjectsController:Controller{
        public IActionResult Index(){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
                Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
                return View(activityList);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult MyProjects(){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
                Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
                
                return View(activityList);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Manage(string projectCode){
            ViewData["test"] = projectCode;
            return View();
        }

        public IActionResult Edit(string projectCode){
            ViewData["test"] = projectCode;
            return View();
        }

        public IActionResult NewSubactivity(){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            return View();
        }
        public IActionResult CloseProject(string projectCode){
            ViewData["test"] = projectCode;
            return View();
        }

        public IActionResult CreateProject(){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                  
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult CreateProject(TimeReportingSystem.Models.Activity a){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            a.active = true;
            a.subactivities = new List<Subactivity>();
            if(ModelState.IsValid){
                string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
                Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
                activityList.activities.Add(a);
                string saveJson = JsonSerializer.Serialize<TimeReportingSystem.Models.Activities>(activityList);
                System.IO.File.WriteAllText("./wwwroot/json/Activities.json", saveJson);
                return View("MyProjects", activityList);
                
            }
            else{
                return View();
            }
        }
    }
}