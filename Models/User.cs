using System;
using System.ComponentModel.DataAnnotations;

namespace TimeReportingSystem.Models
{
    public class User
    {
        public string userName { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string birthDay { get; set; }
    }
}