using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TimeReportingSystem.Models
{
    public class ProjectUserRaportModelView
    {
        public string projectCode;
        public bool projectActive;
        public string userName;
        public string name;
        public string surname;
        public string period;
        public int timeSubmitted;
        public int timeAccepted;
        public bool frozen;
        public bool accepted;
    }

}