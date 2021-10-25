using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TimeReportingSystem.Models
{
    public class Activity
    {
        public string code { get; set; }
        public string manager { get; set; }
        public string name { get; set; }
        public int budget { get; set; }
        public bool active { get; set; }
        public List<Subactivity> subactivities { get; set; }
    }
}
