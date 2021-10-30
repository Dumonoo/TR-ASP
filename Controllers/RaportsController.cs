using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TimeReportingSystem.Models;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using System.IO;


namespace TimeReportingSystem.Controllers
{
    public class RaportsController : Controller
    {
        public Repository appRepository;
        
        public RaportsController(){
            Console.WriteLine("hello");
            appRepository = new Repository();
            appRepository.Load();
            appRepository.LoadRaports();
        }
        public IActionResult Index(string year, string month){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                ViewData["Raport"] = "false";

                if(year == null || month == null){
                    year = DateTime.Now.Year.ToString();
                    month = DateTime.Now.Month.ToString();
                }
                var userName = ViewData["User"].ToString();
                ViewData["Year"] = year;
                ViewData["Month"] = month;

                if(Int32.Parse(month) <= 9){
                    month = "0" + month;
                }
                if(appRepository.UserRaportExists(userName, year, month)){
                    ViewData["Raport"] = "true";
                    return View(appRepository.GetUserRaport(userName, year, month));
                }
                else{
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult CreateEntry(string year, string month){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                ViewData["Month"] = month;
                ViewData["Year"] = year;
                ViewData["projectsInfo"] = ToDictionary(appRepository.GetActivities());
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult CreateEntry(Entry e){

            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                if(e.subcode == "null")
                {
                    e.subcode = null;
                }
                
                var userName = ViewData["User"].ToString();
                string year = e.date.Substring(0,4);
                string month = e.date.Substring(5,2);
                appRepository.InsertEntry(e, year, month, userName); 
                
                return RedirectToAction("Index", "Raports");
                
            }          
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Display(string index, string month, string year)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                var userName = ViewData["User"].ToString();
                if(Int32.Parse(month) <= 9){
                    month = "0" + month;
                }
                if(appRepository.EntryExists(index, userName, year, month)){
                    return View(appRepository.GetEntry(index, userName, year, month));
                }
                else{
                    return View("Error");
                }
            }
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Edit(string index, string month, string year)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                int id = Int32.Parse(index);
                ViewData["Month"] = month;
                ViewData["Year"] = year;
                ViewData["Index"] = index;

                var userName = ViewData["User"].ToString();
                if(Int32.Parse(month) <= 9){
                    month = "0" + month;
                }

                if(appRepository.EntryExists(index, userName, year, month)){

                    ViewData["projectsInfo"] = ToDictionary(appRepository.GetActivities());
                    ViewData["Entry"] = appRepository.GetEntry(index, userName, year, month);
                    return View(appRepository.GetEntry(index, userName, year, month));
                }
                else{
                    return View("Error");
                }
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult Edit(Entry e, string index){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                var year = e.date.Substring(0, 4);
                var month = e.date.Substring(5,2);
                var id = Int32.Parse(index);
                if(e.subcode == "null")
                {
                    e.subcode = null;
                }
                var userName = ViewData["User"].ToString();

                if(appRepository.EntryExists(index, userName, year, month)){

                    appRepository.UpdateEntry(e, index, userName, year, month);
                    return RedirectToAction("Index", "Raports");
                }
                else{
                    return View("Error");
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Delete(string index, string month, string year)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                int id = Int32.Parse(index);
                if(Int32.Parse(month) <= 9){
                    month = "0" + month;
                }

                var userName = ViewData["User"].ToString();

                if(appRepository.EntryExists(index, userName, year, month)){

                    appRepository.DeleteEntry(index, userName, year, month);
                    return RedirectToAction("Index", "Raports");
                }
                else{
                    return View("Error");
                }                
            }
            return RedirectToAction("Index", "Home");
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
    public IActionResult Submit(string month, string year)
    {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                if(Int32.Parse(month) <= 9){
                    month = "0" + month;
                }

                var userName = ViewData["User"].ToString();

                if(appRepository.UserRaportExists(userName, year, month)){
                    appRepository.GetUserRaport(userName, year, month).frozen = true;
                    appRepository.SaveRaports();
                    return RedirectToAction("Index", "Raports");
                }
                else{
                    return View("Error");
                }
            }
            return RedirectToAction("Index", "Home");
    }

    }
}