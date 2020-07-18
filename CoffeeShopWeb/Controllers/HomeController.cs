using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShopWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Учёт продаж кофеен";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Наши контактные данные";

            return View();
        }
    }
}