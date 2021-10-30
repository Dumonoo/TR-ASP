using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace TimeReportingSystem.Models
{
    public class Repository
    {
        private Users usersData;
        private Activities projectsData;
        // private List<Raport> raportsData;
        // private Dictionary<string, Dictionary<string, Raport>> raportsData;
        private Dictionary<string, List<Tuple<string, Raport>>> raportsData;

        public void Load(){
            LoadUsers();
            LoadProjects();
        }
        public void LoadUsers(){
            string json = System.IO.File.ReadAllText("./wwwroot/json/Users.json");
            Users userList = JsonSerializer.Deserialize<Users>(json);
            usersData = userList;
        }
        public void SaveUsers(){
            string saveJson = JsonSerializer.Serialize<Users>(usersData);
            System.IO.File.WriteAllText("./wwwroot/json/Users.json", saveJson);
        }

        public void LoadProjects(){
            string json = System.IO.File.ReadAllText("./wwwroot/json/Activities.json");
            Activities activityList = JsonSerializer.Deserialize<TimeReportingSystem.Models.Activities>(json);
            projectsData = activityList;
        }
        public void SaveProjects(){
            string saveJson = JsonSerializer.Serialize<TimeReportingSystem.Models.Activities>(projectsData);
            System.IO.File.WriteAllText("./wwwroot/json/Activities.json", saveJson);
        }

        public void LoadRaports(){
            string basePath = "./wwwroot/json/UsersData/";
            // raportsData = new Dictionary<string, Dictionary<string, Raport>>();
            raportsData = new Dictionary<string, List<Tuple<string, Raport>>>();
            foreach (var user in usersData.users)
            {
                if(!System.IO.Directory.Exists(basePath + user.userName)){
                    System.IO.Directory.CreateDirectory(basePath + user.userName);
                }
                // Dictionary<string, Raport>  userRaport = new Dictionary<string, Raport>();
                List<Tuple<string, Raport>> userRaports = new List<Tuple<string, Raport>>();
                foreach (var file in System.IO.Directory.GetFiles(basePath + user.userName))
                {
                    var tempDate = Regex.Match(file, "-[0-9]{4}-[0-9]{2}.json").ToString();
                    string date = tempDate.Substring(1,7);
                    string json = System.IO.File.ReadAllText(file);
                    Raport raport = JsonSerializer.Deserialize<Raport>(json);
                    userRaports.Add(new Tuple<string, Raport>(date, raport));
                }
                raportsData.Add(user.userName, userRaports);
            }
        }
        public void SaveRaports(){
            string basePath = "./wwwroot/json/UsersData/";
            foreach (var user in raportsData)
            {
                if(!System.IO.Directory.Exists(basePath + user.Key)){
                    System.IO.Directory.CreateDirectory(basePath + user.Key);
                }
                foreach (var raport in user.Value)
                {
                    var filePath = basePath + user.Key + "/" + user.Key + "-" + raport.Item1 + ".json";
                    string saveJson = JsonSerializer.Serialize<Raport>(raport.Item2);
                    System.IO.File.WriteAllText(filePath, saveJson);
                }
            }
        }

        public Raport GetUserRaport(string userName, string year, string month){
            string period = year + "-" + month;
            return raportsData[userName].Find(e => e.Item1 == period).Item2;
        }

        public bool UserRaportExists(string userName, string year, string month){
            string period = year + "-" + month;
            return raportsData[userName].Exists(e => e.Item1 == period);
        }

        public Entry GetEntry(string index, string userName, string year, string month){
            string period = year + "-" + month;
            return raportsData[userName].Find(e => e.Item1 == period).Item2.entries[Int32.Parse(index)];
        }

        public bool EntryExists(string index, string userName, string year, string month){
            string period = year + "-" + month;
            if(!raportsData[userName].Exists(e => e.Item1 == period)){
                return false;
            }
            if(raportsData[userName].Find(e => e.Item1 == period).Item2.entries[Int32.Parse(index)] == null){
                return false;
            }
            return true;
        }

        
        public Users GetUsersData(){
            return usersData;
        }
        public void InsertUser(User u){
            usersData.users.Add(u);
            string userPath = "./wwwroot/json/UsersData/" + u.userName;
            System.IO.Directory.CreateDirectory(userPath);
            SaveUsers();
        }

        public void UpdateUser(User u){
            // Not in use
        }

        public void DeleteUser(string userName){
            // Not in use
        }

        public Activities GetActivities(){
            return projectsData;
        }
        public Activity GetActivityByCode(string projectCode){
            return projectsData.activities.Find(i => i.code == projectCode);
        }

        public void InsertActivity(Activity a){
            a.active = true;
            a.subactivities = new List<Subactivity>();
            a.budget = a.budget * 60;
            projectsData.activities.Add(a);
            SaveProjects();
        }

        public void UpdateActivity(Activity a){
            Activity old = projectsData.activities.Find(i => i.code == a.code);
            a.budget = a.budget * 60;
            a.subactivities = old.subactivities;
            a.manager = old.manager;
            a.active = old.active;
            projectsData.activities.Remove(old);
            projectsData.activities.Add(a);
            SaveProjects();
        }

        public void DeleteActivity(string projectCode){
            // Not in use
        }

        public void InsertSubActivity(Subactivity subactivity, string projectCode){

            GetActivityByCode(projectCode).subactivities.Add(subactivity);
            SaveProjects();
        }

        public void InsertRaport(Raport raport, string year, string month, string userName){
            // raportsData[userName].Add()
            string period = year + "-" + month;
        }

        public void InsertEntry(Entry entry, string year, string month, string userName){
            string period = year + "-" + month;
            if(raportsData[userName].Exists(e => e.Item1 == period)){
                raportsData[userName].Find(e => e.Item1 == period).Item2.entries.Add(entry);
            }
            else{
                var newRaport = new Raport();
                newRaport.frozen = false;
                newRaport.entries = new List<Entry>();
                newRaport.accepted = new List<Accepted>();
                newRaport.entries.Add(entry);
                var newTuple = new Tuple<string, Raport>(period, newRaport);
                raportsData[userName].Add(newTuple);
            }
            SaveRaports();
        }

        public void UpdateEntry(Entry entry, string index, string userName, string year, string month){
            string period = year + "-" + month;
            DeleteEntry(index, userName, year, month);
            InsertEntry(entry, year, month, userName);
        }

        public void DeleteEntry(string index, string userName, string year, string month){
            string period = year + "-" + month;
            Entry oldEntry = GetEntry(index, userName, year, month);
            raportsData[userName].Find(e => e.Item1 == period).Item2.entries.Remove(oldEntry);
            SaveRaports();
        }

    }
}