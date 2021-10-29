using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TimeReportingSystem.Models
{
    public class Accepted
    {
        [Required]
        public string code { get; set; }
        [Required]
        public int time { get; set; }
    }
    
}