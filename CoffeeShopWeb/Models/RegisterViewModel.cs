using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using CoffeeShopDB;

namespace CoffeeShopWeb.Models
{
    public enum Positions
    {
        Менеджер,
        Кассир,
        Бариста
    }
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        [MaxLength(50, ErrorMessage = "Имя не должно быть длиннее 50 символов")]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [MaxLength(50, ErrorMessage = "Фамилия не должно быть длиннее 50 символов")]
        [Display(Name = "Фамилия")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "Не указано отчество")]
        [MaxLength(50, ErrorMessage = "Отчество не должно быть длиннее 50 символов")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Логин не указан")]
        [MaxLength(50, ErrorMessage = "Логин не должен быть длиннее 50 символов")]
        [Display(Name = "Login")]
        public string Login { get; set; }


        [Required(ErrorMessage = "Password is empty")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public Positions Position { get; set; }
        public string SalesPoint { get; set; }
    }
}