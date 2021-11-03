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
            appRepository = new Repository();
            appRepository.Load();
            appRepository.LoadRaports();
        }
        public IActionResult Index(string year, string month, string day){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                ViewData["Raport"] = "false";

                if(year == null || month == null){
                    year = DateTime.Now.Year.ToString();
                    month = DateTime.Now.Month.ToString();
                }
                if(day != null)
                {
                    ViewData["Day"] = day;
                }
                var userName = ViewData["User"].ToString();
                ViewData["Year"] = year;
                ViewData["Month"] = month;
                ViewData["IsSubmitted"] = false;

                if(Int32.Parse(month) <= 9){
                    month = "0" + Int32.Parse(month).ToString();
                }
                if(appRepository.UserRaportExists(userName, year, month)){
                    ViewData["Raport"] = "true";
                    var userRaports = appRepository.GetUserRaport(userName, year, month);
                    ViewData["IsSubmitted"] = userRaports.frozen;
                    double minutesSum = 0;
                    if(day != null){
                        var dailyRaport = new Raport();
                        dailyRaport.frozen = userRaports.frozen;
                        dailyRaport.accepted = new List<Accepted>();
                        dailyRaport.entries = new List<Entry>();
                        for (int index = 0; index < userRaports.entries.Count; index++){
                            if(Int32.Parse(userRaports.entries[index].date.Substring(8,2)) == Int32.Parse(day))
                            {
                                minutesSum += userRaports.entries[index].time;
                                dailyRaport.entries.Add(userRaports.entries[index]);
                            }
                        }
                        ViewData["Hours"] = minutesSum / 60;
                        userRaports = dailyRaport;
                    }
                    
                    return View(userRaports);
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
                var userName = ViewData["User"].ToString();
                if(!appRepository.IsRaportSubmitted(userName, year, month)){
                    ViewData["Month"] = month;
                    ViewData["Year"] = year;
                    ViewData["projectsInfo"] = ToDictionary(appRepository.GetActivities());
                    return View();
                }                
            }
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult CreateEntry(Entry e){

            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                if(e.subcode == "null" || e.subcode == null)
                {
                    e.subcode = "";
                }
                if(e.description == "null" || e.description == null)
                {
                    e.description = "";
                }
                
                var userName = ViewData["User"].ToString();
                string year = e.date.Substring(0,4);
                string month = e.date.Substring(5,2);
                appRepository.InsertEntry(e, year, month, userName); 
                
                return RedirectToAction("Index", "Raports", new{month = month, year = year});
                
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
                if(e.subcode == "null" || e.subcode == null)
                {
                    e.subcode = "";
                }
                if(e.description == "null" || e.description == null)
                {
                    e.description = "";
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

    public IActionResult MonthSummary(string month, string year){
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
                    var projects = appRepository.GetActivities();
                    var list = new List<Tuple<string,int, int, bool, bool>>();
                    var raports = appRepository.GetUserRaport(userName, year, month);
                    foreach (var project in projects.activities)
                    {
                        if(raports.entries.Exists(e => e.code == project.code)){
                            var submitedTime = raports.entries.Where(e=>e.code == project.code).Select(d => d.time).Sum();
                            var acceptedTime = 0;
                            var isAccepted = raports.accepted.Exists(e=> e.code == project.code);
                            var isSubmitted = raports.frozen;
                            if(isAccepted)
                            {
                                acceptedTime = raports.accepted.Find(e => e.code == project.code).time;
                            }
                            list.Add(new Tuple<string,int, int, bool, bool>(project.code, submitedTime, acceptedTime, isSubmitted, isAccepted));
                        }
                    }
                    ViewData["Summary"] = list;
                    return View();
                }
                else{
                    return View();
                }
            }
            return RedirectToAction("Index", "Home");
    }
        

    }
}