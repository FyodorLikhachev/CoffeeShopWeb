using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShopWeb.Models
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }

    public static class Role
    {
        public const string Manager = "Менеджер";
        public const string Cashier = "Кассир";
        public const string Barista = "Бариста";
    }
}