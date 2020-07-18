using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CoffeeShopWeb.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Login is incorrect")]
        [Display(Name = "Login")]
        public string Login { get; set; }


        [Required(ErrorMessage = "Password is incorrect")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}