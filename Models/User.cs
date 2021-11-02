using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TimeReportingSystem.Models
{
    public class User
    {
        [Required(ErrorMessage = "Proszę podać nazwę użytkownika")]
        [RegularExpression("[a-zA-Z0-9]+", ErrorMessage = "Nazwa użytkownika może zawierac liczby 0-1 i litery bez spacji!")]
        [Remote("VerifyUserName", "Users")]
        [StringLength(20)]
        public string userName { get; set; }
        [Required(ErrorMessage = "Proszę podać prawidłowe imię")]
        // [RegularExpression("[A-Z].[a-z']+", ErrorMessage = "Imię może zawierać litery bez spacji!")]
        [RegularExpression("[A-Z]{1}[^\\s;_A-Z-]+", ErrorMessage = "Imię może zawierać litery bez spacji!")]
        [StringLength(40)]

        public string name { get; set; }
        [Required(ErrorMessage = "Proszę podać prawidłowe nazwisko")]
        // [RegularExpression("[A-Z].[a-z']+", ErrorMessage = "Nazwisko może zawierać litery bez spacji!")]
        [RegularExpression("[A-Z]{1}[^\\s;_A-Z-]+", ErrorMessage = "Nazwisko może zawierać litery bez spacji!")]
        [StringLength(40)]
        public string surname { get; set; }
        public string birthDay { get; set; }
    }
}