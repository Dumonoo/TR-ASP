using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TimeReportingSystem.Models
{
    public class Entry
    {
        [Required]
        public string date { get; set; }
        [Required]
        public string code { get; set; }
        public string subcode { get; set; }
        [Required]
        [RegularExpression("[1-9]{1}[0-9]+", ErrorMessage ="Proszę podać prawidłową liczbę!")]
        public int time { get; set; }
        [StringLength(2000)]
        public string description { get; set; }
    }
}