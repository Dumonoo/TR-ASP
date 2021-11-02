using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System.Linq;

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
                    if(!raport.entries.Any()){
                        continue;
                    }
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

        public List<ProjectUserRaportModelView> GetManageData(string projectCode){
            var returnList = new List<ProjectUserRaportModelView>();
            var allRaports = GetUserRaports();
            var allUsers = GetUsersData();
            GetProjectInfo(projectCode);

            
            foreach (var user in allRaports)
            {
                foreach (var raport in user.Value)
                {
                    if(raport.Item2.entries.Exists(e => e.code == projectCode)){
                        var forViewData = new ProjectUserRaportModelView();
                        var userInfo = allUsers.users.Find(e => e.userName == user.Key);
                        forViewData.projectCode = projectCode;
                        forViewData.projectActive = GetProjectInfo(projectCode).active;
                        forViewData.userName = userInfo.userName;
                        forViewData.name = userInfo.name;
                        forViewData.surname = userInfo.surname;
                        forViewData.period = raport.Item1;
                        forViewData.timeSubmitted = raport.Item2.entries.Where(e => e.code == projectCode).Select( d => d.time).Sum();
                        forViewData.accepted = raport.Item2.accepted.Exists(e => e.code == projectCode);
                        if(forViewData.accepted){
                            forViewData.timeAccepted = raport.Item2.accepted.Find(e => e.code == projectCode).time;
                        }
                        else{
                            forViewData.timeAccepted = 0;
                        }
                        forViewData.frozen = raport.Item2.frozen;
                        returnList.Add(forViewData);
                    }
                    
                }
            }
            return returnList;
        }
        public List<MyProjectModelView> GetMyProjects(string userName){
            var myProjects = new List<MyProjectModelView>();
            foreach (var project in projectsData.activities.OrderByDescending(f => f.active))
            {
                if(project.manager == userName){
                    var myProjectEntry = new MyProjectModelView();
                    myProjectEntry.projectCode = project.code;
                    myProjectEntry.projectName = project.name;
                    myProjectEntry.manager = project.manager;
                    myProjectEntry.active = project.active;
                    // retursn tuple of minutes (notsubmited submited accepted)
                    var HoursTuple = GetHours(project.code);
                    myProjectEntry.notSubmittedHours = HoursTuple.Item1 / 60;
                    myProjectEntry.submittedHours = HoursTuple.Item2 / 60;
                    myProjectEntry.acceptedHours = HoursTuple.Item3 / 60;
                    myProjectEntry.budgetNow = (project.budget - HoursTuple.Item3) / 60;
                    myProjectEntry.startbudget  = project.budget / 60;
                    var subAct = new List<string>();
                    foreach (var subactivity in project.subactivities)
                    {
                        subAct.Add(subactivity.code);
                    }
                    myProjectEntry.subactivituNames = subAct;
                    myProjects.Add(myProjectEntry);
                }
            }
            return myProjects;

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
        public Dictionary<string, Tuple<int, int, int>> GetProjectsInfo(string userName){
            var myProjectDic = new Dictionary<string, Tuple<int, int, int>>();
            foreach (var project in projectsData.activities)
            {
                if(project.manager == userName){
                    myProjectDic.Add(project.code, GetHours(project.code));
                }
            }
            return myProjectDic;         
        }
        public Tuple<int, int, int> GetHours(string projectCode){
            
            var notSubmitted = 0;
            var submitted = 0;
            var accepted = 0;

            foreach (var user in raportsData)
            {
                foreach (var raport in user.Value)
                {
                    if(raport.Item2.frozen == false){
                        notSubmitted += raport.Item2.entries.Where(e => e.code == projectCode).Select(d => d.time).Sum();
                    }
                    else{
                        if(!raport.Item2.accepted.Exists(e => e.code == projectCode)){
                            submitted += raport.Item2.entries.Where(e => e.code == projectCode).Select(d => d.time).Sum();
                        }
                        else{
                            accepted += raport.Item2.accepted.Find(e => e.code == projectCode).time;
                        }
                    }
                }
            }
            return new Tuple<int, int, int>(notSubmitted, submitted, accepted);
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
            a.subactivities = old.subactivities;
            a.budget = a.budget * 60;
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
        public Dictionary<string, List<Tuple<string, Raport>>> GetUserRaports(){
            return raportsData;
        }
        public bool IsRaportSubmitted(string userName, string year, string month){
            string period = year + "-" + month;
            if(raportsData[userName].Any(e => e.Item1 == period))
            {
                return raportsData[userName].Find(e => e.Item1 == period).Item2.frozen;
            }
            return false;
        }
        public bool IsProjectActive(string projectCode){
            return projectsData.activities.Find(e => e.code == projectCode).active;
        }
        public void CloseProject(string projectCode){
            projectsData.activities.Find(e => e.code == projectCode).active = false;
            SaveProjects();
        }
        public Activity GetProjectInfo(string projectCode){
            return projectsData.activities.Find(e => e.code == projectCode);
        }
        public void InsertRaport(Raport raport, string year, string month, string userName){
            // raportsData[userName].Add()+
            // not in use
            string period = year + "-" + month;
        }
        public void AcceptRaport(string year, string month, string userName, string projectCode, int acceptedTime){
            string period = year + "-" + month;
            if(raportsData[userName].Exists(e => e.Item1 == period)){
                var newAcceptedTime = new Accepted();
                newAcceptedTime.code = projectCode;
                newAcceptedTime.time = acceptedTime;
                raportsData[userName].Find(e => e.Item1 == period).Item2.accepted.Add(newAcceptedTime);
                SaveRaports();
            }
            else{
                Console.WriteLine("ERROR - dunno");
            }
        }
        public void ChangeRaport(string year, string month, string userName, string projectCode, int acceptedTime){
            string period = year + "-" + month;
            if(raportsData[userName].Exists(e => e.Item1 == period)){
                raportsData[userName].Find(e => e.Item1 == period).Item2.accepted.Find(e => e.code == projectCode).time = acceptedTime;
                SaveRaports();
            }
            else{
                Console.WriteLine("ERROR - dunno");
            }
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