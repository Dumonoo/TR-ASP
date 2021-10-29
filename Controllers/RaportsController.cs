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
using Microsoft.AspNetCore.Http;
using System.IO;


namespace TimeReportingSystem.Controllers
{
    public class RaportsController : Controller
    {
        public IActionResult Index(string year, string month){
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                ViewData["Raport"] = "false";

                if(year == null || month == null){
                    year = DateTime.Now.Year.ToString();
                    month = DateTime.Now.Month.ToString();
                }
                
                ViewData["Year"] = year;
                ViewData["Month"] = month;

                if(Int32.Parse(month) <= 9){
                    month = "0" + month;
                }
                
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                
                string fileName = ViewData["User"].ToString() + '-' + year + '-' + month + ".json"; 
                if(Directory.Exists(userPath))
                {
                    if(System.IO.File.Exists(userPath + '/' + fileName) && Directory.GetFiles(userPath).Length>0)
                    {
                        string json = System.IO.File.ReadAllText(userPath + '/' + fileName);
                        Raport activityRaport = JsonSerializer.Deserialize<Raport>(json);
                        ViewData["Raport"] = "true";
                        return View(activityRaport);
                    }
                    else{
                        return View();
                    }
                }
                else{
                    Directory.CreateDirectory(userPath);
                    RedirectToAction("Index", "Raports");
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
                string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
                Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
                ViewData["projectsInfo"] = ToDictionary(activityList);
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
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                string fileName = ViewData["User"].ToString() + "-" + e.date.Substring(0,7) +".json"; 
                if(Directory.Exists(userPath))
                {
                    if(System.IO.File.Exists(userPath + '/' + fileName))
                    {
                        string json = System.IO.File.ReadAllText(userPath + '/' + fileName);
                        Raport activityRaport = JsonSerializer.Deserialize<Raport>(json);
                        activityRaport.entries.Add(e);
                        string saveJson = JsonSerializer.Serialize<Raport>(activityRaport);
                        System.IO.File.WriteAllText(userPath + '/' + fileName, saveJson);
                    }
                    else
                    {
                        Raport newRaport = new Raport();
                        newRaport.frozen = false;
                        newRaport.entries = new List<Entry>();
                        newRaport.accepted = new List<Accepted>();
                        newRaport.entries.Add(e);
                        string saveJson = JsonSerializer.Serialize<Raport>(newRaport);
                        System.IO.File.WriteAllText(userPath + '/' + fileName, saveJson);
                    }
                    return RedirectToAction("Index", "Raports");
                }
                else{
                    Directory.CreateDirectory(userPath);
                    return View("Error");
                }
            }          
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Display(string index, string month, string year)
        {
            ViewData["User"] = HttpContext.Session.GetString(Controllers.UsersController.SessionUser);

            if(ViewData["User"] != null)
            {
                int id = Int32.Parse(index);
                if(Int32.Parse(month) <= 9){
                    month = "0" + month;
                }
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                string fileName = ViewData["User"].ToString() + "-" + year + "-" + month +".json";
                if(Directory.Exists(userPath))
                {
                    if(System.IO.File.Exists(userPath + '/' + fileName))
                    {
                        string json = System.IO.File.ReadAllText(userPath + '/' + fileName);
                        Raport activityRaport = JsonSerializer.Deserialize<Raport>(json);
                        return View(activityRaport.entries[id]);
                    }
                    else{
                        return View("Error");
                    }
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


                if(Int32.Parse(month) <= 9){
                    month = "0" + month;
                }
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                string fileName = ViewData["User"].ToString() + "-" + year + "-" + month +".json";
                if(Directory.Exists(userPath))
                {
                    if(System.IO.File.Exists(userPath + '/' + fileName))
                    {
                        string json = System.IO.File.ReadAllText(userPath + '/' + fileName);
                        Raport activityRaport = JsonSerializer.Deserialize<Raport>(json);
                        string json2 = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
                        Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json2);
                        ViewData["projectsInfo"] = ToDictionary(activityList);
                        ViewData["Entry"] = activityRaport.entries[id];
                        return View(activityRaport.entries[id]);
                    }
                    else{
                        return View("Error");
                    }
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
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                string fileName = ViewData["User"].ToString() + "-" + year + "-" + month +".json";
                if(Directory.Exists(userPath))
                {
                    if(System.IO.File.Exists(userPath + '/' + fileName))
                    {
                        string json = System.IO.File.ReadAllText(userPath + '/' + fileName);
                        Raport activityRaport = JsonSerializer.Deserialize<Raport>(json);
                        activityRaport.entries.Remove(activityRaport.entries[id]);
                        activityRaport.entries.Add(e);
                        string saveJson = JsonSerializer.Serialize<Raport>(activityRaport);
                        System.IO.File.WriteAllText(userPath + '/' + fileName, saveJson);
                        return RedirectToAction("Index", "Raports");
                    }
                    else{
                        return View("Error");
                    }
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
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                string fileName = ViewData["User"].ToString() + "-" + year + "-" + month +".json";
                if(Directory.Exists(userPath))
                {
                    if(System.IO.File.Exists(userPath + '/' + fileName))
                    {
                        string json = System.IO.File.ReadAllText(userPath + '/' + fileName);
                        Raport activityRaport = JsonSerializer.Deserialize<Raport>(json);
                        activityRaport.entries.Remove(activityRaport.entries[id]);
                        string saveJson = JsonSerializer.Serialize<Raport>(activityRaport);
                        System.IO.File.WriteAllText(userPath + '/' + fileName, saveJson);
                        return RedirectToAction("Index", "Raports");
                    }
                    else{
                        return View("Error");
                    }
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
                string userPath = "./wwwroot/json/UsersData/" + ViewData["User"];
                string fileName = ViewData["User"].ToString() + "-" + year + "-" + month +".json";
                if(Directory.Exists(userPath))
                {
                    if(System.IO.File.Exists(userPath + '/' + fileName))
                    {
                        string json = System.IO.File.ReadAllText(userPath + '/' + fileName);
                        Raport activityRaport = JsonSerializer.Deserialize<Raport>(json);
                        activityRaport.frozen = true;
                        string saveJson = JsonSerializer.Serialize<Raport>(activityRaport);
                        System.IO.File.WriteAllText(userPath + '/' + fileName, saveJson);
                        return RedirectToAction("Index", "Raports");
                    }
                    else{
                        return View("Error");
                    }
                }
                else{
                     return View("Error");
                }
                
            }
            return RedirectToAction("Index", "Home");
    }

    }
}