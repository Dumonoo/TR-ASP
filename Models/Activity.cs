using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TimeReportingSystem.Models
{
    public class Activity
    {
        [Required(ErrorMessage = "Proszę podać kod projektu")]
        [RegularExpression("[a-zA-Z0-9-_]+", ErrorMessage = "Nazwa projektu może składać sie z dużych i małych liter, cyfr - i _")]
        [Remote("ProjCodeUniqueness", "Projects")]
        [StringLength(30)]
        public string code { get; set; }
        [Required]
        public string manager { get; set; }
        [Required(ErrorMessage = "Proszę podać nazwę projektu")]
        [StringLength(90)]
        public string name { get; set; }
        [Required(ErrorMessage = "Proszę podać budżet projektu")]
        [RegularExpression("[0-9]+\\.?[0-9]?", ErrorMessage = "Budżet może miec jedno miejsce po przecinku")]
        [Range(0, 8000000)]

        public int budget { get; set; }
        public bool active { get; set; }
        
        public List<Subactivity> subactivities { get; set; }
    }
}
