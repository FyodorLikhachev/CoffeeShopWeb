using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CoffeeShopDB;
using Microsoft.Owin.Security;
using CoffeeShopWeb.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace CoffeeShopWeb.Controllers
{
    public class AccountController : Controller
    {
        CoffeeShopDBEntities db = new CoffeeShopDBEntities();

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pas = GetHash(model.Password, "Coffee");
                account user = db.accounts.FirstOrDefault(u => u.login_value == model.Login); //&& u.password_value == pas);
                if (user != null)
                {
                    await Authenticate(user.account_id.ToString(), user.login_value, user.employee.position.ToString()); // аутентификация
                    return RedirectToAction("Index", "Home");
                }
                else ModelState.AddModelError("", "Логин и(или) пароль введены не верно");
            }
            return View(model);
        }

        public ActionResult Register()
        {
            ViewBag.Adresses = db.sales_points
                .Where(x => x.active_to == null)
                .Select(x => new System.Web.Mvc.SelectListItem { Text = x.address, Value = x.address })
                .ToList();
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                account newUser;
                employee newEmployee;
                newUser = db.accounts.FirstOrDefault(u => u.login_value == model.Login);
                if (newUser is null)
                {
                    int salesPoint = db.sales_points
                        .FirstOrDefault(x => x.address == model.SalesPoint && x.active_to == null)
                        .sales_point_id;

                    newEmployee = new employee { position = model.Position.ToString(),
                        active_from = DateTime.UtcNow, active_to = null, first_name = model.Name, 
                        second_name = model.SecondName, middle_name = model.MiddleName, 
                        sales_point_id = salesPoint };

                    db.employees.Add(newEmployee);
                    await db.SaveChangesAsync();

                    newUser = new account
                    {
                        login_value = model.Login,
                        password_value = GetHash(model.Password, "Coffee"),
                        employee_id = newEmployee.employee_id
                    };

                    db.accounts.Add(newUser);
                    await db.SaveChangesAsync();
                    await Authenticate(newUser.account_id.ToString(), newUser.login_value, model.Position.ToString());
                    return RedirectToAction("Index", "Home");
                }
                else ModelState.AddModelError("", "Указанный логин уже зарегистрирован в системе");

            }
            return View(model);
        }

        private async Task Authenticate(string userId, string userName, string userRole)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, userRole)
            };
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, id);
        }

        [AuthorizeRoles(Role.Manager, Role.Cashier, Role.Barista)]
        public async Task<ActionResult> Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //Получение хэш-значения
        public static string GetHash(string password, string salt)
        {
            //Экземпляр объекта MD5
            MD5 md5 = new MD5CryptoServiceProvider();
            //Вычисление хэш-значения
            byte[] digest = md5.ComputeHash(Encoding.UTF8.GetBytes(password + salt));
            //Получение строкового значения из массива байт
            string base64digest = Convert.ToBase64String(digest, 0, digest.Length);
            return base64digest;
        }
    }
}