using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TimeReportingSystem.Models
{
    public class User
    {
        [Required(ErrorMessage = "Proszę podać nazwę użytkownika")]
        [RegularExpression("[a-z0-9]+", ErrorMessage = "Nazwa użytkownika może zawierac liczby 0-1 i litery bez spacji!")]
        // [Remote(action: "VerifyUserName", controller: "Users")] //niedziala
        [Remote("VerifyUserName", "Users")]
        // [Remote("VerifyUserName", "Validation", ErrorMessage = "Enter an existing key")]
        public string userName { get; set; }
        [Required(ErrorMessage = "Proszę podać prawidłowe imię")]
        [RegularExpression("[A-Z].[a-z']+", ErrorMessage = "Imię może zawierać litery bez spacji!")]
        public string name { get; set; }
        [Required(ErrorMessage = "Proszę podać prawidłowe nazwisko")]
        [RegularExpression("[A-Z].[a-z']+", ErrorMessage = "Nazwisko może zawierać litery bez spacji!")]
        public string surname { get; set; }
        public string birthDay { get; set; }
    }
}