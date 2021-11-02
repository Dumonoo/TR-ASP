using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TimeReportingSystem.Models
{
    public class MyProjectModelView
    {
        public string projectCode;
        public string projectName;
        public string manager;
        public bool active;
        public double notSubmittedHours;
        public double submittedHours;
        public double acceptedHours;
        public double budgetNow;
        public double startbudget;
        public List<string> subactivituNames;
    }

}