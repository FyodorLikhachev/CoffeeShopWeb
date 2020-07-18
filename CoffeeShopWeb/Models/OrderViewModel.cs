using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoffeeShopWeb.Models
{
    public enum OrderStatus
    {
        Received = 1,
        Cancelled
    }
    public class OrderViewModel
    {
        public int OrderNumber { get; set; }

        public string CustomerType { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Received;

        public List<OrderPosition> Items { get; set; }
        public bool IsCardPayment { get; set; }
        public bool IsRefund { get; set; }
    }

    public class OrderPosition
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }

    public class ProviderOrderViewModel
    {
        public int OrderNumber { get; set; }
        public bool IsRefund { get; set; }
        public List<ProviderOrderPosition> Items { get; set; }
    }

    public class ProviderOrderPosition
    {
        public string Provider { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class InventoryViewModel
    {
        public List<InventoryItem> Goods { get; set; }
    }

    public class InventoryItem
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }
    }
}