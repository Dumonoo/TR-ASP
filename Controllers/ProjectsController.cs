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

        public IActionResult NewSubactivity(string projectCode){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["ProjectCode"] = projectCode;
            
            if(ViewData["User"] != null){
                  
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult NewSubactivity(Subactivity s, string projectCode){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            if(ViewData["User"] != null){
                  
                if(ModelState.IsValid){
                    string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
                    Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
                    activityList.activities.Find(i => i.code == projectCode).subactivities.Add(s);
                    string saveJson = JsonSerializer.Serialize<TimeReportingSystem.Models.Activities>(activityList);
                    System.IO.File.WriteAllText("./wwwroot/json/Activities.json", saveJson);
                    return View("MyProjects", activityList);
                }
                else{
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult CloseProject(string projectCode){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                  
                return View();
            }
            return RedirectToAction("Index", "Home");
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

            if(ViewData["User"] != null){
                  
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
            return RedirectToAction("Index", "Home");

            
        }

        public JsonResult ProjCodeUniqueness(string code)
        {                     
            if (!ProjectCodeIsInUse(code))
            {
                return Json($"Istnieje już projekt o podanym kodzie!");
            }
            return Json(true);
        }
        public bool ProjectCodeIsInUse(string projectCode)
        {
            bool isNotTaken = true;
            string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
            Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
            activityList.activities.ForEach(delegate(TimeReportingSystem.Models.Activity a){if(a.code == projectCode){isNotTaken = false;}});

            return isNotTaken;
        }
        public JsonResult SubCodeUniqueness(string code, string projectCode)
        {                     
            if (!SubActivityCodeIsInUse(code, projectCode))
            {
                return Json($"Kod {code} w danym projekcie już istnieje.");
            }
            return Json(true);
        }

        public bool SubActivityCodeIsInUse(string code, string projectCode)
        {
            bool isNotTaken = true;
            string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
            Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
            activityList.activities.Find(i => i.code == projectCode).subactivities.ForEach(delegate(Subactivity s){if(s.code == code){isNotTaken = false;}});

            return isNotTaken;
        }
    }
}