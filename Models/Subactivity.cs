using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace TimeReportingSystem.Models
{
    public class Subactivity
    {   
        [Required (ErrorMessage = "Należy podać kod podaktywnośći")]
        [RegularExpression("[a-zA-Z0-9-_]+", ErrorMessage = "Nazwa podaktywności nie może zawierać spacjii!")]
        [Remote("SubCodeUniqueness", "Projects", AdditionalFields ="projectCode")]
        public string code { get; set; }
    }
}