using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TimeReportingSystem.Models
{
    public class Activity
    {
        [Required(ErrorMessage = "Proszę podać kod projektu")]
        [RegularExpression("[a-zA-Z0-9-_]+", ErrorMessage = "Nazwa projektu może zawierac małe i duże oraz cyfry bez spacji!")]
        [Remote("ProjCodeUniqueness", "Projects")]
        public string code { get; set; }
        [Required]
        public string manager { get; set; }
        [Required(ErrorMessage = "Proszę podać nazwę projektu")]
        public string name { get; set; }
        [Required(ErrorMessage = "Proszę podać budżet projektu")]
        [RegularExpression("[0-9]+\\.?[0-9]?", ErrorMessage = "Budżet może miec jedno miejsce po przecinku")]

        public int budget { get; set; }
        public bool active { get; set; }
        
        public List<Subactivity> subactivities { get; set; }
    }
}
