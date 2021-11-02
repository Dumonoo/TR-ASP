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
    public class UsersController : Controller
    {
        public const string SessionUser = "_User";
        public Users usersData;
        public Repository appRepository;
        
        public UsersController(){
            appRepository = new Repository();
            appRepository.LoadUsers();
        }
        public IActionResult Index()
        {
            return View(appRepository.GetUsersData());         
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User u)
        {
            if(ModelState.IsValid){
                appRepository.InsertUser(u);
                return RedirectToAction("Index", "Users");
            }
            else{
                return View();
            }  
        }
        
        public IActionResult SignIn(string userName){
            HttpContext.Session.SetString(SessionUser, userName);
            ViewData["User"] = HttpContext.Session.GetString(SessionUser);
            return RedirectToAction("Index", "Raports");
        }

        public IActionResult LogOut(){
            HttpContext.Session.Remove(SessionUser);
            ViewData["User"] = HttpContext.Session.GetString(SessionUser);
            return RedirectToAction("Index", "Home");
        }

        
        public JsonResult VerifyUserName(string userName)
        {
            bool newUsername = IsInUse(userName);
            
            if (!newUsername)
            {
                return Json($"Nazwa {userName} jest w użyciu.");
            }
            return Json(true);
        }
        public bool IsInUse(string userName){
            
            bool isNotTaken = true;
            appRepository.GetUsersData().users.ForEach(delegate(User u){if(u.userName == userName){isNotTaken = false;}});
            return isNotTaken;
        }
    }
}
