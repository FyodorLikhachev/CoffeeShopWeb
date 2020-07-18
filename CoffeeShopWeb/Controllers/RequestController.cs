using CoffeeShopDB;
using CoffeeShopWeb.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CoffeeShopWeb.Controllers
{
    public class RequestController : Controller
    {
        CoffeeShopDBEntities db = new CoffeeShopDBEntities();

        public ActionResult GetInfo()
        {
            int userId = GetCurrentUserId();
            var employees = GetEmployees();

            if (User.IsInRole(Role.Cashier)) {
                var empCount = employees.ToList().Count();

                var orders = (from emp in employees.ToList()
                              join pos in db.order_positions on emp equals pos.employee_id
                              select pos.order_id).Distinct();

                var ordCount = orders.ToList().Count();

                var data = (from org in orders
                            join ord in db.orders on org equals ord.order_id
                            where ord.order_antipod_id == null &&
                                    ord.order_date.ToShortDateString() == DateTime.Today.ToShortDateString()
                            join cst in db.customer_types on ord.customer_type_id equals cst.customer_type_id
                            select new {
                                НомерЗаказа = ord.order_id,
                                Покупатель = cst.customer_name,
                                ВремяЗаказа = ord.order_date,
                                Сумма = ord.order_sum,
                                Оплата = ord.is_card_payment ? "Карта" : "Наличные"
                            }).ToList();

                var dataCount = data.ToList().Count();

                ViewBag.Data = data.ToList().ToDataTable();
            }
            else if (User.IsInRole(Role.Manager)) {
                var stornoTransactions = GetStornoTransactions();
                var noStornoTransactions = db.transactions
                    .Where(x => !stornoTransactions.Contains(x.tran_order_id) &&
                        employees.Contains(x.employee_id)
                    );

                ViewBag.Data = (from trn in noStornoTransactions
                                join typ in db.transaction_types on trn.transaction_type_id equals typ.transaction_type_id
                                join mtr in db.materials on trn.material_id equals mtr.material_id
                                join prv in db.providers on trn.provider_id equals prv.provider_id
                                select new
                                {
                                    ИдентификаторТранзакции = trn.tran_order_id,
                                    Тип = typ.transaction_type_name,
                                    Продукт = mtr.material_name,
                                    Поставщик = prv.provider_name,
                                    Количество = trn.quantity,
                                    Сумма = trn.transaction_sum,
                                    Дата = trn.transaction_date
                                }).ToList().ToDataTable();
            }
            else if (User.IsInRole(Role.Barista)) {

                ViewBag.Data = (from trn in db.transactions
                                join mtr in db.materials on trn.material_id equals mtr.material_id
                                group new { trn, mtr } by new { mtr.material_name, trn.material_id } into mtrGroup
                                select new { Продукт = mtrGroup.Key.material_name, Количество = mtrGroup.Sum(x => x.trn.quantity) }
                                ).ToList().ToDataTable();
            }

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

        public List<int> GetStornoTransactions()
        {
            var employees = GetEmployees();
            return (from emp in employees
                    join trn in db.transactions on emp equals trn.employee_id
                    join str in db.transactions on trn.tran_order_id equals str.tran_order_id
                    where trn.transaction_sum == -str.transaction_sum && str.transaction_type_id == 5
                    select trn.tran_order_id).ToList();
        }
    }
}