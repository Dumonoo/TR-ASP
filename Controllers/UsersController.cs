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

using Microsoft.AspNetCore.Http;
// using System.Linq;


namespace TimeReportingSystem.Controllers
{
    public class UsersController : Controller
    {
        public const string SessionUser = "_User";
        public Users usersData;
        public Repository appRepository;
        
        public UsersController(){
            Console.WriteLine("helooooooooooooooooooooooooooo");
            appRepository = new Repository();
            appRepository.LoadUsers();
        }
        public IActionResult Index()
        {
            // string json = System.IO.File.ReadAllText("./wwwroot/json/Users.json");
            // Users userList = JsonSerializer.Deserialize<Users>(json);
            // usersData = new Users();         //to  podejscie dziala
            // usersData.Load();
            // return View(usersData);

            return View(appRepository.GetUsersData());         
        }
        public IActionResult SignUp()
        {
            return View();
        }
        public IActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User u)
        {
            if(ModelState.IsValid){
                string json = System.IO.File.ReadAllText("./wwwroot/json/Users.json");
                Users userList = JsonSerializer.Deserialize<Users>(json);
                userList.users.Add(u);
                string saveJson = JsonSerializer.Serialize<Users>(userList);
                System.IO.File.WriteAllText("./wwwroot/json/Users.json", saveJson);
                return View("Index",userList);
            }
            else{
                return View();
            }
            
        }
        
        public IActionResult SignIn(string userName){
            HttpContext.Session.SetString(SessionUser, userName);
            ViewData["User"] = HttpContext.Session.GetString(SessionUser);
            return View();
        }

        public IActionResult LogOut(){
            HttpContext.Session.Remove(SessionUser);
            ViewData["User"] = HttpContext.Session.GetString(SessionUser);
            return RedirectToAction("Index", "Home");
        }

        
        public JsonResult VerifyUserName(string userName)
        {
            string json = System.IO.File.ReadAllText("./wwwroot/json/Users.json");
            Users userList = JsonSerializer.Deserialize<Users>(json);
            
            bool newUsername = IsInUse(userName);
            
            if (!newUsername)
            {
                return Json($"Nazwa {userName} jest w użyciu.");
            }
            return Json(true);
        }
        public bool IsInUse(string userName){
            string json = System.IO.File.ReadAllText("./wwwroot/json/Users.json");
            Users userList = JsonSerializer.Deserialize<Users>(json);
            
            bool isNotTaken = true;
            userList.users.ForEach(delegate(User u){if(u.userName == userName){isNotTaken = false;}});
            return isNotTaken;
        }
    }
}
