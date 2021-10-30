using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json;

namespace TimeReportingSystem.Models
{
    public class Repository
    {
        private Users usersData;
        private Activities projectsData;
        private List<Raport> raportsData;



        public void LoadUsers(){
            string json = System.IO.File.ReadAllText("./wwwroot/json/Users.json");
            Users userList = JsonSerializer.Deserialize<Users>(json);
            usersData = userList;
        }
        public Users GetUsersData(){
            return usersData;
        }

    }
}