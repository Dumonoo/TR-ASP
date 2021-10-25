using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TimeReportingSystem.Models
{
    public class Entry
    {
        public string date { get; set; }
        public string code { get; set; }
        public string subcode { get; set; }
        public int time { get; set; }
        public string description { get; set; }
    }
}