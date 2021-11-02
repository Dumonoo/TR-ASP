using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace TimeReportingSystem.Controllers{
    public class ProjectsController:Controller{
        public Repository appRepository;
        
        public ProjectsController(){
            appRepository = new Repository();
            appRepository.Load();
            appRepository.LoadRaports();
        }
        public IActionResult Index(){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){                
                return View(appRepository.GetActivities());
            }
            return RedirectToAction("Index", "Home");
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult MyProjects(){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){  
                var userName = ViewData["User"].ToString();
                var myProjectsData = appRepository.GetMyProjects(userName);
                return View(myProjectsData);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Manage(string projectCode){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["ProjectCode"] = projectCode;
            
            if(ViewData["User"] != null){
                var manageData = appRepository.GetManageData(projectCode); 
                return View(manageData);
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Edit(string projectCode){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);
            ViewData["ProjectCode"] = projectCode;
            
            if(ViewData["User"] != null){
                return View(appRepository.GetActivityByCode(projectCode));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Edit(TimeReportingSystem.Models.Activity a){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                
                if(ModelState.IsValid){
                    appRepository.UpdateActivity(a);
                    return RedirectToAction("MyProjects", "Projects");
                }
                else{
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
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

                var userName = ViewData["User"].ToString();
                var projectInfo = appRepository.GetProjectInfo(projectCode);
                if(projectInfo.manager == userName)
                {
                    if(ModelState.IsValid){
                        appRepository.InsertSubActivity(s, projectCode);
                        return RedirectToAction("MyProjects", "Projects");
                    }
                    else{
                        return View();
                    }
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult CloseProject(string projectCode){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                var userName = ViewData["User"].ToString();
                var projectInfo = appRepository.GetProjectInfo(projectCode);
                if(projectInfo.active && projectInfo.manager == userName)
                {   
                    appRepository.CloseProject(projectCode);
                    return RedirectToAction("MyProjects", "Projects");
                }
                
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
                  
                if(ModelState.IsValid){
                    appRepository.InsertActivity(a);
                    return RedirectToAction("MyProjects", "Projects");
                }
                else{
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");

            
        }
        public IActionResult Details(string userName, string projectCode, string period){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                var year = period.Substring(0,4);
                var month = period.Substring(5,2);
                ViewData["SelectedUser"] = userName;
                ViewData["ProjectCode"] = projectCode;
                ViewData["Period"] = period;
                ViewData["Users"] = appRepository.GetUsersData();
                return View(appRepository.GetUserRaport(userName, year, month));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public IActionResult Accept (string userName, string projectCode, string period, string acceptedTime){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                var year = period.Substring(0,4);
                var month = period.Substring(5,2);
                var managerName = ViewData["User"].ToString();
                if(appRepository.GetProjectInfo(projectCode).manager == managerName){
                    appRepository.AcceptRaport(year, month, userName, projectCode, Int32.Parse(acceptedTime));
                    return RedirectToAction("Manage", "Projects", new{projectCode = projectCode});
                }
    
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Change (string userName, string projectCode, string period, string acceptedTime){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null){
                var year = period.Substring(0,4);
                var month = period.Substring(5,2);
                appRepository.ChangeRaport(year, month, userName, projectCode, Int32.Parse(acceptedTime));
                return RedirectToAction("Manage", "Projects", new{projectCode = projectCode});
    
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
            appRepository.GetActivities().activities.ForEach(delegate(TimeReportingSystem.Models.Activity a){if(a.code == projectCode){isNotTaken = false;}});

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
            appRepository.GetActivities().activities.Find(i => i.code == projectCode).subactivities.ForEach(delegate(Subactivity s){if(s.code == code){isNotTaken = false;}});

            return isNotTaken;
        }
    }
}