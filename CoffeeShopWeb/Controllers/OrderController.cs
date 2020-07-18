using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CoffeeShopDB;
using CoffeeShopWeb.Models;

namespace CoffeeShopWeb.Controllers
{
    public class OrderController : Controller
    {
        CoffeeShopDBEntities db = new CoffeeShopDBEntities();

        public ActionResult ManageOrder()
        {
            ViewBag.Items = db.goods.Where(x => x.active_to == null)
                .Select(x => x.good_name )
                .ToListAsync().Result;

            ViewBag.Customers = db.customer_types.Where(x => x.active_to == null)
                .Select(x => x.customer_name)
                .ToListAsync().Result;

            return View();
        }

        [HttpPost]
        public ActionResult ManageOrder(OrderViewModel model)
        {
            var order = db.orders.FirstOrDefault(x => x.order_id == model.OrderNumber);
            ViewBag.Items = db.goods.Where(x => x.active_to == null)
                .Select(x => x.good_name)
                .ToListAsync().Result;

            ViewBag.Customers = db.customer_types.Where(x => x.active_to == null)
                .Select(x => x.customer_name)
                .ToListAsync().Result;
            if (order == null && model.IsRefund) {
                ModelState.Clear();
                ModelState.AddModelError("", "Указанного заказа не существует");
                return View(model);
            }
            if (order?.order_antipod_id == null && model.IsRefund) {
                ModelState.Clear();
                ModelState.AddModelError("", "Указанный заказ уже отменён");
                return View(model);
            }
            
            // Оформление возврата
            if (order != null && model.IsRefund) {
                var storno = new order() {
                    order_sum = -order.order_sum,
                    order_date = DateTime.UtcNow,
                    customer_type_id = order.customer_type_id,
                    is_card_payment = order.is_card_payment
                };

                db.orders.Add(storno);
                db.SaveChanges();
                order.order_antipod_id = storno.order_antipod_id;
            }

            if (!model.IsRefund && model?.Items == null) {
                ModelState.Clear();
                ModelState.AddModelError("", "Не указаны позиции заказа");
                return View(model);
            }

            // Оформление заказа
            if (!model.IsRefund) {
                if (model?.Items.Where(x => x.Quantity < 1).Count() > 0) {
                    ModelState.Clear();
                    ModelState.AddModelError("", "Не указано количество товаров в позиции заказа");
                    return View(model);
                }

                var customer = db.customer_types.FirstOrDefault(x => x.customer_name == model.CustomerType);
                var orderSum = (from item in model.Items
                                join good in db.goods on item.ItemName equals good.good_name
                                select good.price * item.Quantity).Sum();

                var newOrder = new order() {
                    order_sum = orderSum * (decimal)(1 - customer.discount * 0.01),
                    order_date = DateTime.UtcNow,
                    customer_type_id = customer.customer_type_id,
                    is_card_payment = model.IsCardPayment
                };
                db.orders.Add(newOrder);
                db.SaveChanges();

                foreach (var item in model?.Items) {
                    var good = db.goods.FirstOrDefault(x => x.good_name == item.ItemName);
                    var orderPosition = new order_positions()
                    {
                        employee_id = GetCurrentUserId(),
                        good_id = good.good_id,
                        quantity = item.Quantity,
                        order_id = newOrder.order_id
                    };
                    db.order_positions.Add(orderPosition);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ManageProviderOrder()
        {
            ViewBag.Providers = db.providers.Where(x => x.active_to == null && x.provider_id != 6)
                .Select(x => x.provider_name)
                .ToListAsync().Result;

            ViewBag.Materials = db.materials
                .Select(x => x.material_name)
                .ToListAsync().Result;

            return View();
        }

        [HttpPost]
        public ActionResult ManageProviderOrder(ProviderOrderViewModel model)
        {
            var providerOrder = db.transactions.FirstOrDefault(x => x.tran_order_id == model.OrderNumber);
            ViewBag.Providers = db.providers.Where(x => x.active_to == null && x.provider_id != 6)
                .Select(x => x.provider_name)
                .ToListAsync().Result;

            ViewBag.Materials = db.materials
                .Select(x => x.material_name)
                .ToListAsync().Result;

            if (providerOrder == null && model.IsRefund)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Указанной закупки не существует");
                return View(model);
            }

            if (providerOrder != null && model.IsRefund)
            {
                // Если найдём сторнирование, то выведем ошибку. Если нет, то сторнируем
                if ((from trn in db.transactions
                     join strTrn in db.transactions on trn.tran_order_id equals strTrn.tran_order_id
                     where trn.tran_order_id == model.OrderNumber &&
                     trn.transaction_sum == -strTrn.transaction_sum
                     select 1).Any())
                {
                    ModelState.Clear();
                    ModelState.AddModelError("", "Указанная закупка уже отменена");
                    return View(model);
                }
                else 
                {
                    var tranForStorno = db.transactions.Where(x => x.tran_order_id == model.OrderNumber).ToList();
                    var curUser = GetCurrentUserId();
                    var dateNow = DateTime.UtcNow;
                    foreach (var trn in tranForStorno) {
                        var stornoTransaction = new transaction()
                        {
                            employee_id = curUser,
                            material_id = trn.material_id,
                            transaction_date = dateNow,
                            provider_id = trn.provider_id,
                            transaction_sum = -trn.transaction_sum,
                            transaction_type_id = 5, // Возврат
                            tran_order_id = trn.tran_order_id,
                            quantity = trn.quantity
                        };

                        db.transactions.Add(stornoTransaction);
                    }
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }

            if (!model.IsRefund && model.Items == null)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Не указаны позиции закупки");
                return View(model);
            }

            if (model.Items.Where(x => x.Quantity <= 0 || x.Price <= 0).Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Цена и количество товара должны быть больше нуля");
                return View(model);
            }

            if (!model.IsRefund && model.Items.Any()) 
            {
                var orderTranId = GetNextTranOrderId();
                foreach (var item in model.Items) {
                    var material = db.materials.FirstOrDefault(x => x.material_name == item.ItemName);
                    var provider = db.providers.FirstOrDefault(x => x.provider_name == item.Provider);
                    var newTransaction = new transaction()
                    {
                        employee_id = GetCurrentUserId(),
                        material_id = material.material_id,
                        transaction_date = DateTime.UtcNow,
                        provider_id = provider.provider_id,
                        transaction_sum = item.Price,
                        transaction_type_id = 1, // Поставка
                        tran_order_id = (int)orderTranId,
                        quantity = item.Quantity
                    };
                    db.transactions.Add(newTransaction);
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ManageInventory()
        {
            var model = new InventoryViewModel() {
                Goods = GetInventory()
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult ManageInventory(InventoryViewModel model)
        {
            var inventory = GetInventory();

            // Если новое количество товаров больше текущего
            if ((from mod in model.Goods
                 join inv in inventory on mod.ItemName equals inv.ItemName
                 where inv.Quantity < mod.Quantity
                 select 1).Any()) 
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Новое количество продуктов на складе больше текущего");
                return View(model);
            }

            if ((from mod in model.Goods
                 join inv in inventory on mod.ItemName equals inv.ItemName
                 where mod.Quantity < 0
                 select 1).Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Количество продуктов на складе не может быть отрицательным");
                return View(model);
            }

            var changes = from mod in model.Goods
                          join inv in inventory on mod.ItemName equals inv.ItemName
                          join mtr in db.materials on inv.ItemName equals mtr.material_name
                          where inv.Quantity != mod.Quantity
                          select new { MaterialId = mtr.material_id , Quantity = mod.Quantity - inv.Quantity };

            if (changes.Any()) {
                foreach (var item in changes.ToList()) {
                    var newTransaction = new transaction()
                    {
                        employee_id = GetCurrentUserId(),
                        material_id = item.MaterialId,
                        transaction_date = DateTime.UtcNow,
                        provider_id = 6, // Наша кофейня
                        transaction_sum = 0, // Ничего не получаем
                        transaction_type_id = 4, // Инвентаризация
                        tran_order_id = (int)GetNextTranOrderId(),
                        quantity = item.Quantity
                    };
                    db.transactions.Add(newTransaction);
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


        public int GetCurrentUserId() 
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            
            var sid = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => int.Parse(c.Value)).SingleOrDefault();

            return sid;
        }

        public long GetNextTranOrderId() 
        {
            long seq = db.Database.SqlQuery<long>("SELECT NEXT VALUE FOR dbo.s_tran_order_id;").FirstOrDefault();
            return seq;
        }

        public List<InventoryItem> GetInventory() 
        {
            var curUser = GetCurrentUserId();
            var employees = from acc in db.accounts
                            where acc.account_id == curUser
                            join emp in db.employees on acc.employee_id equals emp.employee_id
                            join amp in db.employees on emp.sales_point_id equals amp.sales_point_id
                            select amp.employee_id;

            return (from emp in employees
                    join trn in db.transactions on emp equals trn.employee_id
                    join mtr in db.materials on trn.material_id equals mtr.material_id
                    group new { trn, mtr } by new { mtr.material_name, trn.material_id } into mtrGroup
                    select new InventoryItem { ItemName = mtrGroup.Key.material_name, Quantity = mtrGroup.Sum(x => x.trn.quantity) }
                   ).ToList();
        }

        
    }
}