using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TimeReportingSystem.Models
{
    public class Raport
    {
        public bool frozen { get; set; }
        public List<Entry> entries { get; set; }
        public List<Accepted> accepted { get; set; }
    }
}