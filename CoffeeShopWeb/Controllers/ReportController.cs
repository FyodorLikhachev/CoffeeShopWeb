using CoffeeShopDB;
using CoffeeShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShopWeb.Controllers
{
    public class ReportController : Controller
    {
        CoffeeShopDBEntities db = new CoffeeShopDBEntities();
        public ActionResult Reports()
        {
            var minusSevenDate = DateTime.UtcNow.AddDays(-7);//.ToShortDateString();
            var curUser = GetCurrentUserId();
            var employees = GetEmployees();
            var address = (from emp in db.employees
                           join spt in db.sales_points on emp.sales_point_id equals spt.sales_point_id
                           where emp.employee_id == curUser
                           select spt.address).FirstOrDefault();

            var orders = GetOrders();
            var x = from ord in orders
                    join org in db.orders on ord equals org.order_id
                    where org.order_date >= minusSevenDate
                    group org by new { date = org.order_date.ToShortDateString() } into orgGroup
                    select new { Дата = orgGroup.Key.date, Кофейня = address, Сумма = orgGroup.Sum(xx => xx.order_sum) };

            var y = from ord in orders
                    join org in db.orders on ord equals org.order_id
                    join pos in db.order_positions on org.order_id equals pos.order_id
                    join god in db.goods on pos.good_id equals god.good_id
                    where org.order_date >= minusSevenDate
                    group new { org, god, pos } by new { date = org.order_date.ToShortDateString(), god.good_name } into newGroup
                    select new { Дата = newGroup.Key.date, Товар = newGroup.Key.good_name, Количество = newGroup.Sum(xx => xx.pos.quantity) };

            ViewBag.Report1 = x.ToList().ToDataTable();
            ViewBag.Report2 = y.ToList().ToDataTable();
            return View();
        }

        public int GetCurrentUserId() => ((ClaimsPrincipal)Thread.CurrentPrincipal).Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => int.Parse(c.Value))
            .SingleOrDefault();

        public List<int> GetEmployees()
        {
            var curUser = GetCurrentUserId();

            return (from acc in db.accounts
                    where acc.account_id == curUser
                    join emp in db.employees on acc.employee_id equals emp.employee_id
                    join amp in db.employees on emp.sales_point_id equals amp.sales_point_id
                    select amp.employee_id
                    ).ToList();
        }

        public List<int> GetOrders()
        {
            var employees = GetEmployees();

            return (from pos in db.order_positions
                    join ord in db.orders on pos.order_id equals ord.order_id
                    where employees.Contains(pos.employee_id)
                    select ord.order_id
                    ).Distinct().ToList();
        }
    }
}